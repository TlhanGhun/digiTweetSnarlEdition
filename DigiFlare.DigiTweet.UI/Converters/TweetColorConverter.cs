using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Drawing;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.UI.Converters
{
    class TweetColorConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Status)
            {
                Status tweet = value as Status;
                return CategoriesManager.GetUserColor(tweet.User);
            }
            else if(value is DirectMessage)
            {
                DirectMessage dm = value as DirectMessage;
                return CategoriesManager.GetUserColor(dm.Sender);
            }
            return CategoriesManager.DEFAULT_TWEET_COLOR;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
