//-----------------------------------------------------------------------
// <copyright file="EventRegistrar.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>04-Dec-2008</date>
// <author>Peter Blois</author>
// <summary>Class for registering events.</summary>
//-----------------------------------------------------------------------
namespace Blacklight.Controls
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    /// <summary>
    /// Class for registering events.
    /// </summary>
    /// <typeparam name="TClassType">The class type.</typeparam>
    /// <typeparam name="T">The object type.</typeparam>
    public class EventRegistrar<TClassType, T> where T : BubblingEventArgs
    {
        /// <summary>
        /// The bubbling event.
        /// </summary>
        private BubblingEvent<T> bubblingEvent;

        /// <summary>
        /// The event registrar constructor.
        /// </summary>
        /// <param name="bubblingEvent">The bubbling event.</param>
        public EventRegistrar(BubblingEvent<T> bubblingEvent)
        {
            this.bubblingEvent = bubblingEvent;
        }

        /// <summary>
        /// Adds the event.
        /// </summary>
        public event BubblingEvent<T>.Handler<TClassType> Event
        {
            add
            {
                this.bubblingEvent.RegisterClassHandler(value);
            }

            remove
            {
            }
        }
    }
}
