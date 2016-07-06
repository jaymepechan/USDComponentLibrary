/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
//********************************************************************/ 

//***************   EHLLAPI SPECIAL KEY     **************************/ 

//********************************************************************/ 

//		Meaning	                Mnemonic Code
//		  @                     @@
//        Alt	                @A
//        Alt Cursor	        @$
//        AEhllapiFuncntion 	        @A@Q
//        Backspace		        @<
//        Backtab (Left Tab)	@B
//        Clear	                @C
//        Cmd (function) Key	@A@Y
//        Cursor Down	        @V
//        Cursor Left	        @L
//        Cursor Right	        @Z
//        Cursor Select	        @A@J
//        Cursor Up	        @U
//        Delete	                @D
//        Dup	                @S@x
//        End	                @q
//        Enter	                @E
//        Erase EOF	        @F
//        Erase Input	        @A@F
//        Field Exit	        @A@E
//        Field Mark	        @S@y
//        Field -	                @A@-
//        Field +	                @A@+
//        Help	                @H
//        Hexadecimal	        @A@X
//        Home	                @0  ['@' followed by a zero]
//        Insert 	                @I
//        Insert Toggle	        @A@I
//        Local Print	        @P
//        New Line	        @N
//        Page Up	                @u
//        Page Down	        @v
//        Print (PC)	        @A@t
//        Print Screen	        @A@T
//        Record Backspace	@A@<
//        Reset	                @R
//        Shift	                @S
//        Sys Request	        @A@H
//        Tab (Right Tab)	        @T
//        Test	                @A@C
//        PA1	                @x
//        PA2	                @y
//        PA3	                @z
//        PA4	                @+
//        PA5	                @%
//        PA6	                @&
//        PA7	                @'  ['@' followed by a single quote]
//        PA8	                @(
//        PA9	                @)
//        PA10	                @*
//        PF1/F1	                @1
//        PF2/F2	                @2
//        PF3/F3	                @3
//        PF4/F4	                @4
//        PF5/F5	                @5
//        PF6/F6	                @6
//        PF7/F7	                @7
//        PF8/F8	                @8
//        PF9/F9	                @9
//        PF10/F10	        @a
//        PF11/F11	        @b
//        PF12/F12	        @c
//        PF13/F13	        @d
//        PF14/F14	        @e
//        PF15/F15	        @f
//        PF16/F16	        @g
//        PF17/F17	        @h
//        PF18/F18	        @i
//        PF19/F19	        @j
//        PF20/F20	        @k
//        PF21/F21	        @l
//        PF22/F22	        @m
//        PF23/F23	        @n
//        PF24/F24	        @o
//*)

//********************************************************************/ }
//*************** EHLLAPI FUNCTION NUMBERS***************************/ }
//********************************************************************/ }
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.USD.ComponentLibrary
{
    public class EhllapiFunc
    {
        [DllImport(@"C:\Program Files (x86)\IBM\Client Access\Emulator\PCSHLL32.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern UInt32 hllapi(out UInt32 Func, StringBuilder Data, out UInt32 Length, out UInt32 RetC);
    }

    /// <summary>
    /// 
    /// </summary>
    public class EhllapiWrapper
    {
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
        const UInt32
            HA_SEND_FILE = 90; /* 000 Send File function*/
        const UInt32
            HA_RECEIVE_FILE = 91; /* 000 Receive file*/
        const UInt32
            HA_CONVERT_POS_ROW_COL = 99; /* 000 Convert Position*/
        const UInt32
            HA_CONNECT_PM_SRVCS = 101; /* 000 Connect For*/
        const UInt32
            HA_DISCONNECT_PM_SRVCS = 102; /* 000 Disconnect From*/
        const UInt32
            HA_QUERY_WINDOW_COORDS = 103; /* 000 Query Presentation*/
        const UInt32
            HA_PM_WINDOW_STATUS = 104; /* 000 PM Window Status*/
        const UInt32
            HA_CHANGE_SWITCH_NAME = 105; /* 000 Change Switch List*/
        const UInt32
            HA_CHANGE_WINDOW_NAME = 106; /* 000 Change PS Window*/
        const UInt32
            HA_START_PLAYING_MACRO = 110; /* 000 Start playing macro*/
        const UInt32
            HA_START_STRUCTURED_FLD = 120; /* 000 Start Structured*/
        const UInt32
            HA_STOP_STRUCTURED_FLD = 121; /* 000 Stop Structured*/
        const UInt32
            HA_QUERY_BUFFER_SIZE = 122; /* 000 Query Communications*/
        const UInt32
            HA_ALLOCATE_COMMO_BUFF = 123; /* 000 Allocate*/
        const UInt32
            HA_FREE_COMMO_BUFF = 124; /* 000 Free Communications*/
        const UInt32
            HA_GET_ASYNC_COMPLETION = 125; /* 000 Get Asynchronous*/
        const UInt32
            HA_READ_STRUCTURED_FLD = 126; /* 000 Read Structured Field*/
        const UInt32
            HA_WRITE_STRUCTURED_FLD = 127; /* 000 Write Structured*/


        //********************************************************************/ 

        //******************* EHLLAPI RETURN CODES***************************/ 

        //********************************************************************/ 
        const UInt32
            HARC_SUCCESS = 0; /* 000 Good return code.*/
        const UInt32
            HARC99_INVALID_INP = 0; /* 000 Incorrect input*/
        const UInt32
            HARC_INVALID_PS = 1; /* 000 Invalid PS, Not*/
        const UInt32
            HARC_BAD_PARM = 2; /* 000 Bad parameter, or*/
        const UInt32
            HARC_BUSY = 4; /* 000 PS is busy return*/
        const UInt32
            HARC_LOCKED = 5; /* 000 PS is LOCKed, or*/
        const UInt32
            HARC_TRUNCATION = 6; /* 000 Truncation*/
        const UInt32
            HARC_INVALID_PS_POS = 7; /* 000 Invalid PS*/
        const UInt32
            HARC_NO_PRIOR_START = 8; /* 000 No prior start*/
        const UInt32
            HARC_SYSTEM_ERROR = 9; /* 000 A system error*/
        const UInt32
            HARC_UNSUPPORTED = 10; /* 000 Invalid or*/
        const UInt32
            HARC_UNAVAILABLE = 11; /* 000 Resource is*/
        const UInt32
            HARC_SESSION_STOPPED = 12; /* 000 Session has*/
        const UInt32
            HARC_BAD_MNEMONIC = 20; /* 000 Illegal mnemonic*/
        const UInt32
            HARC_OIA_UPDATE = 21; /* 000 A OIA update*/
        const UInt32
            HARC_PS_UPDATE = 22; /* 000 A PS update*/
        const UInt32
            HARC_PS_AND_OIA_UPDATE = 23; /* A PS and OIA update*/
        const UInt32
            HARC_STR_NOT_FOUND_UNFM_PS = 24; /* 000 String not found,*/
        const UInt32
            HARC_NO_KEYS_AVAIL = 25; /* 000 No keys available*/
        const UInt32
            HARC_HOST_UPDATE = 26; /* 000 A HOST update*/
        const UInt32
            HARC_FIELD_LEN_ZERO = 28; /* 000 Field length = 0*/
        const UInt32
            HARC_QUEUE_OVERFLOW = 31; /* 000 Keystroke queue*/
        const UInt32
            HARC_ANOTHER_CONNECTION = 32; /* 000 Successful. Another*/
        const UInt32
            HARC_INBOUND_CANCELLED = 34; /* 000 Inbound structured*/
        const UInt32
            HARC_OUTBOUND_CANCELLED = 35; /* 000 Outbound structured*/
        const UInt32
            HARC_CONTACT_LOST = 36; /* 000 Contact with the*/
        const UInt32
            HARC_INBOUND_DISABLED = 37; /* 000 Host structured field*/
        const UInt32
            HARC_FUNCTION_INCOMPLETE = 38; /* 000 Requested Asynchronous*/
        const UInt32
            HARC_DDM_ALREADY_EXISTS = 39; /* 000 Request for DDM*/
        const UInt32
            HARC_ASYNC_REQUESTS_OUT = 40; /* 000 Disconnect successful.*/
        const UInt32
            HARC_MEMORY_IN_USE = 41; /* 000 Memory cannot be freed*/
        const UInt32
            HARC_NO_MATCH = 42; /* 000 No pending*/
        const UInt32
            HARC_OPTION_INVALID = 43; /* 000 Option requested is*/
        const UInt32
            HARC99_INVALID_PS = 9998; /* 000 An invalid PS id*/
        const UInt32
            HARC99_INVALID_CONV_OPT = 9999; /* 000 Invalid convert*/


        public UInt32 Connect(string sessionID)
        {
            StringBuilder Data = new StringBuilder(4);
            Data.Append(sessionID);
            UInt32 rc = 0;
            UInt32 f = HA_CONNECT_PS;
            UInt32 l = 4;
            return EhllapiFunc.hllapi(out f, Data, out l, out rc);
        }

        public UInt32 Disconnect(string sessionID)
        {
            StringBuilder Data = new StringBuilder(4);
            Data.Append(sessionID);
            UInt32 rc = 0;
            UInt32 f = HA_DISCONNECT_PS;
            UInt32 l = 4;
            return EhllapiFunc.hllapi(out f, Data, out l, out rc);
        }

        public UInt32 SetCursorPos(int p)
        {
            StringBuilder Data = new StringBuilder(0);
            UInt32 rc = (UInt32)p;
            UInt32 f = HA_SET_CURSOR;
            UInt32 l = 0;
            return EhllapiFunc.hllapi(out f, Data, out l, out rc);
        }

        public UInt32 GetCursorPos(out int p)
        {
            StringBuilder Data = new StringBuilder(0);
            UInt32 rc = 0;
            UInt32 f = HA_QUERY_CURSOR_LOC;
            UInt32 l = 0; //return position
            UInt32 r = EhllapiFunc.hllapi(out f, Data, out l, out rc);
            p = (int)l;
            return r;
        }

        public UInt32 SendStr(string cmd)
        {
            StringBuilder Data = new StringBuilder(cmd.Length);
            Data.Append(cmd);
            UInt32 rc = 0;
            UInt32 f = HA_SENDKEY;
            UInt32 l = (UInt32)cmd.Length;
            return EhllapiFunc.hllapi(out f, Data, out l, out rc);
        }

        public UInt32 ReadScreen(int position, int len, out string txt)
        {
            StringBuilder Data = new StringBuilder(3000);
            UInt32 rc = (UInt32)position;
            UInt32 f = HA_COPY_PS_TO_STR;
            UInt32 l = (UInt32)len;
            UInt32 r = EhllapiFunc.hllapi(out f, Data, out l, out rc);
            txt = Data.ToString();
            return r;
        }

        public UInt32 Wait()
        {
            StringBuilder Data = new StringBuilder(0);
            UInt32 rc = 0;
            UInt32 f = HA_WAIT;
            UInt32 l = 0;
            UInt32 r = EhllapiFunc.hllapi(out f, Data, out l, out rc);
            return r;
        }


    }
}
