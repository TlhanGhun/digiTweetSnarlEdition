using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace DigiFlare.DigiTweet
{
    public class SavedSearchesManager
    {

        #region Instance Variables

        string _username = null;
        string _password = null;
        private Dictionary<string, SearchManager> _searchManagers = new Dictionary<string, SearchManager>();

        #endregion

        #region Constructor

        public SavedSearchesManager(string username, string password)
        {
            _searchManagers = new Dictionary<string, SearchManager>();
            _username = username;
            _password = password;
        }

        #endregion

        #region Public Methods

        public void Initialize(
            StringCollection searches,
            SearchManager.SearchManagerOperationCompletedHandler completedHandler, 
            SearchManager.SearchManagerOperationErrorHandler errorHandler)
        {
            SearchManager manager;
            foreach (string search in searches)
            {
                manager = CreateSearch(search, completedHandler, errorHandler);
                manager.SearchAsync();
            }
        }

        public StringCollection GetKeywords()
        {
            StringCollection result = new StringCollection();
            foreach (string keywords in _searchManagers.Keys)
            {
                result.Add(keywords);
            }
            return result;
        }

        public SearchManager CreateSearch(
            string keyword, 
            SearchManager.SearchManagerOperationCompletedHandler completedHandler, 
            SearchManager.SearchManagerOperationErrorHandler errorHandler)
        {
            // create an instance of search manager for this keyword
            SearchManager search = new SearchManager(_username, _password);
            search.Keyword = keyword;
            
            // attach event handlers
            if (null != completedHandler)
            {
                search.SearchManagerOperationCompleted += completedHandler;
            }
            if (null != errorHandler)
            {
                search.SearchManagerOperationError += errorHandler;
            }

            // keep a list of all keyword searches
            _searchManagers.Add(keyword, search);
            return search;
        }

        public void RemoveSearch(string keyword)
        {
            if (_searchManagers.ContainsKey(keyword))
            {
                _searchManagers[keyword].Dispose();
                _searchManagers.Remove(keyword);
            }
        }

        public SearchManager GetSearchManager(string keyword)
        {
            if (_searchManagers.ContainsKey(keyword))
            {
                return _searchManagers[keyword];
            }
            return null;
        }

        #endregion

        #region Refresh Async

        public void RefreshAllAsync()
        {
            RefreshAsync();
        }

        public void RefreshAsync()
        {
            foreach (SearchManager manager in _searchManagers.Values)
            {
                manager.RefreshAsync();
            }
        }

        #endregion
    }
}
