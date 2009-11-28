using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;

namespace DigiFlare.DigiTweet
{
    public class FavouritesManager : BaseCollectionManager<Status>
    {

        #region Constructor

        public FavouritesManager(string username, string password)
            : base(username, password)
        {
        }

        #endregion

        #region Get Favourites Async

        public void GetFavouritesAsync()
        {
            BackgroundWorker favouritesWorker = new BackgroundWorker();
            favouritesWorker.DoWork += new DoWorkEventHandler(GetFavouritesWorker_DoWork);
            favouritesWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GetFavouritesWorker_RunWorkerCompleted);
            favouritesWorker.RunWorkerAsync();
        }

        private void GetFavouritesWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                e.Result = client.Favourites();
                client.Close();
            }
            finally
            {
                client.Abort();
            }
        }

        private void GetFavouritesWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                OnError("Get favourites failed.", e.Error);
                return;
            }

            // notify caller operation is completed
            Statuses result = e.Result as Statuses;
            _tweets.Clear();
            _tweets.Add(result);
            OnCompleted(result);
        }

        #endregion

        #region Favourite Async

        public void FavouriteAsync(Status tweet)
        {
            BackgroundWorker favouriteWorker = new BackgroundWorker();
            favouriteWorker.DoWork += new DoWorkEventHandler(FavouriteWorker_DoWork);
            favouriteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FavouriteWorker_RunWorkerCompleted);
            favouriteWorker.RunWorkerAsync(tweet);
        }

        private void FavouriteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get message argument
            Status oldStatus = e.Argument as Status;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                Status newStatus = client.Favourite(oldStatus.Id);
                client.Close();

                // pass results to completed handler
                Status[] results = new Status[] { oldStatus, newStatus };
                e.Result = results;
            }
            finally
            {
                client.Abort();
            }
        }

        private void FavouriteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                OnError("Favourite tweet failed.", e.Error);
                return;
            }

            // notify caller operation is completed
            Status[] results = e.Result as Status[];
            if (null != results && results.Length == 2)
            {
                _tweets.Add(results[1]);
                RefreshOtherManagers(results[0], results[1]);
                OnCompleted(results);
            }
        }

        #endregion

        #region Unfavourite Async

        public void UnfavouriteAsync(Status tweet)
        {
            BackgroundWorker unfavouriteWorker = new BackgroundWorker();
            unfavouriteWorker.DoWork += new DoWorkEventHandler(UnfavouriteWorker_DoWork);
            unfavouriteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(UnfavouriteWorker_RunWorkerCompleted);
            unfavouriteWorker.RunWorkerAsync(tweet);
        }

        private void UnfavouriteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get message argument
            Status oldStatus = e.Argument as Status;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            e.Result = oldStatus; // default value in case of error
            try
            {
                Status newStatus = client.Unfavourite(oldStatus.Id);
                client.Close();

                // pass results to completed handler
                Status[] results = new Status[] { oldStatus, newStatus };
                e.Result = results;
            }
            finally
            {
                client.Abort();
            }
        }

        private void UnfavouriteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                OnError("Unfavourite tweet failed.", e.Error);
                return;
            }

            // notify caller operation is completed
            Status[] results = e.Result as Status[];
            if (null != results && results.Length == 2)
            {
                // update favourites collection
                _tweets.Remove(results[0]);
                RefreshOtherManagers(results[0], results[1]);
                OnCompleted(results);
            }
        }

        #endregion

        #region Helper Methods

        private void RefreshOtherManagers(Status oldStatus, Status newStatus)
        {
            // TODO: ideally we should have a single repository of all the tweets
            // this may require a major refactoring
            // fix if there's time

            // update collections for other panels
            TweetsManager.DefaultInstance.All.RefreshTweet(oldStatus, newStatus);
            RepliesManager.DefaultInstance.All.RefreshTweet(oldStatus, newStatus);
            UserProfileManager.DefaultInstance.All.RefreshTweet(oldStatus, newStatus);
        }

        #endregion

        #region Properties

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
