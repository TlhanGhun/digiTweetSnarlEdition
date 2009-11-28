using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.Collections.ObjectModel;
using System.Transactions;
using System.Threading;
using System.ComponentModel;

namespace DigiFlare.DigiTweet
{
    public class TwitterApp
    {
        #region Instance Variables

        private ITwitterClient _twitterApiClient = null;
        private ITinyUrlClient _tinyUrlClient = null;
        private ITwitPicClient _twitPicClient = null;
        private ExtendedUser _loggedInUser = null;

        private TweetsManager _tweetsManager = null;
        private RepliesManager _repliesManager = null;
        private DirectMessagesManager _directMessagesManager = null;
        private UserProfileManager _userProfileManager = null;
        private FriendsManager _friendsManager = null;
        private SearchManager _searchManager = null;
        private SavedSearchesManager _savedSearchesManager = null;
        private FavouritesManager _favouritesManager = null;

        #endregion

        #region Custom Events

        // tiny url
        public delegate void TinyUrlCompletedHandler(object sender, TinyUrlCompletedEventArgs e);
        public event TinyUrlCompletedHandler TinyUrlCompleted;

        // twitpic
        public delegate void TwitPicCompletedHandler(object sender, TwitPicCompletedEventArgs e);
        public event TwitPicCompletedHandler TwitPicCompleted;

        #endregion

        #region Constructor

        public TwitterApp(string username, string password, ExtendedUser loggedInUser)
        {
            _twitterApiClient = new TwitterClient(username, password);
            _loggedInUser = loggedInUser;

            // init tiny url client
            _tinyUrlClient = new TinyUrlClient();

            // init twitpic client
            _twitPicClient = new TwitPicClient(username, password);

            // init tweets manager
            _tweetsManager = new TweetsManager(username, password);
            TweetsManager.DefaultInstance = _tweetsManager;

            // init replies manager
            _repliesManager = new RepliesManager(username, password);
            RepliesManager.DefaultInstance = _repliesManager;

            // init direct message manager
            _directMessagesManager = new DirectMessagesManager(username, password);

            // init user profile manager
            _userProfileManager = new UserProfileManager(username, password);
            _userProfileManager.SelectedUser = loggedInUser;
            UserProfileManager.DefaultInstance = _userProfileManager;

            // init friends manager
            _friendsManager = new FriendsManager(username, password);

            // init search manager
            _searchManager = new SearchManager(username, password);

            // init saved search manager
            _savedSearchesManager = new SavedSearchesManager(username, password);

            // init favourites manager
            _favouritesManager = new FavouritesManager(username, password);
        }

        public TwitterApp(ITwitterClient twitterApiClient, ITinyUrlClient tinyUrlClient, ITwitPicClient twitPicClient)
            : this(twitterApiClient)
        {
            _tinyUrlClient = tinyUrlClient;
            _twitPicClient = twitPicClient;
        }

        public TwitterApp(ITwitterClient twitterApiClient)
        {
            // init twitter api client and login
            _twitterApiClient = twitterApiClient;
            _loggedInUser = _twitterApiClient.VerifyCrednetials();

            // init tweets manager
            _tweetsManager = new TweetsManager(_twitterApiClient);
            _tweetsManager.RefreshAllAsync();

            // init replies manager
            _repliesManager = new RepliesManager(_twitterApiClient);
            _repliesManager.RefreshAllAsync();

            // init direct message manager
            _directMessagesManager = new DirectMessagesManager(_twitterApiClient);
            _directMessagesManager.RefreshAllAsync();

            // init user profile manager
            _userProfileManager = new UserProfileManager(_twitterApiClient);
        }

        #endregion

        #region TinyUrl Methods

        public void TinyUrlAsync(string url)
        {
            // setup background worker
            BackgroundWorker tinyUrlWorker = new BackgroundWorker();
            tinyUrlWorker.DoWork += new DoWorkEventHandler(TinyUrlWorker_DoWork);
            tinyUrlWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(TinyUrlWorker_RunWorkerCompleted);

            // start worker
            tinyUrlWorker.RunWorkerAsync(url);
        }

        private void TinyUrlWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string url = e.Argument.ToString();
            string tinyUrl = _tinyUrlClient.Create(url);
            e.Result = tinyUrl;
        }

        private void TinyUrlWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string url = e.Result as string;

            // raise tinyurl completed event
            if (null != TinyUrlCompleted)
            {
                TinyUrlCompletedEventArgs args = new TinyUrlCompletedEventArgs { Url = url };
                TinyUrlCompleted(this, args);
            }
        }

        #endregion

        #region TwitPic Methods

        public void UploadPicAsync(byte[] image, string fileName)
        {
            // setup background worker
            BackgroundWorker twitPicWorker = new BackgroundWorker();
            twitPicWorker.DoWork += new DoWorkEventHandler(UploadPicWorker_DoWork);
            twitPicWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UploadPicWorker_RunWorkerCompleted);

            // start worker
            twitPicWorker.RunWorkerAsync(new object[] { image, fileName });
        }

        private void UploadPicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // extract arguments
            object[] args = e.Argument as object[];
            byte[] image = args[0] as byte[];
            string fileName = args[1] as string;

            // upload image to twit pic
            string url = _twitPicClient.Upload(image, fileName);
            e.Result = url;
        }

        private void UploadPicWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string url = e.Result as string;

            // raise twitpic completed event
            if (null != TwitPicCompleted)
            {
                TwitPicCompletedEventArgs args = new TwitPicCompletedEventArgs { Url = url };
                TwitPicCompleted(this, args);
            }
        }

        #endregion

        #region Properties

        public ExtendedUser LoggedInUser
        {
            get
            {
                return _loggedInUser;
            }
        }

        public TweetsManager TweetsManager
        {
            get
            {
                return _tweetsManager;
            }
        }

        public RepliesManager RepliesManager
        {
            get
            {
                return _repliesManager;
            }
        }

        public DirectMessagesManager DirectMessageManager
        {
            get
            {
                return _directMessagesManager;
            }
        }

        public UserProfileManager UserProfileManager
        {
            get
            {
                return _userProfileManager;
            }
        }

        public FriendsManager FriendsManager
        {
            get
            {
                return _friendsManager;
            }
        }

        public SearchManager SearchManager
        {
            get
            {
                return _searchManager;
            }
        }

        public SavedSearchesManager SavedSearchesManager
        {
            get
            {
                return _savedSearchesManager;
            }
        }

        public FavouritesManager FavouritesManager
        {
            get
            {
                return _favouritesManager;
            }
        }

        #endregion
    }
}