using System;
using System.Windows;
using C1.WPF;
using C1.WPF.Docking;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    public class C1USDDockTabControl : C1DockTabControl
    {
        public C1USDDockTabControl()
        {
            
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                base.OnItemsChanged(e);
                HasOneItem = (Items.Count == 1);
            }
            catch
            {
            }
        }

        public bool HasOneItem
        {
            get { return (bool)GetValue(HasOneItemProperty); }
            set { SetValue(HasOneItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasOneItemProperty =
            DependencyProperty.Register("HasOneItem", typeof(bool), typeof(C1USDDockTabControl), new UIPropertyMetadata(false));
    }
}
