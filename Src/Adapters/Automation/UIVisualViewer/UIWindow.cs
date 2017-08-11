/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.USD.ComponentLibrary.Adapters.Automation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation.UIVisualViewer
{
    public class UIWindow
    {
        private AutomationElement _Win;
        private UIWindow _parent;
        public List<string> actions = new List<string>();
        Dictionary<string, string> extraProperties = new Dictionary<string, string>();
        public UIWindow(UIWindow parent, AutomationElement Win)
        {
            _parent = parent;
            if (Win == null)
                return;
            _Win = Win;

            LoadData();
        }

        void LoadData()
        {
            Trace.WriteLine("===== WINDOW CONTROLS =====");
            Trace.WriteLine("CONTROL: " + _Win.Current.ControlType.LocalizedControlType + " NAME: " + _Win.Current.Name);
            Trace.WriteLine("\tPATTERNS:");
            foreach (AutomationPattern ap in _Win.GetSupportedPatterns())
            {
                Trace.WriteLine("\t\tNAME:" + ap.ProgrammaticName + ", ID:" + ap.Id.ToString() + ", PROGNAME:" + System.Windows.Automation.Automation.PatternName(ap));
                switch (System.Windows.Automation.Automation.PatternName(ap).ToLower())
                {
                    case "expandcollapse":
                        {
                            actions.Add("Expand");
                            actions.Add("Collapse");
                            ExpandCollapsePattern pattern = _Win.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                            extraProperties.Add("ExpandCollapseState", pattern.Current.ExpandCollapseState.ToString());
                        }
                        break;
                    case "scroll":
                        {
                            actions.Add("scrollup");
                            actions.Add("scrolldown");
                            actions.Add("scrollleft");
                            actions.Add("scrollright");
                            actions.Add("pageup");
                            actions.Add("pagedown");
                            actions.Add("pageleft");
                            actions.Add("pageright");
                            ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                            extraProperties.Add("ScrollPosition", pattern.Current.ToString());
                        }
                        break;
                    case "toggle":
                        {
                            actions.Add(System.Windows.Automation.Automation.PatternName(ap));
                            TogglePattern pattern = _Win.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                            extraProperties.Add("ToggleState", pattern.Current.ToggleState.ToString());
                        }
                        break;
                    case "text":
                        // not supported currently
                        break;
                    default:
                        actions.Add(System.Windows.Automation.Automation.PatternName(ap));
                        break;
                }
            }
            Trace.WriteLine("");

            TreeWalker walker = TreeWalker.ControlViewWalker;
            AutomationElement elementNode = walker.GetFirstChild(_Win);
            while (elementNode != null)
            {
                _children.Add(new UIWindow(this, elementNode));
                elementNode = walker.GetNextSibling(elementNode);
            }
        }

        List<UIWindow> _children = new List<UIWindow>();
        public IList<UIWindow> Children
        {
            get { return _children; }
        }

        public void Execute(string action, string parameter)
        {
            switch (action.ToLower())
            {
                case "invoke":
                    {
                        InvokePattern pattern = _Win.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        pattern.Invoke();
                    }
                    break;
                case "toggle":
                    {
                        TogglePattern pattern = _Win.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
                        pattern.Toggle();
                    }
                    break;
                case "expand":
                    {
                        ExpandCollapsePattern pattern = _Win.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                        pattern.Expand();
                    }
                    break;
                case "collapse":
                    {
                        ExpandCollapsePattern pattern = _Win.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;
                        pattern.Collapse();
                    }
                    break;
                case "selection":
                    {
                        SelectionPattern pattern = _Win.GetCurrentPattern(SelectionPattern.Pattern) as SelectionPattern;
                        //pattern.();
                    }
                    break;
                case "pageup":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollVertical(ScrollAmount.LargeIncrement);
                    }
                    break;
                case "pagedown":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollVertical(ScrollAmount.LargeDecrement);
                    }
                    break;
                case "pageleft":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollHorizontal(ScrollAmount.LargeDecrement);
                    }
                    break;
                case "pageright":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollHorizontal(ScrollAmount.LargeIncrement);
                    }
                    break;
                case "scrollup":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollVertical(ScrollAmount.SmallIncrement);
                    }
                    break;
                case "scrolldown":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollVertical(ScrollAmount.SmallDecrement);
                    }
                    break;
                case "scrollleft":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollHorizontal(ScrollAmount.SmallDecrement);
                    }
                    break;
                case "scrollright":
                    {
                        ScrollPattern pattern = _Win.GetCurrentPattern(ScrollPattern.Pattern) as ScrollPattern;
                        pattern.ScrollHorizontal(ScrollAmount.SmallIncrement);
                    }
                    break;
                case "value":
                    {
                        ValuePattern pattern = _Win.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                        pattern.SetValue(parameter);
                    }
                    break;
                case "text":
                    {
                        //TextPattern pattern = _Win.GetCurrentPattern(TextPattern.Pattern) as TextPattern;
                        //pattern.(parameter);
                    }
                    break;
            }
        }

        public AutomationElement Element { get { return _Win; } }
        public UIWindow Parent { get { return _parent; } }
        public string Name { get { return _Win.Current.Name; } }
        public string AcceleratorKey { get { return _Win.Current.AcceleratorKey; } }
        public string AccessKey { get { return _Win.Current.AccessKey; } }
        public string AutomationId { get { return _Win.Current.AutomationId; } }
        public string BoundingRectangle { get { return _Win.Current.BoundingRectangle.ToString(); } }
        public string ClassName { get { return _Win.Current.ClassName; } }
        public string ControlTypeName { get { return _Win.Current.ControlType.ProgrammaticName.ToString(); } }
        public string ControlTypeLocalizedControlType { get { return _Win.Current.ControlType.LocalizedControlType.ToString(); } }
        public string ControlTypeId { get { return _Win.Current.ControlType.Id.ToString(); } }
        public string FrameworkId { get { return _Win.Current.FrameworkId; } }
        public string HasKeyboardFocus { get { return _Win.Current.HasKeyboardFocus.ToString(); } }
        public string HelpText { get { return _Win.Current.HelpText; } }
        public string IsContentElement { get { return _Win.Current.IsContentElement.ToString(); } }
        public string IsControlElement { get { return _Win.Current.IsControlElement.ToString(); } }
        public string IsEnabled { get { return _Win.Current.IsEnabled.ToString(); } }
        public string IsKeyboardFocusable { get { return _Win.Current.IsKeyboardFocusable.ToString(); } }
        public string IsOffscreen { get { return _Win.Current.IsOffscreen.ToString(); } }
        public string IsPassword { get { return _Win.Current.IsPassword.ToString() ; } }
        public string IsRequiredForForm { get { return _Win.Current.IsRequiredForForm.ToString(); } }
        public string ItemStatus { get { return _Win.Current.ItemStatus; } }
        public string ItemType { get { return _Win.Current.ItemType; } }
        public string LabeledBy
        {
            get
            {
                if (_Win.Current.LabeledBy != null)
                    return _Win.Current.LabeledBy.Current.AutomationId;
                return "";
            }
        }
        public string LocalizedControlType { get { return _Win.Current.LocalizedControlType.ToString(); } }
        public string NativeWindowHandle { get { return _Win.Current.NativeWindowHandle.ToString(); } }
        public string Orientation { get { return _Win.Current.Orientation.ToString(); } }
        public string ProcessId { get { return _Win.Current.ProcessId.ToString(); } }
        public string DisplayName { get { return "[" + NativeWindowHandle + "] *** Children=" + Children.Count.ToString() + " Name=" + Name; } }
        public string SuggestedXml
        {
            get
            {
                //   <Controls >
                //      <AccControl name = "clickme" >
                //     <Path >
                //     <NextName offset = "0" > Click Me </NextName >
                //        </Path >
                //    </AccControl >
                //    <AccControl name = "textbox" >
                //         <Path >
                //         <NextRole offset = "0" > text </NextRole >
                //          </ Path >
                //      </AccControl >
                //  </Controls >
                StringBuilder sb = new StringBuilder();
                using (XmlWriter xmlwriter = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true, NewLineChars = "\r\n", NewLineHandling = NewLineHandling.Entitize, NewLineOnAttributes = false, Indent = true }))
                {
                    xmlwriter.WriteStartElement("Controls");
                    xmlwriter.WriteStartElement("AccControl");
                    if (!String.IsNullOrEmpty(Name))
                    {
                        xmlwriter.WriteAttributeString("name", Name.Replace(' ', '_'));
                        xmlwriter.WriteStartElement("Path");
                        WritePathElement(xmlwriter, this);
                        xmlwriter.WriteEndElement();
                    }
                    else
                    {
                        xmlwriter.WriteAttributeString("name", "[name]");
                        xmlwriter.WriteStartElement("Path");
                        WritePathElement(xmlwriter, this);
                        xmlwriter.WriteEndElement();
                    }
                    xmlwriter.WriteEndElement();
                    xmlwriter.WriteEndElement();
                    xmlwriter.Close();
                    return sb.ToString();
                }
            }
        }

        private void WritePathElement(XmlWriter xmlwriter, UIWindow uiwin)
        {
            if (uiwin.Parent == null)
                return; // don't create an element for the top level window

            /// Don't use name...
            /// Reason #1: The name of a textbox is the text in the box and changes frequently
            /// Reason #2: While the name of a button may not change, it is definitely not unique and could also change.  For example an OK button might exist all over.
            /// 
            //if (!String.IsNullOrEmpty(javawin.Name))
            //{
            //    if (javawin.Parent != null)
            //        WritePathElement(xmlwriter, javawin.Parent);
            //    xmlwriter.WriteStartElement("NextName");
            //    xmlwriter.WriteAttributeString("offset", "0");
            //    xmlwriter.WriteString(javawin.Name);
            //    xmlwriter.WriteEndElement();
            //}
            //else


            //{
            //    if (uiwin.Parent != null)
            //        WritePathElement(xmlwriter, uiwin.Parent);
            //    xmlwriter.WriteStartElement("NextRole");
            //    xmlwriter.WriteAttributeString("offset", GetOffset(uiwin, uiwin.Role).ToString());
            //    xmlwriter.WriteString(uiwin.Role);
            //    xmlwriter.WriteEndElement();
            //}
        }

        private int GetOffset(UIWindow uiwin, string role)
        {
            //UIWindow parent = uiwin.Parent;
            //if (parent == null)
            //    return 0;
            //int ioffset = 0;
            //for (int i = 0; i < parent.Children.Count(); i++)
            //{
            //    if (parent.Children[i] == uiwin)
            //        return ioffset;
            //    if (parent.Children[i].Role == role)
            //        ioffset++;
            //}
            return 0;
        }
    }
}
