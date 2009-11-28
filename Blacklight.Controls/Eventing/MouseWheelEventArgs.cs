//-----------------------------------------------------------------------
// <copyright file="MouseWheelEventArgs.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>04-Dec-2008</date>
// <author>Peter Blois</author>
// <summary>Class representing mouse wheel event args.</summary>
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
    /// Class representing mouse wheel event args.
    /// Code ported from Javascript version at http://adomas.org/javascript-mouse-wheel/
    /// </summary>
    public class MouseWheelEventArgs : BubblingEventArgs
    {
        /// <summary>
        /// Stores the plugin position.
        /// </summary>
        private Point pluginPosition;

        /// <summary>
        /// MouseWheel event args constructor.
        /// </summary>
        /// <param name="source">The source element.</param>
        /// <param name="delta">The mouse wheel delta.</param>
        /// <param name="mousePosition">The mouse position.</param>
        public MouseWheelEventArgs(FrameworkElement source, double delta, Point mousePosition) : base(source)
        {
            this.Delta = delta;
            this.pluginPosition = mousePosition;
        }

        /// <summary>
        /// Gets the delta.
        /// </summary>
        public double Delta 
        { 
            get; 
            private set; 
        }

        /// <summary>
        /// Gets the mouse position.
        /// </summary>
        /// <param name="element">The element to get the position on.</param>
        /// <returns>The mouse position.</returns>
        public Point GetPosition(UIElement element)
        {
            GeneralTransform transform = element.TransformToVisual((UIElement)Application.Current.RootVisual);
            return transform.Transform(this.pluginPosition);
        }
    }
}
