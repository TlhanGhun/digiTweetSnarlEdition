using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;

namespace DigiFlare.DigiTweet
{
    public class RepliesManager : TweetsManager
    {
        public static RepliesManager DefaultInstance;

        #region Constructor

        public RepliesManager(ITwitterClient twitterApiClient)
            : base(twitterApiClient)
        {
        }

        public RepliesManager(string username, string password)
            : base(username, password)
        {
        }

        #endregion

        #region Public Methods (Synchronous)

        public override void RefreshAll()
        {
            try
            {
                _tweets.Clear();
                _tweets.Add(_twitterApiClient.Replies());
            }
            catch (Exception ex)
            {
                // TODO: notify UI retrieving all tweets failed
            }
        }

        public override void Refresh()
        {
            try
            {
                // if local tweet count is 0, get all tweets from server
                // otherwise, get tweets since the latest one
                if (0 == _tweets.Count)
                {
                    RefreshAll();
                }
                else
                {
                    // update tweets collection
                    Status newestLocalTweet = _tweets.GetNewest();
                    Statuses newTweets = _twitterApiClient.Replies(newestLocalTweet.Id);
                    _tweets.Add(newTweets);
                }
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer refresh lite failed
            }
        }

        public override void Update(string message)
        {
            throw new NotSupportedException("Cannot add new replies. Replies are from other users. Please use Tweets Manager to add new tweets");
        }

        public override void Delete(Status tweet)
        {
            throw new NotSupportedException("Cannot delete replies.  Replies are from other users. Please use Tweets Manager to delete your own tweets");
        }

        #endregion

        #region Refresh Async

        public override void RefreshAllAsync()
        {
            BackgroundWorker refreshAllWorker = new BackgroundWorker();
            refreshAllWorker.DoWork += new DoWorkEventHandler(RefreshWorker_DoWork);
            refreshAllWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshWorker_RunWorkerCompleted);
            refreshAllWorker.RunWorkerAsync();
        }

        public override void RefreshAsync()
        {
            // if local tweet count is 0, get all tweets from server
            // otherwise, get tweets since the latest one
            if (0 == _tweets.Count)
            {
                RefreshAllAsync();
            }
            else
            {
                // update tweets collection async
                Status newestLocalTweet = _tweets.GetNewest();
                BackgroundWorker refreshWorker = new BackgroundWorker();
                refreshWorker.DoWork += new DoWorkEventHandler(RefreshWorker_DoWork);
                refreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshWorker_RunWorkerCompleted);
                refreshWorker.RunWorkerAsync(newestLocalTweet.Id);
            }
        }

        private void RefreshWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // retrieve id argument
            string id = e.Argument as string;

            // call twitter api
            Statuses tweets = null;
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    tweets = client.Replies();
                }
                else
                {
                    tweets = client.Replies(id);
                }
                client.Close();
                e.Result = new TweetsResult { Id = id, Tweets = tweets };
            }
            finally
            {
                client.Abort();
            }
        }

        private void RefreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscribers on exception
            if (e.Error != null)
            {
                OnError("Refresh replies failed. Please try again later.", e.Error);
                return;
            }

            // verify tweets retrieved successful
            TweetsResult refreshResult = e.Result as TweetsResult;
            if (refreshResult != null)
            {
                if (string.IsNullOrEmpty(refreshResult.Id))
                {
                    _tweets.Clear();
                }
                _tweets.Add(refreshResult.Tweets);
            }

            // notify subscribers operation completed
            OnCompleted(null);
        }

        #endregion
    }
}
