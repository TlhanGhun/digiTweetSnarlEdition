using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DigiFlare.DigiTweet.DataAccess
{
    public class TwitterSearchClient : ClientBase<ITwitterSearchClient>, ITwitterSearchClient
    {
        static object lockMe;

        #region Constructor

        public TwitterSearchClient(string username, string password)
        {
            lockMe = new object();
            base.ClientCredentials.UserName.UserName = username;
            base.ClientCredentials.UserName.Password = password;
        }

        #endregion

        public SearchResults Search(string keyword)
        {
            lock (lockMe)
            {
                return base.Channel.Search(keyword);
            }
        }

        public SearchResults Search(string keyword, int page)
        {
            lock (lockMe)
            {
                return base.Channel.Search(keyword, page);
            }
        }

        public SearchResults Search(string keyword, string sinceId)
        {
            lock (lockMe)
            {
                return base.Channel.Search(keyword, sinceId);
            }
        }
    }
}
