using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DigiFlare.DigiTweet.DataAccess;
using System.IO;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for WebPagePreviewControl.xaml
    /// </summary>
    public partial class WebPagePreviewControl : Window, IDisposable
    {

        #region Constructor

        public WebPagePreviewControl()
        {
            InitializeComponent();

            WebPagePreviewManager.DocumentCompleted += new WebPagePreviewManager.LoadWebPagePreviewCompletedHandler(WebPagePreviewManager_DocumentCompleted);
        }

        #endregion

        #region Public Methods

        public void GetPreview(Uri url)
        {
            txtLoading.Visibility = Visibility.Visible;
            imgPreview.Visibility = Visibility.Collapsed;

            // clean up previous memory stream
            try { ((BitmapImage)imgPreview.Source).StreamSource.Dispose(); }
            catch (Exception) { }
            WebPagePreviewManager.GetPreview(url);
        }

        public void WebPagePreviewManager_DocumentCompleted(object sender, LoadWebPagePreviewCompletedEventArgs e)
        {
            txtLoading.Visibility = Visibility.Collapsed;
            imgPreview.Visibility = Visibility.Visible;

            // init bitmap image source from byte stream
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.StreamSource = new MemoryStream(e.ImageStream);
            source.EndInit();
            imgPreview.Source = source;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // clean up
            imgPreview.Source = null;
            WebPagePreviewManager.DocumentCompleted -= new WebPagePreviewManager.LoadWebPagePreviewCompletedHandler(WebPagePreviewManager_DocumentCompleted);
        }

        #endregion
    }
}
