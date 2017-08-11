/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation.UIVisualViewer
{
    public class UIWindowViewModel : INotifyPropertyChanged
    {
        readonly ReadOnlyCollection<UIWindowViewModel> _children;
        readonly UIWindowViewModel _parent;
        readonly UIWindow _win;

        bool _isExpanded;
        bool _isSelected;

        public UIWindowViewModel(UIWindow win)
            : this(win, null)
        {
        }

        private UIWindowViewModel(UIWindow win, UIWindowViewModel parent)
        {
            _win = win;
            _parent = parent;

            _children = new ReadOnlyCollection<UIWindowViewModel>(
                    (from child in _win.Children
                     select new UIWindowViewModel(child, this))
                     .ToList<UIWindowViewModel>());
        }

        #region Person Properties

        public ReadOnlyCollection<UIWindowViewModel> Children
        {
            get { return _children; }
        }

        public string Name
        {
            get { return _win.Name; }
        }

        public UIWindow Win
        {
            get { return _win; }
        }

        public string DisplayName
        {
            get { return _win.DisplayName; }
        }

        #endregion // Person Properties

        #region Presentation Members

        #region IsExpanded

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    this.OnPropertyChanged("IsExpanded");
                }

                // Expand all the way up to the root.
                if (_isExpanded && _parent != null)
                    _parent.IsExpanded = true;
            }
        }

        #endregion // IsExpanded

        #region IsSelected

        /// <summary>
        /// Gets/sets whether the TreeViewItem 
        /// associated with this object is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    this.OnPropertyChanged("IsSelected");
                }
            }
        }

        #endregion // IsSelected

        #region NameContainsText

        public bool NameContainsText(string text)
        {
            if (String.IsNullOrEmpty(text) || String.IsNullOrEmpty(this.Name))
                return false;

            return this.Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) > -1;
        }

        #endregion // NameContainsText

        #region Parent

        public UIWindowViewModel Parent
        {
            get { return _parent; }
        }

        #endregion // Parent

        #endregion // Presentation Members        

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion // INotifyPropertyChanged Members
    }
}
