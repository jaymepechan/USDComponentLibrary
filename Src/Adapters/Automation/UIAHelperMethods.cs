using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    static class UIAHelperMethods
    {
        // Fields
        private static Dictionary<string, AutomationEventArgsMapping> AutomationEventList;
        private static Dictionary<string, Type> AutomationPropReturnTypes;
        private static Dictionary<string, ControlType> ControlTypeNames;

        // Methods
        internal static string EscapeXml(string content)
        {
            if (content == null)
            {
                return string.Empty;
            }
            if (content.Contains("&"))
            {
                content = content.Replace("&", "&amp;");
            }
            if (content.Contains("<"))
            {
                content = content.Replace("<", "&lt;");
            }
            if (content.Contains(">"))
            {
                content = content.Replace(">", "&gt;");
            }
            if (content.Contains("'"))
            {
                content = content.Replace("'", "&apos;");
            }
            if (content.Contains("\""))
            {
                content = content.Replace("\"", "&quot;");
            }
            return content;
        }

        internal static Dictionary<string, AutomationProperty> GetAutomationElementIdentifiers()
        {
            if (AutomationPropertiesList == null)
            {
                AutomationPropertiesList = new Dictionary<string, AutomationProperty>();
                AutomationProperty property = null;
                foreach (FieldInfo info in typeof(AutomationElementIdentifiers).GetFields())
                {
                    if (info.FieldType.IsEquivalentTo(typeof(AutomationProperty)))
                    {
                        property = (AutomationProperty)info.GetValue(null);
                        if (property != null)
                        {
                            AutomationPropertiesList.Add(System.Windows.Automation.Automation.PropertyName(property), property);
                        }
                    }
                }
            }
            return AutomationPropertiesList;
        }

        internal static Dictionary<string, AutomationEventArgsMapping> GetAutomationEventList()
        {
            if (AutomationEventList == null)
            {
                AutomationEventList = new Dictionary<string, AutomationEventArgsMapping>();
                AutomationEventList.Add(AutomationElementIdentifiers.AsyncContentLoadedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.AsyncContentLoadedEvent, typeof(AsyncContentLoadedEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.AutomationFocusChangedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.AutomationFocusChangedEvent, typeof(AutomationFocusChangedEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.AutomationPropertyChangedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.AutomationPropertyChangedEvent, typeof(AutomationPropertyChangedEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.LayoutInvalidatedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.LayoutInvalidatedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.MenuClosedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.MenuClosedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.MenuOpenedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.MenuOpenedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.ToolTipClosedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.ToolTipClosedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.ToolTipOpenedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.ToolTipOpenedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.ProgrammaticName, new AutomationEventArgsMapping(SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent.ProgrammaticName, new AutomationEventArgsMapping(SelectionItemPatternIdentifiers.ElementRemovedFromSelectionEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(SelectionItemPatternIdentifiers.ElementSelectedEvent.ProgrammaticName, new AutomationEventArgsMapping(SelectionItemPatternIdentifiers.ElementSelectedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(SelectionPatternIdentifiers.InvalidatedEvent.ProgrammaticName, new AutomationEventArgsMapping(SelectionPatternIdentifiers.InvalidatedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(InvokePatternIdentifiers.InvokedEvent.ProgrammaticName, new AutomationEventArgsMapping(InvokePatternIdentifiers.InvokedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(TextPatternIdentifiers.TextChangedEvent.ProgrammaticName, new AutomationEventArgsMapping(TextPatternIdentifiers.TextChangedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(TextPatternIdentifiers.TextSelectionChangedEvent.ProgrammaticName, new AutomationEventArgsMapping(TextPatternIdentifiers.TextSelectionChangedEvent, typeof(AutomationEventArgs)));
                AutomationEventList.Add(WindowPatternIdentifiers.WindowClosedEvent.ProgrammaticName, new AutomationEventArgsMapping(WindowPatternIdentifiers.WindowClosedEvent, typeof(WindowClosedEventArgs)));
                AutomationEventList.Add(AutomationElementIdentifiers.StructureChangedEvent.ProgrammaticName, new AutomationEventArgsMapping(AutomationElementIdentifiers.StructureChangedEvent, typeof(StructureChangedEventArgs)));
                AutomationEventList.Add(WindowPatternIdentifiers.WindowOpenedEvent.ProgrammaticName, new AutomationEventArgsMapping(WindowPatternIdentifiers.WindowOpenedEvent, typeof(AutomationEventArgs)));
            }
            return AutomationEventList;
        }

        internal static Dictionary<string, Type> GetAutomationReturnTypes()
        {
            if (AutomationPropReturnTypes == null)
            {
                AutomationPropReturnTypes = new Dictionary<string, Type>();
                AutomationPropReturnTypes.Add("AcceleratorKey", typeof(string));
                AutomationPropReturnTypes.Add("AccessKey", typeof(string));
                AutomationPropReturnTypes.Add("AutomationId", typeof(string));
                AutomationPropReturnTypes.Add("BoundingRectangle", typeof(Rect));
                AutomationPropReturnTypes.Add("ClassName", typeof(string));
                AutomationPropReturnTypes.Add("ClickablePoint", typeof(Point));
                AutomationPropReturnTypes.Add("ControlType", typeof(ControlType));
                AutomationPropReturnTypes.Add("FrameworkId", typeof(string));
                AutomationPropReturnTypes.Add("HasKeyboardFocus", typeof(bool));
                AutomationPropReturnTypes.Add("HelpText", typeof(string));
                AutomationPropReturnTypes.Add("IsContentElement", typeof(bool));
                AutomationPropReturnTypes.Add("IsControlElement", typeof(bool));
                AutomationPropReturnTypes.Add("IsDockPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsEnabled", typeof(bool));
                AutomationPropReturnTypes.Add("IsExpandCollapsePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsGridItemPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsGridPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsInvokePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsItemContainerPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsKeyboardFocusable", typeof(bool));
                AutomationPropReturnTypes.Add("IsMultipleViewPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsOffscreen", typeof(bool));
                AutomationPropReturnTypes.Add("IsPassword", typeof(bool));
                AutomationPropReturnTypes.Add("IsRangeValuePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsRequiredForForm", typeof(bool));
                AutomationPropReturnTypes.Add("IsScrollItemPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsScrollPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsSelectionItemPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsSelectionPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsSynchronizedInputPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsTableItemPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsTablePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsTextPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsTogglePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsTransformPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsValuePatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsVirtualizedItemPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("IsWindowPatternAvailable", typeof(bool));
                AutomationPropReturnTypes.Add("ItemStatus", typeof(string));
                AutomationPropReturnTypes.Add("ItemType", typeof(string));
                AutomationPropReturnTypes.Add("LocalizedControlType", typeof(string));
                AutomationPropReturnTypes.Add("Name", typeof(string));
                AutomationPropReturnTypes.Add("Orientation", typeof(OrientationType));
                AutomationPropReturnTypes.Add("ProcessId", typeof(string));
                AutomationPropReturnTypes.Add("RuntimeId", typeof(string));
            }
            return AutomationPropReturnTypes;
        }

        internal static object GetControlType(string BindingText) =>
            ControlTypeNames[BindingText];

        internal static object GetPoint(string BindingText)
        {
            Point point = new Point();
            string[] strArray = BindingText.Split(new char[] { ',' });
            if (strArray.Length == 2)
            {
                double num;
                double naN = double.NaN;
                if (double.TryParse(strArray[0], out num) && double.TryParse(strArray[1], out naN))
                {
                    point = new Point(num, naN);
                }
            }
            return point;
        }

        internal static Rect GetRect(string BindingText)
        {
            Rect rect = new Rect();
            string[] strArray = BindingText.Replace("(", "").Replace(")", "").Split(new char[] { ',' });
            if (strArray.Length == 4)
            {
                double num;
                double num2;
                double num3;
                double naN = double.NaN;
                if ((double.TryParse(strArray[0], out num) && double.TryParse(strArray[1], out num2)) && (double.TryParse(strArray[2], out num3) && double.TryParse(strArray[3], out naN)))
                {
                    rect = new Rect(num, num2, num3, naN);
                }
            }
            return rect;
        }

        internal static XmlAttribute GetXmlAttribute(XmlNode xnPropCondition, string AttributeName)
        {
            XmlAttribute attribute = null;
            if ((xnPropCondition != null) && (xnPropCondition.Attributes[AttributeName] != null))
            {
                attribute = xnPropCondition.Attributes[AttributeName];
            }
            return attribute;
        }

        internal static void ProcessExpandCollapsePattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            if (uiaElement.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out patternObject))
            {
                ExpandCollapseState expandCollapseState = ((ExpandCollapsePattern)patternObject).Current.ExpandCollapseState;
                if ((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty))
                {
                    if (opType == OperationType.ExecuteControlAction)
                    {
                        if (expandCollapseState == ExpandCollapseState.Collapsed)
                        {
                            ((ExpandCollapsePattern)patternObject).Expand();
                        }
                        else
                        {
                            ((ExpandCollapsePattern)patternObject).Collapse();
                        }
                    }
                }
            }
        }

        internal static string ProcessInvokePattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject))
            {
                if (opType == OperationType.ExecuteControlAction)
                {
                    ((InvokePattern)patternObject).Invoke();
                }
            }
            return str;
        }

        internal static string ProcessRangeValuePattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            double result = 0.0;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(RangeValuePattern.Pattern, out patternObject))
            {
                try
                {
                    switch (opType)
                    {
                        case OperationType.GetControlValue:
                            return ((RangeValuePattern)patternObject).Current.Value.ToString(CultureInfo.InvariantCulture);

                        case OperationType.SetControlValue:
                            if (double.TryParse(controlValue, out result))
                            {
                                ((RangeValuePattern)patternObject).SetValue(result);
                            }
                            return str;
                    }
                    return str;
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (InvalidOperationException)
                {
                }
            }
            return str;
        }

        internal static string ProcessScrollPattern(AutomationElement uiaElement, OperationType opType, string controlValue = "2,2")
        {
            object patternObject = null;
            string str = string.Empty;
            ScrollAmount noAmount = ScrollAmount.NoAmount;
            ScrollAmount amount = ScrollAmount.NoAmount;
            string[] strArray = controlValue.Split(new char[] { ',' });
            if (strArray.Length == 2)
            {
                int result = -2147483648;
                if ((int.TryParse(strArray[0], out result) && (result >= 0)) && (result <= 4))
                {
                    noAmount = (ScrollAmount)result;
                }
                if ((int.TryParse(strArray[1], out result) && (result >= 0)) && (result <= 4))
                {
                    noAmount = (ScrollAmount)result;
                }
            }
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(ScrollPattern.Pattern, out patternObject))
            {
                switch (opType)
                {
                    case OperationType.GetControlValue:
                        return (((ScrollPattern)patternObject).Current.HorizontalScrollPercent.ToString(CultureInfo.InvariantCulture) + "," + ((ScrollPattern)patternObject).Current.HorizontalScrollPercent.ToString(CultureInfo.InvariantCulture));

                    case OperationType.SetControlValue:
                        if (((ScrollPattern)patternObject).Current.HorizontallyScrollable)
                        {
                            ((ScrollPattern)patternObject).ScrollHorizontal(noAmount);
                        }
                        if (((ScrollPattern)patternObject).Current.VerticallyScrollable)
                        {
                            ((ScrollPattern)patternObject).ScrollVertical(amount);
                        }
                        return str;
                }
            }
            return str;
        }

        internal static string ProcessSelectionItemPattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(SelectionItemPattern.Pattern, out patternObject))
            {
                switch (opType)
                {
                    case OperationType.GetControlValue:
                        return ((SelectionItemPattern)patternObject).Current.IsSelected.ToString();

                    case OperationType.SetControlValue:
                    case OperationType.ExecuteControlAction:
                        ((SelectionItemPattern)patternObject).Select();
                        return str;
                }
            }
            return str;
        }

        internal static string ProcessSelectionPattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(SelectionPattern.Pattern, out patternObject))
            {
                if (opType != OperationType.GetControlValue)
                {
                    return str;
                }
                AutomationElement[] selection = ((SelectionPattern)patternObject).Current.GetSelection();
                bool canSelectMultiple = ((SelectionPattern)patternObject).Current.CanSelectMultiple;
                if (!canSelectMultiple && (selection.Length == 1))
                {
                    return selection[0].GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty).ToString();
                }
                if (!canSelectMultiple || (selection.Length < 1))
                {
                    return str;
                }
                foreach (AutomationElement element in selection)
                {
                    str = str + element.GetCurrentPropertyValue(AutomationElementIdentifiers.NameProperty) + ";";
                }
            }
            return str;
        }

        internal static string ProcessTextPattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObject))
            {
                switch (opType)
                {
                    case OperationType.GetControlValue:
                        return ((TextPattern)patternObject).DocumentRange.GetText(-1);

                    case OperationType.SetControlValue:
                        return str;

                    case OperationType.ExecuteControlAction:
                        ((TextPattern)patternObject).DocumentRange.Select();
                        return str;
                }
            }
            return str;
        }

        internal static string ProcessTogglePattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(TogglePattern.Pattern, out patternObject))
            {
                switch (opType)
                {
                    case OperationType.GetControlValue:
                        return ((TogglePattern)patternObject).Current.ToggleState.ToString();

                    case OperationType.SetControlValue:
                        return str;

                    case OperationType.ExecuteControlAction:
                        ((TogglePattern)patternObject).Toggle();
                        return str;
                }
            }
            return str;
        }

        internal static string ProcessValuePattern(AutomationElement uiaElement, OperationType opType, string controlValue = "")
        {
            object patternObject = null;
            string str = string.Empty;
            if (((bool)uiaElement.GetCurrentPropertyValue(AutomationElementIdentifiers.IsEnabledProperty)) && uiaElement.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject))
            {
                switch (opType)
                {
                    case OperationType.GetControlValue:
                        return ((ValuePattern)patternObject).Current.Value;

                    case OperationType.SetControlValue:
                        ((ValuePattern)patternObject).SetValue(controlValue);
                        return str;
                }
            }
            return str;
        }

        internal static string UnEscapeXml(string content)
        {
            if (content == null)
            {
                return string.Empty;
            }
            if (content.Contains("&amp;"))
            {
                content = content.Replace("&amp;", "&");
            }
            if (content.Contains("&lt;"))
            {
                content = content.Replace("&lt;", "<");
            }
            if (content.Contains("&gt;"))
            {
                content = content.Replace("&gt;", ">");
            }
            if (content.Contains("&apos;"))
            {
                content = content.Replace("&apos;", "'");
            }
            if (content.Contains("&quot;"))
            {
                content = content.Replace("&quot;", "\"");
            }
            return content;
        }

        internal static void UpdateControlTypeNames()
        {
            if (ControlTypeNames == null)
            {
                ControlTypeNames = new Dictionary<string, ControlType>();
                ControlType type = null;
                foreach (FieldInfo info in typeof(ControlType).GetFields())
                {
                    if (info.FieldType.IsEquivalentTo(typeof(ControlType)))
                    {
                        type = (ControlType)info.GetValue(null);
                        if (type != null)
                        {
                            ControlTypeNames.Add(type.ProgrammaticName, type);
                        }
                    }
                }
            }
        }

        // Properties
        private static Dictionary<string, AutomationProperty> AutomationPropertiesList
        {
            get;set;
        }
    }
}
