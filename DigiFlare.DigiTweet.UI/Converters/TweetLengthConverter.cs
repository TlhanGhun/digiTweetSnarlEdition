using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DigiFlare.DigiTweet.UI.Converters
{
    class TweetLengthConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(value as string))
            {
                return Constants.TWEET_MAX_LENGTH;
            }
            return Constants.TWEET_MAX_LENGTH - text.Length;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
