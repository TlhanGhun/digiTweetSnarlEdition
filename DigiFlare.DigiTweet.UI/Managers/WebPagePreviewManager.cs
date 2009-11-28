using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Drawing.Imaging;

namespace DigiFlare.DigiTweet.UI
{
    public static class WebPagePreviewManager
    {
        #region Constants

        public static string PREVIEW_CACHE_DIRECTORY = "Cache";
        public static string HTTP = "http://";
        private static int BROWSER_WIDTH = 820;
        private static int BROWSER_HEIGHT = 620;
        private static int PREVIEW_WIDTH = 800;
        private static int PREVIEW_HEIGHT = 600;

        #endregion

        #region Variables

        private static string _loadingUrl = string.Empty;
        private static WebBrowser _browser = new WebBrowser();
        private static Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

        #endregion

        #region Events

        public delegate void LoadWebPagePreviewCompletedHandler(object sender, LoadWebPagePreviewCompletedEventArgs e);
        public static event LoadWebPagePreviewCompletedHandler DocumentCompleted;

        #endregion

        #region Static Constructor

        static WebPagePreviewManager()
        {
            Directory.CreateDirectory(PREVIEW_CACHE_DIRECTORY);

            // init browser control
            _browser.Width = BROWSER_WIDTH;
            _browser.Height = BROWSER_HEIGHT;

            _browser.IsWebBrowserContextMenuEnabled = false;
            _browser.ScriptErrorsSuppressed = true;
            _browser.ScrollBarsEnabled = false;
            _browser.WebBrowserShortcutsEnabled = false;

            _browser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(_browser_DocumentCompleted);
            _browser.NewWindow += new System.ComponentModel.CancelEventHandler(_browser_NewWindow);
        }

        #endregion

        #region Public Methods

        public static void GetPreview(Uri url)
        {
            // look for url in cache
            if (_cache.ContainsKey(url.AbsoluteUri))
            {
                OnDocumentCompleted(_cache[url.AbsoluteUri]);
            }
            else
            {
                // stop whatever the browser is loading 
                // and navigate to the new requested url
                if (_browser.ReadyState != WebBrowserReadyState.Uninitialized) { _browser.Stop(); }
                _loadingUrl = url.AbsoluteUri;
                _browser.Navigate(url);
            }
        }

        #endregion

        #region Event Handlers

        private static void _browser_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // block popup
            e.Cancel = true;
        }

        private static void _browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // http://support.microsoft.com/kb/180366
            // document completed event gets fired multiple times (once for each frame)
            // the top level frame firing this event last with ReadyState == Complete

            if (_browser.ReadyState == WebBrowserReadyState.Complete)
            {
                Bitmap bitmap = new Bitmap(PREVIEW_WIDTH, PREVIEW_HEIGHT);
                Rectangle rect = new Rectangle(0, 0, PREVIEW_WIDTH, PREVIEW_HEIGHT);
                _browser.DrawToBitmap(bitmap, rect);

                // turn image into byte[] so it's easier to work with
                MemoryStream imageStream = new MemoryStream();
                bitmap.Save(imageStream, ImageFormat.Jpeg);
                byte[] imageBytes = imageStream.ToArray();

                // cache image
                _cache.Add(_loadingUrl, imageBytes);

                // clean up
                bitmap.Dispose();
                imageStream.Close();
                _loadingUrl = string.Empty;

                // notify subscribers
                OnDocumentCompleted(imageBytes);
            }
        }

        #endregion

        #region Helper Methods

        private static void OnDocumentCompleted(byte[] imageStream)
        {
            if (null != DocumentCompleted)
            {
                LoadWebPagePreviewCompletedEventArgs args = new LoadWebPagePreviewCompletedEventArgs
                {
                    ImageStream = imageStream
                };
                DocumentCompleted(_browser, args);
            }
        }

        #endregion
    }
}
