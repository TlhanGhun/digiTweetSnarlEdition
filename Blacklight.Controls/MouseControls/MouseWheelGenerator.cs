//-----------------------------------------------------------------------
// <copyright file="MouseWheelGenerator.cs" company="Microsoft Corporation copyright 2008.">
// (c) 2008 Microsoft Corporation. All rights reserved.
// This source is subject to the Microsoft Public License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// </copyright>
// <date>04-Dec-2008</date>
// <author>Peter Blois</author>
// <summary>Class for hooking up mouse wheel events.</summary>
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
    using System.Windows.Browser;

    /// <summary>
    /// Class for hooking up mouse wheel events.
    /// </summary>
    public class MouseWheelGenerator
    {
        /// <summary>
        /// The mouse wheel event.
        /// </summary>
        public static readonly BubblingEvent<MouseWheelEventArgs> MouseWheelEvent = new BubblingEvent<MouseWheelEventArgs>(RoutingStrategy.Bubble);

        /// <summary>
        /// The generator helper/
        /// </summary>
        private static MouseWheelGenerator helper = new MouseWheelGenerator();

        /// <summary>
        /// Stores whether the scroll viewers have been activated.
        /// </summary>
        private static bool scrollViewersActivated;

        /// <summary>
        /// The mouse position.
        /// </summary>
        private static Point mousePosition;

        /// <summary>
        /// MouseWheelGenerator constructor.
        /// </summary>
        private MouseWheelGenerator()
        {
            if (HtmlPage.IsEnabled)
            {
                HtmlPage.Window.AttachEvent("DOMMouseScroll", this.HandleMouseWheel);
                HtmlPage.Window.AttachEvent("onmousewheel", this.HandleMouseWheel);
                HtmlPage.Document.AttachEvent("onmousewheel", this.HandleMouseWheel);

                HtmlPage.Document.AttachEvent(
                    "onmousemove",
                    delegate(object sender, HtmlEventArgs e)
                    {
                        MouseWheelGenerator.mousePosition = new Point(e.ClientX, e.ClientY);
                    });
            }
        }

        /// <summary>
        /// Wires up the scroll viewers.
        /// </summary>
        public static void AddMouseWheelToScrollViewers()
        {
            if (!scrollViewersActivated)
            {
                scrollViewersActivated = true;
                MouseWheelGenerator.MouseWheelEvent.RegisterClassHandler(
                    typeof(ScrollViewer),
                    delegate(object sender, MouseWheelEventArgs e)
                    {
                        ScrollViewer sv = (ScrollViewer)sender;

                        if (e.Delta > 0 && sv.VerticalOffset > 0)
                        {
                            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta * 50);
                            e.Handled = true;
                        }
                        else if (e.Delta < 0 && sv.VerticalOffset < sv.ScrollableHeight)
                        {
                            sv.ScrollToVerticalOffset(sv.VerticalOffset - e.Delta * 50);
                            e.Handled = true;
                        }
                    },
                    false);
            }
        }

        /// <summary>
        /// Handles the mouse wheel.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="args">Html Event args.</param>
        private void HandleMouseWheel(object sender, HtmlEventArgs args)
        {
            double delta = 0;

            ScriptObject eventObj = args.EventObject;

            if (eventObj.GetProperty("wheelDelta") != null)
            {
                delta = ((double)eventObj.GetProperty("wheelDelta")) / 120;

                if (HtmlPage.Window.GetProperty("opera") != null)
                {
                    delta = -delta;
                }
            }
            else if (eventObj.GetProperty("detail") != null)
            {
                delta = -((double)eventObj.GetProperty("detail")) / 3;

                if (HtmlPage.BrowserInformation.UserAgent.IndexOf("Macintosh") != -1)
                {
                    delta = delta * 3;
                }
            }

            if (delta != 0)
            {
                if (this.OnMouseWheel(delta, args))
                {
                    args.PreventDefault();
                }
            }
        }

        /// <summary>
        /// Handles on mouse wheel.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="e">HtmlEvent args.</param>
        /// <returns>A bool value.</returns>
        private bool OnMouseWheel(double delta, HtmlEventArgs e)
        {
            Point mousePosition = new Point(e.OffsetX, e.OffsetY);

            // in Firefox offsetX/screenX is not set and pageX is giving wacky numbers.
            if (HtmlPage.BrowserInformation.Name == "Netscape")
            {
                mousePosition = MouseWheelGenerator.mousePosition;

                HtmlElement offsetElement = HtmlPage.Plugin;
                while (offsetElement != null && offsetElement != HtmlPage.Document.Body)
                {
                    mousePosition.X -= (double)offsetElement.GetProperty("offsetLeft");
                    mousePosition.Y -= (double)offsetElement.GetProperty("offsetTop");

                    offsetElement = offsetElement.Parent;
                }
            }

            UIElement rootVisual = (UIElement)Application.Current.RootVisual;

            UIElement firstElement = null;
            foreach (UIElement element in VisualTreeHelper.FindElementsInHostCoordinates(mousePosition, rootVisual))
            {
                firstElement = element;
                break;
            }

            if (firstElement != null)
            {
                FrameworkElement source = (FrameworkElement)firstElement;

                MouseWheelEventArgs wheelArgs = new MouseWheelEventArgs(source, delta, mousePosition);
                MouseWheelGenerator.MouseWheelEvent.RaiseEvent(wheelArgs);

                return wheelArgs.Handled;
            }

            return false;
        }
    }
}
