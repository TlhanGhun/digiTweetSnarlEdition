using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DigiFlare.DigiTweet.DataAccess;
using System.Collections.ObjectModel;

namespace DigiFlare.DigiTweet
{
    public class FriendsManager : BaseManager
    {

        #region Instance Variables

        private Users _following;
        private Users _followers;
        private ObservableCollection<User> _friends;

        #endregion

        #region Constructor

        public FriendsManager(string username, string password)
            : base(username, password)
        {
            _friends = new ObservableCollection<User>();
        }

        #endregion

        #region Get Friends Async

        public void GetFriendsAsync()
        {
            BackgroundWorker getFriendsWorker = new BackgroundWorker();
            getFriendsWorker.DoWork += new DoWorkEventHandler(getFriendsWorker_DoWork);
            getFriendsWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(getFriendsWorker_RunWorkerCompleted);
            getFriendsWorker.RunWorkerAsync();
        }

        void getFriendsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get friends from twitter
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                _following = client.Friends();
                //_followers = client.Followers();
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
                throw ex;
            }
        }

        void getFriendsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // make sure no error occured
            if (null != e.Error)
            {
                OnError("Get friends failed. Please try again later.", e.Error);
                return;
            }

            //List<User> friends = _following.Join(
            //    _followers,
            //    (user) => user.Id,
            //    (user) => user.Id,
            //    (user1, user2) => user1).ToList();
            
            // add friends to collection
            _friends.Clear();
            foreach (User friend in _following)
            {
                _friends.Add(friend);
            }
            OnCompleted(null);
        }

        #endregion

        #region Properties

        public ObservableCollection<User> All
        {
            get
            {
                return _friends;
            }
        }

        #endregion

    }
}
