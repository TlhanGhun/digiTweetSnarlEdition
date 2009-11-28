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
using System.IO;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;
using DigiFlare.DigiTweet.UI.Configuration;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        #region Constants

        private const string CACHE_PATH = "cache.txt";
        private const char CACHE_DELIMITER = ',';

        #endregion

        #region Instance Variables

        private string _username = null;
        private string _password = null;

        #endregion

        #region Custom Events

        public delegate void LoginCompletedHandler(object sender, LoginCompletedEventArgs e);
        public event LoginCompletedHandler LoginCompleted;

        #endregion

        #region Constructor

        public LoginControl()
        {
            InitializeComponent();

            // load user credentials from settings
            UserSetting setting = ConfigurationManager.Default.GetPreviousSessionUserSetting();
            if (null != setting)
            {
                tbUsername.Text = setting.Username;
                tbPassword.Password = setting.Password;
                cbRememberMe.IsChecked = true;
            }
        }

        #endregion

        #region UI Event Handlers

        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            _username = tbUsername.Text;
            _password = tbPassword.Password;

            // form data validation
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                txtError.Visibility = Visibility.Visible;
                return;
            }

            // login user async
            SetLoading(true);
            BackgroundWorker loginWorker = new BackgroundWorker();
            loginWorker.DoWork += new DoWorkEventHandler(loginWorker_DoWork);
            loginWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(loginWorker_RunWorkerCompleted);
            loginWorker.RunWorkerAsync();
        }

        #endregion

        #region Login Async

        void loginWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // login user
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                ExtendedUser user = client.VerifyCrednetials();
                e.Result = user;
                client.Close();
            }
            catch (Exception ex)
            {
                // login failed
                client.Abort();
                e.Result = null;
                return;
            }
        }

        void loginWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // tell user call to server is completed
            SetLoading(false);

            // login failed if result is null
            ExtendedUser loggedInUser = null;
            if (null == e.Result)
            {
                txtError.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                loggedInUser = e.Result as ExtendedUser;
            }

            // login successful, see if we need to save user information
            if (cbRememberMe.IsChecked == true)
            {
                ConfigurationManager.Default.SetUserSetting(_username, _password);
            }
            else
            {
                //ConfigurationManager.Default.RemoveUserSetting(_username);
                ConfigurationManager.Default.SetUserSetting(_username, null);
                ConfigurationManager.Default.LastUser = null;
            }
            //Properties.Settings.Default.Save();
            ConfigurationManager.Default.Save();

            // once we get here, hide the error message which may have been displayed in previous login attempts
            txtError.Visibility = Visibility.Hidden;

            // raise login complete event
            if (null != LoginCompleted)
            {
                LoginCompletedEventArgs args = new LoginCompletedEventArgs
                {
                    Username = _username,
                    Password = _password,
                    LoggedInUser = loggedInUser
                };
                LoginCompleted(this, args);
            }
        }

        #endregion

        #region Helper Methods

        void SetLoading(bool isLoading)
        {
            if (isLoading)
            {
                txtLoading.Visibility = Visibility.Visible;
                txtError.Visibility = Visibility.Hidden;
            }
            else
            {
                txtLoading.Visibility = Visibility.Hidden;
            }
        }

        #endregion
    }

    #region Custom EventArgs
    public class LoginCompletedEventArgs : EventArgs
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ExtendedUser LoggedInUser { get; set; }
    }
    #endregion
}
