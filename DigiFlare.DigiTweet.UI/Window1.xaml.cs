using System;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Blacklight.Controls;
using DigiFlare.DigiTweet.DataAccess;
using DigiFlare.DigiTweet.UI.Configuration;
using Microsoft.Samples.CustomControls;
using Snarl;

namespace DigiFlare.DigiTweet.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        #region Instance Variables

        private System.Windows.Forms.NotifyIcon _trayIcon = null;
        private TwitterApp _twitter = null;
        private Timer _timer = null;
        private AlertWindow _notififcation = null;
        private WebPagePreviewControl _browser = null;

        private int _timerInterval = 600;
        private bool _isWebPagePreviewEnabled = false;

        private IntPtr SnarlMsgWndHandle;
        private NativeWindowApplication.SnarlMsgWnd snarlComWindow;
        private string pathToLogoImage = "";
        
        private List<string> alreadyNotifiedTweets = new List<string>();

        #endregion

        #region Dependency Property

        public static readonly DependencyProperty SelectedUserProperty = DependencyProperty.Register(
            "SelectedUser",
            typeof(ExtendedUser),
            typeof(Window1),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty LastUpdatedProperty = DependencyProperty.Register(
            "LastUpdated",
            typeof(DateTime),
            typeof(Window1),
            new PropertyMetadata(new DateTime())
        );

        #endregion

        #region Constructor

        public Window1()
        {
            // initialize xaml
            InitializeComponent();

            // put the main window in the middle
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;

            // handle login complete event
            ucLogin.LoginCompleted += new LoginControl.LoginCompletedHandler(ucLogin_LoginCompleted);

            RegisterWithSnarlIfAvailable();
        }

        ~Window1()
        {
            if (SnarlMsgWndHandle != IntPtr.Zero)
            {
                SnarlConnector.RevokeConfig(SnarlMsgWndHandle);
            }
        }

        void ucLogin_LoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            // retrieve logged in user
            ExtendedUser loggedInUser = e.LoggedInUser;

            // apply user settings after login completed
            UserSetting setting = ConfigurationManager.Default.GetUserSetting(loggedInUser.ScreenName);
            Left = setting.WindowLocation.X;
            Top = setting.WindowLocation.Y;
            Width = setting.WindowSize.Width;
            Height = setting.WindowSize.Height;
            _timerInterval = setting.RefreshTimerInterval;
            _isWebPagePreviewEnabled = setting.IsWebPagePreviewEnabled;

            // show twitter main view
            LoginView.Visibility = Visibility.Hidden;
            MainView.Visibility = Visibility.Visible;

            // init twitter application logic
            _twitter = new TwitterApp(e.Username, e.Password, loggedInUser);

            // hook up twitter app event handlers
            _twitter.TinyUrlCompleted += new TwitterApp.TinyUrlCompletedHandler(TinyUrlCompleted);
            _twitter.TwitPicCompleted += new TwitterApp.TwitPicCompletedHandler(TwitPicCompleted);
            _twitter.TweetsManager.TweetsManagerOperationCompleted += new TweetsManager.TweetsManagerCompletedHandler(TweetsManager_OperationCompleted);
            _twitter.TweetsManager.OperationError += new BaseCollectionManager<Status>.OperationErrorHandler(Twitter_OperationError);
            _twitter.RepliesManager.OperationCompleted += new BaseCollectionManager<Status>.OperationCompletedHandler(RepliesManager_OperationCompleted);
            _twitter.RepliesManager.OperationError += new BaseCollectionManager<Status>.OperationErrorHandler(Twitter_OperationError);
            _twitter.DirectMessageManager.OperationCompleted += new BaseCollectionManager<DirectMessage>.OperationCompletedHandler(DirectMessagesManager_OperationCompleted);
            _twitter.DirectMessageManager.OperationError += new BaseCollectionManager<DirectMessage>.OperationErrorHandler(Twitter_OperationError);
            _twitter.UserProfileManager.UserProfileOperationCompleted += new UserProfileManager.UserProfileOperationCompletedHandler(UserProfileOperationCompleted);
            _twitter.UserProfileManager.OperationError += new BaseManager.OperationErrorHandler(Twitter_OperationError);
            _twitter.FriendsManager.OperationCompleted += new BaseManager.OperationCompletedHandler(FriendsManager_OperationCompleted);
            _twitter.FriendsManager.OperationError += new BaseManager.OperationErrorHandler(Twitter_OperationError);
            //_twitter.SearchManager.OperationCompleted += new BaseManager.OperationCompletedHandler(SearchManager_OperationCompleted);
            //_twitter.SearchManager.OperationError += new BaseManager.OperationErrorHandler(SearchManager_OperationError);
            //_twitter.SavedSearchesManager.Initialize(
            //    ConfigurationManager.Default.SavedSearches,
            //    SearchManager_OperationCompleted,
            //    SearchManager_OperationError);
            _twitter.FavouritesManager.OperationCompleted += new BaseManager.OperationCompletedHandler(FavouritesManager_OperationCompleted);
            _twitter.FavouritesManager.OperationError += new BaseManager.OperationErrorHandler(FavouritesManager_OperationError);

            // create system tray icon
            _trayIcon = new System.Windows.Forms.NotifyIcon();
            _trayIcon.Icon = new System.Drawing.Icon(@"Images\tray.ico");
            _trayIcon.Click += new EventHandler(trayIcon_Click);
            _trayIcon.Visible = true;

            // set data context and data source
            SelectedUser = loggedInUser;
            ddpProfile.DataContext = this;
            icAllTweets.ItemsSource = AllTweets;
            icAllReplies.ItemsSource = AllReplies;
            icDirectMessages.ItemsSource = AllDirectMessages;
            icFriends.ItemsSource = AllFriends;
            icUserTweets.ItemsSource = SelectedUserTweets;
            icAllFavourites.ItemsSource = AllFavourites;
            lbFollowing.ItemsSource = AllFriends;

            // get data from twitter api
            _twitter.TweetsManager.RefreshAllAsync();
            _twitter.RepliesManager.RefreshAllAsync();
            _twitter.DirectMessageManager.RefreshAllAsync();
            _twitter.UserProfileManager.GetUserDetailsAsync(LoggedInUser.Id);
            _twitter.FriendsManager.GetFriendsAsync();
            _twitter.FavouritesManager.GetFavouritesAsync();

            // init categories manager and create tab for each categories
            CategoriesManager.Initialize(loggedInUser.ScreenName, AllTweets);
            foreach (string categoryName in CategoriesManager.GetCategoryNames())
            {
                // init tweets panel control for category
                TweetsPanelControl tweetsPanel = new TweetsPanelControl();
                tweetsPanel.ItemsSource = CategoriesManager.GetCategoryView(categoryName);
                tweetsPanel.IsShowMoreButton = Visibility.Collapsed;
                tweetsPanel.AvatarClicked += new MouseButtonEventHandler(imgAvatar_MouseDown);
                tweetsPanel.TweetTextLoaded += new RoutedEventHandler(TextBlock_Loaded);
                tweetsPanel.ReplyClicked += new RoutedEventHandler(btnReply_Click);
                tweetsPanel.DirectMessageClicked += new RoutedEventHandler(btnDirectMessage_Click);
                tweetsPanel.RetweetClicked += new RoutedEventHandler(btnRetweet_Click);
                tweetsPanel.DeleteClicked += new RoutedEventHandler(btnDelete_Click);
                tweetsPanel.FavouriteClicked += new RoutedEventHandler(cbFavourite_Click);

                // init tab for cateogry
                CategoryTabItem tab = CreateCategoryTab(categoryName, tweetsPanel);

                // add tab to tab host
                tcCategories.Items.Insert(tcCategories.Items.Count - 1, tab);
            }

            // load saved searches
            foreach (string keyword in ConfigurationManager.Default.SavedSearches)
            {
                TabItem searchTab = CreateSearch(keyword);
                tcSearches.Items.Add(searchTab);
                tcSearches.SelectedItem = searchTab;
            }
            tcSearches.SelectedIndex = 0;

            // init refresh timer
            _timer  = new Timer(
                new TimerCallback(RefreshTimer), 
                null,
                TimeSpan.FromSeconds((double)_timerInterval),
                TimeSpan.FromSeconds((double)_timerInterval));

            // init notification window
            _notififcation = new AlertWindow();

            // init url preview control
            _browser = new WebPagePreviewControl();
            _browser.Owner = this;

            // make sure unhandled exceptions will not crash app
            Application.Current.Dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(Dispatcher_UnhandledException);
        }

        #endregion

        #region App Logic Event Handlers

        void TinyUrlCompleted(object sender, TinyUrlCompletedEventArgs e)
        {
            string tinyUrl = e.Url;
            if (!string.IsNullOrEmpty(tinyUrl))
            {
                txtTweet.Text += tinyUrl;
                txtUrl.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("TinyUrl failed.");
            }
            SetLoading(false);
        }

        void TwitPicCompleted(object sender, TwitPicCompletedEventArgs e)
        {
            string url = e.Url;
            if (!string.IsNullOrEmpty(url))
            {
                txtTweet.Text += url;
            }
            else
            {
                MessageBox.Show("TwitPic upload failed.");
            }
            SetLoading(false);
        }

        void TweetsManager_OperationCompleted(object sender, TweetsManagerOperationCompletedEventArgs e)
        {
            // show notification if new tweets received
            //if (WindowState == WindowState.Minimized)
            //{
            if (null != e.NewTweets && 0 < e.NewTweets.Count)
            {
                if (SnarlConnector.GetSnarlWindow() == IntPtr.Zero)
                {
                    ShowAlert(
                        string.Format("New updates from {0}", e.NewTweets[0].User.ScreenName),
                        e.NewTweets[0].User
                    );
                }
                else
                {
                    foreach (Status singleTweet in e.NewTweets)
                    {
                        if(!alreadyNotifiedTweets.Contains(singleTweet.Id))
                        {
                            ShowSnarlAlert(singleTweet,"tweet");
                            alreadyNotifiedTweets.Add(singleTweet.Id);
                        }
                    }
                }
            }
            //}

            txtTweetsLoading.Visibility = Visibility.Collapsed;
            icAllTweets.Visibility = Visibility.Visible;
            icAllTweets.IsShowMoreButton = Visibility.Visible;
            Twitter_OperationCompleted(sender, null);
        }

        void RepliesManager_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            txtRepliesLoading.Visibility = Visibility.Collapsed;
            Twitter_OperationCompleted(sender, e);
            
        }

        void DirectMessagesManager_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            
            txtDirectMessagesLoading.Visibility = Visibility.Collapsed;
            Twitter_OperationCompleted(sender, e);
               }

        void UserProfileOperationCompleted(object sender, UserProfileOperationCompletedEventArgs e)
        {
            ExtendedUser user = e.User;
            if (user != null)
            {
                SelectedUser = user;
                txtUserProfileUrl.Inlines.Clear(); // TODO: fix hack!
                Utils.AppendTo(txtUserProfileUrl, user.Url);
            }
            SetLoading(false);
        }

        void FriendsManager_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            txtFriendsLoading.Visibility = Visibility.Collapsed;
            Twitter_OperationCompleted(sender, e);
        }

        void SearchManager_OperationCompleted(object sender, SearchManagerOperationCompletedEventArgs e)
        {
            if (e.SearchResults.Count > 0)
            {
                // show the "more results" button
                SetMoreSearchResultsButtonVisibility(e.Keyword, Visibility.Visible);

                if (null != e.SearchResults && 0 < e.SearchResults.Count)
                {
                    if (SnarlConnector.GetSnarlWindow() == IntPtr.Zero)
                    {
                        ShowAlert(

                            string.Format("New updates from {0}", e.SearchResults[0].AuthorName),
                            e.SearchResults[0].AuthorAvatarUrl
                        );
                    }
                    else
                    {
                        string tempFile = System.IO.Path.GetTempFileName();
                        WebClient client = new WebClient();
                        try
                        {
                            client.DownloadFile(e.SearchResults[0].AuthorAvatarUrl, tempFile);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            tempFile = pathToLogoImage;
                        }



                        SnarlConnector.ShowMessageEx("Search results", "New search results for " + e.Keyword, string.Format("New updates from {0}", e.SearchResults[0].AuthorName), 10, tempFile, snarlComWindow.Handle, Snarl.WindowsMessage.WM_USER + 63, "");
                        if (tempFile != pathToLogoImage)
                        {
                            System.IO.File.Delete(tempFile);
                        }

                    }
                }
            }
            else
            {
                SetMoreSearchResultsButtonVisibility(e.Keyword, Visibility.Collapsed);
            }
            Twitter_OperationCompleted(sender, e);
        }

        void SearchManager_OperationError(object sender, SearchManagerOperationErrorEventArgs e)
        {
            SetMoreSearchResultsButtonVisibility(e.Keyword, Visibility.Collapsed);
            //ShowMoreSearchResultsVisibility = Visibility.Collapsed;
            Twitter_OperationError(sender, e);
        }

        void FavouritesManager_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            Twitter_OperationCompleted(sender, e);
        }

        void FavouritesManager_OperationError(object sender, OperationErrorEventArgs e)
        {
            Twitter_OperationError(sender, e);
        }

        void Twitter_OperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            LastUpdated = DateTime.Now;
            SetLoading(false);
        }

        void Twitter_OperationError(object sender, OperationErrorEventArgs e)
        {
            SetLoading(false);
            //MessageBox.Show(e.Message);
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Fatal error. Please restart DigiTweet");
            e.Handled = true;
        }

        public void RefreshTimer(object stateInfo)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                SetLoading(true);
                _twitter.TweetsManager.RefreshAsync();
                _twitter.RepliesManager.RefreshAsync();
                _twitter.DirectMessageManager.RefreshAsync();
                _twitter.FriendsManager.GetFriendsAsync();
                _twitter.SavedSearchesManager.RefreshAsync();
            });
        }

        #endregion

        #region UI Event Handlers

        #region Top Panel Events

        void btnHome_Click(object sender, RoutedEventArgs e)
        {
            foreach (DragDockPanel panel in ddphMain.Items)
            {
                panel.PanelState = PanelState.Restored;
            }
        }

        void btnTweet_Click(object sender, RoutedEventArgs e)
        {
            if (SliderPane.Visibility != Visibility.Visible)
            {
                SliderPane.Visibility = Visibility.Visible;
            }
            else
            {
                SliderPane.Visibility = Visibility.Collapsed;
            }
        }

        void btnCloseSlider_Click(object sender, RoutedEventArgs e)
        {
            SliderPane.Visibility = Visibility.Collapsed;
        }

        void btnTweets_Click(object sender, RoutedEventArgs e)
        {
            ddpReplies.PanelState = PanelState.Restored;
            ddpDirectMessages.PanelState = PanelState.Restored;
            ddpProfile.PanelState = PanelState.Restored;
            ddpAllTweets.PanelState = PanelState.Maximized;
        }

        void btnReplies_Click(object sender, RoutedEventArgs e)
        {
            ddpAllTweets.PanelState = PanelState.Restored;
            ddpDirectMessages.PanelState = PanelState.Restored;
            ddpProfile.PanelState = PanelState.Restored;
            ddpReplies.PanelState = PanelState.Maximized;
        }

        void btnDirectMessages_Click(object sender, RoutedEventArgs e)
        {
            ddpAllTweets.PanelState = PanelState.Restored;
            ddpReplies.PanelState = PanelState.Restored;
            ddpProfile.PanelState = PanelState.Restored;
            ddpDirectMessages.PanelState = PanelState.Maximized;
        }

        void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            ddpAllTweets.PanelState = PanelState.Restored;
            ddpReplies.PanelState = PanelState.Restored;
            ddpDirectMessages.PanelState = PanelState.Restored;
            ddpProfile.PanelState = PanelState.Maximized;
        }

        void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow window = new OptionsWindow();
            window.Left = Left + Width / 2 - window.Width / 2;
            window.Top = Top + Height / 2 - window.Height / 2;
            window.RefreshInterval = _timerInterval / 60; // _timeInterval is in seconds, optionsWindow is in minutes
            window.IsWebPreviewEnabled = _isWebPagePreviewEnabled;
            if (window.ShowDialog() != true)
            {
                return;
            }

            // apply options
            _timerInterval = window.RefreshInterval * 60;
            _isWebPagePreviewEnabled = window.IsWebPreviewEnabled;
            _timer.Change(
                TimeSpan.FromSeconds((double)_timerInterval),
                TimeSpan.FromSeconds((double)_timerInterval));

            // save user settings
            UserSetting setting = ConfigurationManager.Default.GetUserSetting(LoggedInUser.ScreenName);
            if (null != setting)
            {
                setting.RefreshTimerInterval = _timerInterval;
                setting.IsWebPagePreviewEnabled = _isWebPagePreviewEnabled;
            }
        }

        #endregion

        #region Tweet Update Panel Events

        private void txtTweet_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSend_Click(btnSend, e);
            }
        }

        void btnSend_Click(object sender, RoutedEventArgs e)
        {
            // verify button click
            Button clicked = sender as Button;
            if (null == clicked)
            {
                ResetTweetPane();
                return;
            }

            // make sure the text box is not empty
            if (string.IsNullOrEmpty(txtTweet.Text))
            {
                ResetTweetPane();
                return;
            }

            // perform different twitter calls depending on the tweet update type
            if (txtTweet.Text.IndexOf(Constants.REPLY_PREFIX) == 0) // reply
            {
                Status context = clicked.DataContext as Status;
                if (null == context)
                {
                    // TODO: notify UI can't reply
                }
                else
                {
                    _twitter.TweetsManager.ReplyAsync(txtTweet.Text, context);
                }
            }
            else if (txtTweet.Text.IndexOf(Constants.DIRECT_MESSAGE_PREFIX) == 0) // direct message
            {
                User context = clicked.DataContext as User;
                if (null == context)
                {
                    MessageBox.Show("Error occured.  Unable to direct message user.");
                }
                else
                {
                    // send direct message thru twitter api
                    SetLoading(true);
                    string message = GetDirectMessageBodyFromText(context, txtTweet.Text);
                    _twitter.DirectMessageManager.Send(context, message);
                }
            }
            else // normal update or retweet
            {
                SetLoading(true);
                _twitter.TweetsManager.UpdateAsync(txtTweet.Text);
            }

            // reset button state
            ResetTweetPane();
        }

        void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            SetLoading(true);
            _twitter.TweetsManager.RefreshAllAsync();
            _twitter.RepliesManager.RefreshAllAsync();
            _twitter.DirectMessageManager.RefreshAllAsync();
            _twitter.UserProfileManager.GetUserDetailsAsync(SelectedUser.Id);
            _twitter.FriendsManager.GetFriendsAsync();
            _twitter.FavouritesManager.GetFavouritesAsync();
            _twitter.SavedSearchesManager.RefreshAllAsync();
        }

        void btnTinyUrl_Click(object sender, RoutedEventArgs e)
        {
            // verify url textbox is not empty
            string url = txtUrl.Text;
            if (string.IsNullOrEmpty(txtUrl.Text))
            {
                return;
            }

            // shorten url async call
            _twitter.TinyUrlAsync(url);
            SetLoading(true);
        }

        void btnTwitPic_Click(object sender, RoutedEventArgs e)
        {
            // init open file dialog
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            // show open file dialog
            if (dialog.ShowDialog() == true)
            {
                byte[] image = File.ReadAllBytes(dialog.FileName);
                _twitter.UploadPicAsync(image, dialog.FileName);
                SetLoading(true);
            }
        }

        #endregion

        #region Tweets Panel Control Events (Reply, Direct Message etc)

        void btnReply_Click(object sender, RoutedEventArgs e)
        {
            // verify button click
            Button clicked = sender as Button;
            if (null == clicked)
            {
                return;
            }

            // verify button's datacontext
            Status replyMe = clicked.DataContext as Status;
            if (null == replyMe)
            {
                return;
            }

            // reply tweet!
            txtTweet.Text = string.Format("{0}{1} ", Constants.REPLY_PREFIX, replyMe.User.ScreenName);
            btnSend.DataContext = replyMe;

            // show the tweet slider pane
            SliderPane.Visibility = Visibility.Visible;

            // set focus to tweet pane
            SetFocus(txtTweet);
        }

        void btnDirectMessage_Click(object sender, RoutedEventArgs e)
        {
            // verify button click
            Button clicked = sender as Button;
            if (null == clicked)
            {
                ResetTweetPane();
                return;
            }

            // verify button's datacontext
            if (null == clicked.DataContext)
            {
                ResetTweetPane();
                return;
            }

            // depending on the context, make a different backend call
            User user = null;
            if (clicked.DataContext is Status)
            {
                Status context = clicked.DataContext as Status;
                user = context.User;
            }
            else if (clicked.DataContext is DirectMessage)
            {
                DirectMessage context = clicked.DataContext as DirectMessage;
                user = context.Sender;
            }

            // verify user is not null
            if (null == user)
            {
                ResetTweetPane();
                return;
            }

            // direct message the user!
            txtTweet.Text = string.Format("{0}{1} ", Constants.DIRECT_MESSAGE_PREFIX, user.ScreenName);
            btnSend.DataContext = user;

            // show the tweet slider pane
            SliderPane.Visibility = Visibility.Visible;

            // set focus to tweet pane
            SetFocus(txtTweet);
        }

        void btnRetweet_Click(object sender, RoutedEventArgs e)
        {
            // verify button click
            Button clicked = sender as Button;
            if (null == clicked)
            {
                ResetTweetPane();
                return;
            }

            // verify button's datacontext
            Status context = clicked.DataContext as Status;
            if (null == context)
            {
                ResetTweetPane();
                return;
            }

            // verify tweet's user
            User user = context.User as User;
            if (null == user)
            {
                ResetTweetPane();
                return;
            }

            // retweet!
            txtTweet.Text = string.Format("{0} {1}{2} {3} ", 
                Constants.RETWEET_PREFIX, 
                Constants.REPLY_PREFIX, 
                user.ScreenName,
                context.Text);

            // show the tweet slider pane
            SliderPane.Visibility = Visibility.Visible;

            // set focus to tweet pane
            SetFocus(txtTweet);
        }

        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // verify button click
            Button clicked = sender as Button;
            if (null == clicked)
            {
                return;
            }

            // verify button's datacontext
            // and depending on the context, call a different method
            object deleteMe = clicked.DataContext;
            if (deleteMe is Status)
            {
                SetLoading(true);
                _twitter.TweetsManager.DeleteAsync(deleteMe as Status);
            }
            else if (deleteMe is DirectMessage)
            {
                SetLoading(true);
                _twitter.DirectMessageManager.DeleteAsync(deleteMe as DirectMessage);
            }
        }

        void btnMoreTweets_Click(object sender, RoutedEventArgs e)
        {
            SetLoading(true);
            _twitter.TweetsManager.MoreAsync();
        }

        void cbFavourite_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = sender as CheckBox;
            if (null == checkbox)
            {
                return;
            }

            // verify checkbox's datacontext
            Status tweet = checkbox.DataContext as Status;
            if (null == tweet)
            {
                return;
            }

            // based on state of the checkbox, favourite or unfavourite tweet
            SetLoading(true);
            if (checkbox.IsChecked == true)
            {
                _twitter.FavouritesManager.FavouriteAsync(tweet);
            }
            else
            {
                _twitter.FavouritesManager.UnfavouriteAsync(tweet);
            }
        }

        //void cbFavourite_Checked(object sender, RoutedEventArgs e)
        //{
        //    // verify checkbox
        //    CheckBox checkbox = sender as CheckBox;
        //    if (null == checkbox)
        //    {
        //        return;
        //    }

        //    // verify checkbox's datacontext
        //    Status favouriteMe = checkbox.DataContext as Status;
        //    if (null == favouriteMe)
        //    {
        //        return;
        //    }

        //    // favourite tweet
        //    SetLoading(true);
        //    _twitter.FavouritesManager.FavouriteAsync(favouriteMe);
        //}

        //void cbFavourite_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    // verify checkbox
        //    CheckBox checkbox = sender as CheckBox;
        //    if (null == checkbox)
        //    {
        //        return;
        //    }

        //    // verify checkbox's datacontext
        //    Status favouriteMe = checkbox.DataContext as Status;
        //    if (null == favouriteMe)
        //    {
        //        return;
        //    }

        //    // favourite tweet
        //    SetLoading(true);
        //    _twitter.FavouritesManager.UnfavouriteAsync(favouriteMe);
        //}

        #endregion

        #region User Profile Related Events

        void btnFollow_Click(object sender, RoutedEventArgs e)
        {
            SetLoading(true);
            if (SelectedUser.Following == true)
            {
                _twitter.UserProfileManager.UnfollowUser(SelectedUser.ScreenName);
            }
            else
            {
                _twitter.UserProfileManager.FollowUser(SelectedUser.ScreenName);
            }
        }

        void imgAvatar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // verify image is clicked
            Image clicked = sender as Image;
            if (null == clicked)
            {
                return;
            }

            // depending on the image's datacontext, get User object from a different property
            string userScreenName = null;
            if (clicked.DataContext is Status)
            {
                userScreenName = ((Status)clicked.DataContext).User.ScreenName;
            }
            else if (clicked.DataContext is DirectMessage)
            {
                userScreenName = ((DirectMessage)clicked.DataContext).Sender.ScreenName;
            }
            else if (clicked.DataContext is User)
            {
                userScreenName = ((User)clicked.DataContext).ScreenName;
            }
            else if (clicked.DataContext is SearchResult)
            {
                userScreenName = ((SearchResult)clicked.DataContext).GetAuthorScreenName();
            }

            // make sure we have a user object to work with
            if (null == userScreenName)
            {
                return;
            }

            // make sure we are not trying to get the same user again
            if (null != SelectedUser && userScreenName == SelectedUser.ScreenName)
            {
                return;
            }

            // retrieve full user details async
            SelectedUser = null;
            txtUserProfileUrl.Inlines.Clear(); // TODO: fix hack!
            _twitter.UserProfileManager.All.Clear();
            _twitter.UserProfileManager.GetUserDetailsAsync(userScreenName);

            // update ui
            btnProfile_Click(sender, e);
            SetLoading(true);
        }

        // this is kinda hacked up, performing string manipulation to retrieve clicked user name
        // ideally we should have a custom hyperlink class that stores the username
        void UserLink_Clicked(object sender, RoutedEventArgs e)
        {
            // verify "user link" is clicked
            Hyperlink userLink = sender as Hyperlink;
            string userName = ((Run)userLink.Inlines.FirstInline).Text.Replace(Constants.REPLY_PREFIX, string.Empty);
            _twitter.UserProfileManager.All.Clear();
            _twitter.UserProfileManager.GetUserDetailsAsync(userName);

            // update ui
            //btnProfile_Click(sender, e);
            SetLoading(true);
        }

        void ProfileAvatar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            Image avatar = sender as Image;
            if (null != sender)
            {
                avatar.ContextMenu = GetAvatarContextMenu(SelectedUser.ScreenName);
                avatar.ContextMenu.IsOpen = true;
            }
        }

        void AddUserToCategory_Click(object sender, RoutedEventArgs e)
        {
            MenuItem categoryItem = sender as MenuItem;
            if (null != categoryItem)
            {
                string categoryName = categoryItem.Header as string;
                string userName = categoryItem.DataContext as string;
                if (!string.IsNullOrEmpty(categoryName) && !string.IsNullOrEmpty(userName))
                {
                    // add the user to the category
                    CategoriesManager.GetCategoryUserNames(categoryName).Add(userName);

                    // refersh category tab
                    CategoryTabItem tab = GetCategoryTab(categoryName);
                    CollectionViewSource.GetDefaultView(tab.TweetsPanel.ItemsSource).Refresh();
                    RefreshViews();
                }
            }
        }

        #endregion 

        #region Categories Event

        void btnSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            // retrieve info from create category form
            string categoryName = txtCategoryName.Text;
            List<string> userNames = lbFollowing.SelectedItems.AsQueryable().OfType<User>().Select(user => user.ScreenName).ToList();
            string categoryColor = txtCategoryColor.Text;

            // verify form is filled in
            if (string.IsNullOrEmpty(categoryName) ||
                string.IsNullOrEmpty(categoryColor) ||
                userNames.Count() == 0)
            {
                return;
            }

            // save category
            CategoriesManager.SaveCategory(categoryName, userNames, categoryColor);

            // create the tab if it doesn't exist
            // then bring the tab to view
            CategoryTabItem edittedTab = GetCategoryTab(categoryName);
            if (null == edittedTab)
            {
                // add tab
                CategoryTabItem tab = CreateCategory(categoryName, userNames, categoryColor);
                tcCategories.Items.Insert(tcCategories.Items.Count - 1, tab);
                tcCategories.SelectedItem = tab;
            }
            else
            {
                tcCategories.SelectedItem = edittedTab;
                CollectionViewSource.GetDefaultView(edittedTab.TweetsPanel.ItemsSource).Refresh();
            }

            // reset category edit/create form
            ResetCreateCategoryForm();
            RefreshViews();
        }

        void tabEdit_Click(object sender, RoutedEventArgs e)
        {
            CategoryTabItem tab = sender as CategoryTabItem;
            if (null != tab)
            {
                ResetCreateCategoryForm();

                // retrieve categories data and populate edit form
                string categoryName = tab.Name;
                IList<string> userNames = CategoriesManager.GetCategoryUserNames(categoryName);
                string categoryColor = CategoriesManager.GetCategoryColor(categoryName);

                // select users in list box
                lbFollowing.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                {
                    lbFollowing.Items.OfType<User>()
                        .Join(
                            userNames,
                            (user) => user.ScreenName,
                            (userName) => userName,
                            (user, userName) => user
                        )
                        .All(
                            user => lbFollowing.SelectedItems.Add(user) >= 0
                        );
                });

                // populate category name and color fields
                txtCategoryName.Text = categoryName;
                txtCategoryColor.Text = categoryColor;

                // show category edit form
                tcCategories.SelectedItem = tabEdit;
            }
        }

        void tabClose_Click(object sender, RoutedEventArgs e)
        {
            CategoryTabItem tab = sender as CategoryTabItem;
            if (null == tab)
            {
                return;
            }

            // confirmation delete
            ConfirmationWindow confirm = new ConfirmationWindow { Message = string.Format("Delete category - {0}", tab.Name) };
            confirm.Left = Left + Width / 2 - confirm.Width / 2;
            confirm.Top = Top + Height / 2 - confirm.Height / 2;
            if (confirm.ShowDialog() != true)
            {
                return;
            }

            // do some cleaning up on the tweets panel in the tab
            tab.TweetsPanel.AvatarClicked -= imgAvatar_MouseDown;
            tab.TweetsPanel.TweetTextLoaded -= TextBlock_Loaded;
            tab.TweetsPanel.ReplyClicked -= btnReply_Click;
            tab.TweetsPanel.DirectMessageClicked -= btnDirectMessage_Click;
            tab.TweetsPanel.RetweetClicked -= btnRetweet_Click;
            tab.TweetsPanel.DeleteClicked -= btnDelete_Click;
            tab.TweetsPanel.Dispose();

            // do some cleaning up on the tab to be closed
            tab.EditClick -= tabEdit_Click;
            tab.Close -= tabClose_Click;
            tab.Dispose();
            tcCategories.Items.Remove(tab);

            // retrieve category name from tab header and remove it from category manager
            string categoryName = tab.Name;
            CategoriesManager.RemoveCategory(categoryName);

            // refresh views
            RefreshViews();
        }

        void btnColorPicker_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cp = new ColorPickerDialog();
            cp.Owner = this;
            cp.Left = Left + Width / 2 - cp.Width / 2;
            cp.Top = Top + Height / 2 - cp.Height / 2;
            if (cp.ShowDialog() == true)
            {
                txtCategoryColor.Text = cp.SelectedColor.ToString();
            }
        }

        #endregion

        #region Search Panel Events/Helpers

        void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch_Click(sender, e);
            }
        }

        void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string keyword = tbSearch.Text;
            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            // update ui
            SetLoading(true);

            // make sure the search term doesn't exist already
            SearchManager theSearchManager = _twitter.SavedSearchesManager.GetSearchManager(keyword);
            if (null == theSearchManager)
            {
                // create new search tab
                TabItem newSearchTab = CreateSearch(keyword);
                tcSearches.Items.Add(newSearchTab);
                tcSearches.SelectedItem = newSearchTab;
            }
            else
            {
                theSearchManager.Keyword = keyword;
                theSearchManager.SearchAsync();
            }
        }

        private TabItem CreateSearch(string keyword)
        {
            // create saved search
            SearchManager theSearch = _twitter.SavedSearchesManager.CreateSearch(
                keyword,
                SearchManager_OperationCompleted,
                SearchManager_OperationError
            );
            theSearch.SearchAsync();

            // create search results panel
            SearchResultsPanelControl searchResultsPanel = new SearchResultsPanelControl(keyword);
            searchResultsPanel.ItemsSource = theSearch.All;
            searchResultsPanel.TextLoaded += new RoutedEventHandler(TextBlock_Loaded);
            searchResultsPanel.MoreResultsClicked += new SearchResultsPanelControl.MoreSearchResultsClickedEventHandler(MoreResults_Click);
            searchResultsPanel.AvatarClicked += new MouseButtonEventHandler(imgAvatar_MouseDown);
            return CreateSearchTab(keyword, searchResultsPanel);
        }

        private TabItem CreateSearchTab(string keyword, SearchResultsPanelControl searchResultsPanel)
        {
            SearchResultTabItem tab = new SearchResultTabItem(keyword, 90, 30, Brushes.White, searchResultsPanel);
            tab.Style = FindResource("GenericTabItem") as Style;
            tab.CloseButtonStyle = FindResource("GenericButton") as Style;
            tab.Close += new RoutedEventHandler(searchTabClose_Click);
            return tab;
        }

        void MoreResults_Click(object sender, MoreSearchResultsClickedEventArgs e)
        {
            // get more results for the keyword search
            string keyword = e.Keyword;
            SearchManager theManager = _twitter.SavedSearchesManager.GetSearchManager(keyword);
            if (null != theManager)
            {
                SetLoading(true);
                theManager.SearchAsync();
            }
        }

        void searchTabClose_Click(object sender, RoutedEventArgs e)
        {
            SearchResultTabItem tab = sender as SearchResultTabItem;
            if (null == tab)
            {
                return;
            }

            // confirmation delete
            ConfirmationWindow confirm = new ConfirmationWindow { Message = string.Format("Delete search - {0}", tab.Name) };
            confirm.Left = Left + Width / 2 - confirm.Width / 2;
            confirm.Top = Top + Height / 2 - confirm.Height / 2;
            if (confirm.ShowDialog() != true)
            {
                return;
            }

            // remove search from saved search
            _twitter.SavedSearchesManager.RemoveSearch(tab.Name);

            // dispose tab and remove from UI
            tab.Close -= new RoutedEventHandler(searchTabClose_Click);
            tab.Dispose();
            tcSearches.Items.Remove(tab);
        }

        #endregion

        #region Rich Textblock Events

        void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            // verify sender is a textblock
            TextBlock textblock = sender as TextBlock;
            if (null == textblock)
            {
                return;
            }

            // verify text is not empty
            string text = textblock.Text;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            // find all occurrences of url and user references
            //string urlPattern = @"https?://[a-zA-Z0-9.%/?=]+";
            string urlPattern = @"https?://[\w\d\-./%&?=_+]+";
            string userPattern = @"@[a-zA-Z0-9_]+";
            string parserPatter = urlPattern + "|" + userPattern;
            MatchCollection matches = Regex.Matches(text, parserPatter);

            // clear textblock for appending text decorations
            textblock.Text = string.Empty;

            // add text decorations for urls and users
            string phrase;
            int lastMatchIndex = 0;
            foreach (Match match in matches)
            {
                phrase = text.Substring(lastMatchIndex, match.Index - lastMatchIndex);

                // append the text before the match
                if (phrase.Length > 0)
                {
                    Utils.AppendTo(textblock, phrase, Utils.Url_Clicked, UserLink_Clicked, url_MouseEnter, url_MouseLeave);
                }

                // append the match
                Utils.AppendTo(textblock, match.Value, Utils.Url_Clicked, UserLink_Clicked, url_MouseEnter, url_MouseLeave);

                // update string pointer
                lastMatchIndex = match.Index + match.Length;
            }

            // append the text after the last match
            Utils.AppendTo(textblock, text.Substring(lastMatchIndex), Utils.Url_Clicked, UserLink_Clicked, url_MouseEnter, url_MouseLeave);
        }

        void url_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_isWebPagePreviewEnabled)
            {
                Hyperlink link = sender as Hyperlink;
                if (null != link)
                {
                    Point mousePosition = e.GetPosition(this);
                    _browser.GetPreview(link.NavigateUri);
                    _browser.Left = mousePosition.X + this.Left + 50;
                    _browser.Top = mousePosition.Y + this.Top - _browser.Height / 2 + 25;
                    _browser.Show();
                }
            }
        }

        void url_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_browser.IsVisible)
            {
                _browser.Hide();
            }
        }

        #endregion

        void window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        void window_Closed(object sender, EventArgs e)
        {
            if (null != _trayIcon)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            // make sure application was initialized properly then save user settings
            if (null != _twitter && null != LoggedInUser.ScreenName)
            {
                // save categories settings
                CategoriesManager.SaveConfiguration();

                // save searches
                ConfigurationManager.Default.SavedSearches = _twitter.SavedSearchesManager.GetKeywords();

                // save windows size and location
                UserSetting setting = ConfigurationManager.Default.GetUserSetting(LoggedInUser.ScreenName);
                setting.WindowSize = new System.Drawing.Size((int)Width, (int)Height);
                setting.WindowLocation = new System.Drawing.Point((int)Left, (int)Top);
                ConfigurationManager.Default.Save();
            }

            // shutdown application
            Application.Current.Shutdown();
        }

        void trayIcon_Click(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Show();
                WindowState = WindowState.Normal;
            }
        }

        #endregion

        #region Helper Methods

        void ResetTweetPane()
        {
            txtTweet.Text = string.Empty;
            btnSend.DataContext = null;
        }

        void ResetCreateCategoryForm()
        {
            txtCategoryName.Text = string.Empty;
            txtCategoryColor.Text = string.Empty;
            lbFollowing.SelectedItems.Clear();
        }

        void SetFocus(UIElement element)
        {
            element.Focus();

            // when focusing on textbox, set cursor to the end
            TextBox textBox = element as TextBox;
            if (null != textBox || !string.IsNullOrEmpty(textBox.Text))
            {
                textBox.SelectionStart = textBox.Text.Length;
            }
        }

        void SetLoading(bool isLoading)
        {
            if (isLoading)
            {
                txtLoading.Visibility = Visibility.Visible;
            }
            else
            {
                txtLoading.Visibility = Visibility.Hidden;
            }
        }

        void SetMoreSearchResultsButtonVisibility(string keyword, Visibility visibility)
        {
            // input argument check
            if (string.IsNullOrEmpty(keyword))
            {
                return;
            }

            foreach (SearchResultTabItem tab in tcSearches.Items)
            {
                if (keyword.Equals(tab.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    tab.SearchResultsPanel.ShowMoreSearchResultsVisibility = visibility;
                }
            }
        }

        string GetDirectMessageBodyFromText(User user, string text)
        {
            string expression = string.Format(@"{0}\s*{1}\s+(.*)", Constants.DIRECT_MESSAGE_PREFIX, user.ScreenName);
            Regex regex = new Regex(expression);
            Match match = regex.Match(text);
            return match.Groups[1].Value;
        }

        private CategoryTabItem CreateCategory(string categoryName, IList<string> userNames, string color)
        {
            // init tweets panel control for category
            TweetsPanelControl tweetsPanel = new TweetsPanelControl();
            tweetsPanel.IsShowMoreButton = Visibility.Collapsed;
            tweetsPanel.AvatarClicked += new MouseButtonEventHandler(imgAvatar_MouseDown);
            tweetsPanel.TweetTextLoaded += new RoutedEventHandler(TextBlock_Loaded);
            tweetsPanel.ReplyClicked += new RoutedEventHandler(btnReply_Click);
            tweetsPanel.DirectMessageClicked += new RoutedEventHandler(btnDirectMessage_Click);
            tweetsPanel.RetweetClicked += new RoutedEventHandler(btnRetweet_Click);
            tweetsPanel.DeleteClicked += new RoutedEventHandler(btnDelete_Click);
            tweetsPanel.FavouriteClicked += new RoutedEventHandler(cbFavourite_Click);

            // init category items source
            tweetsPanel.ItemsSource = CategoriesManager.GetCategoryView(categoryName);
            CollectionViewSource.GetDefaultView(tweetsPanel.ItemsSource).Refresh();

            // create category tab
            return CreateCategoryTab(categoryName, tweetsPanel);
        }

        private CategoryTabItem CreateCategoryTab(string name, TweetsPanelControl content)
        {
            CategoryTabItem tab = new CategoryTabItem(name, 90, 30, Brushes.White, content);
            tab.Style = FindResource("GenericTabItem") as Style;
            tab.CloseButtonStyle = FindResource("GenericButton") as Style;
            tab.EditClick += new RoutedEventHandler(tabEdit_Click);
            tab.Close += new RoutedEventHandler(tabClose_Click);
            return tab;
        }

        private ContextMenu GetAvatarContextMenu(string userName)
        {
            // build context menu root
            ContextMenu menu = new ContextMenu();
            MenuItem rootItem = new MenuItem { Header = "Add To" };
            menu.Items.Add(rootItem);

            // build submenu for categories
            MenuItem categoryItem;
            foreach (string categoryName in CategoriesManager.GetCategoryNames())
            {
                categoryItem = new MenuItem { Header = categoryName };
                // TODO: hook up eventhandlers
                categoryItem.DataContext = userName;
                categoryItem.Click += new RoutedEventHandler(AddUserToCategory_Click);
                rootItem.Items.Add(categoryItem);
            }
            return menu;
        }

        private CategoryTabItem GetCategoryTab(string categoryName)
        {
            // input arg check
            if(string.IsNullOrEmpty(categoryName))
            {
                return null;
            }

            // iterate and find tab for the category
            foreach (TabItem tab in tcCategories.Items)
            {
                CategoryTabItem catTab = tab as CategoryTabItem;
                if (null != catTab && categoryName.Equals(catTab.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return catTab;
                }
            }
            return null;
        }

        private void RefreshViews()
        {
            CollectionViewSource.GetDefaultView(AllTweets).Refresh();
            CollectionViewSource.GetDefaultView(AllReplies).Refresh();
            CollectionViewSource.GetDefaultView(AllDirectMessages).Refresh();
            CollectionViewSource.GetDefaultView(SelectedUserTweets).Refresh();

            //AllTweets.NotifyRebind();
            //AllReplies.NotifyRebind();
            //SelectedUserTweets.NotifyRebind();
        }

        private void ShowAlert(string message, User user)
        {
            _notififcation.Message = message;
            _notififcation.User = user;
            _notififcation.Left = SystemParameters.PrimaryScreenWidth - _notififcation.Width;
            _notififcation.Top = SystemParameters.PrimaryScreenHeight - _notififcation.Height;
            _notififcation.Show(); 
        }

        private void ShowAlert(string message, string authorAvatarUrl)
        {
            _notififcation.Message = message;
            _notififcation.AvatarUrl = authorAvatarUrl;
            _notififcation.Left = SystemParameters.PrimaryScreenWidth - _notififcation.Width;
            _notififcation.Top = SystemParameters.PrimaryScreenHeight - _notififcation.Height;
            _notififcation.Show();
 
        }

        private void ShowSnarlAlert(string title, string text, int displayTime, string iconPath)
        {
            string localIconPath = "";
            if(!File.Exists(iconPath)) {
                localIconPath = pathToLogoImage;
            }
            else
            {
                localIconPath = iconPath;
            }
            SnarlConnector.ShowMessage(title, text, displayTime, localIconPath, SnarlMsgWndHandle, Snarl.WindowsMessage.WM_USER + 43);
        }

        private void ShowSnarlAlert(Status tweet, string type)
        {
            string tempFile = System.IO.Path.GetTempFileName();
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(tweet.User.ProfileImageUrl, tempFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                tempFile = pathToLogoImage;
            }
            // new tweet is the default
            string alertClass = "New tweet";
            if (type == "directMessage")
            {
                alertClass = "New direct message";
            }
            SnarlConnector.ShowMessageEx(alertClass, tweet.User.Name, tweet.Text, 10, tempFile, SnarlMsgWndHandle, Snarl.WindowsMessage.WM_USER + 44, "");
            if (tempFile != pathToLogoImage)
            {
                System.IO.File.Delete(tempFile);
            }

        }

        private void RegisterWithSnarlIfAvailable()
        {
            if (SnarlConnector.GetSnarlWindow() != IntPtr.Zero)
            {
                CreateSnarlMessageWindowForCommunication();
            }
        }

        private void CreateSnarlMessageWindowForCommunication()
        {
            this.snarlComWindow = new NativeWindowApplication.SnarlMsgWnd();
            this.SnarlMsgWndHandle = snarlComWindow.Handle;
            pathToLogoImage = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Images\\Logo.png";
            this.snarlComWindow.pathToIcon = pathToLogoImage;
            SnarlConnector.RegisterConfig(this.SnarlMsgWndHandle, "digiTweet", Snarl.WindowsMessage.WM_USER + 55, pathToLogoImage);
            SnarlConnector.RegisterAlert("digiTweet", "New tweet");
            SnarlConnector.RegisterAlert("digiTweet", "New direct message");
            SnarlConnector.RegisterAlert("digiTweet", "Search results");
        }

        #endregion

        #region Properties

        public SortedTweetCollection<Status> AllTweets
        {
            get
            {
                if (null == _twitter)
                {
                    return null;
                }
                return _twitter.TweetsManager.All;
            }
        }

        public SortedTweetCollection<Status> AllReplies
        {
            get
            {
                return _twitter.RepliesManager.All;
            }
        }

        public SortedTweetCollection<DirectMessage> AllDirectMessages
        {
            get
            {
                return _twitter.DirectMessageManager.All;
            }
        }

        public ObservableCollection<User> AllFriends
        {
            get
            {
                return _twitter.FriendsManager.All;
            }
        }

        public SortedTweetCollection<Status> SelectedUserTweets
        {
            get
            {
                return _twitter.UserProfileManager.All;
            }
        }

        //public ObservableCollection<SearchResult> SearchResults
        //{
        //    get
        //    {
        //        return _twitter.SearchManager.All;
        //    }
        //}

        public SortedTweetCollection<Status> AllFavourites
        {
            get
            {
                return _twitter.FavouritesManager.All;
            }
        }

        public User LoggedInUser
        {
            get
            {
                return _twitter.LoggedInUser;
            }
        }

        public ExtendedUser SelectedUser
        {
            get
            {
                return (ExtendedUser)this.GetValue(SelectedUserProperty);
            }
            set
            {
                this.SetValue(SelectedUserProperty, value);
            }
        }

        public int TweetMaxLength
        {
            get
            {
                return Constants.TWEET_MAX_LENGTH;
            }
        }

        public DateTime LastUpdated
        {
            get
            {
                return (DateTime)this.GetValue(LastUpdatedProperty);
            }
            set
            {
                this.SetValue(LastUpdatedProperty, value);
            }
        }

        #endregion
    }
}
