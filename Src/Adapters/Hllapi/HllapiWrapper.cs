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

namespace Microsoft.USD.ComponentLibrary.Adapters
{
	/// <summary>
	/// 
	/// </summary>
    public class HllapiWrapper
	{
        const UInt32 OemFunction = 0;
		const UInt32 HA_CONNECT_PS = 1; /* 000 Connect PS*/
		const UInt32 HA_DISCONNECT_PS = 2; /* 000 Disconnect PS*/
		const UInt32 HA_SENDKEY = 3; /* 000 Sendkey function*/
		const UInt32 HA_WAIT = 4; /* 000 Wait function*/
		const UInt32 HA_COPY_PS = 5; /* 000 Copy PS function*/
		const UInt32 HA_SEARCH_PS = 6; /* 000 Search PS function*/
		const UInt32 HA_QUERY_CURSOR_LOC = 7; /* 000 Query Cursor*/
		const UInt32 HA_COPY_PS_TO_STR = 8; /* 000 Copy PS to String*/
		const UInt32 HA_SET_SESSION_PARMS = 9; /* 000 Set Session*/
		const UInt32 HA_QUERY_SESSIONS = 10; /* 000 Query Sessions*/
		const UInt32 HA_RESERVE = 11; /* 000 Reserve function*/
		const UInt32 HA_RELEASE = 12; /* 000 Release function*/
		const UInt32 HA_COPY_OIA = 13; /* 000 Copy OIA function*/
		const UInt32 HA_QUERY_FIELD_ATTR = 14; /* 000 Query Field*/
		const UInt32 HA_COPY_STR_TO_PS = 15; /* 000 Copy string to PS*/
		const UInt32 HA_STORAGE_MGR = 17; /* 000 Storage Manager*/
		const UInt32 HA_PAUSE = 18; /* 000 Pause function*/
		const UInt32 HA_QUERY_SYSTEM = 20; /* 000 Query System*/
		const UInt32 HA_RESET_SYSTEM = 21; /* 000 Reset System*/
		const UInt32 HA_QUERY_SESSION_STATUS = 22; /* 000 Query Session*/
		const UInt32 HA_START_HOST_NOTIFY = 23; /* 000 Start Host*/
		const UInt32 HA_QUERY_HOST_UPDATE = 24; /* 000 Query Host Update*/
		const UInt32 HA_STOP_HOST_NOTIFY = 25; /* 000 Stop Host*/
		const UInt32 HA_SEARCH_FIELD = 30; /* 000 Search Field*/
		const UInt32 HA_FIND_FIELD_POS = 31; /* 000 Find Field*/
		const UInt32 HA_FIND_FIELD_LEN = 32; /* 000 Find Field Length*/
		const UInt32 HA_COPY_STR_TO_FIELD = 33; /* 000 Copy String to*/
		const UInt32 HA_COPY_FIELD_TO_STR = 34; /* 000 Copy Field to*/
		const UInt32 HA_SET_CURSOR = 40; /* 000 Set Cursor*/
		const UInt32 HA_START_CLOSE_INTERCEPT = 41; /* 000 Start Close Intercept*/
		const UInt32 HA_QUERY_CLOSE_INTERCEPT = 42; /* 000 Query Close Intercept*/
		const UInt32 HA_STOP_CLOSE_INTERCEPT = 43; /* 000 Stop Close Intercept*/
		const UInt32 HA_START_KEY_INTERCEPT = 50; /* 000 Start Keystroke*/
		const UInt32 HA_GET_KEY = 51; /* 000 Get Key function*/
		const UInt32 HA_POST_INTERCEPT_STATUS = 52; /* 000 Post Intercept*/
		const UInt32 HA_STOP_KEY_INTERCEPT = 53; /* 000 Stop Keystroke*/
		const UInt32 HA_LOCK_PS = 60; /* 000 Lock Presentation*/
		const UInt32 HA_LOCK_PMSVC = 61; /* 000 Lock PM Window*/
		const UInt32 HA_SEND_FILE = 90; /* 000 Send File function*/
		const UInt32 HA_RECEIVE_FILE = 91; /* 000 Receive file*/
		const UInt32 HA_CONVERT_POS_ROW_COL = 99; /* 000 Convert Position*/
		const UInt32 HA_CONNECT_PM_SRVCS = 101; /* 000 Connect For*/
		const UInt32 HA_DISCONNECT_PM_SRVCS = 102; /* 000 Disconnect From*/
		const UInt32 HA_QUERY_WINDOW_COORDS = 103; /* 000 Query Presentation*/
		const UInt32 HA_PM_WINDOW_STATUS = 104; /* 000 PM Window Status*/
		const UInt32 HA_CHANGE_SWITCH_NAME = 105; /* 000 Change Switch List*/
		const UInt32 HA_CHANGE_WINDOW_NAME = 106; /* 000 Change PS Window*/
		const UInt32 HA_START_PLAYING_MACRO = 110; /* 000 Start playing macro*/
		const UInt32 HA_START_STRUCTURED_FLD = 120; /* 000 Start Structured*/
		const UInt32 HA_STOP_STRUCTURED_FLD = 121; /* 000 Stop Structured*/
		const UInt32 HA_QUERY_BUFFER_SIZE = 122; /* 000 Query Communications*/
		const UInt32 HA_ALLOCATE_COMMO_BUFF = 123; /* 000 Allocate*/
		const UInt32 HA_FREE_COMMO_BUFF = 124; /* 000 Free Communications*/
		const UInt32 HA_GET_ASYNC_COMPLETION = 125; /* 000 Get Asynchronous*/
		const UInt32 HA_READ_STRUCTURED_FLD = 126; /* 000 Read Structured Field*/
		const UInt32 HA_WRITE_STRUCTURED_FLD = 127; /* 000 Write Structured*/


		//********************************************************************/ 

		//******************* EHLLAPI RETURN CODES***************************/ 

		//********************************************************************/ 
        public Dictionary<int, string> HAResultString = new Dictionary<int, string>()
        {
            {0, "Good return code."},
            {1, "Invalid PS, Not."},
            {2, "Bad parameter, or"},
            {4, "PS is busy return"},
            {5, "PS is LOCKed, or"},
            {6, "Truncation"},
            {7, "Invalid PS"},
            {8, "No prior start"},
            {9, "A system error"},
            {10, "Invalid or"},
            {11, "Resource is"},
            {12, "Session has"},
            {20, "Illegal mnemonic"},
            {21, "A OIA update"},
            {22, "A PS update"},
            {23, "A PS and OIA update"},
            {24, "String not found"},
            {25, "No keys available"},
            {26, "A HOST update"},
            {28, "Field length = 0"},
            {31, "Keystroke queue"},
            {32, "Successful. Another"},
            {34, "Inbound structured"},
            {35, "Outbound structured"},
            {36, "Contact with the"},
            {37, "Host structured field"},
            {38, "Requested Asynchronous"},
            {39, "Request for DDM"},
            {40, "Disconnect successful."},
            {41, "Memory cannot be freed"},
            {42, "No pending"},
            {43, "Option requested is"},
            {301, "Invalid function number"},
            {302, "File Not Found"},
            {305, "Access Denied"},
            {308, "Insufficient Memory"},
            {310, "Invalid environment"},
            {311, "Invalid format"},
            {9998, "Invalid Presentation Space ID"},
            {9999, "Invalid Row or Column Code"},
            {0xF000, "An async call is already outstanding"},
            {0xF001, "Async Task Id is invalid"},
            {0xF002, "Blocking call was cancelled"},
            {0xF003, "Underlying subsystem not started"},
            {0xF004, "Application version not supported"}
        };
		const UInt32 HARC_SUCCESS = 0; /* 000 Good return code.*/
		const UInt32 HARC99_INVALID_INP = 0; /* 000 Incorrect input*/
		const UInt32 HARC_INVALID_PS = 1; /* 000 Invalid PS, Not*/
		const UInt32 HARC_BAD_PARM = 2; /* 000 Bad parameter, or*/
		const UInt32 HARC_BUSY = 4; /* 000 PS is busy return*/
		const UInt32 HARC_LOCKED = 5; /* 000 PS is LOCKed, or*/
		const UInt32 HARC_TRUNCATION = 6; /* 000 Truncation*/
		const UInt32 HARC_INVALID_PS_POS = 7; /* 000 Invalid PS*/
		const UInt32 HARC_NO_PRIOR_START = 8; /* 000 No prior start*/
		const UInt32 HARC_SYSTEM_ERROR = 9; /* 000 A system error*/
		const UInt32 HARC_UNSUPPORTED = 10; /* 000 Invalid or*/
		const UInt32 HARC_UNAVAILABLE = 11; /* 000 Resource is*/
		const UInt32 HARC_SESSION_STOPPED = 12; /* 000 Session has*/
		const UInt32 HARC_BAD_MNEMONIC = 20; /* 000 Illegal mnemonic*/
		const UInt32 HARC_OIA_UPDATE = 21; /* 000 A OIA update*/
		const UInt32 HARC_PS_UPDATE = 22; /* 000 A PS update*/
		const UInt32 HARC_PS_AND_OIA_UPDATE = 23; /* A PS and OIA update*/
		const UInt32 HARC_STR_NOT_FOUND_UNFM_PS = 24; /* 000 String not found,*/
		const UInt32 HARC_NO_KEYS_AVAIL = 25; /* 000 No keys available*/
		const UInt32 HARC_HOST_UPDATE = 26; /* 000 A HOST update*/
		const UInt32 HARC_FIELD_LEN_ZERO = 28; /* 000 Field length = 0*/
		const UInt32 HARC_QUEUE_OVERFLOW = 31; /* 000 Keystroke queue*/
		const UInt32 HARC_ANOTHER_CONNECTION = 32; /* 000 Successful. Another*/
		const UInt32 HARC_INBOUND_CANCELLED = 34; /* 000 Inbound structured*/
		const UInt32 HARC_OUTBOUND_CANCELLED = 35; /* 000 Outbound structured*/
		const UInt32 HARC_CONTACT_LOST = 36; /* 000 Contact with the*/
		const UInt32 HARC_INBOUND_DISABLED = 37; /* 000 Host structured field*/
		const UInt32 HARC_FUNCTION_INCOMPLETE = 38; /* 000 Requested Asynchronous*/
		const UInt32 HARC_DDM_ALREADY_EXISTS = 39; /* 000 Request for DDM*/
		const UInt32 HARC_ASYNC_REQUESTS_OUT = 40; /* 000 Disconnect successful.*/
		const UInt32 HARC_MEMORY_IN_USE = 41; /* 000 Memory cannot be freed*/
		const UInt32 HARC_NO_MATCH = 42; /* 000 No pending*/
		const UInt32 HARC_OPTION_INVALID = 43; /* 000 Option requested is*/
        const UInt32 WHLLINVALIDFUNCTIONNUM =301;	// Invalid function number 
	    const UInt32 WHLLFILENOTFOUND =	302;	// File Not Found 
	    const UInt32 WHLLACCESSDENIED = 305;	// Access Denied 
	    const UInt32 WHLLMEMORY = 308;	// Insufficient Memory 
	    const UInt32 WHLLINVALIDENVIRONMENT = 310;	// Invalid environment 
	    const UInt32 WHLLINVALIDFORMAT = 311;	// Invalid format 
	    const UInt32 WHLLINVALIDPSID = 9998;	// Invalid Presentation Space ID 
        const UInt32 HARC99_INVALID_PS = 9998; /* 000 An invalid PS id*/
        const UInt32 WHLLINVALIDRC = 9999;	// Invalid Row or Column Code 
        const UInt32 HARC99_INVALID_CONV_OPT = 9999; /* 000 Invalid convert*/
        const UInt32 WHLLREADY = 0xF000; // An async call is already outstanding 
        const UInt32 WHLLINVALID = 0xF001;	// Async Task Id is invalid 
        const UInt32 WHLLCANCEL = 0xF002; // Blocking call was cancelled      
        const UInt32 WHLLSYSNOTREADY = 0xF003;  // Underlying subsystem not started 
        const UInt32 WHLLVERNOTSUPPORTED = 0xF004;	// Application version not supported 

        public enum ChangePresentationSpaceWindowName : byte
        {
            Set = 1,
            Reset = 2
        };

        // Window Status values
        public enum WindowStatus : byte
        {
            Set = 0x01,
            Query = 0x02,
            ExtendedQuery = 0x03,
        };

        public enum SetWindowStatus : ushort
        {
            NULL = 0x0000,
            SIZE = 0x0001,
            MOVE = 0x0002,
            ZORDER = 0x0004,
            SHOW = 0x0008,
            HIDE = 0x0010,
            ACTIVATE = 0x0080,
            DEACTIVATE = 0x0100,
            MINIMIZE = 0x0400,
            MAXIMIZE = 0x0800,
            RESTORE = 0x1000,
        };

        #region WINHLLAPI Setup
        // Import native Windows functions needed for dynamically loading 
        // an unmanaged DLL.
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr FreeLibrary(IntPtr hModule);

        // Declare delegates for the Whllapi functions: WinHLLAPI, WinHLLAPIStatup and
        // WHLLAPICleanup. (Requires .NET Framework version 2.0)
        // 
        // WinHLLAPI function call syntax as documented in the Windows HLLAPI 
        // Specification Version 1.1
        //
        // extern void FAR PASCAL WinHLLAPI(
        //    LPWORD lpwFunction,       /* Function name */
        //    LPBYTE lpbyString,        /* String pointer */   
        //    LPWORD lpwLength,         /* String (data) length */
        //    LPWORD lpwReturnCode);    /* Return code */
        //
        // int WinHLLAPIStartup(WORD wVersionRequired, LPWHLLAPIDATA lpData)
        //
        // BOOL WinHLLAPICleanup(void)
        //
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate UInt32 WinHLLAPIDelegate(out UInt32 lpwFunction, StringBuilder byData, out UInt32 lpwLength, out UInt32 lpwReturnCode);
        //public delegate void WinHLLAPIDelegate(out UInt16 lpwFunction, byte[] byData, out UInt16 lpwLength, out UInt16 lpwReturnCode);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int WinHLLAPIStartupDelegate(short nVersion, byte[] byData);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int WinHLLAPICleanupDelegate();

        // Define variables for our function  delegates
        public WinHLLAPIDelegate winHLLAPIDelegate;
        public WinHLLAPIStartupDelegate winHLLAPIStartupDelegate;
        public WinHLLAPICleanupDelegate winHLLAPICleanupDelegate;

        public IntPtr handleToWhllapiDll;

        // --------------------------------------------------------------------------
        // LoadDll - Dynamically load the WHLLAPI DLL.
        //     1. Map the WHLLAPI DLL into our address space
        //     2. Aquire pointers to the WinHLLAPI, WinHLLAPIStartup and WinHLLAPICleanup
        //        functions in the DLL.
        //     3. Convert the function pointers to delegates.
        // --------------------------------------------------------------------------
        public bool LoadDll(string whllapiDllFilename)
        {
            // Map the WHLLAPI DLL into our address space. 
            handleToWhllapiDll = LoadLibrary(whllapiDllFilename);

            if (handleToWhllapiDll != IntPtr.Zero)
            {
                // Get the addresses of the WinHLLAPI, WinHLLAPIStartup and WinHLLAPICleanup
                // functions in the WHLLAPI DLL.
                IntPtr ptrToWinHLLAPIFunction = GetProcAddress(handleToWhllapiDll, "WinHLLAPI");
                IntPtr ptrToWinHLLAPIStartupFunction = GetProcAddress(handleToWhllapiDll, "WinHLLAPIStartup");
                IntPtr ptrToWinHLLAPICleanupFunction = GetProcAddress(handleToWhllapiDll, "WinHLLAPICleanup");

                if (ptrToWinHLLAPIFunction != IntPtr.Zero &&
                    ptrToWinHLLAPIStartupFunction != IntPtr.Zero &&
                    ptrToWinHLLAPICleanupFunction != IntPtr.Zero)
                {

                    // Assign the function pointers to the Delegates defined above.
                    winHLLAPIDelegate = (WinHLLAPIDelegate)Marshal.GetDelegateForFunctionPointer(
                                                                ptrToWinHLLAPIFunction,
                                                                typeof(WinHLLAPIDelegate));
                    winHLLAPIStartupDelegate = (WinHLLAPIStartupDelegate)Marshal.GetDelegateForFunctionPointer(
                                                                ptrToWinHLLAPIStartupFunction,
                                                                typeof(WinHLLAPIStartupDelegate));
                    winHLLAPICleanupDelegate = (WinHLLAPICleanupDelegate)Marshal.GetDelegateForFunctionPointer(
                                                                ptrToWinHLLAPICleanupFunction,
                                                                typeof(WinHLLAPICleanupDelegate));

                    return true;
                }
            }
            return false;
        }

        // --------------------------------------------------------------------------
        // FreeDll - Release the WHLLAPI DLL.
        // --------------------------------------------------------------------------
        public bool FreeDll()
        {
            if (FreeLibrary(handleToWhllapiDll) != IntPtr.Zero)
                return true;
            else
                return false;
        }
        #endregion

        public UInt32 Connect(string sessionID) 
		{ 
			StringBuilder Data = new StringBuilder(4);
			Data.Append(sessionID);
			UInt32 rc=0;
			UInt32 f=HA_CONNECT_PS;
			UInt32 l=4;
            return winHLLAPIDelegate(out f, Data, out l, out rc);
		}

		public UInt32 Disconnect(string sessionID) 
		{ 
			StringBuilder Data = new StringBuilder(4);
			Data.Append(sessionID);
			UInt32 rc=0;
			UInt32 f=HA_DISCONNECT_PS;
			UInt32 l=4;
            return winHLLAPIDelegate(out f, Data, out l, out rc);
		}
		
		public UInt32 SetCursorPos(int p)
		{
			StringBuilder Data = new StringBuilder(0);
			UInt32 rc=(UInt32) p;
			UInt32 f=HA_SET_CURSOR;
			UInt32 l=0;
            return winHLLAPIDelegate(out f, Data, out l, out rc);
		}

		public UInt32 GetCursorPos(out int p)
		{
			StringBuilder Data = new StringBuilder(0);
			UInt32 rc=0;
			UInt32 f=HA_QUERY_CURSOR_LOC;
			UInt32 l=0; //return position
            UInt32 r = winHLLAPIDelegate(out f, Data, out l, out rc);
			p = (int)l;
			return r;
		}
		
		public UInt32 SendStr(string cmd)
		{
			StringBuilder Data = new StringBuilder(cmd.Length);
			Data.Append(cmd);
			UInt32 rc=0;
			UInt32 f=HA_SENDKEY;
			UInt32 l=(UInt32)cmd.Length;
            return winHLLAPIDelegate(out f, Data, out l, out rc);
		}

		public UInt32 ReadScreen(int position, int len, out string txt)
		{
			StringBuilder Data = new StringBuilder(3000);
			UInt32 rc=(UInt32)position;
			UInt32 f=HA_COPY_PS_TO_STR;
			UInt32 l=(UInt32)len;
            UInt32 r = winHLLAPIDelegate(out f, Data, out l, out rc);
			txt=Data.ToString();
			return r;
		}

		public UInt32 Wait()
		{
			StringBuilder Data = new StringBuilder(0);
			UInt32 rc=0;
			UInt32 f=HA_WAIT ;
			UInt32 l=0;
            UInt32 r = winHLLAPIDelegate(out f, Data, out l, out rc);			
			return r;
		}

	
	}
}
