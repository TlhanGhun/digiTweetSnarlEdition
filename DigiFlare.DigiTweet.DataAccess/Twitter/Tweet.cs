using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [DataContract]
    public class Tweet
    {
        #region Instance Variables
        private string _createdAtString = null;
        #endregion

        #region Properties
        public DateTime CreatedAt { get; set; }
        #endregion

        #region WCF DataMembers

        [DataMember(Name = "created_at")]
        public string CreatedAtString
        {
            get
            {
                return _createdAtString;
            }
            set
            {
                _createdAtString = value;
                CreatedAt = DateTime.ParseExact(value, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
            }
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        #endregion
    }
}
