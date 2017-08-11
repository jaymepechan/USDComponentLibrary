/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.USD.ComponentLibrary.Adapters.Automation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer
{
    public class JavaWindow
    {
        private int _vmId;
        private IntPtr _javaWin;
        private JavaWindow _parent;
        public List<string> javaactions = new List<string>();
        private AccessibleContextInfo accContextInfo;
        public JavaWindow(JavaWindow parent, IntPtr javaWin, int vmId)
        {
            _parent = parent;
            if (javaWin == IntPtr.Zero)
                return;
            _vmId = vmId;
            _javaWin = javaWin;

            LoadData();
        }

        void LoadData()
        {
            accContextInfo = new AccessibleContextInfo();
            System.Windows.Forms.Application.DoEvents();
            bool result = JavaAccNativeMethods.getAccessibleContextInfo(_vmId, _javaWin, out accContextInfo);
            if (result == false)
            {
                System.Diagnostics.Debug.WriteLine("No info");
                return;
            }
            int count = JavaAccNativeMethods.getAccessibleActions(_vmId, _javaWin);
            for (int i = 0; i < count; i++)
            {
                IntPtr actionname = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(JavaAccNativeMethods.ACTIONNAMESIZE);
                if (!actionname.Equals((System.IntPtr)System.IntPtr.Zero) && JavaAccNativeMethods.getAccessibleActionItem(i, actionname, JavaAccNativeMethods.ACTIONNAMESIZE))
                {
                    javaactions.Add(System.Runtime.InteropServices.Marshal.PtrToStringUni(actionname));
                }
            }
            System.Windows.Forms.Application.DoEvents();
            if (accContextInfo.childrenCount == 0)
                return; // we are done!
            // get children
            System.Windows.Forms.Application.DoEvents();
            for (int i = 0; i < accContextInfo.childrenCount; i++)
            {
                IntPtr childPtr = JavaAccNativeMethods.getAccessibleChildFromContext(_vmId, _javaWin, i);
                _children.Add(new JavaWindow(this, childPtr, _vmId));
            }
        }

        List<JavaWindow> _children = new List<JavaWindow>();
        public IList<JavaWindow> Children
        {
            get { return _children; }
        }

        public IntPtr JavaWin { get { return _javaWin; } }
        public int VMId { get { return _vmId; } }
        public JavaWindow Parent { get { return _parent; } }
        public string Name { get { return accContextInfo.name; } }
        public string AccessibleText { get { return accContextInfo.accessibleText.ToString(); } }
        public string AccessibleAction { get { return accContextInfo.accessibleAction.ToString(); } }
        public string AccessibleComponent { get { return accContextInfo.accessibleComponent.ToString(); } }
        public string AccessibleInterfaces { get { return accContextInfo.accessibleInterfaces.ToString(); } }
        public string AccessibleSelection { get { return accContextInfo.accessibleSelection.ToString(); } }
        public string ChildrenCount { get { return accContextInfo.childrenCount.ToString(); } }
        public string Description { get { return accContextInfo.description.ToString(); } }
        public string X { get { return accContextInfo.x.ToString(); } }
        public string Y { get { return accContextInfo.y.ToString(); } }
        public string Width { get { return accContextInfo.width.ToString(); } }
        public string Height { get { return accContextInfo.height.ToString(); } }
        public string IndexInParent { get { return accContextInfo.indexInParent.ToString(); } }
        public string Role { get { return accContextInfo.role; } }
        public string States { get { return accContextInfo.states; } }
        public string Role_en_US { get { return accContextInfo.role_en_US; } }
        public string States_en_US { get { return accContextInfo.states_en_US; } }
        public string DisplayName { get { return "[" + Role + "] *** Children=" + ChildrenCount + " Name=" + Name; } }
        public string SuggestedXml
        {
            get
            {
                //   < Controls >
                //      < JAccControl name = "clickme" >
                //     < Path >
                //     < NextName offset = "0" > Click Me </ NextName >
                //        </ Path >
                //    </ JAccControl >
                //    < JAccControl name = "textbox" >
                //         < Path >
                //         < NextRole offset = "0" > text </ NextRole >
                //          </ Path >
                //      </ JAccControl >
                //  </ Controls >
                StringBuilder sb = new StringBuilder();
                using (XmlWriter xmlwriter = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration=true, NewLineChars = "\r\n", NewLineHandling=NewLineHandling.Entitize, NewLineOnAttributes=false, Indent=true }))
                {
                    xmlwriter.WriteStartElement("Controls");
                    xmlwriter.WriteStartElement("JAccControl");
                    if (!String.IsNullOrEmpty(Name))
                    {
                        xmlwriter.WriteAttributeString("name", Name.Replace(' ', '_'));
                        xmlwriter.WriteStartElement("Path");
                        WritePathElement(xmlwriter, this);
                        xmlwriter.WriteEndElement();
                    }
                    else
                    {
                        xmlwriter.WriteAttributeString("name", "[name]");
                        xmlwriter.WriteStartElement("Path");
                        WritePathElement(xmlwriter, this);
                        xmlwriter.WriteEndElement();
                    }
                    xmlwriter.WriteEndElement();
                    xmlwriter.WriteEndElement();
                    xmlwriter.Close();
                    return sb.ToString();
                }
            }
        }

        private void WritePathElement(XmlWriter xmlwriter, JavaWindow javawin)
        {
            if (javawin.Parent == null)
                return; // don't create an element for the top level window

            /// Don't use name...
            /// Reason #1: The name of a textbox is the text in the box and changes frequently
            /// Reason #2: While the name of a button may not change, it is definitely not unique and could also change.  For example an OK button might exist all over.
            /// 
            //if (!String.IsNullOrEmpty(javawin.Name))
            //{
            //    if (javawin.Parent != null)
            //        WritePathElement(xmlwriter, javawin.Parent);
            //    xmlwriter.WriteStartElement("NextName");
            //    xmlwriter.WriteAttributeString("offset", "0");
            //    xmlwriter.WriteString(javawin.Name);
            //    xmlwriter.WriteEndElement();
            //}
            //else
            {
                if (javawin.Parent != null)
                    WritePathElement(xmlwriter, javawin.Parent);
                xmlwriter.WriteStartElement("NextRole");
                xmlwriter.WriteAttributeString("offset", GetOffset(javawin, javawin.Role).ToString());
                xmlwriter.WriteString(javawin.Role);
                xmlwriter.WriteEndElement();
            }
        }

        private int GetOffset(JavaWindow javawin, string role)
        {
            JavaWindow parent = javawin.Parent;
            if (parent == null)
                return 0;
            int ioffset = 0;
            for (int i = 0; i < parent.Children.Count(); i++)
            {
                if (parent.Children[i] == javawin)
                    return ioffset;
                if (parent.Children[i].Role == role)
                    ioffset++;
            }
            return 0;
        }
    }
}
