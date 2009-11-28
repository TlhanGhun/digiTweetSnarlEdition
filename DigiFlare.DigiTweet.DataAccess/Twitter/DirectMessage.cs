using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [DataContract(Name = "direct_message", Namespace = "")]
    public class DirectMessage : Tweet
    {

        [DataMember(Name = "sender_id")]
        public string SenderId { get; set; }

        [DataMember(Name = "recipient_id")]
        public string RecipientId { get; set; }

        [DataMember(Name = "sender_screen_name")]
        public string SenderScreenName { get; set; }

        [DataMember(Name = "recipient_screen_name")]
        public string RecipientScreenName { get; set; }

        [DataMember(Name = "sender")]
        public User Sender { get; set; }

        [DataMember(Name = "recipient")]
        public User Recipient { get; set; }
    }
}
