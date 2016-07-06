/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
	class MCSWebRequest
	{
		//TODO: Old code... Need to refactor...

		#region Private Fields

		/// <summary>
		/// The object's default cookie container.
		/// </summary>
		private CookieContainer _cookieJar = new CookieContainer();

		/// <summary>
		/// The number of milliseconds to wait for a web request before timing out. Default = 60,000.
		/// </summary>
		private int _timeout = 60000;

		/// <summary>
		/// The UserAgent. Defaults to Firefox 3.
		/// </summary>
		private string _userAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9) Gecko/2008052906 Firefox/3.0";

		/// <summary>
		/// The HTTP method to use (GET or POST).
		/// </summary>
		private string _method = "GET";

		/// <summary>
		/// The content type to use.
		/// </summary>
		private string _contentType = "application/x-www-form-urlencoded";

		/// <summary>
		/// The host for the request.
		/// </summary>
		private string _host = string.Empty;

		/// <summary>
		/// The referrer.
		/// </summary>
		private string _referrer = string.Empty;

		/// <summary>
		/// The proxy credentials.
		/// </summary>
		private IWebProxy _webProxy = null;

		/// <summary>
		/// Accept auto-redirects?
		/// </summary>
		private bool _redirect = true;

		/// <summary>
		/// The accept string.
		/// </summary>
		private string _accept = "*/*";

		/// <summary>
		/// Keep Alive.
		/// </summary>
		private bool _keepAlive = true;

		#endregion

		#region Properties

		/// <summary>
		/// The object's default cookie container.
		/// </summary>
		public CookieContainer CookieJar
		{
			get { return _cookieJar; }
			set { _cookieJar = value; }
		}

		/// <summary>
		/// The number of milliseconds to wait for a web request before timing out. Default = 60,000.
		/// </summary>
		public int Timeout
		{
			get { return _timeout; }
			set { _timeout = value; }
		}

		/// <summary>
		/// // The UserAgent. Defaults to Firefox 3.
		/// </summary>
		public string UserAgent
		{
			get { return _userAgent; }
			set { _userAgent = value; }
		}

		/// <summary>
		/// The HTTP method to use (GET or POST).
		/// </summary>
		public string Method
		{
			get { return _method; }
			set
			{
				if ((value.ToUpper() == "GET") || (value.ToUpper() == "POST"))
					_method = value.ToUpper();
			}
		}

		/// <summary>
		/// The content type to use. Defaults to 'application/x-www-form-urlencoded'.
		/// </summary>
		public string ContentType
		{
			get { return _contentType; }
			set { _contentType = value; }
		}

		/// <summary>
		/// The proxy credentials.
		/// </summary>
		public IWebProxy WebProxy
		{
			get { return _webProxy; }
			set { _webProxy = value; }
		}

		/// <summary>
		/// Accept auto-redirects?
		/// </summary>
		public bool Redirect
		{
			get { return _redirect; }
			set { _redirect = value; }
		}

		/// <summary>
		/// The accept string. Defaults to "*/*".
		/// </summary>
		public string Accept
		{
			get { return _accept; }
			set { _accept = value; }
		}

		/// <summary>
		/// The request's keep-alive setting. Defaults to true.
		/// </summary>
		public bool KeepAlive
		{
			get { return _keepAlive; }
			set { _keepAlive = value; }
		}

		/// <summary>
		/// The referrer.
		/// </summary>
		public string Referrer
		{
			get { return _referrer; }
			set { _referrer = value; }
		}

		#endregion

		#region String Response Methods

		/// <summary>
		/// (Overloaded) Gets a string response from a specified URL via WebRequest.
		/// </summary>
		/// <param name="url">URL to send the request to.</param>
		/// <returns>The body of the returned HTML page.</returns>
		public string GetStringResponse(string url)
		{
			return this.GetStringResponse(url, String.Empty);
		}

		/// <summary>
		/// Gets a string response from a specified URL via WebRequest.
		/// </summary>
		/// <param name="url">URL to send the request to.</param>
		/// <param name="postData">Post data to send with the request.</param>
		/// <returns>The body of the returned HTML page.</returns>
		public string GetStringResponse(string url, string postData)
		{
			// Create the Web Request
			HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(url);

			// Grab the current Web proxy settings
			if (this.WebProxy == null)
			{
				this.WebProxy = WebRequest.DefaultWebProxy;
				this.WebProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
			}

			// Set the current properties to the webrequest
			webReq.CookieContainer = this._cookieJar;
			webReq.Timeout = this.Timeout;
			webReq.UserAgent = this.UserAgent;
			webReq.Method = this.Method;
			webReq.KeepAlive = this.KeepAlive;
			webReq.Accept = this.Accept;
			webReq.AllowAutoRedirect = this.Redirect;
			webReq.Proxy = this.WebProxy;
			webReq.Referer = this.Referrer;
			this.Referrer = url;

			// Process POST reqest
			if (this.Method.ToUpper() == "POST")
			{
				// Create blank data if null
				if (postData == null)
					postData = "";

				// Set the content type of the post data
				webReq.ContentType = this.ContentType;

				// Set the content length of the post data
				webReq.ContentLength = postData.Length;

				// Convert post data to binary
				ASCIIEncoding enc = new ASCIIEncoding();
				byte[] postDataBytes = enc.GetBytes(postData);

				// Write post data to request
				Stream str = null;
				try
				{
					str = webReq.GetRequestStream();
					str.Write(postDataBytes, 0, postDataBytes.Length);
				}
				finally
				{
					if (str != null)
						str.Close();
				}
			}
			else // Process GET request
			{
				webReq.ContentLength = "".Length;
			}

			// Get web response
			HttpWebResponse webRes = (HttpWebResponse)webReq.GetResponse();

			// Transfer cookies from header
			if (webRes.Headers.Get("Set-Cookie") != null)
			{
				// Create temp cookie
				Cookie tempCookie;

				// Loop thru each cookie
				foreach (string cookieHeader in webRes.Headers.GetValues("Set-Cookie"))
				{
					// Initialize tempCookie
					tempCookie = new Cookie();

					// Create a cookie array
					string[] cookie = cookieHeader.Split(';');

					// Loop thru cookie array
					foreach (string cookiePart in cookie)
					{
						// Separate keys and values (on first =)
						string[] keyVal = cookiePart.Split(new char[] { '=' }, 2);

						// Check for blank name
						if (keyVal.Length < 2)
						{
							tempCookie.Name = "THISISABLANKCOOKIENAMEANDSHOULDBEDISCARDED";
							break;
						}

						switch (keyVal[0].Trim().ToLower())
						{
							case "expires":
								// Do nothing...
								break;

							case "domain":
								tempCookie.Domain = keyVal[1];
								break;

							case "path":
								tempCookie.Path = keyVal[1];
								break;

							default:
								// Intercept a blank name field
								if (keyVal.Length < 2)
									continue;
								tempCookie.Name = keyVal[0];
								tempCookie.Value = keyVal[1];
								break;
						}
					}

					// Validate cookie
					if (tempCookie.Name == "THISISABLANKCOOKIENAMEANDSHOULDBEDISCARDED")
						continue;
					if (tempCookie.Path == "")
						tempCookie.Path = "/";
					if (tempCookie.Domain == "")
						tempCookie.Domain = webReq.Headers.Get("Host");

					// Add cookie to container.
					this._cookieJar.Add(tempCookie);
				}
			}

			// Get response stream
			Stream resStream = webRes.GetResponseStream();

			// Convert the stream into a string.
			StreamReader resReader = new StreamReader(resStream);
			return resReader.ReadToEnd();
		}

		#endregion
	}
}
