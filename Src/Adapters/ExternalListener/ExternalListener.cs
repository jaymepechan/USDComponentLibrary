/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Common;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.USD.ComponentLibrary.Adapters;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Example URL to trigger stuff
    /// http://localhost:5000/?eventname=TestEvent&askresponse=true
    /// </summary>
	public class ExternalListener : DynamicsBaseHostedControl
	{
		/// <summary>
		/// The CRM listener
		/// </summary>
        private HttpListener crmListener;

		/// <summary>
		/// The CRM request worker
		/// </summary>
        private Thread crmRequestWorker;

		/// <summary>
		/// The response timeout
		/// </summary>
		private TimeSpan responseTimeout = TimeSpan.FromSeconds(60);

        public ExternalListener()
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="Listener"/> class.
		/// </summary>
		/// <param name="appID">The application identifier.</param>
		/// <param name="appName">Name of the application.</param>
		/// <param name="initString">The initialize string.</param>
        public ExternalListener(Guid appID, string appName, string initString)
				: base (appID, appName, initString)
        {
        }

		/// <summary>
		/// Initializes this Listener.
		/// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

		protected override void DoAction(RequestActionEventArgs args)
		{
            if (args.Action.Equals("startlistener", StringComparison.OrdinalIgnoreCase))
            { 
				this.crmListener = new HttpListener();
					
				// set default prefix
				string listenerUrl = "http://localhost:5000/";

				// grab prefix from options
				string prefixOption = ConfigurationValueReader.ConfigurationReader().ReadAppSettings("ListenerPrefix");

				// grab prefix from start action
				List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
				string prefixParameter = Utility.GetAndRemoveParameter(parameters, "prefix");

				// set prefix by order of precidence: Action parameter, CCD Option, default
				if (!string.IsNullOrEmpty(prefixOption))
					listenerUrl = prefixOption;
				if (!string.IsNullOrEmpty(prefixParameter))
					listenerUrl = prefixParameter;
					
				// Start the listener...
				try
				{
					this.crmListener.Prefixes.Add(listenerUrl);
					this.crmListener.Start();
					this.crmRequestWorker = new Thread(new ParameterizedThreadStart(this.RequestWorker))
					{
						IsBackground = true
					};
					this.crmRequestWorker.Start();
				}
				catch
				{
				}
            }
            else if (args.Action.Equals("settimeout", StringComparison.OrdinalIgnoreCase))
            { 
				// grab timeout from start action
				List<KeyValuePair<string, string>> parameterstimeout = Utility.SplitLines(args.Data, CurrentContext, localSession);
				string sTimeout = Utility.GetAndRemoveParameter(parameterstimeout, "timeout");

				// Set timeout
                if (!string.IsNullOrEmpty(sTimeout))
				{
					double outvar;
					if (double.TryParse(sTimeout, out outvar))
						this.responseTimeout = TimeSpan.FromSeconds(System.Math.Abs(outvar));
				}
				else
				{
					double outvar;
                    // grab timeout from options
                    string timeoutOption = ConfigurationValueReader.ConfigurationReader().ReadAppSettings("ListenerTimeout");

                    if (double.TryParse(timeoutOption, out outvar))
						this.responseTimeout = TimeSpan.FromSeconds(System.Math.Abs(outvar));
				}

            }
            else if (args.Action.Equals("sendmessage", StringComparison.OrdinalIgnoreCase))
            {
                // Grab url from data
				List<KeyValuePair<string, string>> smparameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
				string url = Utility.GetAndRemoveParameter(smparameters, "url");

				// Make request and store return
				var webReq = new MCSWebRequest();
				args.ActionReturnValue = webReq.GetStringResponse(url);
            }
            else if (args.Action.Equals("SetData", StringComparison.OrdinalIgnoreCase))
            {
                // Grab url from data
                List<KeyValuePair<string, string>> smparameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string eventname = Utility.GetAndRemoveParameter(smparameters, "eventname");
                string data = Utility.GetAndRemoveParameter(smparameters, "data");
                if (eventDataReturn.ContainsKey(eventname))
                { 
                    SendResponse(eventDataReturn[eventname].httpContext, data);
                    eventDataReturn[eventname].Dispose();
                }
            }
			base.DoAction(args);
		}

        static Dictionary<string, HttpReturnContext> eventDataReturn = new Dictionary<string, HttpReturnContext>();
        class HttpReturnContext : IDisposable
        {
            public HttpListenerContext httpContext;
            private Timer timeout;
            private string eventName;

            public HttpReturnContext(string eventName, HttpListenerContext httpContext, TimeSpan responseTimeout)
            {
                this.eventName = eventName;
                this.httpContext = httpContext;
                timeout = new Timer(new TimerCallback(Timeout), null, responseTimeout, TimeSpan.Zero);
            }

            private void Timeout(object obj)
            {
                Dispose();
                SendResponse(httpContext, "");
            }

            public void Dispose()
            {
                if (timeout != null)
                    timeout.Dispose();
                lock (eventDataReturn)
                {
                    if (eventDataReturn.ContainsKey(eventName))
                        eventDataReturn.Remove(eventName);
                }
            }
        }

		/// <summary>
		/// The Worker Thread.
		/// </summary>
		/// <param name="obj">The object.</param>
        private void RequestWorker(object obj)
        {
            while (true)
            {
                // Get request data
				HttpListenerContext httpContext = this.crmListener.GetContext();
                string rawdata = "";
                using (StreamReader sr = new StreamReader(httpContext.Request.InputStream))
                {
                    rawdata = sr.ReadToEnd();
                }

				// Get Query parameters
				Dictionary<string, string> qParams = new Dictionary<string,string>();
				List<LookupRequestItem> itemsList = new List<LookupRequestItem>();
				NameValueCollection nvpQuery = HttpUtility.ParseQueryString(rawdata);
				nvpQuery.Add(httpContext.Request.QueryString);
				foreach (string s in nvpQuery.Keys)
				{
					qParams.Add(s, nvpQuery[s]);
					itemsList.Add(new LookupRequestItem(s, nvpQuery[s]));
				}

				// Process the request
                try
                {
					if (qParams["eventname"] != null || qParams["eventname"] != "")
					{
						switch(qParams["eventname"])
						{
							case "ScreenPop":
                                Dispatcher.Invoke(() =>
                                    {
                                        CtiLookupRequest data = new CtiLookupRequest(Guid.NewGuid(), base.ApplicationName, qParams["calltype"], qParams["ani"], qParams["dnis"]);
                                        data.Items.AddRange(itemsList);
                                        base.FireRequestAction(new RequestActionEventArgs("*", CtiLookupRequest.CTILOOKUPACTIONNAME, GeneralFunctions.Serialize<CtiLookupRequest>(data)));
                                        SendResponse(httpContext, "");
                                    }
                                );
								continue;

							default:
                                if (qParams.ContainsKey("askresponse") && qParams["askresponse"].ToLower() == "true")
								{
                                    lock (eventDataReturn)
                                    {
                                        eventDataReturn.Add(qParams["eventname"], new HttpReturnContext(qParams["eventname"], httpContext, responseTimeout));
                                    }
                                    Dispatcher.Invoke(() =>
                                    {
                                        base.FireEvent(qParams["eventname"], qParams);
                                    });
                                    continue;
								}
                                Dispatcher.Invoke(() =>
                                {
                                    base.FireEvent(qParams["eventname"], qParams);
                                });
								break;
						}
					}
                }
                catch
                {
					// TODO: implement error logging
                }
                SendResponse(httpContext, "");
            }
        }

		/// <summary>
		/// Sends the response.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <param name="response">The response.</param>
		private static void SendResponse(HttpListenerContext httpContext, string response)
		{
			try
			{
				using (StreamWriter sw = new StreamWriter(httpContext.Response.OutputStream))
				{
					httpContext.Response.AddHeader("cache-control", "no-cache");
					httpContext.Response.AddHeader("pragma", "no-cache");
					httpContext.Response.AddHeader("expires", "0");
					httpContext.Response.AddHeader("expiresabsolute", "-1");
                    sw.WriteLine(HttpUtility.HtmlEncode(response));
					sw.Close();
				}
			}
			catch
			{
			}
		}
	}
}
