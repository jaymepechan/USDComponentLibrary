using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.AifServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation.UIVisualViewer
{
    /// <summary>
    /// Interaction logic for UIVisualViewer.xaml
    /// </summary>
    public partial class UIVisualViewer : UserControl
    {
        UITreeViewModel _vm;
        UIWindow uiw = null;
        string _applicationName;
        AutomationElement _rootElement;
        public UIVisualViewer(string ApplicationName)
        {
            InitializeComponent();
            _applicationName = ApplicationName;
        }

        public void LoadTree(AutomationElement rootElement)
        {
            _rootElement = rootElement;
            _vm = new UITreeViewModel(new UIWindow(null, rootElement));
            base.DataContext = _vm;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                uiw = (e.NewValue as UIWindowViewModel).Win;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Name:" + uiw.Name);
                sb.AppendLine("ClassName:" + uiw.ClassName);
                sb.AppendLine("ChildrenCount:" + uiw.Children.Count.ToString());
                sb.AppendLine("AcceleratorKey:" + uiw.AcceleratorKey);
                sb.AppendLine("AccessKey:" + uiw.AccessKey);
                sb.AppendLine("AutomationId:" + uiw.AutomationId);
                sb.AppendLine("BoundingRectangle:" + uiw.BoundingRectangle);
                sb.AppendLine("ControlTypeName:" + uiw.ControlTypeName);
                sb.AppendLine("ControlTypeLocalizedControlType:" + uiw.ControlTypeLocalizedControlType);
                sb.AppendLine("ControlTypeId:" + uiw.ControlTypeId);
                sb.AppendLine("FrameworkId:" + uiw.FrameworkId);
                sb.AppendLine("HasKeyboardFocus:" + uiw.HasKeyboardFocus);
                sb.AppendLine("HelpText:" + uiw.HelpText);
                sb.AppendLine("IsContentElement:" + uiw.IsContentElement);
                sb.AppendLine("IsControlElement:" + uiw.IsControlElement);
                sb.AppendLine("IsEnabled:" + uiw.IsEnabled);
                sb.AppendLine("IsKeyboardFocusable:" + uiw.IsKeyboardFocusable);
                sb.AppendLine("IsOffscreen:" + uiw.IsOffscreen);
                sb.AppendLine("IsPassword:" + uiw.IsPassword);
                sb.AppendLine("IsRequiredForForm:" + uiw.IsRequiredForForm);
                sb.AppendLine("ItemStatus:" + uiw.ItemStatus);
                sb.AppendLine("ItemType:" + uiw.ItemType);
                sb.AppendLine("LabeledBy:" + uiw.LabeledBy);
                sb.AppendLine("LocalizedControlType:" + uiw.LocalizedControlType);
                sb.AppendLine("NativeWindowHandle:" + uiw.NativeWindowHandle);
                sb.AppendLine("Orientation:" + uiw.Orientation);
                sb.AppendLine("ProcessId:" + uiw.ProcessId);
                //sb.AppendLine("");
                //sb.AppendLine("");
                //sb.AppendLine("XML");
                //sb.AppendLine("---------");
                //sb.AppendLine(jw.SuggestedXml);
                displayOutput.Text = sb.ToString();
                ddaActionPanel.Items.Clear();
                directActionPanel.Items.Clear();
                foreach (string s in uiw.actions)
                {
                    Button b = new Button();
                    b.Content = s;
                    b.Margin = new Thickness(1);
                    b.Padding = new Thickness(3);
                    b.Click += DDAAction_Click;
                    ddaActionPanel.Items.Add(b);

                    Button b2 = new Button();
                    b2.Content = s;
                    b2.Margin = new Thickness(1);
                    b2.Padding = new Thickness(3);
                    b2.Click += DirectAction_Click;
                    directActionPanel.Items.Add(b2);

                }

            }
            catch
            { }

        }

        private void DDAAction_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            string s = b.Content as string;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(uiw.SuggestedXml);
            XmlNode nodeControl = doc.DocumentElement.FirstChild;
            XmlAttribute x = doc.CreateAttribute("action");
            x.Value = s;
            nodeControl.Attributes.Append(x);
            ActionDefinition ad = new ActionDefinition()
            {
                Application = _applicationName,
                Action = "Execute",
                ActionData = doc.OuterXml,
                Condition = ""
            };
            ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
            CRMWindowRouter.ExecuteActions(new List<ActionDefinition>() { { ad } }, String.Empty, new Dictionary<string, string>());

        }
        private void DirectAction_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            string s = b.Content as string;

            uiw.Execute(s, ParameterInput.Text);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadTree(_rootElement);
        }
    }

    class TreeViewLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TreeViewItem item = (TreeViewItem)value;
            ItemsControl ic = ItemsControl.ItemsControlFromItemContainer(item);
            return ic.ItemContainerGenerator.IndexFromContainer(item) == ic.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return false;
        }
    }

}
