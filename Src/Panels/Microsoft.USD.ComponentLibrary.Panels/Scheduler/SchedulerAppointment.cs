using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    /// <summary>
    /// Represents custom business objects.
    /// Implementation of the INotifyPropertyChanged interface allows 
    /// to notify binding clients, that a property value has changed
    /// </summary>
    public class SchedulerAppointment : INotifyPropertyChanged
    {
        #region ** fields
        private string _subject = "";
        private string _body = "";
        private string _stateCode = "";
        private string _statusCode = "";
        private string _location = "";
        private string _properties = "";
        private DateTime _start = DateTime.Today;
        private DateTime _end = DateTime.Today + TimeSpan.FromDays(1);
        private Guid _id = Guid.NewGuid();
        private bool _isDeleted = false;
        private string _activityType;
        #endregion

        #region ** ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="AppointmentBusinessObject"/> class.
        /// </summary>
        public SchedulerAppointment()
        {
        }
        #endregion

        #region ** object model
        public string Subject
        {
            get { return _subject; }
            set
            {
                if (_subject != value)
                {
                    _subject = value;
                    OnPropertyChanged("Subject");
                }
            }
        }

        public DateTime Start
        {
            get { return _start; }
            set
            {
                if (_start != value)
                {
                    _start = value;
                    OnPropertyChanged("Start");
                }
            }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                if (_end != value)
                {
                    _end = value;
                    OnPropertyChanged("End");
                }
            }
        }

        public string Body
        {
            get { return _body; }
            set
            {
                if (_body != value)
                {
                    _body = value;
                    OnPropertyChanged("Body");
                }
            }
        }

        public string StatusCode
        {
            get { return _statusCode; }
            set
            {
                if (_statusCode != value)
                {
                    _statusCode = value;
                    OnPropertyChanged("StatusCode");
                }
            }
        }

        public string StateCode
        {
            get { return _stateCode; }
            set
            {
                if (_stateCode != value)
                {
                    _stateCode = value;
                    OnPropertyChanged("StateCode");
                }
            }
        }

        public bool IsDeleted
        {
            get { return _statusCode == "Completed"; }
        }

        public string Location
        {
            get { return _location; }
            set
            {
                if (_location != value)
                {
                    _location = value;
                    OnPropertyChanged("Location");
                }
            }
        }

        public string Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                {
                    _properties = value;
                    OnPropertyChanged("Properties");
                }
            }
        }

        public Guid Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string ActivityType
        {
            get { return _activityType; }
            set
            {
                if (_activityType != value && value != null)
                {
                    _activityType = value.ToLower();
                    OnPropertyChanged("ActivityType");
                }
            }
        }
        #endregion

        #region ** INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
        #endregion
    }

    /// <summary>
    /// The <see cref="AppointmentBOList"/> class is a collection of the <see cref="AppointmentBusinessObject"/>
    /// objects that supports data binding.
    /// Note: data binding support is inherited from the BindingList class. 
    /// </summary>
    public class SchedulerAppointmentList : BindingList<SchedulerAppointment>
    {
    }
}
