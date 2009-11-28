using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace DigiFlare.DigiTweet.UI
{
    public class Utils
    {
        public delegate void TwitterEventHandler(object sender, RoutedEventArgs e);

        /// <summary>
        /// Append given string to the textblock
        /// The given string is converted into a recognized inline element
        /// i.e. hyperlink, user link, etc...
        /// </summary>
        /// <param name="textblock"></param>
        /// <param name="word"></param>
        public static void AppendTo(TextBlock textblock, string word)
        {
            AppendTo(textblock, word, Url_Clicked, User_Clicked, Url_MouseEnter, UrlMouseLeave);
        }

        /// <summary>
        /// Append given string to the textblock
        /// The given string is converted into a recognized inline element
        /// i.e. hyperlink, user link, etc...
        /// The event handlers provided will be used
        /// </summary>
        /// <param name="textblock"></param>
        /// <param name="word"></param>
        /// <param name="urlClickedHandler"></param>
        /// <param name="userClickedHandler"></param>
        public static void AppendTo(
            TextBlock textblock, string word, 
            TwitterEventHandler urlClickedHandler, 
            TwitterEventHandler userClickedHandler,
            MouseEventHandler linkMouseEnterHandler,
            MouseEventHandler linkMouseLeaveHandler)
        {
            // input argument check
            if (string.IsNullOrEmpty(word))
            {
                return;
            }

            Inline element = ConvertToInline(word, urlClickedHandler, userClickedHandler, linkMouseEnterHandler, linkMouseLeaveHandler);
            if (null != element)
            {
                textblock.Inlines.Add(element);
            }
        }

        /// <summary>
        /// Converts a string into an inline element
        /// The provided event handlers will be used when element is clicked
        /// </summary>
        /// <param name="word"></param>
        /// <param name="urlClickedHandler"></param>
        /// <param name="userClickedHandler"></param>
        /// <returns></returns>
        public static Inline ConvertToInline(
            string word, 
            TwitterEventHandler urlClickedHandler, 
            TwitterEventHandler userClickedHandler,
            MouseEventHandler linkMouseEnterHandler,
            MouseEventHandler linkMouseLeaveHandler)
        {
            Hyperlink link;

            // input arg check
            if (string.IsNullOrEmpty(word))
            {
                return null;
            }

            // convert string to user link
            if (word.IndexOf(Constants.REPLY_PREFIX) == 0)
            {
                link = new Hyperlink();
                link.Inlines.Add(word);
                link.Foreground = Brushes.LightBlue;
                link.Click += new RoutedEventHandler(userClickedHandler);
                return link;
            }

            // convert string to url
            if (TryParseUrl(word, out link))
            {
                link.Foreground = Brushes.LightBlue;
                link.Click += new RoutedEventHandler(urlClickedHandler);

                // hover url opens a preview browser
                link.MouseEnter += new MouseEventHandler(linkMouseEnterHandler);
                link.MouseLeave += new MouseEventHandler(linkMouseLeaveHandler);

                return link;
            }

            // return plain text
            return new Run(word);
        }

        #region Default Event Handlers

        /// <summary>
        /// Event handler to open a hyperlink in the default browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Url_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(((Hyperlink)sender).NavigateUri.ToString());
            }
            catch
            {
                MessageBox.Show("Unable to open url in default browser.");
            }
        }

        /// <summary>
        /// Dummy event user clicked handler, used when user clicked handler is not provided
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void User_Clicked(object sender, RoutedEventArgs e)
        {
        }

        public static void Url_MouseEnter(object sender, MouseEventArgs e)
        {
        }

        public static void UrlMouseLeave(object sender, MouseEventArgs e)
        {
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Try to parse the url, similar to int.TryParse
        /// </summary>
        /// <param name="url"></param>
        /// <param name="color"></param>
        /// <param name="hyperlink"></param>
        /// <returns></returns>
        private static bool TryParseUrl(string url, out Hyperlink hyperlink)
        {
            hyperlink = null;

            // check input url
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            // try parsing url
            if (0 == url.IndexOf("http://", StringComparison.InvariantCultureIgnoreCase))
            {
                try
                {
                    Uri uri = new Uri(url);
                    Hyperlink link = new Hyperlink { NavigateUri = uri };
                    link.Inlines.Add(url);

                    // set output value
                    hyperlink = link;
                    return true;
                }
                catch (Exception) { }
            }
            return false;
        }

        #endregion
    }
}
