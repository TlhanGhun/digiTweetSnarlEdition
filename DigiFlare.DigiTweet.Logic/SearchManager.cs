using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace DigiFlare.DigiTweet
{
    public class SearchManager : BaseManager, IDisposable
    {
        #region Instance Variable

        private string _username;
        private string _password;
        private ObservableCollection<SearchResult> _searchResults;

        private string _keyword = string.Empty;
        private int _page = 1;

        #endregion

        #region Events

        public delegate void SearchManagerOperationCompletedHandler(object sender, SearchManagerOperationCompletedEventArgs e);
        public event SearchManagerOperationCompletedHandler SearchManagerOperationCompleted;

        public delegate void SearchManagerOperationErrorHandler(object sender, SearchManagerOperationErrorEventArgs e);
        public event SearchManagerOperationErrorHandler SearchManagerOperationError;

        #endregion

        #region Constructor

        public SearchManager(string username, string password)
        {
            _username = username;
            _password = password;
            _searchResults = new ObservableCollection<SearchResult>();
        }

        #endregion

        #region Public Methods

        public SearchResults Search(string keyword)
        {
            TwitterSearchClient client = new TwitterSearchClient(_username, _password);
            SearchResults results = null;
            try
            {
                results = client.Search(keyword);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
            }
            return results;
        }

        #endregion

        #region Search Async

        //public void SearchAsync(string keyword)
        //{
        //    BackgroundWorker worker = new BackgroundWorker();
        //    worker.DoWork += new DoWorkEventHandler(Search_DoWork);
        //    worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Search_RunWorkerCompleted);
        //    worker.RunWorkerAsync(keyword);
        //}

        public void SearchAsync()
        {
            // make sure keyword is set
            if (string.IsNullOrEmpty(_keyword))
            {
                OnError("Invalid keyword", null);
                return;
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new DoWorkEventHandler(Search_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Search_RunWorkerCompleted);
            worker.RunWorkerAsync(Keyword);
        }

        void Search_DoWork(object sender, DoWorkEventArgs e)
        {
            // get search keyword
            string keyword = e.Argument as string;

            // twitter search
            TwitterSearchClient client = new TwitterSearchClient(_username, _password);
            try
            {
                e.Result = client.Search(keyword, _page);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
            }
        }

        void Search_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check for error
            if (null != e.Error)
            {
                OnError("Search failed. Please try again later", e.Error);
                return;
            }

            // get search results
            SearchResults results = e.Result as SearchResults;
            if (null == results)
            {
                OnError("Search failed. Please try again later", e.Error);
                return;
            }

            // by looking at the page number variable
            // we'll be able to distinguish betweet a new search and a get more results
            if (_page <= 1)
            {
                _searchResults.Clear();
            }
            _page++;

            // add search results to the local collection
            foreach (SearchResult result in results)
            {
                _searchResults.Add(result);
            }
            OnCompleted(_keyword, results);
        }

        #endregion

        #region Refresh Async

        public void RefreshAsync()
        {
            // make sure keyword is set
            if (string.IsNullOrEmpty(_keyword))
            {
                OnError("Invalid keyword", null);
                return;
            }

            // if the current search results list is empty, use the normal search instead
            if (_searchResults.Count == 0)
            {
                SearchAsync();
            }

            BackgroundWorker refreshWorker = new BackgroundWorker();
            refreshWorker.DoWork += new DoWorkEventHandler(Refresh_DoWork);
            refreshWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Refresh_RunWorkerCompleted);
            refreshWorker.RunWorkerAsync(Keyword);
        }

        void Refresh_DoWork(object sender, DoWorkEventArgs e)
        {
            // get search keyword
            string keyword = e.Argument as string;

            // twitter search
            TwitterSearchClient client = new TwitterSearchClient(_username, _password);
            try
            {
                string tweetId = GetTweetId(_searchResults[0].Id);
                e.Result = client.Search(keyword, tweetId);
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
            }
        }

        void Refresh_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // check for error
            if (null != e.Error)
            {
                OnError("Refresh failed. Please try again later", e.Error);
                return;
            }

            // get search results
            SearchResults results = e.Result as SearchResults;
            if (null == results)
            {
                OnError("Refresh failed. Please try again later", e.Error);
                return;
            }

            // add the results in reverse order
            for (int i = results.Count - 1; i >= 0; i--)
            {
                _searchResults.Insert(0, results[i]);
            }
            OnCompleted(_keyword, results);
        }

        #endregion

        #region Helpers

        private string GetTweetId(string searchResultId)
        {
            string[] ids = searchResultId.Split(new char[] { ':' });
            if (ids.Length == 3)
            {
                return ids[2];
            }
            return "0";
        }

        private void OnCompleted(string keyword, IList<SearchResult> results)
        {
            SearchManagerOperationCompletedEventArgs args = new SearchManagerOperationCompletedEventArgs
            {
                Keyword = keyword,
                SearchResults = results
            };
            if (null != SearchManagerOperationCompleted)
            {
                SearchManagerOperationCompleted(this, args);
            }
        }

        private void OnError(string message, Exception ex)
        {
            if (null != SearchManagerOperationError)
            {
                SearchManagerOperationErrorEventArgs args = new SearchManagerOperationErrorEventArgs
                {
                    Keyword = _keyword,
                    Message = message,
                    InnerException = ex
                };
                SearchManagerOperationError(this, args);
            }
        }

        #endregion

        #region Properties

        public ObservableCollection<SearchResult> All
        {
            get
            {
                return _searchResults;
            }
        }

        public string Keyword
        {
            get
            {
                return _keyword;
            }
            set
            {
                _keyword = value;
                _page = 1; // reset page whenever keyword is changed
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            // diposing logic?
            _username = null;
            _password = null;
            _searchResults = null;
            _keyword = null;
            _page = -1;
        }

        #endregion
    }
}
