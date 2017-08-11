using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xrm.Tooling.WebResourceUtility;
using Microsoft.Uii.HostedApplicationToolkit.DataDrivenAdapter;
using Accessibility;
using System.Windows.Automation;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.USD.ComponentLibrary.Adapters.Automation.UIVisualViewer;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public class UIAutomationObject : IUSDAutomationObject
    {
        public AutomationElement zero = null;
        public string name = String.Empty;

        public UIAutomationObject(string _name, AutomationElement _zero)
        {
            zero = _zero;
            name = _name;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JavaAutomationObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class UIAutomation : AutomationControl
    {
        AgentDesktopSession localSession;
        TraceLogger LogWriter;
        string ApplicationName;
        private System.Threading.Timer JavaAccEventListenerInstallTimer;
        IntPtr rootWindow = IntPtr.Zero;

        public UIAutomation(IntPtr _rootWindow, string _ApplicationName, AgentDesktopSession _localSession, TraceLogger _LogWriter)
        {
            localSession = _localSession;
            LogWriter = _LogWriter;
            ApplicationName = _ApplicationName;
            rootWindow = _rootWindow;

            //    this.IsWebApp = false;
            this.MainWindowHandle = (IntPtr)_rootWindow;
            RootAutomationElement = AutomationElement.FromHandle(this.MainWindowHandle);
            AutomationProperties = UIAHelperMethods.GetAutomationElementIdentifiers();
            UIAHelperMethods.UpdateControlTypeNames();
            AutomationPropReturnTypes = UIAHelperMethods.GetAutomationReturnTypes();
            AutomationEventList = UIAHelperMethods.GetAutomationEventList();
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }
        #endregion

        public bool Tracing { get; set; }

        public override IUSDAutomationObject GetAutomationObject(XmlNode control)
        {
            /*
            <Controls>
                <UIElement Name="UISystemandSecurityHyperlink">
                    <UIObject MatchCount="1">                            
                        <AndCondition>
                    <PropertyCondition Name="Name">CPCategoryPanel</PropertyCondition>
                    <PropertyCondition Name="ControlType">Pane</PropertyCondition>
                  </AndCondition>
                    <UIObject>                                   
                      <AndCondition>
                        <PropertyCondition Name="Name">System and Security</PropertyCondition>
                        <PropertyCondition Name="ControlType">Hyperlink</PropertyCondition>
                      </AndCondition>                  
                    </UIObject>
                    </UIObject>
                <UIElement>
            </Controls>
            */

            if (control.Name != "AccControl")
                throw new Exception("Invalid node type included: " + control.Name + ". Only AccControl is valid.");

            AutomationElement zero = null;
            zero = this.FindUIAControl(control);
            string controlName = GetAttributeValue(control, "name", "");
            return new UIAutomationObject(controlName, zero);
        }

        public override LookupRequestItem GetValue(IUSDAutomationObject automationObject)
        {
            if (!(automationObject is UIAutomationObject))
                throw new Exception("Invalid automation object");
            string controlValue = String.Empty;
            LookupRequestItem result = null;

            foreach (string propertyKey in AutomationProperties.Keys)
            {
                AutomationProperty property = AutomationProperties[propertyKey];
                object currentPropertyValue = ((UIAutomationObject)automationObject).zero.GetCurrentPropertyValue(property);
                if (currentPropertyValue != null)
                {
                    result = new LookupRequestItem(((UIAutomationObject)automationObject).name, ConvertObjecttoString(currentPropertyValue, AutomationPropReturnTypes[propertyKey]));
                } // which result do we return?
            }
            return result;
        }

        public override void Execute(IUSDAutomationObject automationObject, string action)
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            if (!(automationObject is JavaAutomationObject))
                throw new Exception("Invalid automation object");
        }

        public override void SetValue(IUSDAutomationObject automationObject, string value)
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            if (!(automationObject is JavaAutomationObject))
                throw new Exception("Invalid automation object");
        }

        UITreeViewModel vm;
        UIVisualViewer.UIVisualViewer avv = null;
        Window w = new Window();
        public override void DisplayVisualTree()
        {
            AutomationElement rootAutomationElement = this.RootAutomationElement;
            if (w != null)
            {
                w.Closing -= W_Closing;
                avv = null;
                w = null;
            }
            avv = new UIVisualViewer.UIVisualViewer(ApplicationName);
            avv.LoadTree(rootAutomationElement);
            w = new Window();
            w.Closing += W_Closing;
            w.Content = avv;
            w.Show();
        }

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            w = null;
            avv = null;
        }


        #region WINDOWS
        // Fields
        private AutomationEventHandler OnAutomationEventHandler;
        private AutomationFocusChangedEventHandler OnAutomationFocusChangedEventHandler;
        private AutomationPropertyChangedEventHandler OnAutomationPropertyChangedHandler;
        private StructureChangedEventHandler OnStructureChangedEventHandler;
        public const string PREFIX = "UIA";
        private System.Windows.Automation.Condition BuildAndCondition(XmlNode xnAndCondition)
        {
            System.Windows.Automation.Condition item = null;
            List<System.Windows.Automation.Condition> list = new List<System.Windows.Automation.Condition>();
            foreach (XmlNode node in xnAndCondition.SelectNodes("PropertyCondition"))
            {
                item = this.BuildPropertyCondition(node);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node2 in xnAndCondition.SelectNodes("AndCondition"))
            {
                item = this.BuildAndCondition(node2);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node3 in xnAndCondition.SelectNodes("OrCondition"))
            {
                item = this.BuildOrCondition(node3);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node4 in xnAndCondition.SelectNodes("NotCondition"))
            {
                item = this.BuildNotCondition(node4);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return new AndCondition(list.ToArray());
        }

        private System.Windows.Automation.Condition BuildNotCondition(XmlNode xnNotCondition)
        {
            NotCondition condition = null;
            XmlNode xnPropCondition = xnNotCondition.SelectSingleNode("PropertyCondition");
            XmlNode node2 = xnNotCondition.SelectSingleNode("AndCondition");
            XmlNode node3 = xnNotCondition.SelectSingleNode("OrCondition");
            if (xnPropCondition != null)
            {
                System.Windows.Automation.Condition condition2 = this.BuildPropertyCondition(xnPropCondition);
                if (condition2 != null)
                {
                    condition = new NotCondition(condition2);
                }
                return condition;
            }
            if (node2 != null)
            {
                System.Windows.Automation.Condition condition3 = this.BuildAndCondition(xnPropCondition);
                if (condition3 != null)
                {
                    condition = new NotCondition(condition3);
                }
                return condition;
            }
            if (node3 != null)
            {
                System.Windows.Automation.Condition condition4 = this.BuildOrCondition(xnPropCondition);
                if (condition4 != null)
                {
                    condition = new NotCondition(condition4);
                }
            }
            return condition;
        }

        private System.Windows.Automation.Condition BuildOrCondition(XmlNode xnOrCondition)
        {
            System.Windows.Automation.Condition item = null;
            List<System.Windows.Automation.Condition> list = new List<System.Windows.Automation.Condition>();
            foreach (XmlNode node in xnOrCondition.SelectNodes("PropertyCondition"))
            {
                item = this.BuildPropertyCondition(node);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node2 in xnOrCondition.SelectNodes("AndCondition"))
            {
                item = this.BuildAndCondition(node2);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node3 in xnOrCondition.SelectNodes("OrCondition"))
            {
                item = this.BuildOrCondition(node3);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            foreach (XmlNode node4 in xnOrCondition.SelectNodes("NotCondition"))
            {
                item = this.BuildNotCondition(node4);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return new OrCondition(list.ToArray());
        }

        private System.Windows.Automation.Condition BuildPropertyCondition(XmlNode xnPropCondition)
        {
            System.Windows.Automation.Condition condition = null;
            PropertyConditionFlags none = PropertyConditionFlags.None;
            string str = string.Empty;
            if (xnPropCondition.InnerText == null)
            {
                return condition;
            }
            str = UIAHelperMethods.GetXmlAttribute(xnPropCondition, "Name").Value;
            Type type = AutomationPropReturnTypes[str];
            if (type.Equals(typeof(string)))
            {
                none = PropertyConditionFlags.IgnoreCase;
            }
            return new PropertyCondition(AutomationProperties[str], this.ConvertStringtoObject(xnPropCondition.InnerText, AutomationPropReturnTypes[str]), none);
        }

        private System.Windows.Automation.Condition BuildSearchCondition(XmlNode xnUIAObject)
        {
            System.Windows.Automation.Condition trueCondition = null;
            XmlNode xnPropCondition = xnUIAObject.SelectSingleNode("PropertyCondition");
            XmlNode xnAndCondition = xnUIAObject.SelectSingleNode("AndCondition");
            XmlNode xnOrCondition = xnUIAObject.SelectSingleNode("OrCondition");
            XmlNode xnNotCondition = xnUIAObject.SelectSingleNode("NotCondition");
            if (xnPropCondition != null)
            {
                trueCondition = this.BuildPropertyCondition(xnPropCondition);
            }
            else if (xnAndCondition != null)
            {
                trueCondition = this.BuildAndCondition(xnAndCondition);
            }
            else if (xnOrCondition != null)
            {
                trueCondition = this.BuildOrCondition(xnOrCondition);
            }
            else if (xnNotCondition != null)
            {
                trueCondition = this.BuildNotCondition(xnNotCondition);
            }
            if (trueCondition == null)
            {
                trueCondition = System.Windows.Automation.Condition.TrueCondition;
            }
            return trueCondition;
        }

        protected virtual string ConvertObjecttoString(object BindingObject, Type type)
        {
            string str = string.Empty;
            string fullName = type.FullName;
            if (fullName == null)
            {
                return str;
            }
            if ((fullName != "System.Boolean") && (fullName != "System.String"))
            {
                if (fullName != "System.Windows.Rect")
                {
                    if (fullName == "System.Windows.Point")
                    {
                        Point point = (Point)BindingObject;
                        return (point.X.ToString(CultureInfo.InvariantCulture) + "," + point.Y.ToString(CultureInfo.InvariantCulture));
                    }
                    if (fullName == "System.Windows.Automation.OrientationType")
                    {
                        OrientationType type2 = (OrientationType)BindingObject;
                        return type2.ToString();
                    }
                    if (fullName != "System.Windows.Automation.ControlType")
                    {
                        return str;
                    }
                    ControlType type3 = (ControlType)BindingObject;
                    return type3.ProgrammaticName;
                }
            }
            else
            {
                return BindingObject.ToString();
            }
            Rect rect = (Rect)BindingObject;
            return string.Concat(new object[] { CultureInfo.InvariantCulture, "(", rect.X.ToString(CultureInfo.InvariantCulture), ",", rect.Y.ToString(CultureInfo.InvariantCulture), ",", rect.Width.ToString(CultureInfo.InvariantCulture), ",", rect.Height.ToString(CultureInfo.InvariantCulture), ")" });
        }

        protected virtual object ConvertStringtoObject(string BindingText, Type type)
        {
            object obj2 = null;
            string fullName = type.FullName;
            if (fullName == null)
            {
                return obj2;
            }
            if (fullName != "System.Boolean")
            {
                if (fullName != "System.String")
                {
                    if (fullName == "System.Windows.Rect")
                    {
                        return UIAHelperMethods.GetRect(BindingText);
                    }
                    if (fullName == "System.Windows.Point")
                    {
                        return UIAHelperMethods.GetPoint(BindingText);
                    }
                    if (fullName == "System.Windows.Automation.OrientationType")
                    {
                        OrientationType none = OrientationType.None;
                        Enum.TryParse<OrientationType>(BindingText, out none);
                        return none;
                    }
                    if (fullName != "System.Windows.Automation.ControlType")
                    {
                        return obj2;
                    }
                    return UIAHelperMethods.GetControlType(BindingText);
                }
            }
            else
            {
                bool result = false;
                bool.TryParse(BindingText, out result);
                return result;
            }
            return UIAHelperMethods.UnEscapeXml(BindingText);
        }

        private void ExecuteUIAControl(object objectThread)
        {
            //try
            //{
            //    bool flag = false;
            //    string controlName = ((AutomationThreadParams)objectThread).ControlName;
            //    AutomationElement uiaElement = this.FindUIAControl(controlName, true);
            //    string data = ((AutomationThreadParams)objectThread).Data;
            //    if (uiaElement != null)
            //    {
            //        foreach (AutomationPattern pattern in uiaElement.GetSupportedPatterns())
            //        {
            //            if (((AutomationThreadParams)objectThread).Data == string.Empty)
            //            {
            //                data = pattern.ProgrammaticName;
            //            }
            //            if ((pattern.ProgrammaticName == InvokePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
            //            {
            //                flag = true;
            //                UIAHelperMethods.ProcessInvokePattern(uiaElement, OperationType.ExecuteControlAction, "");
            //                break;
            //            }
            //            if ((pattern.ProgrammaticName == TogglePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
            //            {
            //                flag = true;
            //                UIAHelperMethods.ProcessTogglePattern(uiaElement, OperationType.ExecuteControlAction, "");
            //                break;
            //            }
            //            if ((pattern.ProgrammaticName == ExpandCollapsePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
            //            {
            //                flag = true;
            //                UIAHelperMethods.ProcessExpandCollapsePattern(uiaElement, OperationType.ExecuteControlAction, "");
            //                break;
            //            }
            //            if ((pattern.ProgrammaticName == SelectionItemPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
            //            {
            //                flag = true;
            //                UIAHelperMethods.ProcessSelectionItemPattern(uiaElement, OperationType.ExecuteControlAction, "");
            //                break;
            //            }
            //            if ((pattern.ProgrammaticName == TextPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
            //            {
            //                flag = true;
            //                UIAHelperMethods.ProcessTextPattern(uiaElement, OperationType.ExecuteControlAction, "");
            //                break;
            //            }
            //        }
            //    }
            //    if (!flag)
            //    {
            //        ((AutomationThreadParams)objectThread).ex = new DataDrivenAdapterException("DDA0700_UIA_PATTERN_NOT_FOUND");
            //    }
            //}
            //catch (Exception exception)
            //{
            //    ((AutomationThreadParams)objectThread).ex = exception;
            //}
            //finally
            //{
            //    ((AutomationThreadParams)objectThread).waitHandle.Set();
            //}
        }

        protected AutomationElement FindUIAControl(XmlNode control)
        {
            string controlName = GetAttributeValue(control, "name", "");

            AutomationElement rootAutomationElement = this.RootAutomationElement;
            if (control != null)
            {
                System.Windows.Automation.Condition condition = null;
                int result = 1;
                int num2 = 1;
                XmlAttribute xmlAttribute = UIAHelperMethods.GetXmlAttribute(control, "StartFromDesktop");
                if ((xmlAttribute != null) && (xmlAttribute.Value != null))
                {
                    bool flag = true;
                    if (xmlAttribute.Value == flag.ToString())
                    {
                        rootAutomationElement = AutomationElement.RootElement;
                    }
                }
                XmlNode xnPropCondition = control.SelectSingleNode("UIObject");
                if (xnPropCondition == null)
                {
                    return rootAutomationElement;
                }
                do
                {
                    XmlAttribute attribute2 = UIAHelperMethods.GetXmlAttribute(xnPropCondition, "MatchCount");
                    if ((attribute2 != null) && (attribute2.Value != null))
                    {
                        int.TryParse(attribute2.Value, out result);
                    }
                    condition = this.BuildSearchCondition(xnPropCondition);
                    if (result == 1)
                    {
                        rootAutomationElement = rootAutomationElement.FindFirst(TreeScope.Children, condition);
                    }
                    else
                    {
                        AutomationElementCollection elements = rootAutomationElement.FindAll(TreeScope.Children, condition);
                        if ((result <= elements.Count) && (result > 0))
                        {
                            rootAutomationElement = elements[result - 1];
                        }
                        else
                        {
                            throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "DDA0400_UNABLE_TO_FIND_CONTROL_ON_UI" + string.Format(CultureInfo.CurrentUICulture, "FIND_ERROR_LEVEL", new object[] { num2.ToString(CultureInfo.InvariantCulture) }));
                        }
                    }
                    if (rootAutomationElement == null)
                    {
                        throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "DDA0400_UNABLE_TO_FIND_CONTROL_ON_UI" + string.Format(CultureInfo.CurrentUICulture, "FIND_ERROR_LEVEL", new object[] { num2.ToString(CultureInfo.InvariantCulture) }));
                    }
                    xnPropCondition = xnPropCondition.SelectSingleNode("UIObject");
                    num2++;
                }
                while (xnPropCondition != null);
            }
            return rootAutomationElement;
        }

        public static ReadOnlyCollection<string> GetControlEvents() =>
            new ReadOnlyCollection<string>(new string[] {
        AutomationElementIdentifiers.AsyncContentLoadedEvent.ProgrammaticName, AutomationElementIdentifiers.AutomationFocusChangedEvent.ProgrammaticName, AutomationElementIdentifiers.AutomationPropertyChangedEvent.ProgrammaticName, AutomationElementIdentifiers.LayoutInvalidatedEvent.ProgrammaticName, AutomationElementIdentifiers.MenuClosedEvent.ProgrammaticName, AutomationElementIdentifiers.MenuOpenedEvent.ProgrammaticName, AutomationElementIdentifiers.ToolTipClosedEvent.ProgrammaticName, AutomationElementIdentifiers.ToolTipOpenedEvent.ProgrammaticName, SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.ProgrammaticName, SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent.ProgrammaticName, SelectionItemPatternIdentifiers.ElementSelectedEvent.ProgrammaticName, SelectionPatternIdentifiers.InvalidatedEvent.ProgrammaticName, InvokePatternIdentifiers.InvokedEvent.ProgrammaticName, TextPatternIdentifiers.TextChangedEvent.ProgrammaticName, TextPatternIdentifiers.TextSelectionChangedEvent.ProgrammaticName, WindowPatternIdentifiers.WindowOpenedEvent.ProgrammaticName,
        WindowPatternIdentifiers.WindowClosedEvent.ProgrammaticName, AutomationElementIdentifiers.StructureChangedEvent.ProgrammaticName
            });

        /*
                private void GetUIAControlUsingPatterns(object objectThread, ref string getText, ref bool patternIdentified, AutomationElement uiaElement, ref string data)
                {
                    if (((ControlType)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty)) != null)
                    {
                        foreach (AutomationPattern pattern in uiaElement.GetSupportedPatterns())
                        {
                            if (((AutomationThreadParams)objectThread).Data == string.Empty)
                            {
                                data = pattern.ProgrammaticName;
                            }
                            if ((pattern.ProgrammaticName == ValuePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessValuePattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == TogglePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessTogglePattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == SelectionItemPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessSelectionItemPattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == SelectionPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessSelectionPattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == TextPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessTextPattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == RangeValuePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessRangeValuePattern(uiaElement, OperationType.GetControlValue, "");
                                patternIdentified = true;
                                break;
                            }
                            if ((pattern.ProgrammaticName == ScrollPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                            {
                                getText = UIAHelperMethods.ProcessScrollPattern(uiaElement, OperationType.GetControlValue, "2,2");
                                patternIdentified = true;
                                break;
                            }
                        }
                    }
                    if (!patternIdentified)
                    {
                        try
                        {
                            if (!string.IsNullOrWhiteSpace(((AutomationThreadParams)objectThread).Data))
                            {
                                ((AutomationThreadParams)objectThread).ex = new DataDrivenAdapterException("DDA0702_UIA_PROPERTY_PATTERN_NOT_AVAILABLE");
                            }
                            else
                            {
                                getText = (string)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty);
                            }
                        }
                        catch (ElementNotAvailableException)
                        {
                            ((AutomationThreadParams)objectThread).ex = new DataDrivenAdapterException("DDA0702_UIA_PROPERTY_NOT_AVAILABLE");
                        }
                    }
                }
                */
        /*
        protected override string OperationHandler(OperationType op, string controlName, string controlValue, string data)
        {
            string str2;
            if ((this.IsWebApp && (this.RootAutomationElement != null)) && ((this.RootAutomationElement.Current.AutomationId != null) && (this.RootAutomationElement.Current.AutomationId == "WebBrowserExtended")))
            {
                System.Windows.Automation.Condition condition = new PropertyCondition(AutomationElementIdentifiers.ClassNameProperty, "IEFrame");
                AutomationElement element = this.RootAutomationElement.FindFirst(TreeScope.Children, condition);
                if (element != null)
                {
                    this.RootAutomationElement = element;
                }
            }
            this.ControlConfig = base.GetControlConfig(controlName);
            string name = this.ControlConfig?.Name;
            try
            {
                string str3 = name;
                if (str3 == null)
                {
                    throw new DataDrivenAdapterException(op, controlName, "DDA0101_NAMED_CONTROL_CONFIG_NOT_FOUND");
                }
                if (str3 != "UIElement")
                {
                    throw new DataDrivenAdapterException(op, controlName, "DDA0109_UNSUPPORTED_CONTROL_TYPE");
                }
                switch (op)
                {
                    case OperationType.FindControl:
                        {
                            AutomationThreadParams state = new AutomationThreadParams
                            {
                                throwExceptionIfNotFound = false,
                                ControlName = controlName,
                                waitHandle = new ManualResetEvent(false)
                            };
                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.FindUIAControlAsync), state);
                            state.waitHandle.WaitOne();
                            if (state.ex != null)
                            {
                                throw state.ex;
                            }
                            return ((state.Element == null) ? 0.ToString() : 1.ToString());
                        }
                    case OperationType.GetControlValue:
                        {
                            AutomationThreadParams params3 = new AutomationThreadParams
                            {
                                ControlName = controlName,
                                waitHandle = new ManualResetEvent(false),
                                Data = data
                            };
                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.GetUIAControl), params3);
                            params3.waitHandle.WaitOne();
                            if (params3.ex != null)
                            {
                                throw params3.ex;
                            }
                            return params3.ControlValue;
                        }
                    case OperationType.SetControlValue:
                        {
                            AutomationThreadParams params2 = new AutomationThreadParams
                            {
                                ControlName = controlName,
                                waitHandle = new ManualResetEvent(false),
                                ControlValue = controlValue,
                                Data = data
                            };
                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.SetUIAControl), params2);
                            params2.waitHandle.WaitOne();
                            if (params2.ex != null)
                            {
                                throw params2.ex;
                            }
                            return null;
                        }
                    case OperationType.ExecuteControlAction:
                        {
                            AutomationThreadParams params4 = new AutomationThreadParams
                            {
                                ControlName = controlName,
                                waitHandle = new ManualResetEvent(false),
                                Data = data
                            };
                            ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteUIAControl), params4);
                            bool result = false;
                            bool.TryParse(controlValue, out result);
                            if (!result)
                            {
                                params4.waitHandle.WaitOne();
                            }
                            if (params4.ex != null)
                            {
                                throw params4.ex;
                            }
                            return null;
                        }
                }
                str2 = null;
            }
            catch (Exception exception)
            {
                if (exception is DataDrivenAdapterException)
                {
                    throw;
                }
                throw new DataDrivenAdapterException(op, controlName, exception);
            }
            return str2;
        }
        
        public override bool RegisterEventListener(string eventName, string controlName, EventHandler<ControlChangedEventArgs> listenerCallback, string data)
        {
            AutomationEventHandler handler = null;
            AutomationFocusChangedEventHandler handler2 = null;
            StructureChangedEventHandler handler3 = null;
            AutomationPropertyChangedEventHandler handler4 = null;
            bool flag = false;
            flag = base.RegisterEventListenerBase(eventName, controlName, listenerCallback);
            if (flag && !string.IsNullOrEmpty(controlName))
            {
                AutomationProperty[] properties = null;
                if ((data != string.Empty) && AutomationProperties.ContainsKey(data))
                {
                    properties = new AutomationProperty[] { AutomationProperties[data] };
                }
                else
                {
                    properties = new AutomationProperty[] { AutomationElement.NameProperty, AutomationElement.IsEnabledProperty };
                }
                this.ControlConfig = base.GetControlConfig(controlName);
                AutomationThreadParams state = new AutomationThreadParams
                {
                    ControlName = controlName,
                    waitHandle = new ManualResetEvent(false)
                };
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.FindUIAControlAsync), state);
                state.waitHandle.WaitOne();
                if (state.ex != null)
                {
                    throw state.ex;
                }
                AutomationElement element = state.Element;
                if (element == null)
                {
                    throw new DataDrivenAdapterException("DDA0400_UNABLE_TO_FIND_CONTROL_ON_UI");
                }
                if (!AutomationEventList.ContainsKey(eventName))
                {
                    throw new DataDrivenAdapterException("DDA0701_EVENTIDENTIFIER_NOT_FOUND");
                }
                Type eventArgsType = AutomationEventList[eventName].EventArgsType;
                if ((eventArgsType.Equals(typeof(AutomationEventArgs)) || eventArgsType.Equals(typeof(WindowClosedEventArgs))) || eventArgsType.Equals(typeof(AsyncContentLoadedEventArgs)))
                {
                    if (handler == null)
                    {
                        handler = delegate (object src, AutomationEventArgs args) {
                            this.RaiseEvent(src, args.EventId.ProgrammaticName, controlName, string.Empty);
                        };
                    }
                    System.Windows.Automation.Automation.AddAutomationEventHandler(AutomationEventList[eventName].Event, element, TreeScope.Element, this.OnAutomationEventHandler = handler);
                    return flag;
                }
                if (eventArgsType.Equals(typeof(AutomationFocusChangedEventArgs)))
                {
                    if (handler2 == null)
                    {
                        handler2 = delegate (object src, AutomationFocusChangedEventArgs args) {
                            this.RaiseEvent(src, args.EventId.ProgrammaticName, controlName, string.Empty);
                        };
                    }
                    System.Windows.Automation.Automation.AddAutomationFocusChangedEventHandler(this.OnAutomationFocusChangedEventHandler = handler2);
                    return flag;
                }
                if (eventArgsType.Equals(typeof(StructureChangedEventArgs)))
                {
                    if (handler3 == null)
                    {
                        handler3 = delegate (object src, StructureChangedEventArgs args) {
                            this.RaiseEvent(src, args.EventId.ProgrammaticName, controlName, string.Empty);
                        };
                    }
                    System.Windows.Automation.Automation.AddStructureChangedEventHandler(element, TreeScope.Element, this.OnStructureChangedEventHandler = handler3);
                    return flag;
                }
                if (!eventArgsType.Equals(typeof(AutomationPropertyChangedEventArgs)))
                {
                    return flag;
                }
                if (handler4 == null)
                {
                    handler4 = delegate (object src, AutomationPropertyChangedEventArgs args) {
                        this.RaiseEvent(src, args.EventId.ProgrammaticName, controlName, string.Empty);
                    };
                }
                System.Windows.Automation.Automation.AddAutomationPropertyChangedEventHandler(element, TreeScope.Element, this.OnAutomationPropertyChangedHandler = handler4, properties);
            }
            return flag;
        }

        private void SetUIAControl(object objectThread)
        {
            try
            {
                bool flag = false;
                string controlValue = ((AutomationThreadParams)objectThread).ControlValue;
                string controlName = ((AutomationThreadParams)objectThread).ControlName;
                AutomationElement uiaElement = this.FindUIAControl(controlName, true);
                string data = ((AutomationThreadParams)objectThread).Data;
                if ((uiaElement != null) && (((ControlType)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.ControlTypeProperty)) != null))
                {
                    foreach (AutomationPattern pattern in uiaElement.GetSupportedPatterns())
                    {
                        if (((AutomationThreadParams)objectThread).Data == string.Empty)
                        {
                            data = pattern.ProgrammaticName;
                        }
                        if ((pattern.ProgrammaticName == ValuePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessValuePattern(uiaElement, OperationType.SetControlValue, controlValue);
                            break;
                        }
                        if ((pattern.ProgrammaticName == TogglePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessTogglePattern(uiaElement, OperationType.SetControlValue, "");
                            break;
                        }
                        if ((pattern.ProgrammaticName == SelectionItemPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessSelectionItemPattern(uiaElement, OperationType.SetControlValue, "");
                            break;
                        }
                        if ((pattern.ProgrammaticName == SelectionPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessSelectionPattern(uiaElement, OperationType.SetControlValue, "");
                            break;
                        }
                        if ((pattern.ProgrammaticName == RangeValuePattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessRangeValuePattern(uiaElement, OperationType.SetControlValue, controlValue);
                            break;
                        }
                        if ((pattern.ProgrammaticName == ScrollPattern.Pattern.ProgrammaticName) && (data == pattern.ProgrammaticName))
                        {
                            flag = true;
                            UIAHelperMethods.ProcessScrollPattern(uiaElement, OperationType.SetControlValue, controlValue);
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    ((AutomationThreadParams)objectThread).ex = new DataDrivenAdapterException(Resources.DDA0700_UIA_PATTERN_NOT_FOUND);
                }
            }
            catch (Exception exception)
            {
                ((AutomationThreadParams)objectThread).ex = exception;
            }
            finally
            {
                ((AutomationThreadParams)objectThread).waitHandle.Set();
            }
        }

        public override bool UnregisterEventListener(string eventName, string controlName, EventHandler<ControlChangedEventArgs> listenerCallback)
        {
            bool flag = false;
            flag = base.UnregisterEventListenerBase(eventName, controlName, listenerCallback);
            if (flag && !string.IsNullOrEmpty(controlName))
            {
                this.ControlConfig = base.GetControlConfig(controlName);
                AutomationElement element = null;
                try
                {
                    AutomationThreadParams state = new AutomationThreadParams
                    {
                        ControlName = controlName,
                        waitHandle = new ManualResetEvent(false)
                    };
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.FindUIAControlAsync), state);
                    state.waitHandle.WaitOne();
                    if (state.ex != null)
                    {
                        throw state.ex;
                    }
                    element = state.Element;
                }
                catch (DataDrivenAdapterException)
                {
                }
                if ((element == null) || !AutomationEventList.ContainsKey(eventName))
                {
                    return flag;
                }
                Type eventArgsType = AutomationEventList[eventName].EventArgsType;
                if ((eventArgsType.Equals(typeof(AutomationEventArgs)) || eventArgsType.Equals(typeof(WindowClosedEventArgs))) || eventArgsType.Equals(typeof(AsyncContentLoadedEventArgs)))
                {
                    if (this.OnAutomationEventHandler != null)
                    {
                        System.Windows.Automation.Automation.RemoveAutomationEventHandler(AutomationEventList[eventName].Event, element, this.OnAutomationEventHandler);
                    }
                    return flag;
                }
                if (eventArgsType.Equals(typeof(AutomationFocusChangedEventArgs)))
                {
                    if (this.OnAutomationFocusChangedEventHandler != null)
                    {
                        System.Windows.Automation.Automation.RemoveAutomationFocusChangedEventHandler(this.OnAutomationFocusChangedEventHandler);
                    }
                    return flag;
                }
                if (eventArgsType.Equals(typeof(StructureChangedEventArgs)))
                {
                    if (this.OnStructureChangedEventHandler != null)
                    {
                        System.Windows.Automation.Automation.RemoveStructureChangedEventHandler(element, this.OnStructureChangedEventHandler);
                    }
                    return flag;
                }
                if (eventArgsType.Equals(typeof(AutomationPropertyChangedEventArgs)) && (this.OnAutomationPropertyChangedHandler != null))
                {
                    System.Windows.Automation.Automation.RemoveAutomationPropertyChangedEventHandler(element, this.OnAutomationPropertyChangedHandler);
                }
            }
            return flag;
        }
        */
        // Properties
        protected static Dictionary<string, AutomationEventArgsMapping> AutomationEventList
        {
            get; set;
        }

        protected static Dictionary<string, AutomationProperty> AutomationProperties
        {
            get; set;
        }

        protected static Dictionary<string, Type> AutomationPropReturnTypes
        {
            get; set;
        }

        private XmlNode ControlConfig { get; set; }

        private bool IsWebApp { get; set; }

        private IntPtr MainWindowHandle { get; set; }

        private AutomationElement RootAutomationElement { get; set; }
        #endregion
    }
}
