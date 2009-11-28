using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;

namespace DigiFlare.DigiTweet
{
    public class DirectMessagesManager : BaseCollectionManager<DirectMessage>
    {
        #region Constructor

        public DirectMessagesManager(ITwitterClient twitterApiClient) : base(twitterApiClient)
        {
        }

        public DirectMessagesManager(string username, string password) : base(username, password)
        {
        }

        #endregion

        #region Public Methods (Synchronous)

        public void RefreshAll()
        {
            try
            {
                _tweets.Clear();
                _tweets.Add(_twitterApiClient.DirectMessages());
            }
            catch (Exception ex)
            {
                // TODO: notify UI retrieving all tweets failed
            }
        }

        public void Refresh()
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
                    DirectMessage newestDirectMessage = _tweets.GetNewest();
                    DirectMessages newDirectMessages = _twitterApiClient.DirectMessages(newestDirectMessage.Id);
                    _tweets.Add(newDirectMessages);
                }
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer refresh lite failed
            }
        }

        public void New(User user, string message)
        {
            try
            {
                DirectMessage directMessage = _twitterApiClient.New(user.ScreenName, message);
                _tweets.Add(directMessage);
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer direct message failed
            }
        }

        public void Delete(DirectMessage directMessage)
        {
            try
            {
                _twitterApiClient.DestroyDirectMessage(directMessage.Id);
                _tweets.Remove(directMessage);
            }
            catch (Exception ex)
            {
                // TODO: notify UI layer delete failed
            }
        }

        #endregion

        #region Refresh Async

        public void RefreshAllAsync()
        {
            BackgroundWorker refreshAllWorker = new BackgroundWorker();
            refreshAllWorker.DoWork += new DoWorkEventHandler(RefreshWorker_DoWork);
            refreshAllWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RefreshWorker_RunWorkerCompleted);
            refreshAllWorker.RunWorkerAsync();
        }

        public void RefreshAsync()
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
                DirectMessage newestLocalTweet = _tweets.GetNewest();
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
            TwitterClient client = new TwitterClient(_username, _password);
            DirectMessages directMessages = null;
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    directMessages = client.DirectMessages();
                }
                else
                {
                    directMessages = client.DirectMessages(id);
                }
                client.Close();
                e.Result = new DirectMessagesResult { Id = id, DirectMessages = directMessages };
            }
            finally
            {
                client.Abort();
            }
        }

        private void RefreshWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscirber on exception
            if (e.Error != null)
            {
                OnError("Refresh direct messages failed. Please try again later.", e.Error);
                return;
            }

            // verify tweets retrieved successful
            DirectMessagesResult refreshResult = e.Result as DirectMessagesResult;
            if (refreshResult != null)
            {
                if (string.IsNullOrEmpty(refreshResult.Id))
                {
                    _tweets.Clear();
                }
                _tweets.Add(refreshResult.DirectMessages);
            }

            // notify subscriber operation compmleted
            OnCompleted(null);
        }

        #endregion

        #region Send Async

        public void Send(User user, string message)
        {
            BackgroundWorker directMessageWorker = new BackgroundWorker();
            directMessageWorker.DoWork += new DoWorkEventHandler(directMessageWorker_DoWork);
            directMessageWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(directMessageWorker_RunWorkerCompleted);
            directMessageWorker.RunWorkerAsync(new object[] { user, message });
        }

        private void directMessageWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // retrieve parameters
            object[] args = e.Argument as object[];
            User user = args[0] as User;
            string message = args[1] as string;

            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            DirectMessage directMessage = null;
            try
            {
                directMessage = client.New(user.Id, message);
                client.Close();
                e.Result = directMessage;
            }
            finally
            {
                client.Abort();
            }
        }

        private void directMessageWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscriber on error
            if (null != e.Error)
            {
                OnError("Direct message failed. Please try again later.", e.Error);
                return;
            }

            // TODO: do something with the direct message sent?

            // notify subscriber operation is completed
            OnCompleted(null);
        }

        #endregion

        #region Delete Async

        public void DeleteAsync(DirectMessage directMessage)
        {
            BackgroundWorker deleteWorker = new BackgroundWorker();
            deleteWorker.DoWork += new DoWorkEventHandler(deleteWorker_DoWork);
            deleteWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(deleteWorker_RunWorkerCompleted);
            deleteWorker.RunWorkerAsync(directMessage);
        }

        private void deleteWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            DirectMessage deleteMe = e.Argument as DirectMessage;
            
            // call twitter api
            TwitterClient client = new TwitterClient(_username, _password);
            try
            {
                client.DestroyDirectMessage(deleteMe.Id);
                client.Close();
                e.Result = deleteMe;
            }
            finally
            {
                client.Abort();
            }
        }

        private void deleteWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // notify subscriber on exception
            if (null != e.Error)
            {
                OnError("Delete direct message failed. Please try again later.", e.Error);
                return;
            }

            // delete direct message in the local collection
            DirectMessage deleteMe = e.Result as DirectMessage;
            _tweets.Remove(deleteMe);
            OnCompleted(null);
        }

        #endregion

        #region Properties

        public SortedTweetCollection<DirectMessage> All
        {
            get
            {
                return _tweets;
            }
        }

        #endregion
    }
}
