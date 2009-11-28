using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DigiFlare.DigiTweet.UI
{
    public class MoreSearchResultsClickedEventArgs : EventArgs
    {
        public string Keyword { get; set; }
    }

    public class LoadWebPagePreviewCompletedEventArgs : EventArgs
    {
        public byte[] ImageStream { get; set; }
    }
}
