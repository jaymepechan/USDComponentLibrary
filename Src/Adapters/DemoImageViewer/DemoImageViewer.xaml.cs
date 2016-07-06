/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility.DataLoader;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.AifServices;
using Microsoft.USD.ComponentLibrary.Utilities;
using Microsoft.Xrm.Tooling.WebResourceUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for DemoImageViewer.xaml
    /// </summary>
    public partial class DemoImageViewer : DynamicsBaseHostedControl
    {
        private Microsoft.Xrm.Tooling.WebResourceUtility.TraceLogger LogWriter = null;
        string[] images = null;
        int imagePosition = -1;

        public DemoImageViewer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public DemoImageViewer(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
            // This will create a log writer with the default provider for Unified Service desk
            LogWriter = new Microsoft.Xrm.Tooling.WebResourceUtility.TraceLogger();

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

        }

        protected override void DoAction(Uii.Csr.RequestActionEventArgs args)
        {
            if (args.Action.Equals("ShowImages", StringComparison.InvariantCultureIgnoreCase))
            {
                string imagelist = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
                images = imagelist.Replace("\r\n", "\n").Replace('\r','\n').Split('\n');
                imagePosition = -1;
                ShowNextImage();
                return;
            }

            base.DoAction(args);
        }

        void ShowNextImage()
        {
            if (images == null || images.Count() < 1)
                return;
            imagePosition++;
            if (imagePosition > images.Count() - 1)
                imagePosition = 0;
            BitmapImage bi = RetrieveImage(images[imagePosition].Trim());
            if (bi == null)
                return;
            imageDisplay.Source = bi;
        }

        private void Image_Click(object sender, RoutedEventArgs e)
        {
            ShowNextImage();
        }

        System.Windows.Media.Imaging.BitmapImage RetrieveImage(string value)
        {
            System.Windows.Media.Imaging.BitmapImage btm = ImageCache.Retrieve(value);
            if (btm != null)
            {
                return btm;
            }

            System.Windows.Controls.Image img3;
            img3 = new System.Windows.Controls.Image();
            IUsdConfiguraitonManager _cfgMgr = AifServiceContainer.Instance.GetService<IUsdConfiguraitonManager>();
            if (_cfgMgr != null && _cfgMgr.IsUsdConfigDataReady)
            {
                ImageResources _crmWebResource = new ImageResources(_cfgMgr.CrmManagementSvc);
                System.Windows.Media.Imaging.BitmapImage bm;
                bm = _crmWebResource.GetImageFromCRMWebResource((string)value);
                if (bm != null)
                {
                    ImageCache.Store((string)value, bm);
                    return bm.Clone();
                }
                else
                    return null;
            }
            else
            {
                return null;
            }
        }
    }
}
