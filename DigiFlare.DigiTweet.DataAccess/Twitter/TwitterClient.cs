using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace DigiFlare.DigiTweet.DataAccess
{
    public partial class TwitterClient : ClientBase<ITwitterClient>, ITwitterClient
    {
        #region Constructor

        public TwitterClient(string username, string password)
        {
            base.ClientCredentials.UserName.UserName = username;
            base.ClientCredentials.UserName.Password = password;
        }

        #endregion

        #region Status Methods

        public Statuses PublicTimeline()
        {
            return base.Channel.PublicTimeline();
        }

        public Statuses FriendsTimeline()
        {
            return base.Channel.FriendsTimeline();
        }

        public Statuses FriendsTimelineSince(string sinceId)
        {
            return base.Channel.FriendsTimelineSince(sinceId);
        }

        public Statuses FriendsTimelineUntil(string maxId)
        {
                return base.Channel.FriendsTimelineUntil(maxId);
        }

        public Statuses UserTimeline()
        {
            return base.Channel.UserTimeline();
        }

        public Statuses UserTimelineById(string id)
        {
            return base.Channel.UserTimelineById(id);
        }

        public Status ShowStatus(string id)
        {
            return base.Channel.ShowStatus(id);
        }

        public Status Update(string status)
        {
            return base.Channel.Update(status);
        }

        public Status UpdateWithSource(string status, string source)
        {
            return base.Channel.UpdateWithSource(status, source);
        }

        public Status Update(string status, string id)
        {
            return base.Channel.Update(status, id);
        }

        public Status UpdateWithSource(string status, string id, string source)
        {
            return base.Channel.UpdateWithSource(status, id, source);
        }

        public Statuses Replies()
        {
            return base.Channel.Replies();
        }

        public Statuses Replies(string sinceId)
        {
            return base.Channel.Replies(sinceId);
        }

        public Status DestroyStatus(string id)
        {
            return base.Channel.DestroyStatus(id);
        }

        #endregion

        #region User Methods

        public Users Friends()
        {
            return base.Channel.Friends();
        }

        public Users Friends(string id)
        {
            return base.Channel.Friends(id);
        }

        public Users Followers()
        {
            return base.Channel.Followers();
        }

        public Users Followers(string id)
        {
            return base.Channel.Followers(id);
        }

        public ExtendedUser ShowUser(string id)
        {
            return base.Channel.ShowUser(id);
        }

        #endregion

        #region Direct Message Methods

        public DirectMessages DirectMessages()
        {
            return base.Channel.DirectMessages();
        }

        public DirectMessages DirectMessages(string sinceId)
        {
            return base.Channel.DirectMessages(sinceId);
        }

        public DirectMessages Sent()
        {
            return base.Channel.Sent();
        }

        public DirectMessage New(string user, string text)
        {
            return base.Channel.New(user, text);
        }

        public DirectMessage DestroyDirectMessage(string id)
        {
            return base.Channel.DestroyDirectMessage(id);
        }

        #endregion

        #region Friendship Methods

        public ExtendedUser FriendshipsCreate(string id)
        {
            return base.Channel.FriendshipsCreate(id);
        }

        public ExtendedUser FriendshipsDestroy(string id)
        {
            return base.Channel.FriendshipsDestroy(id);
        }

        #endregion

        #region Account Methods

        public ExtendedUser VerifyCrednetials()
        {
            return base.Channel.VerifyCrednetials();
        }

        #endregion

        #region Favourite Methods

        public Status Favourite(string id)
        {
            return base.Channel.Favourite(id);
        }

        public Status Unfavourite(string id)
        {
            return base.Channel.Unfavourite(id);
        }

        public Statuses Favourites()
        {
            return base.Channel.Favourites();
        }

        #endregion
    }
}