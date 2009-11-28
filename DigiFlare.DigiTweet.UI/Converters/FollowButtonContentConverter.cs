using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DigiFlare.DigiTweet.UI.Converters
{
    public class FollowButtonContentConverter :IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? isFollowing = value as bool?;
            if (isFollowing == true)
            {
                return "Remove";
            }
            return "Follow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
