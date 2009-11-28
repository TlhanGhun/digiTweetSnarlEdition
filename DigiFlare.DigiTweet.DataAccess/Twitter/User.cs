using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [DataContract(Name = "user", Namespace = "")]
    public class User
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; }

        [DataMember(Name = "profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "protected")]
        public bool? Protected { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }
    }
}
