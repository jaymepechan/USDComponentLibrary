/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Microsoft.Uii.AifServices;
using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for MCSBrowserCache.xaml
    /// </summary>
    public partial class MCSBrowserCache : DynamicsBaseHostedControl
    {
        private TraceLogger LogWriter = null;
        private int cachesize = 5;
        private bool allowGrow = true;
        List<MCSWebBrowser> cache = new List<MCSWebBrowser>();

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public MCSBrowserCache(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
            // This will create a log writer with the default provider for Unified Service desk
            LogWriter = new TraceLogger();

            #region Enhanced LogProvider Info
            // This will create a log writer with the same name as your hosted control. 
            // LogWriter = new TraceLogger(traceSourceName:"MyTraceSource");

            // If you utilize this feature,  you would need to add a section to the system.diagnostics settings area of the UnifiedServiceDesk.exe.config
            //<source name="MyTraceSource" switchName="MyTraceSwitchName" switchType="System.Diagnostics.SourceSwitch">
            //    <listeners>
            //        <add name="console" type="System.Diagnostics.DefaultTraceListener"/>
            //        <add name="fileListener"/>
            //        <add name="USDDebugListener" />
            //        <remove name="Default"/>
            //    </listeners>
            //</source>

            // and then in the switches area : 
            //<add name="MyTraceSwitchName" value="Verbose"/>

            #endregion

            if (AifServiceContainer.Instance.GetService<MCSBrowserCache>() == null)
            {
                AifServiceContainer.Instance.AddService<MCSBrowserCache>(typeof(MCSBrowserCache), this);
            }
            LoadupCache();
        }
        
        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            if (args.Action.Equals("SetCacheSize", StringComparison.InvariantCultureIgnoreCase))
            {
                List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string cachesizeString = Utility.GetAndRemoveParameter(parameters, "cachesize");
                string allowGrowString = Utility.GetAndRemoveParameter(parameters, "allowgrow");
                if (!String.IsNullOrEmpty(allowGrowString))
                {
                    allowGrow = bool.Parse(allowGrowString);
                }
                cachesize = 20;
                if (!String.IsNullOrEmpty(cachesizeString))
                {
                    cachesize = int.Parse(cachesizeString);
                }
                LoadupCache();
            }
            else if (args.Action.Equals("ClearCache", StringComparison.InvariantCultureIgnoreCase))
            {
                ClearCache();
            }
            base.DoAction(args);
        }

        private void ClearCache()
        {
            lock (BrowserStorage)
            {
                cache.Clear();
                while (BrowserStorage.Children.Count > 0)
                {
                    MCSWebBrowser b = BrowserStorage.Children[0] as MCSWebBrowser;
                    BrowserStorage.Children.RemoveAt(0);
                    b.Dispose();
                }
            }
        }

        public override void Close()
        {
            ClearCache();
            base.Close();
        }

        void LoadupCache()
        {
            lock (BrowserStorage)
            {
                for (int i = BrowserStorage.Children.Count; i < cachesize; i++)
                {
                    MCSWebBrowser browser = new MCSWebBrowser();
                    BrowserStorage.Children.Add(browser);
                    browser.BrowserObjectAppeared += browser_BrowserObjectAppeared;
                    browser.Navigate("about:blank");    // trigger the load of the browser instance
                }
            }
        }

        void browser_BrowserObjectAppeared(object sender, BrowserObjectAppearedEventArgs e)
        {
            lock (cache)
            {
                if (!cache.Contains(sender as MCSWebBrowser))
                    cache.Add(sender as MCSWebBrowser);
                (sender as MCSWebBrowser).BrowserObjectAppeared -= browser_BrowserObjectAppeared;
            }
        }

        public MCSWebBrowser BrowserFromCache(string applicationName)
        {
            WaitForBrowsersToInitialize();

            lock (BrowserStorage)
            {
                if (cache.Count > 0)
                {   // initialized browsers
                    MCSWebBrowser b = cache[0] as MCSWebBrowser;
                    if (BrowserStorage.Children.Contains(cache[0]))
                        BrowserStorage.Children.Remove(cache[0]);
                    cache.RemoveAt(0);
                    b.Reset(applicationName);
                    return b;
                }
                else
                {
                    Debug.Assert(BrowserStorage.Children.Count == 0);
                    Trace.WriteLine("Browser Cache empty, providing uninitialized browser!");
                    MCSWebBrowser b = new MCSWebBrowser();
                    b.Reset(applicationName);
                    return b;
                }
            }
        }

        private void WaitForBrowsersToInitialize()
        {
            int cacheCount, childrenCount;
            lock (cache)
                cacheCount = cache.Count;
            lock (BrowserStorage)
                childrenCount = BrowserStorage.Children.Count;
            DateTime dtTimeout = DateTime.Now + TimeSpan.FromSeconds(5);
            while (cacheCount == 0 && childrenCount > 0)
            {
                if (DateTime.Now > dtTimeout)
                {
                    lock (BrowserStorage)
                    {
                        for (int i = 0; i < BrowserStorage.Children.Count; i++)
                        {
                            MCSWebBrowser b = BrowserStorage.Children[i] as MCSWebBrowser;
                            if (b != null)
                                b.Navigate("about:blank"); // try again to get this going
                        }
                        dtTimeout = DateTime.Now + TimeSpan.FromSeconds(5);
                    }
                }

                // browsers initializing
                System.Windows.Forms.Application.DoEvents();
                lock (cache)
                    cacheCount = cache.Count;
                lock (BrowserStorage)
                    childrenCount = BrowserStorage.Children.Count;
            }
        }

        public void BrowserToCache(MCSWebBrowser webBrowser)
        {
            lock (BrowserStorage)
            {
                if (allowGrow == false && BrowserStorage.Children.Count >= cachesize)
                {
                    webBrowser.Dispose();
                    return;
                }
                BrowserStorage.Children.Add(webBrowser);
                cache.Add(webBrowser); // assume this is an initialized browser
                webBrowser.HaltUse();
            }
        }
    }
}
