using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;

namespace DigiFlare.DigiTweet.UI.Configuration
{
    [Serializable]
    public class UserSetting
    {
        // default user settings
        public UserSetting()
        {
            Username = string.Empty;
            Password = string.Empty;
            WindowLocation = new Point(0, 0);
            WindowSize = new Size(1024, 768);
            Categories = new StringCollection();
            RefreshTimerInterval = 600;
            IsWebPagePreviewEnabled = false;
        }

        public string Username { get; set; }
        public string Password { get; set; }
        public Point WindowLocation { get; set; }
        public Size WindowSize { get; set; }
        public StringCollection Categories { get; set; }

        public int RefreshTimerInterval { get; set; }
        public bool IsWebPagePreviewEnabled { get; set; }
    }
}
