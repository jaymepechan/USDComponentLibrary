/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Diagnostics;
using System.Security;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.USD.ComponentLibrary.Utilities
{
    public class Win32
    {
        #region Minimize, Memory Release
        [DllImport("KERNEL32.DLL", EntryPoint = "SetProcessWorkingSetSize", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool SetProcessWorkingSetSize(IntPtr pProcess, int dwMinimumWorkingSetSize, int dwMaximumWorkingSetSize);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetCurrentProcess", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr GetCurrentProcess();

        public static Window GetParentWindow(DependencyObject child)
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null)
            {
                return null;
            }

            Window parent = parentObject as Window;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return GetParentWindow(parentObject);
            }
        }

        public static void MinimizeRelease()
        {
            IntPtr pHandle = GetCurrentProcess();
            SetProcessWorkingSetSize(pHandle, -1, -1);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static void SetWorkingSet(long minimum, long maximum, bool autoFlush)
        {
            try
            {
                System.Diagnostics.Process loProcess = System.Diagnostics.Process.GetCurrentProcess();
                try
                {
                    if (maximum > 0 && loProcess.WorkingSet64 > maximum && autoFlush == true)
                    {
                        loProcess.MaxWorkingSet = (IntPtr)maximum;
                        return;
                    }

                    if (maximum <= 0)
                        loProcess.MaxWorkingSet = (IntPtr)((long)loProcess.MaxWorkingSet - 1);
                    else
                        loProcess.MaxWorkingSet = (IntPtr)maximum;
                    if (minimum <= 0)
                        loProcess.MinWorkingSet = (IntPtr)((long)loProcess.MinWorkingSet - 1);
                    else
                        loProcess.MinWorkingSet = (IntPtr)minimum;
                }
                catch (System.Exception)
                {
                    loProcess.MaxWorkingSet = (IntPtr)((long)800000);
                    loProcess.MinWorkingSet = (IntPtr)((long)200000);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to set working set.  Possible permission issue.  Please contact an administrator.\r\n" + ex.Message);
            }
        }
        #endregion

        #region Web Interfaces
        [ComImport]
        [Guid("00000118-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleClientSite
        {
            void SaveObject();
            void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);
            void GetContainer(ref object ppContainer);
            void ShowObject();
            void OnShowWindow(bool fShow);
            void RequestNewObjectLayout();
        }

        [ComImport]
        [Guid("00000112-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleObject
        {
            void SetClientSite(IOleClientSite pClientSite);
            void GetClientSite(ref IOleClientSite ppClientSite);
            void SetHostNames(object szContainerApp, object szContainerObj);
            void Close(uint dwSaveOption);
            void SetMoniker(uint dwWhichMoniker, object pmk);
            void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
            void InitFromData(IDataObject pDataObject, bool fCreation, uint dwReserved);
            void GetClipboardData(uint dwReserved, ref IDataObject ppDataObject);
            void DoVerb(uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect);
            void EnumVerbs(ref object ppEnumOleVerb);
            void Update();
            void IsUpToDate();
            void GetUserClassID(uint pClsid);
            void GetUserType(uint dwFormOfType, uint pszUserType);
            void SetExtent(uint dwDrawAspect, uint psizel);
            void GetExtent(uint dwDrawAspect, uint psizel);
            void Advise(object pAdvSink, uint pdwConnection);
            void Unadvise(uint dwConnection);
            void EnumAdvise(ref object ppenumAdvise);
            void GetMiscStatus(uint dwAspect, uint pdwStatus);
            void SetColorScheme(object pLogpal);
        };

        public static Guid IID_IOleObject = new Guid("00000112-0000-0000-C000-000000000046");
        #endregion
    }
}
