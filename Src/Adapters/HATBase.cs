/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Uii.HostedApplicationToolkit.AutomationHosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
    /// <summary>
    /// HAT Base adapter is to be configured as a USD Hosted Control
    /// The Automation XML is not populated
    /// Instead create a <HAT> </HAT> tag in the Extension XML and include the initstring for HAT inside that
    /// </summary>
    public class HATBase : SetParentBase
    {
        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public HATBase(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {

        }

        AutomationAdapter _AutomationAdapter = new AutomationAdapter();
        protected override void DesktopReady()
        {
            base.DesktopReady();

            AddImplicitAction(AutomationAdapter.DdaActionFindControl);
            AddImplicitAction(AutomationAdapter.DdaActionGetControlValue);
            AddImplicitAction(AutomationAdapter.DdaActionSetControlValue);
            AddImplicitAction(AutomationAdapter.DdaActionExecuteControlAction);
            AddImplicitAction(AutomationAdapter.DdaActionAddDoActionEventTrigger);
            AddImplicitAction(AutomationAdapter.DdaActionRemoveDoActionEventTrigger);
            this.WindowAttached += HATBase_WindowAttached;
        }

        protected override void DoAction(Uii.Csr.RequestActionEventArgs args)
        {
            // EXAMPLE ACTION
            //if (args.Action.Equals("Test1", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    string param = "<DdaParameters><ControlName>UIA:3</ControlName><ControlValue></ControlValue><Data></Data></DdaParameters>";
            //    RequestActionEventArgs raArgs = new RequestActionEventArgs(args.SessionId, this.ApplicationName, AutomationAdapter.DdaActionFindControl, param);
            //    bool ret = _AutomationAdapter.DoAction(actions[AutomationAdapter.DdaActionFindControl], raArgs);
            //    Trace.WriteLine("RET=" + raArgs.ActionReturnValue);
            //    ret = _AutomationAdapter.DoAction(actions[AutomationAdapter.DdaActionExecuteControlAction], raArgs);
            //    Trace.WriteLine("RET=" + raArgs.ActionReturnValue);
            //    //System.Threading.Thread run = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(RunCalcCallback));
            //    //run.Start("45/98*324-78+541");

            //    //RunCalc("45/98*324-78+541");
            //}

            base.DoAction(args);
        }

        private XmlDocument CaptureHATInitString()
        {
            XmlNode nodeBindings = InitString.SelectSingleNode(".//HAT");
            if (nodeBindings != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.AppendChild(doc.ImportNode(nodeBindings.FirstChild, true));
                return doc;
            }
            return null;
        }

        protected virtual void HATBase_WindowAttached(object sender, EventArgs e)
        {
            _AutomationAdapter.SessionWorkItem = AppHostWorkItem;
            _AutomationAdapter.DdaApplicationObject = win.Handle;
            _AutomationAdapter.Name = ApplicationName;
            _AutomationAdapter.ApplicationInitString = CaptureHATInitString().OuterXml;
            _AutomationAdapter.AdapterFireRequestActionEvent += DoAction;
            _AutomationAdapter.AdapterContextChangedEvent += _AutomationAdapter_AdapterContextChangedEvent;
            _AutomationAdapter.Initialize();
        }

        void _AutomationAdapter_AdapterContextChangedEvent(Uii.Csr.Context newContext)
        {
            FireChangeContext(new Uii.Csr.ContextEventArgs(newContext));
        }
    }
}
