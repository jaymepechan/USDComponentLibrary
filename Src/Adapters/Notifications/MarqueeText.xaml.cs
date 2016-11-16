// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using System.Windows.Media.Animation;

namespace Microsoft.USD.ComponentLibrary.Adapters.Notifications
{
    /// <summary>
    /// Interaction logic for MarqueeText.xaml
    /// This is a base control for building Unified Service Desk Aware add-ins
    /// See USD API documentation for full API Information available via this control.
    /// </summary>
    public partial class MarqueeText : MicrosoftBase
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private TraceLogger LogWriter = null;
        class NotifyItem
        {
            public UIElement animatedObject;
            public DateTime timeout = DateTime.Now + TimeSpan.FromSeconds(60);
        }
        List<NotifyItem> notifyItems = new List<NotifyItem>();

        #endregion

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public MarqueeText(Guid appID, string appName, string initString)
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

        }

        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            // this will populate any toolbars assigned to this control in config. 
            PopulateToolbars(ProgrammableToolbarTray);
            base.DesktopReady();
            //System.Threading.Timer tCheck = new System.Threading.Timer(new System.Threading.TimerCallback(tCheckValue), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            RegisterAction("AddNotification", AddNotification);
            RegisterAction("SetWidth", SetWidth);

        }

        private void SetWidth(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string width = Utility.GetAndRemoveParameter(parms, "width");
            MyCanvas.Width = double.Parse(width);
        }
        private void AddNotification(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string text = Utility.GetAndRemoveParameter(parms, "text");
            string milliseconds = Utility.GetAndRemoveParameter(parms, "milliseconds");
            NotifyItem item = new NotifyItem();
            TextBlock tb = new TextBlock();
            tb.Text = text;
            item.animatedObject = tb;
            int result;
            if (int.TryParse(milliseconds, out result))
                item.timeout = DateTime.Now + TimeSpan.FromMilliseconds(result);
            lock (notifyItems)
                notifyItems.Add(item);
            if (notifyItems.Count == 1)
                SetMarqueeText();
        }

        void tCheckValue(Object obj)
        {
            lock(notifyItems)
            {
                for (int i = notifyItems.Count()-1; i >= 0 ; i--)
                {
                    if (notifyItems[i].timeout > DateTime.Now)
                    {
                        notifyItems.RemoveAt(i);
                    }
                }
            }
        }

        private void TextAnimation_Completed(object sender, EventArgs e)
        {
            MessageBox.Show("Restart");
            SetMarqueeText();
        }

        void SetMarqueeText()
        {
            MyCanvas.Children.Clear();
            foreach (NotifyItem item in notifyItems)
            MyCanvas.Children.Add(item.animatedObject);

            TranslateTransform tt = new TranslateTransform(0, 0);
            MyCanvas.RenderTransform = tt;
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(new DoubleAnimation(0, 50, new Duration(TimeSpan.FromSeconds(20))));
            Storyboard.SetTarget(storyboard, tt);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Y"));

                //        < TextBlock.RenderTransform >
                //< TranslateTransform x: Name = "AnimatedTranslateTransform" X = "0" Y = "0" />
      
                //  </ TextBlock.RenderTransform >
      
                //  < TextBlock.Triggers >
      
                //      < EventTrigger RoutedEvent = "TextBlock.TextInput" >
       
                //           < BeginStoryboard >
       
                //               < Storyboard x: Name = "MyStoryboard" >
         

                //                 </ Storyboard >
         
                //             </ BeginStoryboard >
         
                //         </ EventTrigger >
         
                //     </ TextBlock.Triggers >

                     //< DoubleAnimation
                     //                    Storyboard.TargetName = "AnimatedTranslateTransform"
                     //                    Storyboard.TargetProperty = "X"
                     //                    From = "{DynamicResource MarqueeValue.Width}" To = "30" Duration = "0:0:20"
                     //                    Completed = "TextAnimation_Completed" />
                     storyboard.Begin();
        }
    }
}
