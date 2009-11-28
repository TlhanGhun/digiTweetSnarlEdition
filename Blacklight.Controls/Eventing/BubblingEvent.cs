//-----------------------------------------------------------------------
// <copyright file="BubblingEvent.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>04-Dec-2008</date>
// <author>Peter Blois</author>
// <summary>Class for bubbling events.</summary>
//-----------------------------------------------------------------------
namespace Blacklight.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Reflection;
    using System.Windows.Media;

    /// <summary>
    /// The bubbling event handler.
    /// </summary>
    /// <typeparam name="T">The bubbling event type.</typeparam>
    /// <param name="sender">The sender object.</param>
    /// <param name="args">The event args.</param>
    public delegate void BubblingEventHandler<T>(object sender, T args) where T : BubblingEventArgs;

    /// <summary>
    /// Half-implemented extensible routed event system.
    /// Declare a routed event with the syntax such as:
    /// <example>
    ///		public static readonly BubblingEvent(ContextMenuEventArgs) ContextMenuEvent = new BubblingEvent(ContextMenuEventArgs)("ContextMenuEventArgs", RoutingStrategy.Bubble);
    ///	</example>
    ///	
    /// Register a type handler for the event:
    /// <example>
    ///		static Page() {
    ///			ContextMenuGenerator.ContextMenuEvent.RegisterClassHandler(typeof(Page), Page.HandleContextMenuEvent, false);
    ///		}
    /// </example>
    /// </summary>
    /// <typeparam name="T">The event type.</typeparam>
    public class BubblingEvent<T> where T : BubblingEventArgs
    {
        /// <summary>
        /// Stores the registrations by type.
        /// </summary>
        private Dictionary<Type, BubblingEventRegistration> registeredTypes = new Dictionary<Type, BubblingEventRegistration>();

        /// <summary>
        /// BubblingEvent constructor.
        /// </summary>
        /// <param name="routingStrategy">The rooting strategy.</param>
        public BubblingEvent(RoutingStrategy routingStrategy)
        {
            this.RoutingStrategy = routingStrategy;
        }

        /// <summary>
        /// The handler.
        /// </summary>
        /// <typeparam name="TClassType">The class type.</typeparam>
        /// <param name="sender">The sender object.</param>
        /// <param name="eventArgs">The event args.</param>
        public delegate void Handler<TClassType>(TClassType sender, T eventArgs);

        /// <summary>
        /// Gets the routing strategy.
        /// </summary>
        public RoutingStrategy RoutingStrategy 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Registers the class handler.
        /// </summary>
        /// <param name="classType">The class type.</param>
        /// <param name="handler">The handler.</param>
        public void RegisterClassHandler(Type classType, EventHandler<T> handler)
        {
            this.RegisterClassHandler(classType, handler, false);
        }

        /// <summary>
        /// Registers the class handler.
        /// </summary>
        /// <param name="classType">The class type.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="handledEventsToo">Whether the class handles events too.</param>
        public void RegisterClassHandler(Type classType, EventHandler<T> handler, bool handledEventsToo)
        {
            if (!this.registeredTypes.ContainsKey(classType))
            {
                this.registeredTypes[classType] = new BubblingEventRegistration(classType, handler, handledEventsToo);
            }
        }

        /// <summary>
        /// Registers a class handler.
        /// </summary>
        /// <typeparam name="TClassType">The class type.</typeparam>
        /// <param name="handler">The handler.</param>
        public void RegisterClassHandler<TClassType>(Handler<TClassType> handler)
        {
            this.RegisterClassHandler(handler, false);
        }

        /// <summary>
        /// Resgisters a class handler.
        /// </summary>
        /// <typeparam name="TClassType">The class type.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="handled">Whether the event has been handled.</param>
        public void RegisterClassHandler<TClassType>(Handler<TClassType> handler, bool handled)
        {
            Type t = handler.GetType().GetGenericArguments()[1];
            this.RegisterClassHandler(
                t,
                delegate(object sender, T args)
                {
                    handler((TClassType)sender, args);
                });
        }

        /// <summary>
        /// Raises an event.
        /// </summary>
        /// <param name="evt">The event to raise.</param>
        public void RaiseEvent(T evt)
        {
            object target = evt.Source;
            FrameworkElement element = target as FrameworkElement;
            switch (this.RoutingStrategy)
            {
                case RoutingStrategy.Bubble:
                    if (element == null)
                    {
                        throw new ArgumentException();
                    }

                    this.RaiseBubblingEvent(evt, element);
                    break;
                case RoutingStrategy.Tunnel:
                    if (element == null)
                    {
                        throw new ArgumentException();
                    }

                    this.RaiseTunnelingEvent(evt, element);
                    break;
                case RoutingStrategy.Direct:
                    this.RaiseDirectEvent(evt, target);
                    break;
            }
        }

        /// <summary>
        /// Raises bubbling event.
        /// </summary>
        /// <param name="args">The event args.</param>
        /// <param name="element">The element.</param>
        private void RaiseBubblingEvent(T args, FrameworkElement element)
        {
            IList<BubblingEventDelegate> delegates = this.GetDelegates(element);

            for (int i = 0; i < delegates.Count; ++i)
            {
                delegates[i].Invoke(args);
            }
        }

        /// <summary>
        /// Raises a tunneling event.
        /// </summary>
        /// <param name="args">The event args.</param>
        /// <param name="element">The element.</param>
        private void RaiseTunnelingEvent(T args, FrameworkElement element)
        {
            IList<BubblingEventDelegate> delegates = this.GetDelegates(element);

            for (int i = delegates.Count - 1; i >= 0; --i)
            {
                delegates[i].Invoke(args);
            }
        }

        /// <summary>
        /// Raises a direct event.
        /// </summary>
        /// <param name="args">The event args.</param>
        /// <param name="target">The target element.</param>
        private void RaiseDirectEvent(T args, object target)
        {
            IList<BubblingEventDelegate> delegates = this.GetDelegates(target);

            for (int i = delegates.Count - 1; i >= 0; --i)
            {
                delegates[i].Invoke(args);
            }
        }

        /// <summary>
        /// Gets the delegates for an element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>A list of delegate.</returns>
        private IList<BubblingEventDelegate> GetDelegates(FrameworkElement element)
        {
            List<BubblingEventDelegate> delegates = new List<BubblingEventDelegate>();

            while (element != null)
            {
                Type classType = element.GetType();
                while (classType != null)
                {
                    BubblingEventRegistration registration;
                    if (this.registeredTypes.TryGetValue(classType, out registration))
                    {
                        delegates.Add(new BubblingEventDelegate(element, registration));
                    }

                    classType = classType.BaseType;
                }

                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
            }

            return delegates;
        }

        /// <summary>
        /// Gets the delegates from a target.
        /// </summary>
        /// <param name="target">The target element.</param>
        /// <returns>A list of delegates.</returns>
        private IList<BubblingEventDelegate> GetDelegates(object target)
        {
            List<BubblingEventDelegate> delegates = new List<BubblingEventDelegate>();

            Type classType = target.GetType();
            while (classType != null)
            {
                BubblingEventRegistration registration;
                if (this.registeredTypes.TryGetValue(classType, out registration))
                {
                    delegates.Add(new BubblingEventDelegate(target, registration));
                }

                classType = classType.BaseType;
            }

            return delegates;
        }

        /// <summary>
        /// The bubbling event registration.
        /// </summary>
        private class BubblingEventRegistration
        {
            /// <summary>
            /// BubblingEventRegistration constructor.
            /// </summary>
            /// <param name="classType">The class type.</param>
            /// <param name="handler">The event handler.</param>
            /// <param name="handledEvents">Handled events.</param>
            public BubblingEventRegistration(Type classType, EventHandler<T> handler, bool handledEvents)
            {
                this.ClassType = classType;
                this.Handler = handler;
                this.HandledEvents = handledEvents;
            }

            /// <summary>
            /// Gets or sets the class type.
            /// </summary>
            public Type ClassType 
            { 
                get; set; 
            }

            /// <summary>
            /// Gets or sets the event handler.
            /// </summary>
            public EventHandler<T> Handler 
            { 
                get; set; 
            }

            /// <summary>
            /// Gets or sets a value indicating whether events have been handled.
            /// </summary>
            public bool HandledEvents 
            { 
                get; set; 
            }
        }

        /// <summary>
        /// Bubbling event delegate.
        /// </summary>
        private class BubblingEventDelegate
        {
            /// <summary>
            /// Bubbling event delegate constructor.
            /// </summary>
            /// <param name="source">The source object.</param>
            /// <param name="registration">The event registration.</param>
            public BubblingEventDelegate(object source, BubblingEventRegistration registration)
            {
                this.Source = source;
                this.EventRegistration = registration;
            }

            /// <summary>
            /// Gets or sets the event registration.
            /// </summary>
            public BubblingEventRegistration EventRegistration 
            { 
                get; set; 
            }

            /// <summary>
            /// Gets or sets the source.
            /// </summary>
            public object Source 
            { 
                get; set; 
            }

            /// <summary>
            /// Invokes the delegate.
            /// </summary>
            /// <param name="args">The event args.</param>
            public void Invoke(T args)
            {
                if (!args.Handled || this.EventRegistration.HandledEvents)
                {
                    this.EventRegistration.Handler(this.Source, args);
                }
            }
        }
    }
}