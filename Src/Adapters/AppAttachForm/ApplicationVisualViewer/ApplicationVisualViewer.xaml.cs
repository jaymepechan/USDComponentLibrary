/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.AifServices;
using Microsoft.USD.ComponentLibrary.Adapters.Java;
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
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer
{
    /// <summary>
    /// Interaction logic for ApplicationVisualViewer.xaml
    /// </summary>
    public partial class ApplicationVisualViewer : UserControl
    {
        JavaTreeViewModel _jtvm;
        JavaWindow jw;
        string _applicationName;
        public ApplicationVisualViewer(string ApplicationName)
        {
            InitializeComponent();
            _applicationName = ApplicationName;
        }

        public void LoadTree(JavaTreeViewModel jtvm)
        {
            _jtvm = jtvm;
            base.DataContext = _jtvm;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                jw = (e.NewValue as JavaWindowViewModel).Win;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Role:" + jw.Role);
                sb.AppendLine("Name:" + jw.Name);
                sb.AppendLine("IndexInParent:" + jw.IndexInParent);
                sb.AppendLine("AccessibleAction:" + jw.AccessibleAction);
                sb.AppendLine("AccessibleComponent:" + jw.AccessibleComponent);
                sb.AppendLine("AccessibleInterfaces:" + jw.AccessibleInterfaces);
                sb.AppendLine("AccessibleSelection:" + jw.AccessibleSelection);
                sb.AppendLine("AccessibleText:" + jw.AccessibleText);
                sb.AppendLine("ChildrenCount:" + jw.ChildrenCount.ToString());
                sb.AppendLine("Description:" + jw.Description);
                sb.AppendLine("X:" + jw.X);
                sb.AppendLine("Y:" + jw.Y);
                sb.AppendLine("Width:" + jw.Width);
                sb.AppendLine("Height:" + jw.Height);
                sb.AppendLine("Role_en_US:" + jw.Role_en_US);
                sb.AppendLine("States:" + jw.States);
                sb.AppendLine("States_en_US:" + jw.States_en_US);
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine("XML");
                sb.AppendLine("---------");
                sb.AppendLine(jw.SuggestedXml);
                displayOutput.Text = sb.ToString();
                ddaActionPanel.Items.Clear();
                foreach (string s in jw.javaactions)
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
                    b2.Click += JABAction_Click;
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
            doc.LoadXml(jw.SuggestedXml);
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
        private void JABAction_Click(object sender, RoutedEventArgs e)
        {
            Button b = e.Source as Button;
            string s = b.Content as string;

            int failure;
            JavaAccHelperMethods.DoAction(jw.JavaWin, out failure, jw.VMId, false, s);
            System.Windows.Forms.Application.DoEvents();
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
