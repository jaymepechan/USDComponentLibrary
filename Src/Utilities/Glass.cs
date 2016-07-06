/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Microsoft.USD.ComponentLibrary.Utilities
{
    public class Glass
    {
        #region Glass extending

        [StructLayout(LayoutKind.Sequential)]
        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }
        [DllImport("dwmapi.dll")]
        static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [DllImport("dwmapi.dll")]
        extern static int DwmIsCompositionEnabled(ref int en);
        const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        public static void ExtendGlass(Window w)
        {
            try
            {
                int isGlassEnabled = 0;
                DwmIsCompositionEnabled(ref isGlassEnabled);
                if (Environment.OSVersion.Version.Major > 5 && isGlassEnabled > 0)
                {
                    WindowInteropHelper helper = new WindowInteropHelper(w);
                    HwndSource mainWindowSrc = (HwndSource)HwndSource.FromHwnd(helper.Handle);

                    mainWindowSrc.CompositionTarget.BackgroundColor = Colors.Transparent;
                    w.Background = Brushes.Transparent;

                    MARGINS margins = new MARGINS();
                    margins.cxLeftWidth = -1;
                    margins.cxRightWidth = -1;
                    margins.cyBottomHeight = -1;
                    margins.cyTopHeight = -1;

                    DwmExtendFrameIntoClientArea(mainWindowSrc.Handle, ref margins);
                }
            }
            catch (DllNotFoundException) { }
        }

        #endregion
    }
}
