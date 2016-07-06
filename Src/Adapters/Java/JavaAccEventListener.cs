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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.USD.ComponentLibrary.Adapters.Java
{

    public static class JavaAccEventListener
    {
        private static JavaCaretUpdateEventHandler _CaretUpdate;
        private static bool _EventsRegistered;
        private static JavaFocusGainEventHandler _FocusGain;
        private static JavaFocusLostEventHandler _FocusLost;
        private static JavaShutdownEventHandler _JavaShutdown;
        private static JavaMenuCancelEventHandler _MenuCancel;
        private static JavaMenuDeselectEventHandler _MenuDeselect;
        private static JavaMenuSelectEventHandler _MenuSelect;
        private static JavaMouseClickEventHandler _MouseClick;
        private static JavaMouseEnterEventHandler _MouseEnter;
        private static JavaMouseExitEventHandler _MouseExit;
        private static JavaMousePressEventHandler _MousePress;
        private static JavaMouseReleaseEventHandler _MouseRelease;
        private static JavaPopupMenuCanceledEventHandler _PopupMenuCancel;
        private static JavaPopupMenuInvisibleEventHandler _PopupMenuInvisible;
        private static JavaPopupMenuVisibleEventHandler _PopupMenuVisible;
        private static JavaPropertyActiveDescendentChangeEventHandler _PropertyActiveDescendentChange;
        private static JavaPropertyCaretChangeEventHandler _PropertyCaretChange;
        private static JavaPropertyChildChangeEventHandler _PropertyChildChange;
        private static JavaPropertyDescriptionChangeEventHandler _PropertyDescriptionChange;
        private static JavaPropertyNameChangeEventHandler _PropertyNameChange;
        private static JavaPropertySelectionChangeEventHandler _PropertySelectionChange;
        private static JavaPropertyStateChangeEventHandler _PropertyStateChange;
        private static JavaPropertyTableModelChangeEventHandler _PropertyTableModelChange;
        private static JavaPropertyTextChangeEventHandler _PropertyTextChange;
        private static JavaPropertyValueChangeEventHandler _PropertyValueChange;
        private static JavaPropertyVisibleDataChangeEventHandler _PropertyVisibleDataChange;

        public static event System.EventHandler<Uii.HostedApplicationToolkit.DataDrivenAdapter.JavaAccEventArgs> JavaAccEventOccurred;

        public static event System.EventHandler<Uii.HostedApplicationToolkit.DataDrivenAdapter.JavaAccEventArgs> JavaControlChanged;

        static JavaAccEventListener()
        {
            try
            {
                Trace.WriteLine("Loading java event listener..");
                System.AppDomain.CurrentDomain.DomainUnload += new System.EventHandler(JavaAccEventListener.CurrentDomain_DomainUnload);
                Trace.WriteLine("Initializing java event handlers..");
                SetJavaEventHandlers();
                SetAccEventHandlers();
                SetEventHandlers();
            }
            catch (System.Exception exception)
            {
                if (!JavaAccHelperMethods.IsJavaAccException(exception) || !JavaAccHelperMethods.IsJavaAccExceptionMaskable(exception))
                {
                    throw new Uii.HostedApplicationToolkit.DataDrivenAdapter.DataDrivenAdapterException("Java listener init failed", exception);
                }
            }
        }

        private static void CurrentDomain_DomainUnload(object sender, System.EventArgs e)
        {
            JavaAccEventOccurred = null;
            JavaControlChanged = null;
        }

        private static void FireAccEvent<T>(System.IntPtr source, string eventName, int vmId, System.Type type, T oldObject, T newObject)
        {
            if (!source.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                JavaAccEventOccurred((System.IntPtr)source, new Uii.HostedApplicationToolkit.DataDrivenAdapter.JavaAccEventArgs(type, eventName, source, vmId, oldObject, newObject));
            }
            else
            {
                JavaAccEventOccurred(null, new Uii.HostedApplicationToolkit.DataDrivenAdapter.JavaAccEventArgs(type, eventName, source, vmId, oldObject, newObject));
            }
        }

        private static void FireCheckBoxEvents<T>(System.IntPtr source, int vmId, T oldObject, T newObject)
        {
            if ((newObject != null) && (oldObject != null))
            {
                if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_SELECTED, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_NULL, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName);
                }
                else if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_NULL, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_SELECTED, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.CheckBoxClearedEventName);
                }
            }
        }

        private static void FireControlChangedEvent(System.IntPtr source, int vmId, string eventName)
        {
            if (JavaControlChanged != null)
            {
                JavaControlChanged((System.IntPtr)source, new Uii.HostedApplicationToolkit.DataDrivenAdapter.JavaAccEventArgs(null, eventName, source, vmId, null, null));
            }
        }

        private static void FireEvent<T>(System.IntPtr source, string eventName, System.IntPtr jEvent, int vmId, T oldObject, T newObject)
        {
            System.Type type = null;
            try
            {
                if (JavaControlChanged != null)
                {
                    if (eventName.Equals(JavaDataDrivenAdapterConstants.PropertyStateChangedEventName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        switch (JavaAccHelperMethods.GetRoleID(source, vmId))
                        {
                            case 1:
                                FireCheckBoxEvents<T>(source, vmId, oldObject, newObject);
                                break;

                            case 2:
                                FireRadioButtonEvents<T>(source, vmId, oldObject, newObject);
                                break;

                            case 5:
                                FireTreeNodeEvents<T>(source, vmId, oldObject, newObject);
                                break;
                        }
                    }
                    else if (eventName.Equals(JavaDataDrivenAdapterConstants.MousePressedEventName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (JavaAccHelperMethods.GetRoleID(source, vmId) == 0)
                        {
                            FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.ButtonPressedEventName);
                        }
                    }
                    else if (eventName.Equals(JavaDataDrivenAdapterConstants.MouseReleasedEventName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        if (JavaAccHelperMethods.GetRoleID(source, vmId) == 0)
                        {
                            FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.ButtonReleasedEventName);
                        }
                    }
                    else if ((eventName.Equals(JavaDataDrivenAdapterConstants.GotFocusEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.LostFocusEventName, System.StringComparison.OrdinalIgnoreCase)) || ((eventName.Equals(JavaDataDrivenAdapterConstants.MenuSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, System.StringComparison.OrdinalIgnoreCase)) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuCanceledEventName, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        FireControlChangedEvent(source, vmId, eventName);
                    }
                }
                if (JavaAccEventOccurred != null)
                {
                    type = (oldObject == null) ? typeof(object) : oldObject.GetType();
                    FireAccEvent<T>(source, eventName, vmId, type, oldObject, newObject);
                }
            }
            catch (System.Exception exception)
            {
                if (!JavaAccHelperMethods.IsJavaAccException(exception) || !JavaAccHelperMethods.IsJavaAccExceptionMaskable(exception))
                {
                    Trace.WriteLine("An error occurred while listening to java events..");
                }
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(source, vmId);
                JavaAccHelperMethods.ReleaseObject(jEvent, vmId);
                if (((System.IntPtr)source).GetType().Equals(type))
                {
                    object obj2 = oldObject;
                    object obj3 = newObject;
                    JavaAccHelperMethods.ReleaseObject((System.IntPtr)((System.IntPtr)obj2), vmId);
                    JavaAccHelperMethods.ReleaseObject((System.IntPtr)((System.IntPtr)obj3), vmId);
                }
            }
        }

        private static void FireRadioButtonEvents<T>(System.IntPtr source, int vmId, T oldObject, T newObject)
        {
            if ((newObject != null) && (oldObject != null))
            {
                if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_SELECTED, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_NULL, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName);
                }
                else if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_NULL, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_SELECTED, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.RadioButtonClearedEventName);
                }
            }
        }

        private static void FireTreeNodeEvents<T>(System.IntPtr source, int vmId, T oldObject, T newObject)
        {
            if ((newObject != null) && (oldObject != null))
            {
                if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_EXPANDED, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_COLLAPSED, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName);
                }
                else if (newObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_COLLAPSED, System.StringComparison.OrdinalIgnoreCase) && oldObject.ToString().StartsWith(JavaDataDrivenAdapterConstants.STATE_EXPANDED, System.StringComparison.OrdinalIgnoreCase))
                {
                    FireControlChangedEvent(source, vmId, JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName);
                }
            }
        }

        internal static void FreeEventHandlers()
        {
            if (((JavaControlChanged == null) && (JavaAccEventOccurred == null)) && _EventsRegistered)
            {
                _EventsRegistered = false;
            }
        }

        private static void SetAccEventHandlers()
        {
            //_PropertyActiveDescendentChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldActiveDescendent, System.IntPtr newActiveDescendent) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<System.IntPtr>(source, JavaDataDrivenAdapterConstants.PropertyActiveDescendentChangedEventName, jEvent, vmId, oldActiveDescendent, newActiveDescendent);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyActiveDescendentChangeFP(_PropertyActiveDescendentChange);
            //_PropertyCaretChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, int oldPosition, int newPosition) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<int>(source, JavaDataDrivenAdapterConstants.PropertyCaretChangedEventName, jEvent, vmId, oldPosition, newPosition);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyCaretChangeFP(_PropertyCaretChange);
            //_PropertyChildChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldChild, System.IntPtr newChild) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<System.IntPtr>(source, JavaDataDrivenAdapterConstants.PropertyChildChangedEventName, jEvent, vmId, oldChild, newChild);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyChildChangeFP(_PropertyChildChange);
            //_PropertySelectionChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertySelectionChangedEventName, jEvent, vmId, string.Empty, string.Empty);
            //    }
            //};
            //JavaAccNativeMethods.setPropertySelectionChangeFP(_PropertySelectionChange);
            //_PropertyStateChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldState, System.IntPtr newState) {
            //    if (_EventsRegistered)
            //    {
            //        string oldObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(oldState);
            //        string newObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(newState);
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyStateChangedEventName, jEvent, vmId, oldObject, newObject);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyStateChangeFP(_PropertyStateChange);
            //_PropertyTextChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyTextChangedEventName, jEvent, vmId, string.Empty, string.Empty);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyTextChangeFP(_PropertyTextChange);
            //_PropertyVisibleDataChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source) {
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyVisibleDataChangedEventName, jEvent, vmId, string.Empty, string.Empty);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyVisibleDataChangeFP(_PropertyVisibleDataChange);
            //_PropertyNameChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldName, System.IntPtr newName) {
            //    if (_EventsRegistered)
            //    {
            //        string oldObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(oldName);
            //        string newObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(newName);
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyNameChangedEventName, jEvent, vmId, oldObject, newObject);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyNameChangeFP(_PropertyNameChange);
            //_PropertyDescriptionChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldDescription, System.IntPtr newDescription) {
            //    if (_EventsRegistered)
            //    {
            //        string oldObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(oldDescription);
            //        string newObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(newDescription);
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyDescriptionChangedEventName, jEvent, vmId, oldObject, newObject);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyDescriptionChangeFP(_PropertyDescriptionChange);
            //_PropertyValueChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldValue, System.IntPtr newValue) {
            //    if (_EventsRegistered)
            //    {
            //        string oldObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(oldValue);
            //        string newObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(newValue);
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyValueChangedEventName, jEvent, vmId, oldObject, newObject);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyValueChangeFP(_PropertyValueChange);
            //_PropertyTableModelChange = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldValue, System.IntPtr newValue) {
            //    if (_EventsRegistered)
            //    {
            //        string oldObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(oldValue);
            //        string newObject = System.Runtime.InteropServices.Marshal.PtrToStringUni(newValue);
            //        FireEvent<string>(source, JavaDataDrivenAdapterConstants.PropertyTableModelChangedEventName, jEvent, vmId, oldObject, newObject);
            //    }
            //};
            //JavaAccNativeMethods.setPropertyTableModelChangeFP(_PropertyTableModelChange);
        }

        internal static void SetEventHandlers()
        {
            if (!_EventsRegistered)
            {
                _EventsRegistered = true;
            }
        }

        private static void SetJavaEventHandlers()
        {
            object nullObj = null;
            //_MouseClick = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MouseClickedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMouseClickedFP(_MouseClick);
            //_MousePress = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MousePressedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMousePressedFP(_MousePress);
            //_MouseRelease = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MouseReleasedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMouseReleasedFP(_MouseRelease);
            //_FocusGain = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.GotFocusEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setFocusGainedFP(_FocusGain);
            //_FocusLost = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.LostFocusEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setFocusLostFP(_FocusLost);
            //_JavaShutdown = delegate (int vmId)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(System.IntPtr.Zero, JavaDataDrivenAdapterConstants.ShutdownEventName, System.IntPtr.Zero, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setJavaShutdownFP(_JavaShutdown);
            //_MenuSelect = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MenuSelectedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMenuSelectedFP(_MenuSelect);
            //_MenuDeselect = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMenuDeselectedFP(_MenuDeselect);
            //_MenuCancel = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MenuCanceledEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMenuCanceledFP(_MenuCancel);
            //_MouseEnter = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MouseEnteredEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMouseEnteredFP(_MouseEnter);
            //_MouseExit = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.MouseExitedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setMouseExitedFP(_MouseExit);
            //_PopupMenuCancel = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.PopupMenuCanceledEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setPopupMenuCanceledFP(_PopupMenuCancel);
            //_PopupMenuInvisible = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.PopupMenuInvisibleEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setPopupMenuWillBecomeInvisibleFP(_PopupMenuInvisible);
            //_PopupMenuVisible = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.PopupMenuVisibleEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setPopupMenuWillBecomeVisibleFP(_PopupMenuVisible);
            //_CaretUpdate = delegate (int vmId, System.IntPtr jEvent, System.IntPtr source)
            //{
            //    if (_EventsRegistered)
            //    {
            //        FireEvent<object>(source, JavaDataDrivenAdapterConstants.CaretUpdatedEventName, jEvent, vmId, nullObj, nullObj);
            //    }
            //};
            //JavaAccNativeMethods.setCaretUpdateFP(_CaretUpdate);
        }
    }
}
