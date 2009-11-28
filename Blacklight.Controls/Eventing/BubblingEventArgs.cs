//-----------------------------------------------------------------------
// <copyright file="BubblingEventArgs.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>04-Dec-2008</date>
// <author>Peter Blois</author>
// <summary>Class for bubbling event args.</summary>
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
    /// Routing strategy.
    /// </summary>
    public enum RoutingStrategy
    {
        /// <summary>
        /// Tunnel route.
        /// </summary>
        Tunnel,

        /// <summary>
        /// Bubble route.
        /// </summary>
        Bubble,

        /// <summary>
        /// Direct route.
        /// </summary>
        Direct,
    }

    /// <summary>
    /// Bubbling event args class.
    /// </summary>
    public class BubblingEventArgs : EventArgs
    {
        /// <summary>
        /// Bubbling event args constructor.
        /// </summary>
        /// <param name="source">The source object.</param>
        public BubblingEventArgs(object source)
        {
            this.Source = source;
        }

        /// <summary>
        /// Gets the source.
        /// </summary>
        public object Source
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the event has been handled.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }
    }
}
