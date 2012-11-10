using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [DataContract(Name = "user", Namespace = "")]
    public class ExtendedUser : User
    {
        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor { get; set; }

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor { get; set; }

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor { get; set; }

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor { get; set; }

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor { get; set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; set; }

        [DataMember(Name = "utc_offset")]
        public string UtcOffset { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; }

        [DataMember(Name = "profile_background_image_url")]
        public string ProfileBackgroundImageUrl { get; set; }

        [DataMember(Name = "profile_background_tile")]
        public bool? ProfileBackgroundTile { get; set; }

        [DataMember(Name = "following")]
        public string Following { get; set; }

        [DataMember(Name = "notifications")]
        public string Notifications { get; set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }
    }
}
