using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.DataAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            // test thumbalizr
            //ThumbalizrClient thumb = new ThumbalizrClient();
            //try
            //{
            //    Stream asdf = thumb.GetPreview("dee16934b538faf7ad766faf7b695a8e", "http://www.live.com");
            //    thumb.Close();
            //}
            //catch (Exception ex)
            //{
            //    thumb.Abort();
            //}

            //// test twitter search
            //TwitterSearchClient searcher = new TwitterSearchClient("le_o", "sugarman");
            //try
            //{
            //    SearchResults result = searcher.Search("digitweet");
            //    searcher.Close();
            //}
            //catch (Exception ex)
            //{
            //    searcher.Abort();
            //}

            // test twitter client
            TwitterClient client = new TwitterClient("le_o", "sugarman");
            try
            {
                // test status methods
                //Statuses publicTimeline = client.PublicTimeline();
                //Statuses fdTimeline = client.FriendsTimeline();
                //Statuses userTimeline = client.UserTimeline();
                //Statuses userTimelineById = client.UserTimelineById("reeeg");
                //Status showStatus = client.ShowStatus(userTimelineById[0].Id);
                //Statuses replies = client.Replies();
                //Status update = client.Update("testing wcf service");
                //Status updateReply = client.Update(string.Format("@{0} testing wcf service reply", showStatus.User.ScreenName), showStatus.Id);
                //Status destroyUpdate = client.DestroyStatus(update.Id);
                //Status destroyUpdateReply = client.DestroyStatus(updateReply.Id);

                // test user methods
                //Users friends = client.Friends();
                //Users friendsById = client.Friends("reeeg");
                //Users followers = client.Followers();
                //Users followersById = client.Followers("reeeg");
                //ExtendedUser showUser = client.ShowUser("reeeg");

                // test direct message methods
                //DirectMessages directMessages = client.DirectMessages();
                //DirectMessages sent = client.Sent();
                //DirectMessage newMessage = client.New("reeeg", "hello world");
                //DirectMessage destroyMessage = client.DestroyDirectMessage(newMessage.Id);

                // test account methods
                //ExtendedUser logMeIn = client.VerifyCrednetials();

                // close connections
                client.Close();
            }
            catch (Exception ex)
            {
                client.Abort();
            }

            // test tiny url client
            //using (TinyUrlClient tinyClient = new TinyUrlClient())
            //{
            //    string tinyUrl = tinyClient.Create("http://www.theweathernetwork.com/weather/caon0409/");
            //}

            // test twittpic client
            //TwitPicClient twitPicClient = new TwitPicClient("le_o", "sugarman");
            //try
            //{
            //    string fileName = @"C:\Users\Leo\Pictures\BeefCutBottomSirloin.png";
            //    byte[] fileStream = File.ReadAllBytes(fileName);
            //    XDocument doc = twitPicClient.Upload(fileStream, fileName);
            //    string url = doc.Descendants("mediaurl").Single().Value;
            //}
            //catch (Exception ex)
            //{
            //    object a = null;
            //}
        }
    }
}
