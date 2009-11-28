using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DigiFlare.DigiTweet.DataAccess;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Reflection;
using DigiFlare.DigiTweet.UI.Configuration;

namespace DigiFlare.DigiTweet.UI
{
    public static class CategoriesManager
    {
        #region Constants

        public static string DEFAULT_TWEET_COLOR = "WHITE";
        private const string CATEGORY_NAME_DELIMITER = ":";
        private const string CATEGORY_USER_DELIMITER = ",";

        #endregion

        #region Instance Variable

        private static string _username;
        private static ObservableCollection<Status> _tweets;
        private static IDictionary<string, IList<string>> _config;
        private static IDictionary<string, ListCollectionView> _categories;
        private static IDictionary<string, string> _colors;

        #endregion

        #region Constructor

        static CategoriesManager()
        {
            _username = null;
            _config = new Dictionary<string, IList<string>>();
            _categories = new Dictionary<string, ListCollectionView>();
            _colors = new Dictionary<string, string>();
        }

        #endregion

        #region Public Methods

        public static void Initialize(string username, ObservableCollection<Status> tweets)
        {
            _username = username;
            _tweets = tweets;
            LoadConfiguration();
        }

        public static void SaveCategory(string categoryName, IList<string> userNames, string color)
        {
            // init collection view for category
            ListCollectionView view = new ListCollectionView(_tweets);
            view.Filter = delegate(object item)
            {
                Status status = item as Status;
                if (null != status && IsUserInCategory(status.User.ScreenName, categoryName))
                {
                    return true;
                }
                return false;
            };

            // save categories
            if (_config.ContainsKey(categoryName))
            {
                _config[categoryName] = userNames;
                _categories[categoryName] = view;
                _colors[categoryName] = color;
            }
            else
            {
                _config.Add(categoryName, userNames);
                _categories.Add(categoryName, view);
                _colors.Add(categoryName, color);
            }
        }

        public static void RemoveCategory(string categoryName)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                // some clean up for the category's collection view
                _categories[categoryName].Filter = null;
                DetachCollectionAndView(_categories[categoryName]);

                _config.Remove(categoryName);
                _categories.Remove(categoryName);
                _colors.Remove(categoryName);
            }
        }

        public static ICollectionView GetCategoryView(string categoryName)
        {
            if (_categories.ContainsKey(categoryName))
            {
                return _categories[categoryName];
            }
            return null;
        }

        public static IList<string> GetCategoryUserNames(string categoryName)
        {
            if (_config.ContainsKey(categoryName))
            {
                return _config[categoryName];
            }
            return null;
        }

        public static string GetCategoryColor(string categoryName)
        {
            if (_colors.ContainsKey(categoryName))
            {
                return _colors[categoryName];
            }
            return null;
        }

        public static ICollection<string> GetCategoryNames()
        {
            return _config.Keys;
        }

        public static string GetTweetColor(Status tweet)
        {
            return GetUserColor(tweet.User);
        }

        public static string GetUserColor(User user)
        {
            // retrieve category name of the user associated with the given tweet
            var config = _config.Where(keyValue =>
                keyValue.Value.Where(userName =>
                    userName.Equals(user.ScreenName, StringComparison.InvariantCultureIgnoreCase)
                ).Count() > 0
            ).FirstOrDefault();

            if (null != config.Key && null != config.Value)
            {
                return _colors[config.Key];
            }

            // can't find user, return default color
            return DEFAULT_TWEET_COLOR;
        }

        #endregion

        #region Helper methods

        public static void LoadConfiguration()
        {
            // load the settings
            UserSetting userSetting = ConfigurationManager.Default.GetUserSetting(_username);

            // "deserialize" categories settings
            StringCollection categorySettings = userSetting.Categories;
            string[] serializedCategory, userNames;
            string[] categoryNameDelimiter = new string[] { CATEGORY_NAME_DELIMITER };
            string[] userNameDelimiter = new string[] { CATEGORY_USER_DELIMITER };
            if (null != categorySettings && categorySettings.Count > 0)
            {
                foreach (string category in categorySettings)
                {
                    serializedCategory = category.Split(categoryNameDelimiter, StringSplitOptions.RemoveEmptyEntries);

                    // make sure the category serialization is valid
                    if (serializedCategory.Length == 3)
                    {
                        string categoryName = serializedCategory[0];
                        string color = serializedCategory[1];
                        userNames = serializedCategory[2].Split(userNameDelimiter, StringSplitOptions.RemoveEmptyEntries);

                        // make sure there are users in the category
                        if (userNames.Length > 0)
                        {
                            SaveCategory(categoryName, new List<string>(userNames), color);
                        }
                    }
                }
            }
        }

        public static void SaveConfiguration()
        {
            StringCollection categoriesSetting = new StringCollection();
            foreach (string key in _config.Keys)
            {
                // ignore empty categories (categories with no users)
                if (_config[key].Count > 0)
                {
                    // build the string in the following format:
                    // [category name]:[color]:[user1],[user2],[user3] etc
                    StringBuilder categoryStringBuilder = new StringBuilder();

                    // append [category name]:
                    categoryStringBuilder.Append(key);
                    categoryStringBuilder.Append(CATEGORY_NAME_DELIMITER);

                    // append [color]:
                    categoryStringBuilder.Append(_colors[key]);
                    categoryStringBuilder.Append(CATEGORY_NAME_DELIMITER);

                    foreach (string userName in _config[key])
                    {
                        categoryStringBuilder.Append(userName);
                        categoryStringBuilder.Append(CATEGORY_USER_DELIMITER);
                    }
                    categoryStringBuilder.Remove(categoryStringBuilder.Length - 1, 1);
                    categoriesSetting.Add(categoryStringBuilder.ToString());
                }
            }
            
            // save categories
            UserSetting userSetting = ConfigurationManager.Default.GetUserSetting(_username);
            if (userSetting != null)
            {
                userSetting.Categories = categoriesSetting;
            }
            ConfigurationManager.Default.Save();
        }

        private static bool IsUserInCategory(string userName, string categoryName)
        {
            if (_categories.ContainsKey(categoryName))
            {
                foreach (string item in _config[categoryName])
                {
                    if (item.Equals(userName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool DetachCollectionAndView(CollectionView view)
        {
            // make sure view implements INotifyCollectionChanged
            INotifyCollectionChanged iCollectionChanged = view.SourceCollection as INotifyCollectionChanged;
            if (iCollectionChanged == null)
            {
                return false;
            }

            // Get the method that subscribes to OnCollectionChanged
            MethodInfo onCollectionChanged = view.GetType().GetMethod("OnCollectionChanged",
               BindingFlags.NonPublic | BindingFlags.Instance, null,
               new Type[] { typeof(object), typeof(NotifyCollectionChangedEventArgs) },
               null
            );
            NotifyCollectionChangedEventHandler handler = Delegate.CreateDelegate(typeof(NotifyCollectionChangedEventHandler), 
                view,
                onCollectionChanged
            ) as NotifyCollectionChangedEventHandler;

            // remove collection changed handler
            _tweets.CollectionChanged -= handler;

            return true;
        }

        #endregion

    }
}
