using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DigiFlare.DigiTweet
{
    public class UserProfileManager : BaseCollectionManager<Status>
    {
        public static UserProfileManager DefaultInstance;

        #region Instance Variable

        private ITwitterClient _twitterApiClient = null;
        private ExtendedUser _selectedUser = null;

        #endregion

        #region Constructor

        public UserProfileManager(ITwitterClient twitterApiClient)
            : base(twitterApiClient)
        {
            //_twitterApiClient = twitterApiClient;
            _selectedUser = new ExtendedUser();
            //_tweets = new SortedTweetCollection<Status>();
        }

        public UserProfileManager(string username, string password)
            : base(username, password)
        {
            //_username = username;
            //_password = password;
            _selectedUser = new ExtendedUser();
            //_tweets = new SortedTweetCollection<Status>();
        }

        #endregion

        #region Override Events

        public delegate void UserProfileOperationCompletedHandler(object sender, UserProfileOperationCompletedEventArgs e);
        public event UserProfileOperationCompletedHandler UserProfileOperationCompleted;

        //public delegate void GetFullUserCompletedHandler(object sender, UserProfileOperationCompletedEventArgs e);
        //public event GetFullUserCompletedHandler GetFullUserCompleted;

        //public delegate void FollowUserCompletedHandler(object sender, UserProfileOperationCompletedEventArgs e);
        //public event FollowUserCompletedHandler FollowUserCompleted;

        #endregion

        #region GetUserDetails Method Async

        public void GetUserDetailsAsync(string id)
        {
            // setup background worker to get user profile
            BackgroundWorker getFullUserAsyncWorker = new BackgroundWorker();
            getFullUserAsyncWorker.DoWork += new DoWorkEventHandler(GetUserDetailsAsyncWorker_DoWork);
            getFullUserAsyncWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetUserDetailsAsyncWorker_RunWorkerCompleted);

            // setup background worker to get user tweets
            BackgroundWorker getUserTweetsWorker = new BackgroundWorker();
            getUserTweetsWorker.DoWork += new DoWorkEventHandler(GetUserTweetsWorker_DoWork);
            getUserTweetsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetUserTweetsWorker_RunWorkerCompleted);

            // start worker
            getFullUserAsyncWorker.RunWorkerAsync(id);
            getUserTweetsWorker.RunWorkerAsync(id);
        }

        private void GetUserDetailsAsyncWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get user id argument
            string id = e.Argument.ToString();

            // twitter api call
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                e.Result = client.ShowUser(id);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                throw;
            }
        }

        private void GetUserDetailsAsyncWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // verify no exception happened
            if (e.Error != null)
            {
                OnError("Get user details failed. Please try again later.", e.Error);
                return;
            }

            ExtendedUser user = e.Result as ExtendedUser;
            if (null != user)
            {
                _selectedUser = user;
            }

            // raise get user details completed event
            if (null != UserProfileOperationCompleted)
            {
                UserProfileOperationCompletedEventArgs args = new UserProfileOperationCompletedEventArgs { User = user };
                UserProfileOperationCompleted(this, args);
            }
        }

        public void GetUserTweetsAsync()
        {
            // make sure selected user is not null
            if (null == _selectedUser)
            {
                return;
            }

            Statuses userTweets = _twitterApiClient.UserTimelineById(_selectedUser.Id);
            _tweets.Clear();
            _tweets.Add(userTweets);
        }

        private void GetUserTweetsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get user id argument
            string id = e.Argument.ToString();
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                e.Result = client.UserTimelineById(id);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        private void GetUserTweetsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // verify no exception happened
            if (e.Error != null)
            {
                OnError("Get user tweets failed. Please try again later.", e.Error);
                return;
            }

            // verify async call results
            Statuses result = e.Result as Statuses;
            _tweets.Clear();
            if (null == result || 0 == result.Count)
            {
                return;
            }
            _tweets.Add(result);
        }

        #endregion

        #region Follow User Async

        public void FollowUser(string id)
        {
            BackgroundWorker followUserWorker = new BackgroundWorker();
            followUserWorker.DoWork += new DoWorkEventHandler(followUserWorker_DoWork);
            followUserWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(followUserWorker_RunWorkerCompleted);
            followUserWorker.RunWorkerAsync(id);
        }

        private void followUserWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get user id
            string userId = e.Argument as string;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                e.Result = client.FriendshipsCreate(userId);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        private void followUserWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check for error
            if (null != e.Error)
            {
                OnError("Follow user failed. Please try again later", e.Error);
                return;
            }

            // notify caller follow operation completed, pass back the followed User
            ExtendedUser followedUser = e.Result as ExtendedUser;
            UserProfileOperationCompletedEventArgs args = new UserProfileOperationCompletedEventArgs { User = followedUser };
            UserProfileOperationCompleted(sender, args);
        }

        #endregion

        #region Unfollow User Async

        public void UnfollowUser(string id)
        {
            BackgroundWorker unfollowUserWorker = new BackgroundWorker();
            unfollowUserWorker.DoWork += new DoWorkEventHandler(unfollowUserWorker_DoWork);
            unfollowUserWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(unfollowUserWorker_RunWorkerCompleted);
            unfollowUserWorker.RunWorkerAsync(id);
        }

        private void unfollowUserWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get user id
            string userId = e.Argument as string;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                e.Result = client.FriendshipsDestroy(userId);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        private void unfollowUserWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check for error
            if (null != e.Error)
            {
                OnError("Unfollow user failed. Please try again later", e.Error);
                return;
            }

            // notify caller follow operation completed, pass back the followed User
            ExtendedUser unfollowedUser = e.Result as ExtendedUser;
            UserProfileOperationCompletedEventArgs args = new UserProfileOperationCompletedEventArgs { User = unfollowedUser };
            UserProfileOperationCompleted(sender, args);
        }

        #endregion

        #region Properties

        public ExtendedUser SelectedUser
        {
            get
            {
                return _selectedUser;
            }
            set
            {
                _selectedUser = value;
            }
        }

        public SortedTweetCollection<Status> All
        {
            get
            {
                return _tweets;
            }
        }

        #endregion
    }
}
