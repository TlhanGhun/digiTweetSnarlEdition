using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DigiFlare.DigiTweet.DataAccess;
using System.Windows;

namespace DigiFlare.DigiTweet.UI.Converters
{
    class DeleteTweetConverter: IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //verify binding values
            User user = values[1] as User;
            if (null == user)
            {
                return false;
            }

            if (values.Length == 2 &&
                values[0] != null &&
                values[1] != null &&
                string.Equals(values[0].ToString(), user.Id))
            {
                return true;
            }
            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
