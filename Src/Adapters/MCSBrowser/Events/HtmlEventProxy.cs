/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Globalization;
using mshtml;

namespace HtmlEventsSmple
{    
    public class HtmlEventProxy : IDisposable,IReflect
    {
        // Fields
        private EventHandler eventHandler;
        private object sender;
        private IReflect typeIReflectImplementation;
        private IHTMLElement2 htmlElement = null;
        private string eventName = null;

        // private CTOR
        private HtmlEventProxy(string eventName, IHTMLElement2 htmlElement, EventHandler eventHandler)
        {
            this.eventName = eventName;
            this.htmlElement = htmlElement;
            this.sender = this;
            this.eventHandler = eventHandler;
            Type type = typeof(HtmlEventProxy);
            this.typeIReflectImplementation = type;
        }

        public static HtmlEventProxy Create(string eventName,object htmlElement, EventHandler eventHandler)
        {
            IHTMLElement2 elem = (IHTMLElement2)htmlElement;

            HtmlEventProxy newProxy = new HtmlEventProxy(eventName,elem, eventHandler);
            elem.attachEvent(eventName, newProxy);
            return newProxy;            
        }

        /// <summary>
        /// detach only once (thread safe)
        /// </summary>
        public void Detach()
        {
            lock (this)
            {
                if (this.htmlElement != null)
                {
                    IHTMLElement2 elem = (IHTMLElement2)htmlElement;
                    elem.detachEvent(this.eventName, this);
                    this.htmlElement = null;
                }
            }
        }                

        /// <summary>
        /// HtmlElemet property  
        /// </summary>
        public IHTMLElement2 HTMLElement
        {
            get
            {
                return this.htmlElement;
            }
        }

        #region IReflect

        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetField(name, bindingAttr);
        }

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetFields(bindingAttr);
        }

        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetMember(name, bindingAttr);
        }

        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetMembers(bindingAttr);
        }

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetMethod(name, bindingAttr);
        }

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return this.typeIReflectImplementation.GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetMethods(bindingAttr);
        }

        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetProperties(bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
        {
            return this.typeIReflectImplementation.GetProperty(name, bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return this.typeIReflectImplementation.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            if (name == "[DISPID=0]")
            {
                if (this.eventHandler != null)
                {
                    this.eventHandler(this.sender, EventArgs.Empty);
                }                
            }

            return null; 
        }



        Type IReflect.UnderlyingSystemType
        {
            get
            {
                return this.typeIReflectImplementation.UnderlyingSystemType;
            }
        }

        #endregion
        
        #region IDisposable Members

        public void Dispose()
        {
            Detach();  
        }

        #endregion
    }
}
