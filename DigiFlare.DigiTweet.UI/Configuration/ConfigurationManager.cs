using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Collections.Specialized;

namespace DigiFlare.DigiTweet.UI.Configuration
{
    public class ConfigurationManager : ApplicationSettingsBase
    {
        private static ConfigurationManager defaultInstance = ((ConfigurationManager)(ApplicationSettingsBase.Synchronized(new ConfigurationManager())));

        public static ConfigurationManager Default
        {
            get
            {
                return defaultInstance;
            }
        }

        //[UserScopedSettingAttribute()]
        //[DebuggerNonUserCodeAttribute()]
        //public List<string> Usernames
        //{
        //    get
        //    {
        //        if (null == this["Usernames"])
        //        {
        //            this["Usernames"] = new List<string>();
        //        }
        //        return (List<string>)this["Usernames"];
        //    }
        //}

        [UserScopedSettingAttribute()]
        [DebuggerNonUserCodeAttribute()]
        public UserSettings UserSettings
        {
            get
            {
                if (null == this["UserSettings"])
                {
                    this["UserSettings"] = new UserSettings();
                }
                return ((UserSettings)(this["UserSettings"]));
            }
        }

        [UserScopedSettingAttribute()]
        [DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public string LastUser
        {
            get
            {
                return (string)this["LastUser"];
            }
            set
            {
                this["LastUser"] = value;
            }
        }

        [UserScopedSettingAttribute()]
        [DebuggerNonUserCodeAttribute()]
        [DefaultSettingValueAttribute("")]
        public StringCollection SavedSearches
        {
            get
            {
                return (StringCollection)this["SavedSearches"];
            }
            set
            {
                this["SavedSearches"] = value;
            }
        }

        public UserSetting GetPreviousSessionUserSetting()
        {
            return GetUserSetting(LastUser);
        }

        public UserSetting GetUserSetting(string username)
        {
            // check input argument
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // iterate over user settings to find the setting with the give username
            foreach (UserSetting setting in UserSettings)
            {
                if (username.Equals(setting.Username, StringComparison.InvariantCultureIgnoreCase))
                {
                    return setting;
                }
            }
            return null;
        }

        public UserSetting SetUserSetting(string username, string password)
        {
            // input argument check
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // find the user setting
            UserSetting setting = GetUserSetting(username);

            // if setting not found, create new
            // otherwise, modify existing setting
            if (null == setting)
            {
                setting = new UserSetting
                {
                    Username = username,
                    Password = password
                };
                UserSettings.Add(setting);
            }
            else
            {
                setting.Username = username;
                setting.Password = password;
            }
            LastUser = username;
            return setting;
        }

        public UserSetting RemoveUserSetting(string username)
        {
            // input argument check
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            // remove setting from collection
            UserSetting setting = GetUserSetting(username);
            if (null != setting)
            {
                UserSettings.Remove(setting);
                LastUser = null;
            }
            return setting;
        }

    }
}
