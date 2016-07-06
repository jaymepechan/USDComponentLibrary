# USDComponentLibrary
Component Library for Unified Service Desk

Copy your C:\Program Files\Microsoft Dynamics CRM USD\USD contents to the SupportLibs folder


Controls that are available and how to configure them:

AppAttachForm
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.AppAttachForm
Actions:
1) AttachWindow - Given a handle, it will attach the desktop window to the hosted control
	windowhandle - parameter to specify the window handle (numeric either base 10 or hex without the 0x)
2) DetachWindow - Detach the currently attached window from the hosted control
3) CloseWindow - Close the attached window and leave the hosted control blank
Description:
This control was created to allow an arbitrary window application to be attached to a tab. Originally envisioned to be used with ExternalListerner 
so that an application can request that one of it's windows should be attached, this could work with other controls as long as they can determine a window handle to pass to the action.

ExternalListener
USD Type: CTI Desktop Manager
Global: True
Type: Microsoft.USD.ComponentLibrary.ExternalListener
Actions:
1) StartListener - Start listing for a local connection for externally sourced events.  This action must be called to startup the listening process.  Generally it is called in DesktopReady
	prefix - The URL that would be used for listening on the localmachine. This should resemble the following: http://localhost:5000/
2) SetTimeout - When the external application requests data from the ExternalListener, this is the amount of time that is allowed to elapse before an empty is returned to the client. The default is 60 seconds. To be effective, this should be between the time it takes to retrieve the data and the timeout value of the browser.
3) SendMessage - Requests (GET) a URL and returns the response
4) SetData - For requests that expect a response, this is how the configuration will set the return data to make it available for use in the response.
	eventname - This is the name of the event that prompted the original request
	data - This is the data to respond to the web request with
Description:
This component is designed to allow for application outside USD to communicate with USD. This component allows the admin to specify events and to control what features and capabilities are available to the external applications. 
The admin defines the events that may be called from external. He does this by adding an event to the configuration.  The URL requested by the external application then specifies the event name in the querystring or POST message.
The following URL is an example of one that would trigger the TestEvent event and would wait for a response containing data (see SendData).  http://localhost:5000/?eventname=TestEvent&askresponse=true
One event has special meaning to support CTI requests. This eventname is ScreenPop.  This message will trigger the Window Navigation Route rules for CTI. 
http://localhost:5000/?eventname=ScreenPop&ani=1234&dnis=2345&calltype=phonecall&direction=inbound&variable1=value1

FormViewer
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.FormViewer
Actions:
1) ShowForm - Merges and displays the Form as configured in the form entity in CRM settings into it's content
	name - The name of the form as seen in the USD configuration settings in CRM
2) ReadForm - Reads the textboxes and other readable fields and places the data into the DataParameter list.
	clear - optional parameter that if set to true will replace the data in the DataParameter list for this application instead of merge it
3) ClearData - Clears the replacement parameter list for this application
Description:
This component is designed to generate and display forms as configured in the USD forms entity in CRM Settings. Things like buttons, when 
clicked will fire events and edit boxes or other data collection elements can be read using the ReadForm action.

Hllapi
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.Hllapi
Actions:
1) Connect - Specify either captionRegex to attach an already running session, or pathToLaunch to launch a new session and attach it to the window.
	captionRegex - Regular expression to find the window caption of an already running emulator session to attach to the window
	pathToLaunch - The path to launch the emulator session.
2) Disconnect - Disconnect the Hllapi session
NOTE: All the remaining commands require an active connection / hllapi session
3) SendCommand - Sends a command to the emulator
	command - BACKSPACE, CLEAR, CURSOR_LEFT, DELETE, DELETE_CHARACTER, ENTER, ERASE_EOF, HOME, INSERT, JUMP, NEWLINE, LEFTTAB, RIGHTTAB, RESET, 
			  SPACE, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24, PA1, PA2, PA3
4) ScreenContains - If the row and column are specified, the result is to search for the text at only this location. If the row or column are specified, it will look for the text anywhere in the row or column. If neither
					are specified it will look for the text on the entire screen.
	row - Optional parameter to specify the row to find the text.
	col - Optional parameter to specify the column to find the text.
	searchText - The text to find
5) ReadText - If row is specified by itself, it will return the whole line. If row and endrow are specified, it will return all the lines between and including these rows. if row, col, length are specified, it will capture "length" 
					characters starting at row,col.  If any other combination is specified, the full screen contents will be returned.
	row - The first row to capture for text.
	endrow - The last row to capture text.
	col - The column to start capturing text.
	length - The length of text to capture.
6) ReadInteger - 
	row - Row to find the number
	col - Col to find the number
	default - The default value if no number was found at the specified location
	length - The number of characters to read
7) ReadLong - 
	*Same as ReadInteger
8) ReadDouble - 
	*Same as ReadInteger
9) ReadYesNo - Looks for a Y or N at the specified position. throws an exception if it can't find this text at this position
	row - Row to find the value
	col - Col to find the value
10) ReadShortDate - Looks for date in form MMDDYY
	row - Row to find the date
	col - Col to find the date
	default - The default value if no date was found at the specified location
11) ReadLongDate - Looks for date in form MMDDYYYY
	row - Row to find the date
	col - Col to find the date
	default - The default value if no date was found at the specified location
12) WriteShortDate - 
	row - Row to place the date
	col - Col to place the date
	value - The value in the form MMDDYY
13) WriteLongDate - 
	row - Row to place the date
	col - Col to place the date
	value - The value in the form MMDDYYYY
14) WriteText
	row - Row to place the text
	col - Col to place the text
	text - The text to place at the location
	verify - Optional parameter, true means to verify that the data was written to the location.
15) WriteNumber
	row - Row to place the text
	col - Col to place the text
	num - The number to place at the location
	width - The number of digits to write
14) WriteYesNo
	row - Row to place the value
	col - Col to place the value
	value - The value in the form MMDDYYYY
15) WaitForDisplayContent
	timeoutInMilliseconds - (Optional) number of seconds to wait for timeout
	row - (Optional) Row to find text. When using this column, supply row, col, length.
	col - (Optional) Col to find text. When using this column, supply row, col, length.
	length - (Optional) The length of text to wait for at the specified row/col. The text must be this length (spaces get trimmed off) for this to succeed.  When using this column, supply row, col, length.
	[remainder of lines] - (Optional) Text to look for on the screen. Each line is a different string that must exist for this to succeed. All remaining lines need to be found to succeed. These will only be used if row, col, and length are not supplied
16) CalculatePosition - Calculates the position on the screen given the x,y coordinates. returns the result in $Return.
	row - row to locate the positon
	col - col to locate the position
17) ReadDate - Reads a date at the specified position.  Returns result in $Return
	row - row to locate the position
	col - col to locate the position
	width - Should be 6 (short date - MMDDYY) or 8 (long date - MMDDYYYY)

MCSBrowserCache
USD Type: USD Hosted Control
Global: True
Type: Microsoft.USD.ComponentLibrary.MCSBrowserCache
Actions:
1) SetCacheSize - Sets the cache size to preload browser instances.
	cachesize - The number of browser instance to precreate and initialize
	allowgrow - When we run out of cached entries, uninitialized instance will automatically be created and returned. This can mean that the number of outstanding browser instances
				exceeds the cachesize. If this is false, then when the returns instances exceed the cachesize, extras will be disposed.
2) ClearCache - Clears out and disposes of all items in the cache
Description:
This component is designed to initialize and maintain a cache of browser instances that may be used for displaying content.  This caching can dramatically improve performance as new instances can avoid the overhead of loading a IE instance.

MCSBrowser
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.MCSBrowser
Actions:
1) navigate - Navigate to the specified URL.
	url - The URL to navigate to
2) runscript - Injects script into the page
	[remainder of lines] - the javascript to execute in the context of the page
3) errors - turn on/off error hiding
	hide - True/False on whether to hide or show script errors to the user. The default is true.
4) closeactive - 
5) gohome - 
6) refresh - 
7) goback - 
8) goforward - 
9) reroute - 
10) runxrmcommand - 
11) startdialog - 
12) find - 
13) ScanForDataParameters - Scan the page for data parameters
14) LoadDataParameters - Load data parameters for this entity from the CRM web services (loads all data for contact by default)
Events:
1) BrowserDocumentComplete - called when DocumentComplete for the browser is fired. For CRM Pages, this is called each time a page is loaded that has a CRM form (usually once)
	url - Browser.Url
	location - The URL that was requested by Navigate or Routing Rules. When redirections occurs or when frames are involved, this may not be the same as the url parameter.
	title - The page title
2) PageLoadComplete - called when DocumentComplete for the browser is fired.
	url - Browser.Url
	location - The URL that was requested by Navigate or Routing Rules. When redirections occurs or when frames are involved, this may not be the same as the url parameter.
	title - The page title
3) PopupRouted - called when a popup has been routed with Window Navigation Rules
	SUBJECTURL - The URL of the popup that was routed.
4) CRMFormLoaded - Event fired only for CRM Pages to notify system that it contains a CRM form and that it is loaded.
	url - Browser.Url
	location - The URL that was requested by Navigate or Routing Rules. When redirections occurs or when frames are involved, this may not be the same as the url parameter.
	title - The page title
5) CRMDataAvailable - Fired after a call to ScanForDataParameters, once the data is available in the Data Parameter list for use.
6) [Custom Events] - This component supports custom events supporting http://event/?eventname=MyEvent&param1=value1 syntax.
Description:
This component is designed to provide a highly effecient browser instance. It is designed to work with the MCSBrowserCache to maintain initialized instances of the browser.
Currently the component supports non-CRM pages. Features may be added to support CRM pages soon.
This component DOES support being a target of Window Navigation Rules

ArticleList (Parature)
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.ArticleList
Actions:
1) Load - Loads the control and sets authentication parameters used in connecting to Parature
	AcctID - AcctId from Parature
	pageSize - (Optional) The number of results to display at a time.
	folderName - (Optional) The name of the folder or folder path to search within
	folderID - (Optional) The folder Id to perform searches
	drafts - (Optional) True/False to display draft results as well.  Defaults to False
	DeptID - Department ID from Parature
	APIToken - The API token from Parature - This may be specified in the Search action, but is required in one of these. It is recommended here.
	style - (Optional) The style used to display the search results. This is a URL and could be located in CRM web resources.
	APIDomain - (Optional) Default is Demo.  This is the domain or servers on which to find the articles.
2) Search - Search and display relevant articles
	AcctID - (Optional if specified in Load) AcctId from Parature
	pageSize - (Optional) The number of results to display at a time.
	folderName - (Optional) The name of the folder or folder path to search within
	folderID - (Optional) The folder Id to perform searches
	drafts - (Optional) True/False to display draft results as well.  Defaults to False
	DeptID - (Optional if specified in Load) Department ID from Parature
	APIToken - (Optional if specified in Load) The API token from Parature - This may be specified in the Search action, but is required in one of these. It is recommended here.
	style - (Optional if specified in Load) The style used to display the search results. This is a URL and could be located in CRM web resources.
	APIDomain - (Optional) Default is Demo.  This is the domain or servers on which to find the articles.
Events:
1) ArticleChosen - An article was clicked by the user and should be displayed
	questionid - The questionid of the article to display.  May be passed to the ArticleDisplay control to show it.
Description:
This component displays results of a search. An edit box is displayed to allow the user to manually search or the search action may be called to initiate a search in the configuration.

ArticleDisplay (Parature)
USD Type: USD Hosted Control
Type: Microsoft.USD.ComponentLibrary.ArticleList
Actions:
1) ShowArticle - Loads the control and sets authentication parameters used in connecting to Parature. Calling this action will also populate the following replacement parameters: externallink, articleid, title, text
	url - (Optional) The number of results to display at a time. This is used to display articles that may not exist in Parature. It allows them to use the same control for display and allow any URL to be treated as an Article.
	title - The name of the folder or folder path to search within
	articleid - (Optional if url is specified) AcctId from Parature
	AcctID - (Optional if url is specified) The folder Id to perform searches
	DeptID - (Optional if url is specified) Department ID from Parature
	APIToken - (Optional if url is specified) The API token from Parature - This may be specified in the Search action, but is required in one of these. It is recommended here.
	style - (Optional) The style used to display the search results. This is a URL and could be located in CRM web resources.
	APIDomain - (Optional) Default is Demo.  This is the domain or servers on which to find the articles.
Description:
This component is designed to display an article either located within Parature or an arbitrary URL.

PopupPanel - Popup panel displays a single hosted control in a popup window that floats above other windows. This panel is derived from System.Windows.Controls.Primitives.Popup and inherits it's attributes
			 It must contain a child with a <ContentPresenter /> entry. This content presenter will be the location that the hosted control is displayed within the popup.
			 Opening a dynamic hosted control that has it's Display Group set to this panel will automatically open the popup. Closing the hosted control will automatically close the popup.

	EXAMPLE 1
       <usdcomponentlibrary:PopupPanel IsOpen="False" PlacementTarget="{Binding ElementName=KBSearch}" Placement="Left" x:Name="KBPopup" Width="500" Height="768" >
         <Grid>
           <Grid.RowDefinitions>
             <RowDefinition Height="20" />
             <RowDefinition Height="*" />
           </Grid.RowDefinitions> 
           <Border Background="#cccccc" Grid.Row="0" >
             <TextBlock Text="KB Article preview" VerticalAlignment="Center" Margin="10,0,0,0" />
           </Border>
           <Border BorderThickness="1" Grid.Row="1" BorderBrush="#cccccc" Background="White">
             <ContentPresenter />
           </Border>
         </Grid>
        </usdcomponentlibrary:PopupPanel>

	EXAMPLE 2
		<usdcomponentlibrary:PopupPanel IsOpen="False" PlacementTarget="{Binding ElementName=AboutPanel}" Placement="Bottom" x:Name="NotifyPanel" Width="300" Height="60" >
             <ContentPresenter />
        </usdcomponentlibrary:PopupPanel>

CRMCachingImageConverter - An updated image converter for loading images from CRM web resources.  This image converter will only load an image once for the user and will avoid extra hits to the server
						   for subsequent requests for the same image. This is particularly helpful when using images in session lines where refreshes of the content could request the image again.
	
	EXAMPLE 1
     xmlns:usdcomponentlibrary="clr-namespace:Microsoft.USD.ComponentLibrary;assembly=Microsoft.USD.ComponentLibrary"
	 ...
     <Grid.Resources>
       <usdcomponentlibrary:CRMCachingImageConverter x:Key="CRMImageLoader" />
     </Grid.Resources>
     ....
          <Image Grid.Column="0" Source="{Binding Source=msdyusd_Logo, Converter={StaticResource CRMImageLoader}}"  Style="{DynamicResource ImageLogo}"   />

Controller
USD Type: USD Hosted Control
Global: True
Type: Microsoft.USD.ComponentLibrary.Controller
Actions:
1) ShowPopup - Show a Popup
	panelname - the x:Name of the popup or popup panel to show
2) HidePopup - Hide a Popup
	panelname - the x:Name of the popup or popup panel to hide
3) CopyToClipboard - Copies the specified data parameter to the clipboard
4) SetReplacementParameter - Set specific replacement parameter values
	global - True/False to indicate whether the values are set in the session or global list
	appname - The application name under which these values will be set.
	[remainder of lines] - Name=Value pairs representing the data parameters to set.
5) SetTimedEvent - Fire a custom event after a timer expires
	appname - name where it appears the event originates from. Matches the event configuration name
	eventname - The name of the event to fire.
	milliseconds - The time to elapse before the event is fired.
6) CreateEntityAssociation - Creates a record representing a N:N relationship
	entityname1 - The logical name of the first entity
	entityguid1 - The ID of the first entity
	entityname2 - The logical name of the second entity
	entityguid2 - The ID of the second entity
	relationshipname - N:N relationship name
7) New_CRM_Page - Opens a CRM page and runs it through the Window Navigation Rules for placement
	appname - (Optional - default is Controllers name) Application name from which to make it appear this CRM Page popup originates
	sessionid - (Optional) The session from which to originate the request.
	extraqs - (Optional) The encoded extraqs parameters to use
	extraqs_decoded - (Optional) The decoded extraqs parameters to use. extraqs and extraqs_decoded are mutulally exclusive
	showtab - (Optional) The application to show after the processing is complete
	frame - (Optional) The frame to appear to originate the request from. This affects the Window Navigation Rule processing.
	allowreplace - (Optional) True/False to tell the system whether to replace the displayed page on the destination page if something is already there.
	entity - (Optional - LogicalName and entity are mutually exclusive) The entity that this page is based upon. Example is phonecall
	LogicalName - (Optional - LogicalName and entity are mutually exclusive) Same thing as entity. Only 1 is needed.
	id - (Optional) Specify this if you want to open an existing entity instead. Normally don't specify this.
8) CloseIncident - Close an active incident
	id - Guid of the incident to close
	statuscode - (Optional) Status code to change the case to. Defaults to 5
9) CloseOpportunity - Close an active oppotunity
	id - Guid of the opportunity to close
	statuscode - (Optional) Status code to change the case to. Defaults to 3
10) CloseQuote - Close an active quote
	id - Guid of the quote to close
	statuscode - (Optional) Status code to change the case to. Defaults to 3
11) RetrieveEntityOptionSet - Retrieves an optionset defined for a field in an entity
	LogicalName - The logical name of the entity
	FieldName - The field that the option set is defined
	global - Whether the values should go in the active session or the global session
	appname - The name under which the data parameters are shown
12) RetrieveGlobalOptionSet - Retrieve a global optionset
	OptionsetName - The name of the CRM optionset
	global - Whether the values should go in the active session or the global session
	appname - The name under which the data parameters are shown
13) CopyReplacementParameter - 
	fromsession - Session Id (Guid) The session to copy the entry from.  This can be global or session based but is typically session based.
	tosession - Session id (Guid) The session to copy the entry to. This can be globla or session based but is typically global.
	appfrom - The data parameter name that is being copied from (top level tree item in the debugger)
	appto - The data parameter name that is being copied to (top level tree item in the debugger)

Events:
1) [Custom Events] - Custom events fired from SetTimedEvent.
Description:
This component provide useful functions that extend the capabilities of USD.
