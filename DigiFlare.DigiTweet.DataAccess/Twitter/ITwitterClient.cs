using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DigiFlare.DigiTweet.DataAccess
{
    [ServiceContract]
    public interface ITwitterClient
    {
        #region Status Methods

        /// <summary>
        /// Returns the 20 most recent statuses from non-protected users who have set a custom user icon.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statuses/public_timeline.json",
            BodyStyle = WebMessageBodyStyle.Bare, 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json)]
        Statuses PublicTimeline();

        /// <summary>
        /// Returns the 20 most recent statuses posted by the authenticating user and that user's friends. 
        /// This is the equivalent of /home on the Web
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name="FriendsTimeline")]
        [WebGet(UriTemplate = "/statuses/friends_timeline.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses FriendsTimeline();

        /// <summary>
        /// Returns the 20 most recent statuses starting but not including the status id specified
        /// </summary>
        /// <param name="sinceId"></param>
        /// <returns></returns>
        [OperationContract(Name="FriendsTimelineSince")]
        [WebGet(UriTemplate = "/statuses/friends_timeline.json?since_id={sinceId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses FriendsTimelineSince(string sinceId);

        /// <summary>
        /// Returns the 20 most recent statuses older than the specified id
        /// </summary>
        /// <param name="maxId"></param>
        /// <returns></returns>
        [OperationContract(Name = "FriendsTimelineUntil")]
        [WebGet(UriTemplate = "/statuses/friends_timeline.json?max_id={maxId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses FriendsTimelineUntil(string maxId);

        /// <summary>
        /// Returns the 20 most recent statuses posted from the authenticating user.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statuses/user_timeline.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses UserTimeline();

        /// <summary>
        /// Returns the 20 most recent statuses posted from the specified user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statuses/user_timeline.json?id={id}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses UserTimelineById(string id);

        /// <summary>
        /// Returns a single status specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/statuses/show/{id}.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status ShowStatus(string id);

        /// <summary>
        /// Updates the authenticating user's status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [OperationContract(Name = "Update")]
        [WebInvoke(UriTemplate = "/statuses/update.json?status={status}",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status Update(string status);

        /// <summary>
        /// Updates the authenticating user's status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [OperationContract(Name = "UpdateWithSource")]
        [WebInvoke(UriTemplate = "/statuses/update.json?status={status}&source={source}",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status UpdateWithSource(string status, string source);

        /// <summary>
        /// Updates posted in reply to the specified status id
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract(Name="ReplyUpdate")]
        [WebInvoke(UriTemplate = "/statuses/update.json?status={status}&in_reply_to_status_id={id}",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status Update(string status, string id);

        /// <summary>
        /// Updates posted in reply to the specific status id with source
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [OperationContract(Name = "ReplyUpdateWithSource")]
        [WebInvoke(UriTemplate = "/statuses/update.json?status={status}&in_reply_to_status_id={id}&source={source}",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status UpdateWithSource(string status, string id, string source);

        /// <summary>
        /// Returns the 20 most recent @replies (status updates prefixed with @username) for the authenticating user.
        /// </summary>
        /// <returns></returns>
        [OperationContract(Name="Replies")]
        [WebGet(UriTemplate = "/statuses/replies.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses Replies();

        /// <summary>
        /// Returns the 20 most recent @replies (status updates prefixed with @username) for the authenticating user.
        /// </summary>
        /// <returns></returns>
        [OperationContract (Name="RepliesSince")]
        [WebGet(UriTemplate = "/statuses/replies.json?since_id={sinceId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses Replies(string sinceId);


        /// <summary>
        /// Destroys the status specified by the required ID parameter.  The authenticating user must be the author of the specified status.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/statuses/destroy/{id}.json",
            Method = "DELETE",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status DestroyStatus(string id);

        #endregion

        #region User Methods

        [OperationContract(Name="Friends")]
        [WebGet(UriTemplate = "/statuses/friends.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Users Friends();

        [OperationContract(Name="FriendsById")]
        [WebGet(UriTemplate = "/statuses/friends/{id}.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Users Friends(string id);

        [OperationContract(Name = "Followers")]
        [WebGet(UriTemplate = "/statuses/followers.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Users Followers();

        [OperationContract(Name = "FollowersById")]
        [WebGet(UriTemplate = "/statuses/followers/{id}.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Users Followers(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/users/show/{id}.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ExtendedUser ShowUser(string id);

        #endregion

        #region Direct Message Methods

        [OperationContract(Name="DirectMessages")]
        [WebGet(UriTemplate = "/direct_messages.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DirectMessages DirectMessages();

        [OperationContract(Name="DirectMessagesSince)")]
        [WebGet(UriTemplate = "/direct_messages.json?since_id={sinceId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DirectMessages DirectMessages(string sinceId);

        [OperationContract]
        [WebGet(UriTemplate = "/direct_messages/sent.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DirectMessages Sent();

        [OperationContract]
        [WebInvoke(UriTemplate = "/direct_messages/new.json?user={user}&text={text}",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DirectMessage New(string user, string text);

        [OperationContract]
        [WebInvoke(UriTemplate = "/direct_messages/destroy/{id}.json",
            Method = "DELETE",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DirectMessage DestroyDirectMessage(string id);
        #endregion

        #region List methods

        /// <summary>
        /// Get list of all lists of the user
        /// </summary>        
        /// <returns></returns>
        [OperationContract(Name = "UpdateWithSource")]
        [WebInvoke(UriTemplate = "/user/lists.json",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
         Lists AllLists();

        #endregion

        #region Friendship Methods

        [OperationContract]
        [WebInvoke(UriTemplate = "/friendships/create/{id}.json",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ExtendedUser FriendshipsCreate(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/friendships/destroy/{id}.json",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ExtendedUser FriendshipsDestroy(string id);

        #endregion

        #region Account Methods

        [OperationContract]
        [WebGet(UriTemplate = "/account/verify_credentials.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        ExtendedUser VerifyCrednetials();

        #endregion

        #region Favourite Methods

        [OperationContract]
        [WebInvoke(UriTemplate = "/favorites/create/{id}.json",
            Method="POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status Favourite(string id);

        [OperationContract]
        [WebInvoke(UriTemplate = "/favorites/destroy/{id}.json",
            Method = "POST",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Status Unfavourite(string id);

        [OperationContract]
        [WebGet(UriTemplate = "/favorites.json",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Statuses Favourites();

        #endregion
    }
}
