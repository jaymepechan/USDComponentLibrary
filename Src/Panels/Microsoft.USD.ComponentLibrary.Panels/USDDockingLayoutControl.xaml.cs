using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Xml.Linq;
using C1.WPF;
using C1.WPF.Docking;
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility.DataLoader;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility.UserProfileManager;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts;
using System.Windows.Markup;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.Desktop.UI.Controls;
using Microsoft.Xrm.Tooling.Connector;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class USDDockControl : PanelLayoutBase, IComponentConnector
    {
        #region Fields

        private Dictionary<string, Dictionary<string, object>> _userSettings;
        private Dictionary<string, string> _panelLocations; //Any time the MoveToPanel is used will add or update this. 

        #endregion

        #region Constructors

        public USDDockControl()
        {
            InitializeComponent();            
        }

        public USDDockControl(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            this.InitializeComponent();
        }

        #endregion

        #region Overrides

        public override void DesktopLoadingComplete()
        {
            if (_client == null)
            {
                _client = AifServiceContainer.Instance.GetService<ICrmUtilityHostedControl>();
            }
            _userSettings = GetUserSettingsCache();
            base.DesktopLoadingComplete();
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised when an app in panel changes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectedAppChangedHander(object sender, EventArgs e)
        {
            string selectedAppName = String.Empty;
            if (DesktopApplicationUIBase.AppWithFocus != null)
                selectedAppName = DesktopApplicationUIBase.AppWithFocus.ApplicationName;

            if (sender is IUSDPanel)
            {
                string panelName = ((IUSDPanel)sender).Name;
                if (!string.IsNullOrEmpty(panelName))
                    FireEvent("SelectedAppChanged", new Dictionary<string, string>() { { "Panel", panelName }, { "Selection", selectedAppName } });
            }
        }

        private void mainLayout_ViewChanged(object sender, EventArgs e)
        {
            var dock = (C1UsdDockControl)sender;
            if (dock != null)
            {
                //Should always have the 2 dock groups. Only look when count over 2
                for (int i = 2; i < dock.Items.Count; i++)
                {
                    if (dock.Items[i] is C1USDTabBasePanel)
                    {
                        var tabPanel = (C1USDTabBasePanel)dock.Items[i];

                        if (tabPanel.Items.Count > 0 && !((C1USDTabBasePanel)dock.Items[i]).Name.StartsWith("Main Layout"))
                        {
                            var id = Guid.NewGuid().ToString();
                            ((C1USDTabBasePanel)dock.Items[i]).Name = "Main Layout/" + id;

                            var style = this.FindResource("MainPanelC1USDTabBasePanelStyle") as Style;
                            ((C1USDTabBasePanel)dock.Items[i]).Style = style;
                            ((C1USDTabBasePanel)dock.Items[i]).DockWidth = 600;
                            ((C1USDTabBasePanel)dock.Items[i]).DockHeight = 600;
                            ((C1USDTabBasePanel)dock.Items[i]).DockMode = DockMode.Floating;
                            ((C1USDTabBasePanel)dock.Items[i]).ItemsChanged += newFloatingPanel_ItemsChanged;
                            if (desktopFeatureAccess == null)
                                desktopFeatureAccess = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
                            var addPanelResult = desktopFeatureAccess.AddPanel(((C1USDTabBasePanel)dock.Items[i]));
                            if (tabPanel.ApplicationList.Count > 0)
                            {
                                var app = (DynamicsBaseHostedControl)tabPanel.ApplicationList[0];

                                DoPanelRegistration();
                                var pan = desktopFeatureAccess.GetPanel("Main Layout/" + id);

                                if (_panelLocations != null)
                                {
                                    if (_panelLocations.ContainsKey(app.ApplicationName))
                                    {
                                        var previousPanel = _panelLocations[app.ApplicationName];
                                        var thisPanel = ((C1USDTabBasePanel)dock.Items[i]).Name;
                                        var userSetting = GetUserDisplayGroup(app.ApplicationName + "PanelLocation");

                                        //This panel is different than last moved to panel and my saved user setting.
                                        //Update the user panel
                                        if (!thisPanel.Equals(previousPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                            !thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase))
                                        {

                                            var fullPanelName = thisPanel;

                                            UpsertUserSetting(app.ApplicationName + "PanelLocation", thisPanel);
                                        }
                                        else if (previousPanel.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                                 !userSetting.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase))
                                        {
                                            MovePanel(app);
                                        }
                                        else if (thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase) &&
                                                 previousPanel != thisPanel)
                                        {
                                            MovePanel(app);
                                        }
                                    }
                                }
                            }

                            

                        }
                        ////Create new panel and set panel id to userDisplayGroup
                        //var newFloatingPanel = new C1USDTabBasePanel();

                        //var style = this.FindResource("MainPanelC1USDTabBasePanelStyle") as Style;
                        //newFloatingPanel.Name = "Main Layout/" + ((C1USDTabBasePanel)dock.Items[i]).Name;
                        //newFloatingPanel.Style = style;
                        //newFloatingPanel.DockWidth = 600;
                        //newFloatingPanel.DockHeight = 600;
                        //newFloatingPanel.DockMode = DockMode.Floating;
                        //newFloatingPanel.Visibility = Visibility.Visible;
                        //newFloatingPanel.ItemsChanged += newFloatingPanel_ItemsChanged;

                        //for (int x = 0; x < ((C1USDTabBasePanel) dock.Items[i]).Items.Count; x++)
                        //{
                        //    newFloatingPanel.Items.Add(((C1USDTabBasePanel)dock.Items[i]).Items[x]);
                        //}

                        //foreach (var item in ((C1USDTabBasePanel) dock.Items[i]).Items)
                        //{
                            
                        //}

                        //foreach (var item in tabPanel.Items)
                        //{
                            
                            
                        //    if (!_floaingTabs.Contains((C1USDDockTabItem)item))
                        //    {
                        //        _floaingTabs.Add((C1USDDockTabItem)item);
                        //        var app = (DynamicsBaseHostedControl)((C1USDDockTabItem)item).Tag;

                        //        UpsertUserSetting(app.ApplicationName + "PanelLocation", ((C1USDTabBasePanel)dock.Items[i]).Name);
                        //        tabPanel.DockWidth = 800;
                        //        tabPanel.DockHeight = 600;
                        //    }
                        //}
                    }
                }
            }
        }
        private void mainLayout_PickerLoading(object sender, C1.WPF.Docking.PickerLoadingEventArgs e)
        {
            e.ShowLeftInnerPart = false;
            e.ShowLeftOuterPart = false;
            e.ShowTopInnerPart = false;
            e.ShowTopOuterPart = false;
            e.ShowRightInnerPart = false;
            e.ShowRightOuterPart = false;
            e.ShowBottomInnerPart = false;
            e.ShowBottomOuterPart = false;
        }

        private void newFloatingPanel_ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var dock = (C1USDTabBasePanel)sender;
           
            //Use this to keep track of the tabs within the main panel
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (dock.DockMode == DockMode.Hidden)
                    dock.DockMode = DockMode.Floating;
                if (e.NewItems.Count > 0)
                {
                    var newTabItem = (C1USDDockTabItem)e.NewItems[0];
                    var app = (DynamicsBaseHostedControl)newTabItem.Tag;
                    if (app != null)
                    {
                        if (_panelLocations != null)
                        {
                            if (_panelLocations.ContainsKey(app.ApplicationName))
                            {
                                var previousPanel = _panelLocations[app.ApplicationName];
                                var thisPanel = dock.Name.Remove(0,12);
                                var userSetting = GetUserDisplayGroup(app.ApplicationName + "PanelLocation");

                                //This panel is different than last moved to panel and my saved user setting.
                                //Update the user panel
                                if (!thisPanel.Equals(previousPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                    !thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    UpsertUserSetting(app.ApplicationName + "PanelLocation", thisPanel);
                                }
                                else if (previousPanel.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                         !userSetting.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    MovePanel(app);
                                }
                                else if (thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase) &&
                                         previousPanel != thisPanel)
                                {
                                    MovePanel(app);
                                }
                            }
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (dock.Items.Count == 0)
                    dock.DockMode = DockMode.Hidden;
            }
        }

        private void MainPanel_OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var dock = (C1USDDockTabControl)sender;
            
            //Use this to keep track of the tabs within the main panel
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    var newTabItem = (C1USDDockTabItem)e.NewItems[0];
                    var app = (DynamicsBaseHostedControl)newTabItem.Tag;
                    if (app != null)
                    {
                        if (_panelLocations != null)
                        {
                            if (_panelLocations.ContainsKey(app.ApplicationName))
                            {
                                var previousPanel = _panelLocations[app.ApplicationName];
                                var thisPanel = dock.Name;
                                var userSetting = GetUserDisplayGroup(app.ApplicationName + "PanelLocation");

                                //This panel is different than last moved to panel and my saved user setting.
                                //Update the user panel
                                if (!thisPanel.Equals(previousPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                    !thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    var newPanelName = (C1USDDockTabControl)sender;
                                    var fullPanelName = "Main Layout/" + newPanelName.Name;

                                    UpsertUserSetting(app.ApplicationName + "PanelLocation", newPanelName.Name);
                                }
                                else if (previousPanel.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase) &&
                                         !userSetting.Equals(thisPanel, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    MovePanel(app);
                                }
                                else if (thisPanel.Equals(userSetting, StringComparison.InvariantCultureIgnoreCase) &&
                                         previousPanel != thisPanel)
                                {
                                    MovePanel(app);
                                }
                            }
                        }
                        
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var oldTabItem = (C1USDDockTabItem)e.OldItems[0];
                var item = (C1USDDockTabControl)sender;

                Debug.WriteLine(item.DockMode);
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {

            }
        }

        private void LeftPanel2_OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MainPanel_OnItemsChanged(sender, e);
        }

        private void SessionExplorerPanel_OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MainPanel_OnItemsChanged(sender, e);
        }

        private void HiddenBasePanel_OnItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems.Count > 0)
                {
                    var newTabItem = (C1USDDockTabItem)e.NewItems[0];
                    var app = (DynamicsBaseHostedControl)newTabItem.Tag;
                    if (app != null)
                    {
                        MovePanel(app);
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        private string GetUserDisplayGroup(string name)
        {
            var result = string.Empty;
            if (_client == null)
                _client = AifServiceContainer.Instance.GetService<ICrmUtilityHostedControl>();
            string fetchXML = string.Format(USDConfigurationFetchXML.GetCurrentActiveUserSettingByName, name);
            var response = _client.CrmInterface.GetEntityDataByFetchSearch(fetchXML);
            if (response != null)
                result = _client.CrmInterface.GetDataByKeyFromResultsSet<string>(response.First().Value, "msdyusd_settingvalue");
            return result;
        }

        private IDesktopFeatureAccess desktopFeatureAccess;
        private ICRMWindowRouter router;

        private void MovePanel(object tabTag)
        {
            //Move Panel
            var userSettingPanel = string.Empty;
            if (desktopFeatureAccess == null)
                desktopFeatureAccess = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            if (router == null)
                router = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
            var control = ((DynamicsBaseHostedControl)tabTag);
            var defaultPanel = Utility.GetConfigurationValue(control.ApplicationName + "Panel");
            userSettingPanel = GetUserDisplayGroup(control.ApplicationName + "PanelLocation");

            var panelNameToMoveTo = string.Empty;
            
            if (string.IsNullOrEmpty(userSettingPanel))
            {
                if (string.IsNullOrEmpty(defaultPanel))
                    panelNameToMoveTo = "MainPanel"; //Defaults to main panel
                else
                    panelNameToMoveTo = defaultPanel;
            }
            else if (userSettingPanel.Equals("MainPanel") || userSettingPanel.Equals("SessionExplorerPanel") ||
                     userSettingPanel.Equals("LeftPanel2"))
            {
                panelNameToMoveTo = userSettingPanel;
            }
            else
            {
                panelNameToMoveTo = userSettingPanel;
                //Determine if the panel exists. Will be floating panel. If doesn't exist create it.
                //var panel = this.FindName(userSettingPanel);
                var panelExists = false;

                foreach (var item in mainLayout.Items)
                {
                    if (item is C1USDTabBasePanel)
                    {
                        if (((C1USDTabBasePanel) item).Name.Contains(userSettingPanel))
                        {
                            panelExists = true;
                            break;
                        }
                    }
                }

                if (!panelExists)
                {
                    //Create new panel and set panel id to userDisplayGroup
                    var newFloatingPanel = new C1USDTabBasePanel();

                    var style = this.FindResource("MainPanelC1USDTabBasePanelStyle") as Style;
                    newFloatingPanel.Name = "Main Layout/" + panelNameToMoveTo;
                    newFloatingPanel.Style = style;
                    newFloatingPanel.DockWidth = 600;
                    newFloatingPanel.DockHeight = 600;
                    newFloatingPanel.DockMode = DockMode.Floating;
                    newFloatingPanel.Visibility = Visibility.Visible;
                    newFloatingPanel.ItemsChanged += newFloatingPanel_ItemsChanged;

                    var addPanelResult = desktopFeatureAccess.AddPanel(newFloatingPanel);

                    DoPanelRegistration();
                    var pan = desktopFeatureAccess.GetPanel("Main Layout/" + panelNameToMoveTo);

                    var eventInnerParams = new Dictionary<string, string>();
                    eventInnerParams.Add("Panel", "Main Layout/" + panelNameToMoveTo);

                    router.FireEvent(control.localSession, control.ApplicationName, "MoveToPanel", eventInnerParams);
                    mainLayout.UpdateLayout();
                    if (_panelLocations == null)
                        _panelLocations = new Dictionary<string, string>();
                    if (_panelLocations.ContainsKey(control.ApplicationName))
                        _panelLocations[control.ApplicationName] = panelNameToMoveTo;
                    else
                        _panelLocations.Add(control.ApplicationName, panelNameToMoveTo);

                    mainLayout.Items.Add(newFloatingPanel);
                    return;
                }
            }

            //desktopFeatureAccess = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            //desktopFeatureAccess.MoveApplicationToPanel(GetApp(control.ApplicationName), "Main Layout/" + panelNameToMoveTo);
            var eventParams = new Dictionary<string, string>();
            eventParams.Add("Panel", "Main Layout/" + panelNameToMoveTo);

            var eventResult = router.FireEvent(control.localSession, control.ApplicationName, "MoveToPanel", eventParams);
            mainLayout.UpdateLayout();
            if (_panelLocations == null)
                _panelLocations = new Dictionary<string, string>();
            if (_panelLocations.ContainsKey(control.ApplicationName))
                _panelLocations[control.ApplicationName] = panelNameToMoveTo;
            else
                _panelLocations.Add(control.ApplicationName, panelNameToMoveTo);
        }

        /// <summary>
        /// Return true if already exists with same value. Otherwise returns false.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckUserLayoutSetting(string name, string value)
        {
            if (_userSettings == null)
                return false;
            foreach (var setting in _userSettings)
            {
                var nameSetting = _client.CrmInterface.GetDataByKeyFromResultsSet<string>(setting.Value, "msdyusd_name");
                if (nameSetting.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    var valueSetting = _client.CrmInterface.GetDataByKeyFromResultsSet<string>(setting.Value, "msdyusd_settingvalue");
                    if (!string.IsNullOrEmpty(valueSetting) && valueSetting.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private Dictionary<string, Dictionary<string, object>> GetUserSettingsCache()
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            result = _client.CrmInterface.GetEntityDataByFetchSearch(USDConfigurationFetchXML.GetUserSettings);
            return result;
        }

        private void UpsertUserSetting(string name, string value)
        {
            if (CheckUserLayoutSetting(name, value))
            {
                return;
            }
            
            Guid myUserId = _client.CrmInterface.GetMyCrmUserId();
            
            // Create settings data payload. 
            var settingData = new Dictionary<string, CrmDataTypeWrapper>();
            settingData.Add("msdyusd_name", new CrmDataTypeWrapper(name, CrmFieldType.String));
            settingData.Add("msdyusd_settingvalue", new CrmDataTypeWrapper(value, CrmFieldType.String));
            settingData.Add("msdyusd_user", new CrmDataTypeWrapper(myUserId, CrmFieldType.Lookup, "systemuser"));
            
            bool completed = false;
            
            string fetchXML = string.Format(USDConfigurationFetchXML.GetCurrentActiveUserSettingByName, name);
            var response = _client.CrmInterface.GetEntityDataByFetchSearch(fetchXML);
            if (response != null && response.Count > 0)
            {
                // Have value.. do an update.
                // get ID 
                Guid guSettingId = _client.CrmInterface.GetDataByKeyFromResultsSet<Guid>(response.First().Value, "msdyusd_usersettingsid");
                if (guSettingId != Guid.Empty)
                {
                    completed = _client.CrmInterface.UpdateEntity("msdyusd_usersettings", "msdyusd_usersettingsid", guSettingId, settingData);
                }
            }
            else
            {
                // Do not have a value, do a create. 
                Guid guSettingId = _client.CrmInterface.CreateNewRecord("msdyusd_usersettings", settingData);
                if (guSettingId != Guid.Empty)
                    completed = true;
            }
            _userSettings = GetUserSettingsCache();
        }

        private void SaveLayout(Uii.Csr.RequestActionEventArgs args)
        {
            var currentPanelLayout = mainLayout.Save().Root.ToString();
            UpsertUserSetting("C1PanelLayout", currentPanelLayout);
        }

        private void LoadLayout()
        {
            Guid myUserId = _client.CrmInterface.GetMyCrmUserId();

            // Create settings data payload. 

            var completed = false;
            var fetchXML = string.Format(USDConfigurationFetchXML.GetCurrentActiveUserSettingByName, "C1Layout");
            var response = _client.CrmInterface.GetEntityDataByFetchSearch(fetchXML);
            if (response != null && response.Count > 0)
            {
                // Have value.. do an update.
                // get ID 
                var guSettingId = _client.CrmInterface.GetDataByKeyFromResultsSet<Guid>(response.First().Value, "msdyusd_usersettingsid");

                if (guSettingId != Guid.Empty)
                {
                    var layout = _client.CrmInterface.GetDataByKeyFromResultsSet<string>(response.First().Value,
                        "msdyusd_settingvalue");
                    var layoutElement = XElement.Parse(layout);
                    mainLayout.Load(layoutElement);
                }
            }
        }

        private void SetLeftPanelWidth(string width)
        {
            try
            {
                LeftPanel2.DockWidth = Convert.ToDouble(width);
                SessionExplorerPanel.DockWidth = Convert.ToDouble(width);
            }
            catch
            {

            }
        }
        
        #endregion

        #region DoAction

        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            if (args.Action.Equals("SaveLayout", StringComparison.InvariantCultureIgnoreCase))
            {
                //Save to user settings the layout
                //TODO: Save layout to users settings.
                SaveLayout(args);
            }
            else if (args.Action.Equals("SetLeftPanelWidth", StringComparison.InvariantCultureIgnoreCase))
            {
                SetLeftPanelWidth(args.Data);
            }
            else if (args.Action.Equals("LoadLayout", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadLayout();
            }
            else
            {
                base.DoAction(args);
            }
        }
        
        #endregion
    }
}
