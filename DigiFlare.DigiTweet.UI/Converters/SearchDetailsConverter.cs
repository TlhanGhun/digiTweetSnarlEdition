using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using DigiFlare.DigiTweet.DataAccess;
using System.Text.RegularExpressions;

namespace DigiFlare.DigiTweet.UI.Converters
{
    public class SearchDetailsConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // verify datacontext
            SearchResult result = value as SearchResult;
            if (null == result || null == result.AuthorName)
            {
                return null;
            }

            // construct tweet's detail
            string detailsFormat = "{0}, {1} via {2}";
            string source = Regex.Replace(result.Source, "<.*?>", string.Empty);
            return string.Format(detailsFormat, result.AuthorName, result.Published.ToString(), source);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
