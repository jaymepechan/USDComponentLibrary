/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Uii.Csr;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for FormViewer.xaml
    /// </summary>
    public partial class FormViewer : MicrosoftBase
    {
        public FormViewer(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
            PopulateToolbars(ProgrammableToolbarTray);
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("ShowForm", ShowForm);
            RegisterAction("ReadForm", ReadForm);
            RegisterAction("ClearData", ClearData);
        }

        private void ClearData(RequestActionEventArgs args)
        {
            DynamicsCustomerRecord customerRecord = (DynamicsCustomerRecord)localSession.Customer.DesktopCustomer;
            customerRecord.ClearReplaceableParameter(this.ApplicationName);
        }

        private void ReadForm(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string clear = Utility.GetAndRemoveParameter(lines, "clear");
            bool clearBool = false;
            if (!String.IsNullOrEmpty(clear) && bool.TryParse(clear, out clearBool) && clearBool == true)
            {
                DynamicsCustomerRecord customerRecord = (DynamicsCustomerRecord)localSession.Customer.DesktopCustomer;
                customerRecord.ClearReplaceableParameter(this.ApplicationName);
            }
            ReadForm();
        }

        private void ShowForm(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string formname = Utility.GetAndRemoveParameter(lines, "name");
            try
            {
                ParserContext pc = new ParserContext();
                pc.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                Entity theform = base.CRMWindowRouter.usdForms.Where(a => a.Attributes["msdyusd_name"].ToString().Equals(formname)).FirstOrDefault();
                if (theform != null)
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        string replacedDisplay = Utility.GetContextReplacedString(theform.Attributes["msdyusd_markup"].ToString(), CurrentContext, localSession);
                        if (Utility.IsAllReplacementValuesReplaced(replacedDisplay))
                        {
                            byte[] displayData = System.Text.ASCIIEncoding.Unicode.GetBytes(replacedDisplay);
                            ms.Write(displayData, 0, displayData.Length);
                            ms.Seek(0, System.IO.SeekOrigin.Begin);
                            UIElement displayElement = XamlReader.Load(ms, pc) as UIElement;
                            MainGrid.Children.Add(displayElement);

                            HookupButtonClickEvents();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error in Form Viewer");
                Trace.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void HookupButtonClickEvents()
        {
            List<UIElement> elements = new List<UIElement>();
            foreach (object child in MainGrid.Children)
            {
                if (child is UIElement)
                    elements.AddRange(DiscoverNamedChildren(child as UIElement));
            }
            foreach (UIElement elem in elements)
            {
                if (elem is ButtonBase)
                {
                    ((ButtonBase)elem).Click += AButtonClick_Click;
                }
                if (elem is TextBoxBase)
                {
                    ((TextBoxBase)elem).TextChanged += FormViewer_TextChanged;
                }
            }
        }

        private void FormViewer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBoxBase)
            {
                FireEvent(((TextBoxBase)sender).Name);   // fire event named after button
                FireEvent("TextChanged", new Dictionary<string, string>() { { "Name", ((TextBoxBase)sender).Name } });   
            }
        }

        void AButtonClick_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ButtonBase)
            {
                FireEvent(((ButtonBase)sender).Name);   // fire event named after button
                FireEvent("Click", new Dictionary<string, string>() { { "Name", ((ButtonBase)sender).Name } });   // also fire Click event
            }
        }

        private void ReadForm()
        {
            List<UIElement> elements = new List<UIElement>();
            List<LookupRequestItem> data = new List<LookupRequestItem>();
            foreach (object child in MainGrid.Children)
            {
                if (child is UIElement)
                    elements.AddRange(DiscoverNamedChildren(child as UIElement));
            }
            foreach (UIElement elem in elements)
            {
                if (elem is FrameworkElement)
                {
                    string name = ((FrameworkElement)elem).Name;
                    string value = String.Empty;
                    if (elem is TextBox)
                        value = ((TextBox)elem).Text;
                    else if (elem is ToggleButton && ((ToggleButton)elem).IsChecked != null)
                        value = ((ToggleButton)elem).IsChecked.ToString();
                    else if (elem is Selector && ((Selector)elem).SelectedValue != null)
                        value = ((Selector)elem).SelectedValue.ToString();
                    else if (elem is PasswordBox && ((PasswordBox)elem).Password != null)
                        value = ((PasswordBox)elem).Password;
                    //else if (elem is RichTextBox)
                    //    value = String.Empty;
                    else if (elem is Control)
                        value = elem.ToString();

                    data.Add(new LookupRequestItem(name, value));
                }
            }
            try
            {
                DynamicsCustomerRecord customerRecord = (DynamicsCustomerRecord)localSession.Customer.DesktopCustomer;
                customerRecord.MergeReplacementParameter(this.ApplicationName, data);
            }
            catch
            {
            }
        }

        protected List<UIElement> DiscoverNamedChildren(UIElement child)
        {
            List<UIElement> elements = new List<UIElement>();
            FrameworkElement fe = child as FrameworkElement;
            if (fe != null)
            {
                if (!String.IsNullOrEmpty(((FrameworkElement)child).Name))
                    elements.Add(child);
                try { fe.ApplyTemplate(); }
                catch { }
                int childrenCount = VisualTreeHelper.GetChildrenCount(fe);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child1 = VisualTreeHelper.GetChild(child, i);
                    elements.AddRange(DiscoverNamedChildren(child1 as UIElement));
                }
            }
            ContentControl c = child as ContentControl;
            if (c != null)
            {
                elements.AddRange(DiscoverNamedChildren(c.Content as UIElement));
            }
            ItemsControl ic = child as ItemsControl;
            if (ic != null)
            {
                foreach (Object elem in ic.Items)
                {
                    if (elem is UIElement)
                        elements.AddRange(DiscoverNamedChildren(elem as UIElement));
                }
            }
            return elements;
        }

    }
}
