/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Uii.Csr.Browser.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.USD.ComponentLibrary
{
    public interface IBrowserCallback
    {
        IWebBrowser2 WebBrowser
        {
            get;
        }

        void Hide();
        void ShowBrowser();
    }
}
