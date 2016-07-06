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
using System.Diagnostics;

namespace Microsoft.USD.ComponentLibrary
{
    internal static class CachePurger
    {
        // Methods
        public static void Purge()
        {
            Trace.WriteLine("CachePurger: Start");
            long lpGroupId = 0L;
            int lpdwFirstCacheEntryInfoBufferSize = 0;
            IntPtr zero = IntPtr.Zero;
            IntPtr hFind = NativeMethods.FindFirstUrlCacheGroup(0, 0, IntPtr.Zero, 0, ref lpGroupId, IntPtr.Zero);
            if (!hFind.Equals(IntPtr.Zero) && Win32ErrorNoMoreItems())
            {
                return;
            }
            do
            {
                NativeMethods.DeleteUrlCacheGroup(lpGroupId, 2, IntPtr.Zero);
            }
            while (NativeMethods.FindNextUrlCacheGroup(hFind, ref lpGroupId, IntPtr.Zero));
            if (!NativeMethods.FindFirstUrlCacheEntry(null, IntPtr.Zero, ref lpdwFirstCacheEntryInfoBufferSize).Equals(IntPtr.Zero) && Win32ErrorNoMoreItems())
            {
                return;
            }
            try
            {
                int cb = lpdwFirstCacheEntryInfoBufferSize;
                zero = Marshal.AllocHGlobal(cb);
                hFind = NativeMethods.FindFirstUrlCacheEntry(null, zero, ref lpdwFirstCacheEntryInfoBufferSize);
                while (true)
                {
                    do
                    {
                        INTERNET_CACHE_ENTRY_INFOA internet_cache_entry_infoa = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(zero, typeof(INTERNET_CACHE_ENTRY_INFOA));
                        lpdwFirstCacheEntryInfoBufferSize = cb;
                        NativeMethods.DeleteUrlCacheEntry(internet_cache_entry_infoa.lpszSourceUrlName);
                    }
                    while (NativeMethods.FindNextUrlCacheEntry(hFind, zero, ref lpdwFirstCacheEntryInfoBufferSize));
                    if (Win32ErrorNoMoreItems() || lpdwFirstCacheEntryInfoBufferSize.Equals(0))
                    {
                        goto Label_0137;
                    }
                    if (lpdwFirstCacheEntryInfoBufferSize > cb)
                    {
                        cb = lpdwFirstCacheEntryInfoBufferSize;
                        zero = Marshal.ReAllocHGlobal(zero, (IntPtr)cb);
                    }
                    NativeMethods.FindNextUrlCacheEntry(hFind, zero, ref lpdwFirstCacheEntryInfoBufferSize);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(zero);
            }
        Label_0137:
            Trace.WriteLine("CachePurger: End");
        }

        private static bool Win32ErrorNoMoreItems()
        {
            return Marshal.GetLastWin32Error().Equals(0x103);
        }

        // Nested Types
        [StructLayout(LayoutKind.Explicit, Size = 80)]
        private struct INTERNET_CACHE_ENTRY_INFOA
        {
            // Fields
            [FieldOffset(4)]
            public IntPtr lpszSourceUrlName;
        }
    }
}