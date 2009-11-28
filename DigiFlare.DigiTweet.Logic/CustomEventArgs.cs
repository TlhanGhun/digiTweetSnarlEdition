using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet
{

    public class OperationErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Exception InnerException { get; set; }
    }

    public class OperationCompletedEventArgs : EventArgs
    {
        public object Info { get; set; }
    }

    public class TinyUrlCompletedEventArgs : OperationCompletedEventArgs
    {
        public string Url { get; set; }
    }

    public class TwitPicCompletedEventArgs : OperationCompletedEventArgs
    {
        public string Url { get; set; }
    }

    public class UserProfileOperationCompletedEventArgs : OperationCompletedEventArgs
    {
        public ExtendedUser User { get; set; }
    }

    public class TweetsManagerOperationCompletedEventArgs : OperationCompletedEventArgs
    {
        public IList<Status> NewTweets { get; set; }
    }

    public class DirectMessageManagerOperationCompletedEventArgs : OperationCompletedEventArgs
    {
        public IList<Status> NewDirectMessages { get; set; }
    }

    public class SearchManagerOperationCompletedEventArgs : OperationCompletedEventArgs
    {
        public string Keyword { get; set; }
        public IList<SearchResult> SearchResults { get; set; }
    }

    public class SearchManagerOperationErrorEventArgs : OperationErrorEventArgs
    {
        public string Keyword { get; set; }
    }
}
