using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet
{
    public class BaseCollectionManager<T> : BaseManager
        where T : Tweet
    {

        #region Instance Variables

        protected ITwitterClient _twitterApiClient = null;
        protected SortedTweetCollection<T> _tweets = null;

        #endregion

        #region Constructor

        public BaseCollectionManager(ITwitterClient twitterApiClient)
        {
            _twitterApiClient = twitterApiClient;
            _tweets = new SortedTweetCollection<T>();
        }

        protected BaseCollectionManager(string username, string password)
            : base(username, password)
        {
            _username = username;
            _password = password;
            _tweets = new SortedTweetCollection<T>();
        }

        #endregion
    }
}
