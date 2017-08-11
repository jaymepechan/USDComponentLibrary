/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation.UIVisualViewer
{
    public class UITreeViewModel
    {
        private UIWindowViewModel _rootWindow;
        private ReadOnlyCollection<UIWindowViewModel> _firstGeneration;
        public UITreeViewModel(UIWindow rootWindow)
        {
            _rootWindow = new UIWindowViewModel(rootWindow);
            _firstGeneration = new ReadOnlyCollection<UIWindowViewModel>(new UIWindowViewModel[] { _rootWindow });
        }
        public ReadOnlyCollection<UIWindowViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }
    }
}
