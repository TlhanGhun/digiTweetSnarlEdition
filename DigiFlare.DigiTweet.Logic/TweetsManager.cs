using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DigiFlare.DigiTweet
{
    public class TweetsManager : BaseCollectionManager<Status>
    {
        public const string APP_NAME = "digitweet";

        public static TweetsManager DefaultInstance;

        #region Public Events

        public delegate void TweetsManagerCompletedHandler(object sender, TweetsManagerOperationCompletedEventArgs e);
        public event TweetsManagerCompletedHandler TweetsManagerOperationCompleted;

        #endregion

        #region Constructor

        public TweetsManager(ITwitterClient twitterApiClient)
            : base(twitterApiClient)
        {
        }

        public TweetsManager(string username, string password)
            : base(username, password)
        {
        }

        #endregion

        #region Public Methods (Synchronous)

        public virtual void RefreshAll()
        {
            try
            {
                _tweets.Clear();
                _tweets.Add(_twitterApiClient.FriendsTimeline());
            }
            catch (Exception ex)
            {
                // TODO: notify UI retrieving all tweets failed
            }
        }

        public virtual void Refresh()
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
                    Statuses newTweets = _twitterApiClient.FriendsTimelineSince(newestLocalTweet.Id);
                    _tweets.Add(newTweets);
                }
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer refresh lite failed
            }
        }

        public virtual void Update(string message)
        {
            try
            {
                Status update = _twitterApiClient.UpdateWithSource(message, APP_NAME);
                _tweets.Add(update);
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer update failed
            }
        }

        public virtual void Delete(Status tweet)
        {
            try
            {
                _twitterApiClient.DestroyStatus(tweet.Id);
                _tweets.Remove(tweet);
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer delete failed
            }
        }

        public virtual void Reply(string message, Status tweet)
        {
            try
            {
                Status reply = _twitterApiClient.UpdateWithSource(message, tweet.Id, APP_NAME);
                _tweets.Add(reply);
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer reply failed
            }
        }

        #endregion

        #region Async Refresh

        public virtual void RefreshAllAsync()
        {
            BackgroundWorker refreshAllWorker = new BackgroundWorker();
            refreshAllWorker.DoWork += new DoWorkEventHandler(RefreshWorker_DoWork);
            refreshAllWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshWorker_RunWorkerCompleted);
            refreshAllWorker.RunWorkerAsync();
        }

        public virtual void RefreshAsync()
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
                    tweets = client.FriendsTimeline();
                }
                else
                {
                    tweets = client.FriendsTimelineSince(id);
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
            // notify caller on exception
            if (e.Error != null)
            {
                OnError("Refresh tweets failed. Please try again later.", e.Error);
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

            // notify caller operation is completed
            OnCompleted(refreshResult.Tweets);
        }

        #endregion

        #region Async More

        public void MoreAsync()
        {
            if (0 == _tweets.Count)
            {
                RefreshAllAsync();
            }
            else
            {
                BackgroundWorker moreWorker = new BackgroundWorker();
                Status oldestLocalTweet = _tweets.GetOldest();
                moreWorker.DoWork += new DoWorkEventHandler(moreWorker_DoWork);
                moreWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(moreWorker_RunWorkerCompleted);
                moreWorker.RunWorkerAsync(oldestLocalTweet.Id);
            }
        }

        private void moreWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // retrieve parameter to call twitter api
            string id = e.Argument as string;

            // twitter api call
            TwitterClient client = new TwitterClient(_username, _password);
            Statuses results = null;
            try
            {
                results = client.FriendsTimelineUntil(id);
                client.Close();
                e.Result = results;
            }
            finally
            {
                client.Abort();
            }
        }

        private void moreWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscribers on error
            if (null != e.Error)
            {
                OnError("Retrieving more tweets failed. Please try again later.", e.Error);
                return;
            }

            // add statuses to local collection
            Statuses results = e.Result as Statuses;
            _tweets.Add(results);

            // notify subscribers on completed
            OnCompleted(null);
        }

        #endregion

        #region Async Update(Send)

        public void UpdateAsync(string message)
        {
            BackgroundWorker updateWorker = new BackgroundWorker();
            updateWorker.DoWork += new DoWorkEventHandler(updateWorker_DoWork);
            updateWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(updateWorker_RunWorkerCompleted);
            updateWorker.RunWorkerAsync(message);
        }

        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get message argument
            string message = e.Argument as string;

            // call twitter api
            Status status = null;
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                status = client.UpdateWithSource(message, APP_NAME);
                client.Close();
                e.Result = status;
            }
            finally
            {
                client.Abort();
            }
        }

        private void updateWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify UI on exception
            if (null != e.Error)
            {
                OnError("Update tweet failed. Please try again later.", e.Error);
                return;
            }

            // add result to collection
            Status result = e.Result as Status;
            _tweets.Add(result);

            // notify caller operation is completed
            OnCompleted(null);
        }

        #endregion

        #region Async Delete

        public void DeleteAsync(Status tweet)
        {
            BackgroundWorker deleteWorker = new BackgroundWorker();
            deleteWorker.DoWork += new DoWorkEventHandler(deleteWorker_DoWork);
            deleteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(deleteWorker_RunWorkerCompleted);
            deleteWorker.RunWorkerAsync(tweet);
        }


        private void deleteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // get tweet to delete
            Status tweet = e.Argument as Status;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                client.DestroyStatus(tweet.Id);
                e.Result = tweet;
                client.Close();
            }
            finally
            {
                client.Abort();
            }
        }

        private void deleteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify caller on exception
            if (null != e.Error)
            {
                OnError("Delete tweet failed. Please try again later.", e.Error);
                return;
            }

            // delete tweet from local collection if twitter api call was succeessful
            Status deleteMe = e.Result as Status;
            _tweets.Remove(deleteMe);

            // notify caller operation is completed
            OnCompleted(null);
        }

        #endregion

        #region Async Reply

        public void ReplyAsync(string message, Status tweet)
        {
            BackgroundWorker replyWorker = new BackgroundWorker();
            replyWorker.DoWork += new DoWorkEventHandler(replyWorker_DoWork);
            replyWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(replyWorker_RunWorkerCompleted);
            replyWorker.RunWorkerAsync(new object[] { message, tweet });
        }

        private void replyWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // retrieve arguments needed to make api call
            object[] args = e.Argument as object[];
            string message = args[0] as string;
            Status tweet = args[1] as Status;

            // twitter api call
            TwitterClient client = new TwitterClient(_username, _password);
            Status reply = null;
            try
            {
                reply = client.UpdateWithSource(message, tweet.Id, APP_NAME);
                client.Close();
                e.Result = reply;
            }
            finally
            {
                client.Abort();
            }
        }

        private void replyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscribers on error
            if (null != e.Error)
            {
                OnError("Reply to tweet failed. Please try again later.", e.Error);
                return;
            }

            // add the tweet to the local collection
            Status reply = e.Result as Status;
            _tweets.Add(reply);

            // notify subscribers operation is completed
            OnCompleted(null);
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

        #region Base Manager Overrides

        protected void OnCompleted(IList<Status> newTweets)
        {
            base.OnCompleted(newTweets);
            if (null != TweetsManagerOperationCompleted)
            {
                TweetsManagerOperationCompletedEventArgs e = new TweetsManagerOperationCompletedEventArgs { NewTweets = newTweets };
                TweetsManagerOperationCompleted(this, e);
            }
        }

        #endregion
    }
}
