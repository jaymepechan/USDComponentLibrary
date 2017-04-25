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

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for UpgradeUSD.xaml
    /// </summary>
    public partial class UpgradeUSD : MicrosoftBase
    {
        public UpgradeUSD(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
            PopulateToolbars(ProgrammableToolbarTray);
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("CheckUSDVersion", CheckUSDVersion);
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
                    FireEvent("VersionCheckEqual", versionParams);
                }
                else if (versionComponents.Length == 4 && (
                       fileVersion.Major > int.Parse(versionComponents[0])
                    || fileVersion.Minor > int.Parse(versionComponents[1])
                    || fileVersion.Build > int.Parse(versionComponents[2])
                    || fileVersion.Revision > int.Parse(versionComponents[3])))
                {
                    FireEvent("VersionCheckNewer", versionParams);
                }
                else
                {
                    FireEvent("VersionCheckOlder", versionParams);
                }
            }
            catch (ArgumentNullException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
            }
            catch (FormatException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
            }
            catch (OverflowException)
            {
                FireEvent("VersionCheckUnknown", versionParams);
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
