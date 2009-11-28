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
using System.Windows.Shapes;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        #region UI Events

        void OK_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                DialogResult = true;
            }
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #region Properties

        // refresh interval in miniutes
        public int RefreshInterval { get; set; }

        // flag indicating whether web page preview is enabled
        public bool IsWebPreviewEnabled { get; set; }

        #endregion

        #region Helpers

        private bool ValidateForm()
        {
            int interval;
            if (int.TryParse(txtInterval.Text, out interval))
            {
                RefreshInterval = interval;
                IsWebPreviewEnabled = cbWebPagePreview.IsChecked == true;
                return true;
            }
            return false;
        }

        #endregion

    }
}
