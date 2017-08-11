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
using Microsoft.Uii.Csr;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System.Globalization;
using Microsoft.Win32;
using System.Management;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for UpgradeUSD.xaml
    /// </summary>
    public partial class SystemRequirements : MicrosoftBase
    {
        public SystemRequirements(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
            PopulateToolbars(ProgrammableToolbarTray);
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("CheckUSDVersion", CheckUSDVersion);
            RegisterAction("CheckIESettings", CheckIESettings);
            RegisterAction("ManagementInfo", ManagementInfo);
        }

        private void ManagementInfo(RequestActionEventArgs args)
        {
            // --- Check CPU Cores
            //Win32_1394Controller, Win32_1394ControllerDevice
            //Win32_Account
            //Win32_AccountSID
            //Win32_ACE
            //Win32_ActionCheck
            //Win32_AllocatedResource
            //Win32_ApplicationCommandLine
            //Win32_ApplicationService
            //Win32_AssociatedBattery
            //Win32_AssociatedProcessorMemory
            //Win32_BaseBoard
            //Win32_BaseService
            //Win32_Battery
            //Win32_Binary
            //Win32_BindImageAction
            //Win32_BIOS
            //Win32_BootConfiguration
            //Win32_Bus
            //Win32_CacheMemory
            //Win32_CDROMDrive
            //Win32_CheckCheck
            //Win32_CIMLogicalDeviceCIMDataFile
            //Win32_ClassicCOMApplicationClasses
            //Win32_ClassicCOMClass
            //Win32_ClassicCOMClassSetting
            //Win32_ClassicCOMClassSettings
            //Win32_ClassInfoAction
            //Win32_ClientApplicationSetting
            //Win32_CodecFile
            //Win32_COMApplication
            //Win32_COMApplicationClasses
            //Win32_COMApplicationSettings
            //Win32_COMClass
            //Win32_ComClassAutoEmulator
            //Win32_ComClassEmulator
            //Win32_CommandLineAccess
            //Win32_ComponentCategory
            //Win32_ComputerSystem
            //Win32_ComputerSystemProcessor
            //Win32_ComputerSystemProduct
            //Win32_COMSetting
            //Win32_Condition
            //Win32_CreateFolderAction
            //Win32_CurrentProbe
            //Win32_DCOMApplication
            //Win32_DCOMApplicationAccessAllowedSetting
            //Win32_DCOMApplicationLaunchAllowedSetting
            //Win32_DCOMApplicationSetting
            //Win32_DependentService
            //Win32_Desktop
            //Win32_DesktopMonitor
            //Win32_DeviceBus
            //Win32_DeviceMemoryAddress
            //Win32_DeviceSettings
            //Win32_Directory
            //Win32_DirectorySpecification
            //Win32_DiskDrive
            //Win32_DiskDriveToDiskPartition
            //Win32_DiskPartition
            //Win32_DisplayConfiguration
            //Win32_DisplayControllerConfiguration
            //Win32_DMAChannel
            //Win32_DriverVXD
            //Win32_DuplicateFileAction
            //Win32_Environment
            //Win32_EnvironmentSpecification
            //Win32_ExtensionInfoAction
            //Win32_Fan
            //Win32_FileSpecification
            //Win32_FloppyController
            //Win32_FloppyDrive
            //Win32_FontInfoAction
            //Win32_Group
            //Win32_GroupUser
            //Win32_HeatPipe
            //Win32_IDEController
            //Win32_IDEControllerDevice
            //Win32_ImplementedCategory
            //Win32_InfraredDevice
            //Win32_IniFileSpecification
            //Win32_InstalledSoftwareElement
            //Win32_IRQResource
            //Win32_Keyboard
            //Win32_LaunchCondition
            //Win32_LoadOrderGroup
            //Win32_LoadOrderGroupServiceDependencies
            //Win32_LoadOrderGroupServiceMembers
            //Win32_LogicalDisk
            //Win32_LogicalDiskRootDirectory
            //Win32_LogicalDiskToPartition
            //Win32_LogicalFileAccess
            //Win32_LogicalFileAuditing
            //Win32_LogicalFileGroup
            //Win32_LogicalFileOwner
            //Win32_LogicalFileSecuritySetting
            //Win32_LogicalMemoryConfiguration
            //Win32_LogicalProgramGroup
            //Win32_LogicalProgramGroupDirectory
            //Win32_LogicalProgramGroupItem
            //Win32_LogicalProgramGroupItemDataFile
            //Win32_LogicalShareAccess
            //Win32_LogicalShareAuditing
            //Win32_LogicalShareSecuritySetting
            //Win32_ManagedSystemElementResource
            //Win32_MemoryArray
            //Win32_MemoryArrayLocation
            //Win32_MemoryDevice
            //Win32_MemoryDeviceArray
            //Win32_MemoryDeviceLocation
            //Win32_MethodParameterClass
            //Win32_MIMEInfoAction
            //Win32_MotherboardDevice
            //Win32_MoveFileAction
            //Win32_MSIResource
            //Win32_NetworkAdapter
            //Win32_NetworkAdapterConfiguration
            //Win32_NetworkAdapterSetting
            //Win32_NetworkClient
            //Win32_NetworkConnection
            //Win32_NetworkLoginProfile
            //Win32_NetworkProtocol
            //Win32_NTEventlogFile
            //Win32_NTLogEvent
            //Win32_NTLogEventComputer
            //Win32_NTLogEventLog
            //Win32_NTLogEventUser
            //Win32_ODBCAttribute
            //Win32_ODBCDataSourceAttribute
            //Win32_ODBCDataSourceSpecification
            //Win32_ODBCDriverAttribute
            //Win32_ODBCDriverSoftwareElement
            //Win32_ODBCDriverSpecification
            //Win32_ODBCSourceAttribute
            //Win32_ODBCTranslatorSpecification
            //Win32_OnBoardDevice
            //Win32_OperatingSystem
            //Win32_OperatingSystemQFE
            //Win32_OSRecoveryConfiguration
            //Win32_PageFile
            //Win32_PageFileElementSetting
            //Win32_PageFileSetting
            //Win32_PageFileUsage
            //Win32_ParallelPort
            //Win32_Patch
            //Win32_PatchFile
            //Win32_PatchPackage
            //Win32_PCMCIAController
            //Win32_Perf
            //Win32_PerfRawData
            //Win32_PerfRawData_ASP_ActiveServerPages
            //Win32_PerfRawData_ASPNET_114322_ASPNETAppsv114322
            //Win32_PerfRawData_ASPNET_114322_ASPNETv114322
            //Win32_PerfRawData_ASPNET_ASPNET
            //Win32_PerfRawData_ASPNET_ASPNETApplications
            //Win32_PerfRawData_IAS_IASAccountingClients
            //Win32_PerfRawData_IAS_IASAccountingServer
            //Win32_PerfRawData_IAS_IASAuthenticationClients
            //Win32_PerfRawData_IAS_IASAuthenticationServer
            //Win32_PerfRawData_InetInfo_InternetInformationServicesGlobal
            //Win32_PerfRawData_MSDTC_DistributedTransactionCoordinator
            //Win32_PerfRawData_MSFTPSVC_FTPService
            //Win32_PerfRawData_MSSQLSERVER_SQLServerAccessMethods
            //Win32_PerfRawData_MSSQLSERVER_SQLServerBackupDevice
            //Win32_PerfRawData_MSSQLSERVER_SQLServerBufferManager
            //Win32_PerfRawData_MSSQLSERVER_SQLServerBufferPartition
            //Win32_PerfRawData_MSSQLSERVER_SQLServerCacheManager
            //Win32_PerfRawData_MSSQLSERVER_SQLServerDatabases
            //Win32_PerfRawData_MSSQLSERVER_SQLServerGeneralStatistics
            //Win32_PerfRawData_MSSQLSERVER_SQLServerLatches
            //Win32_PerfRawData_MSSQLSERVER_SQLServerLocks
            //Win32_PerfRawData_MSSQLSERVER_SQLServerMemoryManager
            //Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationAgents
            //Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationDist
            //Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationLogreader
            //Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationMerge
            //Win32_PerfRawData_MSSQLSERVER_SQLServerReplicationSnapshot
            //Win32_PerfRawData_MSSQLSERVER_SQLServerSQLStatistics
            //Win32_PerfRawData_MSSQLSERVER_SQLServerUserSettable
            //Win32_PerfRawData_NETFramework_NETCLRExceptions
            //Win32_PerfRawData_NETFramework_NETCLRInterop
            //Win32_PerfRawData_NETFramework_NETCLRJit
            //Win32_PerfRawData_NETFramework_NETCLRLoading
            //Win32_PerfRawData_NETFramework_NETCLRLocksAndThreads
            //Win32_PerfRawData_NETFramework_NETCLRMemory
            //Win32_PerfRawData_NETFramework_NETCLRRemoting
            //Win32_PerfRawData_NETFramework_NETCLRSecurity
            //Win32_PerfRawData_Outlook_Outlook
            //Win32_PerfRawData_PerfDisk_PhysicalDisk
            //Win32_PerfRawData_PerfNet_Browser
            //Win32_PerfRawData_PerfNet_Redirector
            //Win32_PerfRawData_PerfNet_Server
            //Win32_PerfRawData_PerfNet_ServerWorkQueues
            //Win32_PerfRawData_PerfOS_Cache
            //Win32_PerfRawData_PerfOS_Memory
            //Win32_PerfRawData_PerfOS_Objects
            //Win32_PerfRawData_PerfOS_PagingFile
            //Win32_PerfRawData_PerfOS_Processor
            //Win32_PerfRawData_PerfOS_System
            //Win32_PerfRawData_PerfProc_FullImage_Costly
            //Win32_PerfRawData_PerfProc_Image_Costly
            //Win32_PerfRawData_PerfProc_JobObject
            //Win32_PerfRawData_PerfProc_JobObjectDetails
            //Win32_PerfRawData_PerfProc_Process
            //Win32_PerfRawData_PerfProc_ProcessAddressSpace_Costly
            //Win32_PerfRawData_PerfProc_Thread
            //Win32_PerfRawData_PerfProc_ThreadDetails_Costly
            //Win32_PerfRawData_RemoteAccess_RASPort
            //Win32_PerfRawData_RemoteAccess_RASTotal
            //Win32_PerfRawData_RSVP_ACSPerRSVPService
            //Win32_PerfRawData_Spooler_PrintQueue
            //Win32_PerfRawData_TapiSrv_Telephony
            //Win32_PerfRawData_Tcpip_ICMP
            //Win32_PerfRawData_Tcpip_IP
            //Win32_PerfRawData_Tcpip_NBTConnection
            //Win32_PerfRawData_Tcpip_NetworkInterface
            //Win32_PerfRawData_Tcpip_TCP
            //Win32_PerfRawData_Tcpip_UDP
            //Win32_PerfRawData_W3SVC_WebService
            //Win32_PhysicalMemory
            //Win32_PhysicalMemoryArray
            //Win32_PhysicalMemoryLocation
            //Win32_PNPAllocatedResource
            //Win32_PnPDevice
            //Win32_PnPEntity
            //Win32_PointingDevice
            //Win32_PortableBattery
            //Win32_PortConnector
            //Win32_PortResource
            //Win32_POTSModem
            //Win32_POTSModemToSerialPort
            //Win32_PowerManagementEvent
            //Win32_Printer
            //Win32_PrinterConfiguration
            //Win32_PrinterController
            //Win32_PrinterDriverDll
            //Win32_PrinterSetting
            //Win32_PrinterShare
            //Win32_PrintJob
            //Win32_PrivilegesStatus
            //Win32_Process
            //Win32_Processor
            //Win32_ProcessStartup
            //Win32_Product
            //Win32_ProductCheck
            //Win32_ProductResource
            //Win32_ProductSoftwareFeatures
            //Win32_ProgIDSpecification
            //Win32_ProgramGroup
            //Win32_ProgramGroupContents
            //Win32_ProgramGroupOrItem
            //Win32_Property
            //Win32_ProtocolBinding
            //Win32_PublishComponentAction
            //Win32_QuickFixEngineering
            //Win32_Refrigeration
            //Win32_Registry
            //Win32_RegistryAction
            //Win32_RemoveFileAction
            //Win32_RemoveIniAction
            //Win32_ReserveCost
            //Win32_ScheduledJob
            //Win32_SCSIController
            //Win32_SCSIControllerDevice
            //Win32_SecurityDescriptor
            //Win32_SecuritySetting
            //Win32_SecuritySettingAccess
            //Win32_SecuritySettingAuditing
            //Win32_SecuritySettingGroup
            //Win32_SecuritySettingOfLogicalFile
            //Win32_SecuritySettingOfLogicalShare
            //Win32_SecuritySettingOfObject
            //Win32_SecuritySettingOwner
            //Win32_SelfRegModuleAction
            //Win32_SerialPort
            //Win32_SerialPortConfiguration
            //Win32_SerialPortSetting
            //Win32_Service
            //Win32_ServiceControl
            //Win32_ServiceSpecification
            //Win32_ServiceSpecificationService
            //Win32_SettingCheck
            //Win32_Share
            //Win32_ShareToDirectory
            //Win32_ShortcutAction
            //Win32_ShortcutFile
            //Win32_ShortcutSAP
            //Win32_SID
            //Win32_SMBIOSMemory
            //Win32_SoftwareElement
            //Win32_SoftwareElementAction
            //Win32_SoftwareElementCheck
            //Win32_SoftwareElementCondition
            //Win32_SoftwareElementResource
            //Win32_SoftwareFeature
            //Win32_SoftwareFeatureAction
            //Win32_SoftwareFeatureCheck
            //Win32_SoftwareFeatureParent
            //Win32_SoftwareFeatureSoftwareElements
            //Win32_SoundDevice
            //Win32_StartupCommand
            //Win32_SubDirectory
            //Win32_SystemAccount
            //Win32_SystemBIOS
            //Win32_SystemBootConfiguration
            //Win32_SystemDesktop
            //Win32_SystemDevices
            //Win32_SystemDriver
            //Win32_SystemDriverPNPEntity
            //Win32_SystemEnclosure
            //Win32_SystemLoadOrderGroups
            //Win32_SystemLogicalMemoryConfiguration
            //Win32_SystemMemoryResource
            //Win32_SystemNetworkConnections
            //Win32_SystemOperatingSystem
            //Win32_SystemPartitions
            //Win32_SystemProcesses
            //Win32_SystemProgramGroups
            //Win32_SystemResources
            //Win32_SystemServices
            //Win32_SystemSetting
            //Win32_SystemSlot
            //Win32_SystemSystemDriver
            //Win32_SystemTimeZone
            //Win32_SystemUsers
            //Win32_TapeDrive
            //Win32_TemperatureProbe
            //Win32_Thread
            //Win32_TimeZone
            //Win32_Trustee
            //Win32_TypeLibraryAction
            //Win32_UninterruptiblePowerSupply
            //Win32_USBController
            //Win32_USBControllerDevice
            //Win32_UserAccount
            //Win32_UserDesktop
            //Win32_VideoConfiguration
            //Win32_VideoController
            //Win32_VideoSettings
            //Win32_VoltageProbe
            //Win32_WMIElementSetting
            //Win32_WMISetting
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string key = Utility.GetAndRemoveParameter(lines, "Key").Trim();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + key);


            foreach (ManagementObject share in searcher.Get())
            {
                // Some Codes ...
                foreach (PropertyData PC in share.Properties)
                {
                    //some codes ...
                    string val = "";
                    if (PC.Value != null)
                        val = PC.Value.ToString();
                    Trace.WriteLine(PC.Name.ToString() + " = " + val);
                }
            }
        }

        private void CheckIESettings(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            bool updateSetting = false;
            bool.TryParse(Utility.GetAndRemoveParameter(lines, "UpdateSettings"), out updateSetting);

            string TabProcGrowth;
            TabProcGrowth = Utility.GetAndRemoveParameter(lines, "TabProcGrowth");
            if (String.IsNullOrEmpty(TabProcGrowth))
                TabProcGrowth = "large";

            int TabShutdownDelay;
            string TabShutdownDelayString = Utility.GetAndRemoveParameter(lines, "TabShutdownDelay");
            if (String.IsNullOrEmpty(TabShutdownDelayString))
                TabShutdownDelayString = "0";
            int.TryParse(TabShutdownDelayString, out TabShutdownDelay);

            int MaxConnectionsPerServer;
            string MaxConnectionsPerServerString = Utility.GetAndRemoveParameter(lines, "MaxConnectionsPerServer");
            if (String.IsNullOrEmpty(MaxConnectionsPerServerString))
                MaxConnectionsPerServerString = "10";
            int.TryParse(MaxConnectionsPerServerString, out MaxConnectionsPerServer);

            Dictionary<string, string> results = new Dictionary<string, string>();

            try
            {
                using (RegistryKey regDynamics = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", false))
                {
                    object objTemp = regDynamics.GetValue("MaxConnectionsPerServer");
                    int maxConnectionsPerServer = -1;
                    if (objTemp != null)
                        maxConnectionsPerServer = (int)regDynamics.GetValue("MaxConnectionsPerServer");
                    if (maxConnectionsPerServer != MaxConnectionsPerServer)
                    {
                        if (updateSetting)
                        {
                            using (RegistryKey regDynamicsUpdate = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true))
                                regDynamicsUpdate.SetValue("MaxConnectionsPerServer", MaxConnectionsPerServer, RegistryValueKind.DWord);
                            results.Add("MaxConnectionsPerServer", "Updated to " + MaxConnectionsPerServer.ToString());
                        }
                        else
                        {
                            results.Add("MaxConnectionsPerServer", "Not Updated " + MaxConnectionsPerServer.ToString());
                        }
                    }
                    else
                    {
                        results.Add("MaxConnectionsPerServer", "OK");
                    }
                }
            }
            catch (Exception ex)
            {  
                Dictionary<string, string> errorParams = new Dictionary<string, string>();
                errorParams.Add("Message", ex.Message);
                errorParams.Add("Value", "MaxConnectionsPerServer");
                FireEvent("Error", errorParams);
            }

            //HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main
            try
            {
                using (RegistryKey regMain = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", false))
                {
                    object tabProcGrowth = regMain.GetValue("TabProcGrowth");
                    if (tabProcGrowth == null)
                        tabProcGrowth = String.Empty;
                    int tabProcGrowthInt = 0;
                    if (tabProcGrowth is int)
                    {
                        tabProcGrowthInt = (int)tabProcGrowth;
                        if (tabProcGrowthInt.ToString() != TabProcGrowth)
                        {
                            if (updateSetting)
                            {
                                using (RegistryKey regMainWrite = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true))
                                {
                                    int TabProcGrowthInt = 0;
                                    if (int.TryParse(TabProcGrowth, out TabProcGrowthInt))
                                        regMainWrite.SetValue("TabProcGrowth", TabProcGrowthInt, RegistryValueKind.DWord);
                                    else 
                                        regMainWrite.SetValue("TabProcGrowth", TabProcGrowth, RegistryValueKind.String);
                                    results.Add("TabProcGrowth", "Updated to " + TabProcGrowth);
                                }
                            }
                            else
                            {
                                results.Add("TabProcGrowth", "Not Updated " + TabProcGrowth);
                            }
                        }
                        else
                        {
                            results.Add("TabProcGrowth", "OK");
                        }
                    }
                    else if (tabProcGrowth is string)
                    {
                        if ((string)tabProcGrowth != TabProcGrowth)
                        {
                            if (updateSetting)
                            {
                                using (RegistryKey regMainWrite = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true))
                                {
                                    try { regMainWrite.DeleteValue("TabProcGrowth"); } catch { }
                                    regMainWrite.SetValue("TabProcGrowth", TabProcGrowth, RegistryValueKind.String);
                                    results.Add("TabProcGrowth", "Updated to " + TabProcGrowth);
                                }
                            }
                            else
                            {
                                results.Add("TabProcGrowth", "Not Updated " + TabProcGrowth);
                            }
                        }
                        else
                        {
                            results.Add("TabProcGrowth", "OK");
                        }
                    }
                }
            }
            catch (Exception ex)
            {   // ignore
                Dictionary<string, string> errorParams = new Dictionary<string, string>();
                errorParams.Add("Message", ex.Message);
                errorParams.Add("Value", "TabProcGrowth");
                FireEvent("Error", errorParams);
            }

            //HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main
            try
            {
                using (RegistryKey regMain = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", false))
                {
                    if ((int)regMain.GetValue("TabShutdownDelay") != TabShutdownDelay)
                    {
                        if (updateSetting)
                        {
                            using (RegistryKey regMainWrite = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\Main", true))
                            {
                                regMainWrite.SetValue("TabShutdownDelay", TabShutdownDelay, RegistryValueKind.DWord);
                                results.Add("TabShutdownDelay", "Updated to " + TabShutdownDelay);
                            }
                        }
                        else
                        {
                            results.Add("TabShutdownDelay", "Not Updated " + TabShutdownDelay);
                        }
                    }
                    else
                    {
                        results.Add("TabShutdownDelay", "OK");
                    }
                }
            }
            catch (Exception ex)
            {   // ignore
                Dictionary<string, string> errorParams = new Dictionary<string, string>();
                errorParams.Add("Message", ex.Message);
                errorParams.Add("Value", "TabShutdownDelay");
                FireEvent("Error", errorParams);
            }

            //Computer\HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\2

            //         Value Setting
            //------------------------------
            //0        My Computer
            //1        Local Intranet Zone
            //2        Trusted sites Zone
            //3        Internet Zone
            //4        Restricted Sites Zone

            //            Value Setting
            //   ----------------------------------------------------------------------------------
            //   1001     ActiveX controls and plug-ins: Download signed ActiveX controls
            //   1004     ActiveX controls and plug-ins: Download unsigned ActiveX controls
            //   1200     ActiveX controls and plug-ins: Run ActiveX controls and plug - ins
            //   1201     ActiveX controls and plug-ins: Initialize and script ActiveX controls not marked as safe for scripting
            //   1206     Miscellaneous: Allow scripting of Internet Explorer Web browser control ^
            //   1207     Reserved #
            //   1208     ActiveX controls and plug - ins: Allow previously unused ActiveX controls to run without prompt ^
            //   1209     ActiveX controls and plug - ins: Allow Scriptlets
            //   120A     ActiveX controls and plug - ins:
            // ActiveX controls and plug - ins: Override Per - Site(domain - based) ActiveX restrictions
            //   120B     ActiveX controls and plug - ins: Override Per - Site(domain - based) ActiveX restrict
            //ions
            //   1400     Scripting: Active scripting
            //   1402     Scripting: Scripting of Java applets
            //   1405     ActiveX controls and plug - ins: Script ActiveX controls marked as safe for scripting

            //     1406     Miscellaneous: Access data sources across domains

            //     1407     Scripting: Allow Programmatic clipboard access

            //     1408     Reserved #
            //   1409     Scripting: Enable XSS Filter

            //     1601     Miscellaneous: Submit non - encrypted form data

            //     1604     Downloads: Font download

            //     1605     Run Java #
            //   1606     Miscellaneous: Userdata persistence ^
            //     1607     Miscellaneous: Navigate sub - frames across different domains

            //     1608     Miscellaneous: Allow META REFRESH * ^
            //     1609     Miscellaneous: Display mixed content *
            //     160A     Miscellaneous: Include local directory path when uploading files to a server ^
            //     1800     Miscellaneous: Installation of desktop items

            //     1802     Miscellaneous: Drag and drop or copy and paste files

            //     1803     Downloads: File Download ^
            //     1804     Miscellaneous: Launching programs and files in an IFRAME

            //     1805     Launching programs and files in webview #
            //   1806     Miscellaneous: Launching applications and unsafe files
            //   1807     Reserved * * #
            //   1808     Reserved * * #
            //   1809     Miscellaneous: Use Pop-up Blocker * * ^
            //   180A Reserved # 
            //   180B Reserved #
            //   180C Reserved #
            //   180D     Reserved #
            //   180E     Allow OpenSearch queries in Windows Explorer #
            //   180F     Allow previewing and custom thumbnails of OpenSearch query results in Windows Explorer #
            //   1A00 User Authentication: Logon
            //   1A02 Allow persistent cookies that are stored on your computer #
            //   1A03 Allow per - session cookies(not stored) #
            //   1A04 Miscellaneous: Don't prompt for client certificate selection when no 
            //                           certificates or only one certificate exists * ^
            //   1A05 Allow 3rd party persistent cookies *
            //1A06 Allow 3rd party session cookies *
            //1A10 Privacy Settings *
            //1C00 Java permissions #
            //   1E05     Miscellaneous: Software channel permissions
            //   1F00     Reserved * * #
            //   2000     ActiveX controls and plug-ins: Binary and script behaviors
            //   2001.NET Framework - reliant components: Run components signed with Authenticode
            //   2004.NET Framework - reliant components: Run components not signed with Authenticode
            //   2007.NET Framework - Reliant Components: Permissions for Components with Manifests

            //2100     Miscellaneous: Open files based on content, not file extension * * ^
            //2101     Miscellaneous: Web sites in less privileged web content zone can navigate into this zone * *
            //2102     Miscellaneous: Allow script initiated windows without size or position constraints * * ^
            //2103     Scripting: Allow status bar updates via script ^
            //2104     Miscellaneous: Allow websites to open windows without address or status bars ^
            //2105     Scripting: Allow websites to prompt for information using scripted windows ^
            //2200     Downloads: Automatic prompting for file downloads ** ^
            //2201     ActiveX controls and plug - ins: Automatic prompting for ActiveX controls ** ^
            //  2300     Miscellaneous: Allow web pages to use restricted protocols for active content **
            //  2301     Miscellaneous: Use Phishing Filter ^
            //  2400.NET Framework: XAML browser applications

            //  2401.NET Framework: XPS documents

            //  2402.NET Framework: Loose XAML

            //  2500     Turn on Protected Mode[Vista only setting] # (0=On; 3=Off)
            //   2600     Enable.NET Framework setup ^
            //  2702     ActiveX controls and plug - ins: Allow ActiveX Filtering

            //  2708     Miscellaneous: Allow dragging of content between domains into the same window

            //  2709     Miscellaneous: Allow dragging of content between domains into separate windows

            //  270B     Miscellaneous: Render legacy filters

            //  270C     ActiveX Controls and plug - ins: Run Antimalware software on ActiveX controls

            object obj = 0;
            int dynamicszone = 3; // assume it is in the Internet Zone
            int dynamicszonepm = 3; // assume protected mode is off
            try
            {
                using (RegistryKey regDynamics = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap\Domains\dynamics.com", false))
                {
                    if (regDynamics.SubKeyCount > 0) // more specific zone path specified. Lets just look at one of them.
                    {
                        using (RegistryKey regChildDynamics = regDynamics.OpenSubKey(regDynamics.GetSubKeyNames()[0], false))
                        {
                            if (regChildDynamics.ValueCount > 0)
                            {
                                dynamicszone = (int)regChildDynamics.GetValue(regChildDynamics.GetValueNames()[0]);
                            }
                        }
                    }
                    else if (regDynamics.ValueCount > 0)
                    {
                        dynamicszone = (int)regDynamics.GetValue(regDynamics.GetValueNames()[0]);
                    }
                }
            }
            catch
            {   // ignore
            }

            try
            {
                using (RegistryKey regDynamics = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\5.0\Cache\Content", false))
                {
                    object cacheLimit = regDynamics.GetValue("CacheLimit");
                    if (cacheLimit != null && cacheLimit is int)
                    {
                        if ((int)cacheLimit < 1000000)
                        {
                            if (updateSetting)
                            {
                                using (RegistryKey regDynamicsUpdate = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\5.0\Cache\Content", true))
                                {
                                    regDynamicsUpdate.SetValue("CacheLimit", "1024000", RegistryValueKind.DWord);
                                    results.Add("CacheLimit", "Updated");
                                    results.Add("CacheLimitValue", "1024000");
                                }
                            }
                            else
                            {
                                results.Add("CacheLimit", "Not Updated");
                                results.Add("CacheLimitValue", cacheLimit.ToString());
                            }
                        }
                        else
                        {
                            results.Add("CacheLimit", "OK");
                            results.Add("CacheLimitValue", cacheLimit.ToString());
                        }
                    }
                    else
                    {
                        Dictionary<string, string> errorParams = new Dictionary<string, string>();
                        errorParams.Add("Message", "CacheLimit Null or not Int");
                        errorParams.Add("Value", "CacheLimit");
                        FireEvent("Error", errorParams);
                    }
                }
            }
            catch
            {   // ignore
            }

            // Now lets check the protected mode setting of the dynamics zone
            try
            {
                using (RegistryKey regDynamicsZone = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\" + dynamicszone.ToString(), false))
                {
                    dynamicszonepm = (int)regDynamicsZone.GetValue("2500");
                }
            }
            catch
            {   // ignore
            }

            int localintranetzonepm = 3;
            try
            {
                using (RegistryKey regLocalIntranetZone = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\1", false))
                {
                    localintranetzonepm = (int)regLocalIntranetZone.GetValue("2500");
                }
            }
            catch (Exception ex)
            {   // ignore
                Dictionary<string, string> errorParams = new Dictionary<string, string>();
                errorParams.Add("Message", ex.Message);
                errorParams.Add("Value", "ZoneRead");
                FireEvent("Error", errorParams);
                return;
            }

            if (localintranetzonepm != dynamicszonepm)
            {
                // this needs to be updated
                if (updateSetting)
                {
                    try
                    {
                        using (RegistryKey regLocalIntranetZoneWrite = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\Zones\1", true))
                        {
                            regLocalIntranetZoneWrite.SetValue("2500", dynamicszonepm, RegistryValueKind.DWord);
                            results.Add("ZoneUpdate", "Updated");
                            results.Add("ZoneUpdateValue", dynamicszonepm.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Dictionary<string, string> eventParams = new Dictionary<string, string>();
                        eventParams.Add("errorMessage", ex.Message);
                        FireEvent("ZoneUpdateFailed", eventParams);
                        results.Add("ZoneUpdateValue", dynamicszonepm.ToString());
                    }
                }
                else
                {
                    results.Add("ZoneUpdate", "Not Updated");
                    results.Add("ZoneUpdateValue", dynamicszonepm.ToString());
                }
            }
            else
            {
                results.Add("ZoneUpdate", "OK");
                results.Add("ZoneUpdateValue", dynamicszonepm.ToString());
            }


            // CPU Cores
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
                foreach (ManagementObject share in searcher.Get())
                {
                    // Some Codes ...
                    PropertyData PC = share.Properties["NumberOfLogicalProcessors"];
                    if (PC.Value != null)
                    {
                        results.Add("CPU Cores", PC.Value.ToString());
                        if (PC.Value is int && (int)PC.Value <= 1)
                        {
                            FireEvent("CPUCoresInsufficient");
                        }
                    }
                    else
                    {
                        results.Add("CPU Cores", "-1");
                    }

                    PC = share.Properties["TotalPhysicalMemory"];
                    if (PC.Value != null)
                    {
                        results.Add("Physical Memory", PC.Value.ToString());
                        if (PC.Value is int && (int)PC.Value <= 1)
                        {
                            FireEvent("PhysicalMemoryInsufficient");
                        }
                    }
                    else
                    {
                        results.Add("Physical Memory", "-1");
                    }

                    break;
                }
            }
            catch (Exception ex)
            {
                Dictionary<string, string> errorParams = new Dictionary<string, string>();
                errorParams.Add("Message", ex.Message);
                errorParams.Add("Value", "CPU Cores/Physical Memory");
                FireEvent("Error", errorParams);
            }

            FireEvent("IESettings", results);
        }

        private void CheckUSDVersion(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string versionExpectedString = Utility.GetAndRemoveParameter(lines, "VersionExpected").Trim();

            
            Version fileVersion = GetFileVersion(Assembly.GetEntryAssembly());

            Dictionary<string, string> versionParams = new Dictionary<string, string>();
            versionParams.Add("fileVersion", fileVersion.ToString());
            versionParams.Add("format", System.Environment.Is64BitProcess ? "64bit" : "32bit");
            versionParams.Add("versionExpected", versionExpectedString);


            string[] versionComponents = versionExpectedString.Split('.');
            try
            {
                if (fileVersion.ToString() == versionExpectedString)
                {
                    versionParams.Add("Result", "OK");
                    FireEvent("VersionCheckEqual", versionParams);
                }
                else if (versionComponents.Length == 4 && (
                       fileVersion.Major > int.Parse(versionComponents[0])
                    || fileVersion.Minor > int.Parse(versionComponents[1])
                    || fileVersion.Build > int.Parse(versionComponents[2])
                    || fileVersion.Revision > int.Parse(versionComponents[3])))
                {
                    versionParams.Add("Result", "NEWER");
                    FireEvent("VersionCheckNewer", versionParams);
                }
                else
                {
                    versionParams.Add("Result", "OLD");
                    FireEvent("VersionCheckOlder", versionParams);
                }
                if (!(fileVersion.Major > 2 || fileVersion.Minor > 1))
                {
                    versionParams.Add("versionDeprecated", "A minimum of version 2.1 is required to handle the ACS deprecation");
                }
                FireEvent("VersionCheckComplete", versionParams);
            }
            catch (ArgumentNullException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
                FireEvent("VersionCheckComplete", versionParams);
            }
            catch (FormatException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
                FireEvent("VersionCheckComplete", versionParams);
            }
            catch (OverflowException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
                FireEvent("VersionCheckComplete", versionParams);
            }
            catch
            {
                throw;
            }
        }

        private Version GetFileVersion(Assembly executingAssembly)
        {
            if (executingAssembly != null)
            {
                AssemblyName asmName = new AssemblyName(executingAssembly.FullName);
                Version fileVersion = asmName.Version;

                // try to get the build version
                string localPath = string.Empty;

                Uri fileUri = null;
                if (Uri.TryCreate(executingAssembly.CodeBase, UriKind.Absolute, out fileUri))
                {
                    if (fileUri.IsFile)
                        localPath = fileUri.LocalPath;

                    if (!string.IsNullOrEmpty(localPath))
                        if (System.IO.File.Exists(localPath))
                        {
                            FileVersionInfo fv = FileVersionInfo.GetVersionInfo(localPath);
                            if (fv != null)
                            {
                                fileVersion = new Version(fv.FileVersion);
                            }
                        }
                }
                return fileVersion;
            }
            return null;
        }
    }
}
