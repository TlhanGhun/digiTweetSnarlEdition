using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [DataContract(Name = "status", Namespace = "")]
    public class Status : Tweet
    {
        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "truncated")]
        public bool? Truncated { get; set; }

        [DataMember(Name = "in_reply_to_status_id")]
        public string InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_user_id")]
        public string InReplyToUserId { get; set; }

        [DataMember(Name = "favorited")]
        public bool? Favourited { get; set; }

        [DataMember(Name = "user")]
        public User User { get; set; }
    }
}
