using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Xrm.Tooling.Ui.Styles;

namespace Microsoft.USD.ComponentLibrary.Adapters.Notifications
{
    /// <summary>
    /// Interaction logic for OutlookNotification.xaml
    /// </summary>
    public partial class OutlookNotification : Window
    {
        private Notification notificationControl;
        ObservableCollection<Notification.NotificationItem> reminders;

        public OutlookNotification(Notification notificationControl, ObservableCollection<Notification.NotificationItem> reminders
            , string snoozeText, string dismissallText, string dismissText, string openitemText, string clicksnoozeText, string dueText, string subjectText)
        {
            InitializeComponent();
            this.notificationControl = notificationControl;
            this.reminders = reminders;
            ReminderList.ItemsSource = this.reminders;

            SubjectColumn.Content = subjectText;
            DueColumn.Content = dueText;
            OpenItemBtn.Content = openitemText;
            DismissItem.Content = dismissText;
            ClickSnoozeText.Text = clicksnoozeText;
            SnoozeBtn.Content = snoozeText;
            DismissAllBtn.Content = dismissallText;
        }

        private void DismissAll_Click(object sender, RoutedEventArgs e)
        {
            notificationControl.DismissAll();
        }

        private void OpenItem_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderList.SelectedItem == null)
                return;
            notificationControl.OpenItem(ReminderList.SelectedItem as Notification.NotificationItem);
        }

        private void Dismiss_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderList.SelectedItem == null)
                return;
            notificationControl.Dismiss(ReminderList.SelectedItem as Notification.NotificationItem);
        }

        private void Snooze_Click(object sender, RoutedEventArgs e)
        {
            if (ReminderList.SelectedItem == null)
                return;
            TimeSpan ts = new TimeSpan(0, int.Parse(((ComboBoxItem)SnoozeCombo.SelectedItem).Tag as string), 0);
            notificationControl.Snooze(DateTime.Now + ts, ReminderList.SelectedItem as Notification.NotificationItem);
        }

        private void ReminderList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            notificationControl.OpenItem(ReminderList.SelectedItem as Notification.NotificationItem);
        }

        private void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null && headerClicked.Role != GridViewColumnHeaderRole.Padding)
            {
                this.SetSort(headerClicked);
            }
        }

        /// <summary>
        /// Handler that gets called when reminder list is loaded.
        /// Sorts by dueColumn.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewReminderList_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetSort((GridViewColumnHeader)dueColumn.Header);
        }

        /// <summary>
        /// Sorts the grid view column
        /// </summary>
        /// <param name="column"></param>
        private void SetSort(GridViewColumnHeader column)
        {
            try
            {
                if (column != null)
                {
                    String field = column.Tag as String;

                    // Clear the previous data.
                    if (currentSortCol != null)
                    {
                        AdornerLayer.GetAdornerLayer(currentSortCol).Remove(currentAdorner);
                        ReminderList.Items.SortDescriptions.Clear();
                    }

                    ListSortDirection newDir = ListSortDirection.Ascending;
                    if (currentSortCol == column && currentAdorner.Direction == newDir)
                    {
                        newDir = ListSortDirection.Descending;
                    }

                    currentSortCol = column;
                    currentAdorner = new SortAdorner(currentSortCol, newDir);
                    ReminderList.Items.SortDescriptions.Add(new SortDescription(field, newDir));

                    // Check to see if the adorner
                    var Layer = AdornerLayer.GetAdornerLayer(currentSortCol);
                    if (Layer != null)
                    {
                        Layer.Add(currentAdorner);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private GridViewColumnHeader currentSortCol = null;
        private SortAdorner currentAdorner = null;
    }
}
