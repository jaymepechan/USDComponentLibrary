/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
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
using Microsoft.USD.ComponentLibrary;
using System.IO;
using Microsoft.Uii.Csr;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Xrm.Sdk;
using System.Diagnostics;

namespace Microsoft.USD.ComponentLibrary.Adapters.Email
{
    /// <summary>
    /// Interaction logic for MultiEmailAttachment.xaml
    /// </summary>
    public partial class MultiEmailAttachment : MicrosoftBase
    {
        private string id = String.Empty;
        public MultiEmailAttachment()
        {
            InitializeComponent();
        }
        public MultiEmailAttachment(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            base.Initialize();
            // do not put initialization code here
        }
        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("CaptureEmailAttachments", CaptureEmailAttachments);
        }

        private void CaptureEmailAttachments(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            id = Utility.GetAndRemoveParameter(parameters, "id");
            FileList.Items.Clear();

        }

        private void File_PreviewDragEnter(object sender, DragEventArgs e)
        {
            bool isCorrect = true;

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true) == true)
            {
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
                foreach (string filename in filenames)
                {
                    if (File.Exists(filename) == false)
                    {
                        isCorrect = false;
                        break;
                    }
                    FileInfo info = new FileInfo(filename);
                    //if (info.Extension != ".txt")
                    //{
                    //    isCorrect = false;
                    //    break;
                    //}
                }
            }
            if (isCorrect == true)
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;


        }

        private void File_PreviewDrop(object sender, DragEventArgs e)
        {
            if (String.IsNullOrEmpty(id))
                return; // only attach files if an email is associated
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop, true);
            foreach (string filename in filenames)
            {
                // save temporarily. when user is ready to upload, we load and store
                //FileShowTextBox.Text += File.ReadAllText(filename);
                FileList.Items.Add(filename);
            }
            e.Handled = true;
        }

        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (string filename in FileList.Items)
                {
                    byte[] filecontents = File.ReadAllBytes(filename);
                    CreateAttachment(String.Empty, filename, filecontents);
                }
            }
            catch { }
            FireEvent("Attached");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            FireEvent("Cancelled");
        }

        #region Supporting Functions
        private void CreateAttachment(string subject, string filename, byte[] filecontents)
        {
            System.Collections.Generic.Dictionary<string, CrmDataTypeWrapper> dictionary = new System.Collections.Generic.Dictionary<string, CrmDataTypeWrapper>();

            //        ActivityMimeAttachment _sampleAttachment = new ActivityMimeAttachment
            //        {
            //            ObjectId = new EntityReference(Email.EntityLogicalName, _emailId),
            //            ObjectTypeCode = Email.EntityLogicalName,
            //            Subject = "Sample Attachment”,
            //Body = System.Convert.ToBase64String(new ASCIIEncoding().GetBytes("Example Attachment")),
            //            FileName = "ExampleAttachment.txt"
            //        };

            dictionary.Add("filename", new CrmDataTypeWrapper(System.IO.Path.GetFileName(filename), CrmFieldType.Raw));
            dictionary.Add("objecttypecode", new CrmDataTypeWrapper(new OptionSetValue(4200), CrmFieldType.Raw));
            dictionary.Add("objectid", new CrmDataTypeWrapper(new EntityReference("email", Guid.Parse(id)), CrmFieldType.Raw));
            dictionary.Add("subject", new CrmDataTypeWrapper(subject, CrmFieldType.Raw));
            dictionary.Add("body", new CrmDataTypeWrapper(System.Convert.ToBase64String(filecontents), CrmFieldType.Raw));
            try
            {
                System.Guid guid = base._client.CrmInterface.CreateNewRecord("activitymimeattachment", dictionary, "", false, new System.Guid());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
        #endregion
    }
}
