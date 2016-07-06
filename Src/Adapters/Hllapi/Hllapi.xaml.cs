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
using Microsoft.USD.ComponentLibrary.Adapters;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for Hllapi.xaml
    /// </summary>
    public partial class Hllapi : SetParentBase
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private TraceLogger LogWriter = null;

        private bool _isConnected = false;
        EhllapiWrapper hllapiWrapper = new EhllapiWrapper();

        public bool IsConnected { get { return _isConnected; } }
        private string sessionid;
        private string hllapiDll;
        #endregion

        #region Constants

        // Rows and columns shown on the screen
        private const int SCREENROWS = 24;
        private const int SCREENCOLUMNS = 80;

        protected const int LenShortDate = 6;
        protected const int LenLongDate = 8;

        #endregion

        #region Mainframe Commands
        public Dictionary<string, string> MainframeCommands = new Dictionary<string, string>()
        {
            //  Commands uses in the mainframe...
            {"BACKSPACE", "@<"},
            {"CLEAR", "@C"},
            {"CURSOR_LEFT", "@L"},
            {"DELETE", "@D"},
            {"DELETE_CHARACTER", ">"},
            {"ENTER", "@E"},
            {"ERASE_EOF", "@F"},
            {"HOME", "@0"},
            {"INSERT", "@I"},
            {"JUMP", "@J"},
            {"NEWLINE", "@N"},
            {"LEFTTAB", "@B"},
            {"RIGHTTAB", "@T"},
            {"RESET", "@R"},
            {"SPACE", "@O"},
            {"F1", "@1"},
            {"F2", "@2"},
            {"F3", "@3"},
            {"F4", "@4"},
            {"F5", "@5"},
            {"F6", "@6"},
            {"F7", "@7"},
            {"F8", "@8"},
            {"F9", "@9"},
            {"F10", "@a"},
            {"F11", "@b"},
            {"F12", "@c"},
            {"F13", "@d"},
            {"F14", "@e"},
            {"F15", "@f"},
            {"F16", "@g"},
            {"F17", "@h"},
            {"F18", "@i"},
            {"F19", "@j"},
            {"F20", "@k"},
            {"F21", "@l"},
            {"F22", "@m"},
            {"F23", "@n"},
            {"F24", "@o"},
            {"PA1", "@x"},
            {"PA2", "@y"},
            {"PA3", "@z"}
        };
        #endregion

        /// <summary>
		/// UII Constructor 
		/// </summary>
		/// <param name="appID">ID of the application</param>
		/// <param name="appName">Name of the application</param>
		/// <param name="initString">Initializing XML for the application</param>
		public Hllapi(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();

            // This will create a log writer with the default provider for Unified Service desk
            LogWriter = new TraceLogger();

            #region Enhanced LogProvider Info
            // This will create a log writer with the same name as your hosted control. 
            // LogWriter = new TraceLogger(traceSourceName:"MyTraceSource");

            // If you utilize this feature,  you would need to add a section to the system.diagnostics settings area of the UnifiedServiceDesk.exe.config
            //<source name="MyTraceSource" switchName="MyTraceSwitchName" switchType="System.Diagnostics.SourceSwitch">
            //    <listeners>
            //        <add name="console" type="System.Diagnostics.DefaultTraceListener"/>
            //        <add name="fileListener"/>
            //        <add name="USDDebugListener" />
            //        <remove name="Default"/>
            //    </listeners>
            //</source>

            // and then in the switches area : 
            //<add name="MyTraceSwitchName" value="Verbose"/>

            #endregion

        }

        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            // this will populate any toolbars assigned to this control in config. 
            PopulateToolbars(ProgrammableToolbarTray);
            WindowAttached += new EventHandler<EventArgs>(WindowAttached_Handler);
            base.DesktopReady();
        }

        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            if (args.Action.Equals("Connect", StringComparison.InvariantCultureIgnoreCase))
            {
                sessionid = Utility.GetAndRemoveParameter(parameters, "sessionid");
                string captionRegex = Utility.GetAndRemoveParameter(parameters, "captionRegex");
                string pathToLaunch = Utility.GetAndRemoveParameter(parameters, "pathToLaunch");
                hllapiDll = Utility.GetAndRemoveParameter(parameters, "hllapiDll");
                if (!String.IsNullOrEmpty(pathToLaunch))
                {
                    Process p = Process.Start(pathToLaunch);
                    ProcessAttachApplicationWindow(p, myhost);
                }
                else if (!String.IsNullOrEmpty(captionRegex))
                {
                    LookForAndAttachApplicationWindow(captionRegex, myhost);
                }
            }
            else if (args.Action.Equals("Disconnect", StringComparison.InvariantCultureIgnoreCase))
            {
                Disconnect();
            }
            else
            {
                if (_isConnected == false && args.Action != "default")
                    throw new Exception("Not connected to session");
                if (args.Action.Equals("SendCommand", StringComparison.InvariantCultureIgnoreCase))
                {
                    string command = Utility.GetAndRemoveParameter(parameters, "command");
                    if (MainframeCommands.ContainsKey(command))
                    {
                        SendCommand(MainframeCommands[command]);
                    }
                    else
                    {
                        throw new Exception("Unknown command");
                    }
                }
                else if (args.Action.Equals("ScreenContains", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string searchText = Utility.GetAndRemoveParameter(parameters, "searchText");
                    if (!String.IsNullOrEmpty(row) && String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(searchText))
                        args.ActionReturnValue = ScreenContains(int.Parse(row), searchText).ToString();
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(searchText))
                        args.ActionReturnValue = ScreenContains(int.Parse(row), int.Parse(col), searchText).ToString();
                    else if (String.IsNullOrEmpty(row) && String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(searchText))
                        args.ActionReturnValue = ScreenContains(searchText).ToString();
                }
                else if (args.Action.Equals("ReadText", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string endrow = Utility.GetAndRemoveParameter(parameters, "endrow");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string len = Utility.GetAndRemoveParameter(parameters, "length");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len))
                        args.ActionReturnValue = ReadText(int.Parse(row), int.Parse(col), int.Parse(len));
                    else if (!String.IsNullOrEmpty(row) && String.IsNullOrEmpty(endrow))
                        args.ActionReturnValue = ReadText(int.Parse(row));
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(endrow))
                        args.ActionReturnValue = ReadText(int.Parse(row), int.Parse(endrow));
                    else
                        args.ActionReturnValue = ReadTextAll();
                }
                else if (args.Action.Equals("ReadInteger", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string len = Utility.GetAndRemoveParameter(parameters, "length");
                    string def = Utility.GetAndRemoveParameter(parameters, "default");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadInteger(int.Parse(row), int.Parse(col), int.Parse(len)).ToString();
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len) && !String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadInteger(int.Parse(row), int.Parse(col), int.Parse(len), int.Parse(def)).ToString();
                }
                else if (args.Action.Equals("ReadLong", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string len = Utility.GetAndRemoveParameter(parameters, "length");
                    string def = Utility.GetAndRemoveParameter(parameters, "default");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadLong(int.Parse(row), int.Parse(col), int.Parse(len)).ToString();
                }
                else if (args.Action.Equals("ReadDouble", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string len = Utility.GetAndRemoveParameter(parameters, "length");
                    string def = Utility.GetAndRemoveParameter(parameters, "default");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len) && !String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadDouble(int.Parse(row), int.Parse(col), int.Parse(len), double.Parse(def)).ToString();
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadDouble(int.Parse(row), int.Parse(col), int.Parse(len)).ToString();
                }
                else if (args.Action.Equals("ReadYesNo", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col))
                        args.ActionReturnValue = ReadYesNo(int.Parse(row), int.Parse(col)).ToString();
                }
                else if (args.Action.Equals("ReadShortDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string def = Utility.GetAndRemoveParameter(parameters, "default");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadShortDate(int.Parse(row), int.Parse(col)).ToString();
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadShortDate(int.Parse(row), int.Parse(col), DateTime.Parse(def)).ToString();
                }
                else if (args.Action.Equals("ReadLongDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string def = Utility.GetAndRemoveParameter(parameters, "default");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadLongDate(int.Parse(row), int.Parse(col)).ToString();
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(def))
                        args.ActionReturnValue = ReadLongDate(int.Parse(row), int.Parse(col), DateTime.Parse(def)).ToString();
                }
                else if (args.Action.Equals("WriteShortDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string val = Utility.GetAndRemoveParameter(parameters, "value");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(val))
                        WriteShortDate(int.Parse(row), int.Parse(col), DateTime.Parse(val));
                }
                else if (args.Action.Equals("WriteLongDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string val = Utility.GetAndRemoveParameter(parameters, "value");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && String.IsNullOrEmpty(val))
                        WriteLongDate(int.Parse(row), int.Parse(col), DateTime.Parse(val));
                }
                else if (args.Action.Equals("WriteText", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string text = Utility.GetAndRemoveParameter(parameters, "text");
                    string verify = Utility.GetAndRemoveParameter(parameters, "verify");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(text) && String.IsNullOrEmpty(verify))
                        WriteText(int.Parse(row), int.Parse(col), text);
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(text) && !String.IsNullOrEmpty(verify))
                        WriteText(int.Parse(row), int.Parse(col), text, bool.Parse(verify));
                }
                else if (args.Action.Equals("WriteNumber", StringComparison.InvariantCultureIgnoreCase))
                {
                    int iObj;
                    long lObj;
                    double dObj;
                    decimal decObj;
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string num = Utility.GetAndRemoveParameter(parameters, "num");
                    string width = Utility.GetAndRemoveParameter(parameters, "width");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(num) && !String.IsNullOrEmpty(width) && int.TryParse(num, out iObj))
                        WriteNumber(int.Parse(row), int.Parse(col), int.Parse(num), int.Parse(width));
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(num) && !String.IsNullOrEmpty(width) && long.TryParse(num, out lObj))
                        WriteNumber(int.Parse(row), int.Parse(col), long.Parse(num), int.Parse(width));
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(num) && !String.IsNullOrEmpty(width) && double.TryParse(num, out dObj))
                        WriteNumber(int.Parse(row), int.Parse(col), double.Parse(num), int.Parse(width));
                    else if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(num) && !String.IsNullOrEmpty(width) && decimal.TryParse(num, out decObj))
                        WriteNumber(int.Parse(row), int.Parse(col), decimal.Parse(num), int.Parse(width));
                }
                else if (args.Action.Equals("WriteYesNo", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string val = Utility.GetAndRemoveParameter(parameters, "value");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(val))
                        WriteYesNo(int.Parse(row), int.Parse(col), bool.Parse(val));
                }
                else if (args.Action.Equals("WaitForDisplayContent", StringComparison.InvariantCultureIgnoreCase))
                {
                    string timeoutInMilliseconds = Utility.GetAndRemoveParameter(parameters, "timeoutInMilliseconds");
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string len = Utility.GetAndRemoveParameter(parameters, "length");
                    if (parameters.Count > 0 && String.IsNullOrEmpty(row) && String.IsNullOrEmpty(col) && String.IsNullOrEmpty(len))
                    {
                        List<String> remainderArray = new List<string>();
                        foreach (KeyValuePair<string, string> p in parameters)
                            remainderArray.Add(p.Value);
                        if (String.IsNullOrEmpty(timeoutInMilliseconds))
                            WaitForDisplayContent(3, remainderArray.ToArray());
                        else
                            WaitForDisplayContent(int.Parse(timeoutInMilliseconds), remainderArray.ToArray());
                    }
                    else if (parameters.Count == 0 && !String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(len))
                    {
                        if (String.IsNullOrEmpty(timeoutInMilliseconds))
                            WaitForDisplayContent(15, int.Parse(row), int.Parse(col), int.Parse(len));
                        else
                            WaitForDisplayContent(15, int.Parse(row), int.Parse(col), int.Parse(len));
                    }
                }
                else if (args.Action.Equals("CalculatePosition", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col))
                        args.ActionReturnValue = CalculatePosition(int.Parse(row), int.Parse(col)).ToString();
                }
                else if (args.Action.Equals("ReadDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    string row = Utility.GetAndRemoveParameter(parameters, "row");
                    string col = Utility.GetAndRemoveParameter(parameters, "col");
                    string width = Utility.GetAndRemoveParameter(parameters, "width");
                    if (!String.IsNullOrEmpty(row) && !String.IsNullOrEmpty(col) && !String.IsNullOrEmpty(width))
                        args.ActionReturnValue = ReadDate(int.Parse(row), int.Parse(col), int.Parse(width)).ToString();
                }
            }

            base.DoAction(args);
        }

        private void WindowAttached_Handler(object sender, EventArgs args)
        {
            Connect(hllapiDll, sessionid);
        }

        public override void Close()
        {
            try
            {
                this.Disconnect();
            }
            catch
            {
            }
            base.Close();
        }

        /// <summary>
        /// Raised when a context change occurs in USD
        /// </summary>
        /// <param name="context"></param>
        public override void NotifyContextChange(Microsoft.Uii.Csr.Context context)
        {
            base.NotifyContextChange(context);
        }

        #region Wrapper Functions
        protected void Connect(string hllapiDll, string sessionid)
        {
            if (!_isConnected)
            {
                //bool suc = hllapiWrapper.LoadDll(hllapiDll);
                uint del = hllapiWrapper.Connect(sessionid);
                uint wait = hllapiWrapper.Wait();
                this.sessionid = sessionid;

                //int res = hllapiWrapper.Startup();

                _isConnected = true;
            }
            else
            {
                throw new Exception("Already connected to sessionid=" + sessionid);
            }
        }

        protected void Disconnect()
        {
            if (_isConnected)
            {
                if (myhost != null)
                    myhost.Child = null;
                hllapiWrapper.Disconnect(sessionid);
                _isConnected = false;
            }
            else
            {
                throw new Exception("Not connected to session");
            }
        }

        //<summary>
        //Send a Mainframe command to the mainframe through the Ehllapi Wrapper.
        //</summary>
        //<param name="command"></param>
        //<remarks></remarks>
        protected void SendCommand(string command)
        {
            //  Send the command to the mainframe
            uint result = hllapiWrapper.SendStr(command);
            if (result != 0)
            {
                throw new Exception("");
            }
            hllapiWrapper.Wait();
        }


        //<summary>
        //Check to see if the current mainframe screen contains the specified string at a specified location.
        //</summary>
        //<param name="row">Row number where the text should be found.</param>
        //<param name="col">Column number where the text should be found.</param>
        //<param name="searchText">Text being searched for.</param>
        //<returns>True if the text is found, false if it is not.</returns>
        //<remarks></remarks>
        protected bool ScreenContains(int row, int col, string searchText)
        {
            //  Read the screen text at the specified position.
            string screenText = ReadText(row, col, searchText.Length);
            //  If the text on the screen matches what was specified, return true, else false.
            if ((screenText == searchText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool ScreenContains(int row, string searchText)
        {
            //  Read the screen text at the specified position.
            string screenText = ReadText(row);
            //  If the text on the screen matches what was specified, return true, else false.
            if (screenText.Contains(searchText))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //<summary>
        //Check to see if the current mainframe screen contains the specified string anywhere on the screen.
        //</summary>
        //<param name="searchText">Text being searched for.</param>
        //<returns>True if the text is found, false if it is not.</returns>
        //<remarks></remarks>
        protected bool ScreenContains(params string[] searchText)
        {
            //  Read all the text on the screen...
            string screenText = ReadTextAll().ToUpper();

            //  If the screen text contains the specified screen text, return true, else false.
            foreach (string text in searchText)
            {
                if (screenText.Contains(text.ToUpper()))
                    return true;
            }

            return false;
        }

        //<summary>
        //Check to see if the current mainframe screen contains the specified string in the specified screen row.
        //</summary>
        //<param name="row"></param>
        //<returns></returns>
        //<remarks></remarks>
        protected string ReadText(int row)
        {
            //  Ensure that the row value is a valid value.
            if (((row > SCREENROWS)
                        || (row < 0)))
            {
                throw new ArgumentException("Invalid row number to read.");
            }
            //  Read all the text in the specified row.
            return ReadText(row, 1, SCREENCOLUMNS);
        }

        protected string ReadText(int startRow, int endRow)
        {
            //  Ensure that the row value is a valid value.
            if (((startRow > SCREENROWS)
                        || (startRow < 0)))
            {
                throw new ArgumentException("Invalid start row number.");
            }
            if (((endRow > SCREENROWS)
                        || ((endRow < 0)
                        || (endRow < startRow))))
            {
                throw new ArgumentException("Invalid end row number.");
            }
            string screenText = String.Empty;
            for (int i = startRow; (i <= endRow); i++)
            {
                screenText = (screenText + ReadText(i, 1, SCREENCOLUMNS));
            }
            return screenText;
        }

        //<summary>
        //Reads text from the current mainframe screen at the specified location and length.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="lenToRead">Number of positions to read from the starting row and column positions.</param>
        //<returns>Returns the text at the specified position.</returns>
        //<remarks></remarks>
        protected string ReadText(int row, int col, int lenToRead)
        {
            try
            {
                //  Read the mainframe screen at the specified position
                string screenText = String.Empty;
                hllapiWrapper.ReadScreen(CalculatePosition(row, col), lenToRead, out screenText);
                //  Ensure that the text return from the hllapiWrapper is of a length of at least the requested amount.
                if ((screenText.Length < lenToRead))
                    throw new Exception("Screen scrap failed. Text length returned from the screen scrape does not match the length requested.");

                //  The hllapiWrapper read screen method returns more data than necessary. 
                //  Trim the returned string to the requested amount.
                screenText = screenText.Substring(0, lenToRead);
                //  Return the text to the caller.
                return screenText;
            }
            catch (Exception ex)
            {
                throw new Exception("Screen scrap failed. Could not read data on screen.", ex);
            }
        }

        //<summary>
        //Reads all the test off of the current mainframe screen.
        //</summary>
        //<returns></returns>
        //<remarks></remarks>
        protected string ReadTextAll()
        {
            int lengthToRead = (SCREENCOLUMNS * SCREENROWS);
            //  Read all the text on the screen
            string screenText = String.Empty;
            hllapiWrapper.ReadScreen(CalculatePosition(1, 1), lengthToRead, out screenText);
            if ((screenText.Length < lengthToRead))
            {
                return screenText;
                //throw new Exception(string.Format("Screen text read is not of expected length of \'{0}\'. ", lengthToRead));
            }
            else
            {
                return screenText.Substring(0, lengthToRead);
            }
        }

        //<summary>
        //Reads an integer from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="lenToRead">Number of positions to read from the starting row and column positions.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected int ReadInteger(int row, int col, int lenToRead)
        {
            //  Read the requested text from the mainframe
            string text = ReadText(row, col, lenToRead).Trim();
            //  Try to parse the data into an Integer type else throw an error if it can't.
            int returnValue;
            if (int.TryParse(text, out returnValue))
            {
                return returnValue;
            }
            else
            {
                string msg = "Screen scrape data , \'{0}\' (from row {1}, column {2}, length {3}), could not be converted" +
                " to type Integer.";
                msg = string.Format(msg, text, row, col, lenToRead);
                throw new Exception(msg);
            }
        }

        //<summary>
        //Reads an integer from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="len">Number of positions to read from the starting row and column positions.</param>
        //<param name="defaultValue"></param>
        //<returns></returns>
        //<remarks></remarks>
        protected int ReadInteger(int row, int col, int len, int defaultValue)
        {
            //  Read the requested text from the mainframe
            string text = ReadText(row, col, len).Trim();
            //  Try to parse the data into an Integer type else return the default value if it cant.
            int returnValue;
            if (int.TryParse(text, out returnValue))
            {
                return returnValue;
            }
            else
            {
                return defaultValue;
            }
        }

        //<summary>
        //Reads a long value from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="lenToRead">Number of positions to read from the starting row and column positions.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected long ReadLong(int row, int col, int lenToRead)
        {
            //  Read the requested text from the mainframe
            string text = ReadText(row, col, lenToRead).Trim();
            //  Try to parse the data into an Long type else throw an error if it can't.
            long returnValue;
            if (long.TryParse(text, out returnValue))
            {
                return returnValue;
            }
            else
            {
                string msg = "Screen scrape data , \'{0}\' (from row {1}, column {2}, length {3}), could not be converted" +
                " to type Long.";
                msg = string.Format(msg, text, row, col, lenToRead);
                throw new Exception(msg);
            }
        }

        protected double ReadDouble(int row, int col, int lenToRead, double defaultValue)
        {
            //  Read the requested text from the mainframe
            string text = ReadText(row, col, lenToRead).Trim();
            //  Try to parse the data into an Double type else throw an error if it can't.
            double returnValue;
            if (double.TryParse(text, out returnValue))
            {
                return returnValue;
            }
            else
            {
                return defaultValue;
            }
        }

        //<summary>
        //Reads a double value from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="lenToRead">Number of positions to read from the starting row and column positions.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected double ReadDouble(int row, int col, int lenToRead)
        {
            //  Read the requested text from the mainframe
            string text = ReadText(row, col, lenToRead).Trim();
            //  Try to parse the data into an Double type else throw an error if it can't.
            double returnValue;
            if (double.TryParse(text, out returnValue))
            {
                return returnValue;
            }
            else
            {
                string msg = "Screen scrape data , \'{0}\' (from row {1}, column {2}, length {2}), could not be converted" +
                " to type Double.";
                msg = string.Format(msg, text, row, col, lenToRead);
                throw new Exception(msg);
            }
        }

        //<summary>
        //Reads a boolean value from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected bool ReadYesNo(int row, int col)
        {
            string text = ReadText(row, col, 1).Trim().ToUpper();
            //  Try to parse the data into an Boolean type else throw an error if it can't.
            if ((text == "Y"))
            {
                return true;
            }
            else if (text == "N" || text.Length == 0)
            {
                return false;
            }
            else
            {
                string msg = "Screen scrape data , \'{0}\' (from row {1}, column {2}, length {3}), could not be converted" +
                " to type Boolean.";
                msg = string.Format(msg, text, row, col, 1);
                throw new Exception(msg);
            }
        }

        //<summary>
        //Reads a short date from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected DateTime ReadShortDate(int row, int col)
        {
            return ReadDate(row, col, LenShortDate);
        }

        //<summary>
        //Reads a short date from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="defaultDate">Default date to return if the requested date could not be parsed from the mainframe.</param>d
        //<returns></returns>
        //<remarks></remarks>
        protected DateTime ReadShortDate(int row, int col, DateTime defaultDate)
        {
            DateTime d = ReadDate(row, col, LenShortDate);
            if ((d == null))
            {
                return defaultDate.Date;
            }
            else
            {
                return d.Date;
            }
        }

        //<summary>
        //Reads a long date from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<returns></returns>
        //<remarks></remarks>
        protected DateTime ReadLongDate(int row, int col)
        {
            return ReadDate(row, col, LenLongDate);
        }

        //<summary>
        //Reads a long date from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="defaultDate">Default date to return if the requested date could not be parsed from the mainframe.</param>d
        //<returns></returns>
        //<remarks></remarks>
        protected DateTime ReadLongDate(int row, int col, DateTime defaultDate)
        {
            DateTime d = ReadDate(row, col, LenLongDate);
            if ((d == null))
            {
                return defaultDate;
            }
            else
            {
                return d;
            }
        }

        //<summary>
        //Writes the specified date to the specified location on the current mainframe frame screen.
        //</summary>
        //<param name="row">Row position where to begin writing.</param>
        //<param name="col">Column position where to begin writing.</param>
        //<param name="d">Date value to write to the screen.</param>
        //<remarks></remarks>
        protected void WriteShortDate(int row, int col, DateTime d)
        {
            string dateText;
            //  If the date is null, place blank spaces in the field, else the formatted date.
            if ((d == null))
            {
                dateText = "".PadRight(6);
            }
            else
            {
                dateText = d.ToString("MMddyy");
            }
            WriteText(row, col, dateText);
        }

        //<summary>
        //Writes the specified date to the specified location on the current mainframe frame screen.
        //</summary>
        //<param name="row">Row position where to begin writing.</param>
        //<param name="col">Column position where to begin writing.</param>
        //<param name="d">Date value to write to the screen.</param>
        //<remarks></remarks>
        protected void WriteLongDate(int row, int col, DateTime d)
        {
            string dateText;
            //  If the date is null, place blank spaces in the field, else the formatted date.
            if ((d == null))
            {
                dateText = "".PadRight(8);
            }
            else
            {
                dateText = d.ToString("MMddyyyy");
            }
            WriteText(row, col, dateText);
        }

        //<summary>
        //Writes a string the specified location on the current mainframe frame screen.
        //</summary>
        //<param name="row">Row position where to begin writing.</param>
        //<param name="col">Column position where to begin writing.</param>
        //<param name="text">Text to write to the mainframe screen.</param>
        //<remarks></remarks>
        protected void WriteText(int row, int col, string text)
        {
            this.WriteText(row, col, text, true);
        }

        //<summary>
        //Writes a string the specified location on the current mainframe frame screen.
        //</summary>
        //<param name="row">Row position where to begin writing.</param>
        //<param name="col">Column position where to begin writing.</param>
        //<param name="text">Text to write to the mainframe screen.</param>
        //<remarks></remarks>
        protected void WriteText(int row, int col, string text, bool verifyWrite)
        {
            try
            {
                //  Set the cursor to the requested position
                hllapiWrapper.SetCursorPos(CalculatePosition(row, col));
                //  Write the specified text to that position
                hllapiWrapper.SendStr(text);
                hllapiWrapper.Wait();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not write to screen.", ex);
            }
            if (verifyWrite)
            {
                if ((this.ReadText(row, col, text.Length) != text))
                {
                    throw new Exception(string.Format("Attempting to write \'{0}\' at row {1}, column {2} failed. Possible cause not enough space in field to " +
                            "write string.", text, row, col));
                }
            }
        }

        protected void WriteNumber(int row, int col, int num, int width)
        {
            WriteText(row, col, num.ToString().PadLeft(width, '0'));
        }
        protected void WriteNumber(int row, int col, long num, int width)
        {
            WriteText(row, col, num.ToString().PadLeft(width, '0'));
        }
        protected void WriteNumber(int row, int col, double num, int width)
        {
            WriteText(row, col, num.ToString().PadLeft(width, '0'));
        }
        protected void WriteNumber(int row, int col, decimal num, int width)
        {
            WriteText(row, col, num.ToString().PadLeft(width, '0'));
        }

        //<summary>
        // Writes Y or N to the specified location on the current mainframe frame screen.
        //</summary>
        //<param name="row">Row position where to begin writing.</param>
        //<param name="col">Column position where to begin writing.</param>
        //<param name="val">Boolean value used to write Y or N on the screen.</param>
        //<remarks></remarks>
        protected void WriteYesNo(int row, int col, bool val)
        {
            //  Write a Y to represent TRUE and N to represent FALSE.
            if (val)
            {
                WriteText(row, col, "Y");
            }
            else
            {
                WriteText(row, col, "N");
            }
        }



        //<summary>
        //Sleeps the application until the specified text prompt is found on the screen. 
        //The method also contains a time-out and will throw an error when the time out period is reached.
        //</summary>
        //<param name="prompt"></param>
        //<remarks></remarks>
        protected void WaitForDisplayContent(int timeoutInSeconds, params string[] prompts)
        {
            if (prompts.Length == 0)
                return;

            //  Timeout amount in seconds
            int seconds = timeoutInSeconds;

            //  Loop until you find the prompt or until the timeout is reached...
            for (int i = 1; (i <= (seconds * 4)); i++)
            {
                foreach (string prompt in prompts)
                {
                    Trace.WriteLine(("Waiting for prompt..." + (i.ToString() + ("..." + prompt))));
                    if (ScreenContains(prompt))
                        return;
                }
                System.Threading.Thread.Sleep(250);
            }
            //  Timeout is reached, throw an error...
            throw new Exception(string.Format("Waiting for prompt(s) ('{0}') time out.", string.Join("', '", prompts)));
        }

        /// <summary>
        /// Sleeps the application until any text is found on the screen at the specified location. 
        /// The method also contains a time-out and will throw an error when the time out period is reached.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="length"></param>
        protected void WaitForDisplayContent(int timeoutInSeconds, int row, int column, int length)
        {
            //  Timeout amount in seconds
            int seconds = timeoutInSeconds;
            //  Loop until you find the prompt or until the timeout is reached...
            for (int i = 1; (i
                        <= (seconds * 4)); i++)
            {
                if ((ReadText(row, column, length).Trim().Length == length))
                {
                    return;
                }

                Trace.WriteLine(("Waiting for text..."
                                + (i.ToString() + "...")));
                System.Threading.Thread.Sleep(250);
            }
            //  Timeout is reached, throw an error...
            throw new Exception(string.Format("Waiting for text of length \'{0}\' timed out.", length));
        }

        //<summary>
        //Calculates an index position for the EHLLAPI class based on our row/col position.
        //</summary>
        //<param name="row"></param>
        //<param name="col"></param>
        //<returns></returns>
        //<remarks></remarks>
        private int CalculatePosition(int row, int col)
        {
            return (((row - 1)
                        * SCREENCOLUMNS)
                        + col);
        }

        //<summary>
        //Reads a date from the specified location on the current mainframe screen.
        //</summary>
        //<param name="row">Row position where to begin reading.</param>
        //<param name="col">Column position where to begin reading.</param>
        //<param name="width">Number of positions to read from the starting row and column positions.</param>
        //<returns></returns>
        //<remarks></remarks>
        private DateTime ReadDate(int row, int col, int width)
        {
            string text = String.Empty;
            try
            {
                if ((width < 6))
                {
                    throw new ArgumentException("Cannot read dates that are less than 6 characters.");
                }
                //  Read the requested text from the mainframe
                text = ReadText(row, col, width);
                //  Return nothing if no date text was found
                if ((text.Trim().Length == 0))
                {
                    return DateTime.MinValue;
                }
                //  Parse the date parts from the text
                int month = int.Parse(text.Substring(0, 2));
                int day = int.Parse(text.Substring(2, 2));
                int year = int.Parse(text.Substring(4));
                //  If the year is a 2 digit year, covert it to a 4 digit year
                if ((year.ToString().Length <= 2))
                {
                    //  1993 is the year of the first HHM policy.
                    if ((year < 93))
                    {
                        //  If the parsed year is less than 93, assume it is a 2000 or later.
                        year = (year + 2000);
                    }
                    else
                    {
                        //  If the year is greater than 93 (less than 100 though), assume the date
                        //  is in between 1993 and 1999
                        year = (year + 1900);
                    }
                }
                if (((month < 1)
                            || (month > 12)))
                {
                    return DateTime.MinValue;
                }
                else if (((day < 1)
                            || (day > 31)))
                {
                    return DateTime.MinValue;
                }
                //  Convert the date parts to a date and return
                DateTime convertedDate = new DateTime(year, month, day);
                return convertedDate;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Screen scrape data, \'{0}\', could not be converted to type Date.", text));
            }
        }
        #endregion
    }
}
