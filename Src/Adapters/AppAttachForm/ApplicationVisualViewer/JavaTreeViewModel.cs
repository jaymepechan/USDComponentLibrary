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

namespace Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer
{
    public class JavaTreeViewModel
    {
        private JavaWindowViewModel _rootWindow;
        private ReadOnlyCollection<JavaWindowViewModel> _firstGeneration;
        public JavaTreeViewModel(JavaWindow rootWindow)
        {
            _rootWindow = new JavaWindowViewModel(rootWindow);
            _firstGeneration = new ReadOnlyCollection<JavaWindowViewModel>(new JavaWindowViewModel[] { _rootWindow });
        }
        public ReadOnlyCollection<JavaWindowViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }
    }
}
