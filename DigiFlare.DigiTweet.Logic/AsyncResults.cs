using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet
{

    #region TweetsResult for TweetsManager

    public class TweetsResult
    {
        public string Id { get; set; }
        public Statuses Tweets { get; set; }
    }

    #endregion

    #region DirectMessagesResult for DirectMessageManager

    public class DirectMessagesResult
    {
        public string Id { get; set; }
        public DirectMessages DirectMessages { get; set; }
    }

    #endregion

}
