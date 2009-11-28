using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.UnitTest
{
    [TestClass]
    public class TweetsManagerTests
    {
        private MockRepository _mocks;
        private ITwitterClient _twitterApiClient;
        private TweetsManager _tweetsManager;

        [TestInitialize]
        public void Setup()
        {
            _mocks = new MockRepository();
            _twitterApiClient = _mocks.StrictMock<ITwitterClient>();
            _tweetsManager = new TweetsManager(_twitterApiClient);
        }

        [TestMethod]
        public void IsTweetsCollectionEmptyOnInit()
        {
            Assert.AreEqual(0, _tweetsManager.All.Count);
        }

        [TestMethod]
        public void CanRefreshAllReplaceAllTweetsWhenTweetsCollectionIsEmpty()
        {
            // setup twitter api mock return values
            Statuses statuses = new Statuses();
            statuses.Add(new Status { Id = "000", CreatedAt = DateTime.Today });
            statuses.Add(new Status { Id = "111", CreatedAt = DateTime.Today.AddDays(-2) });
            statuses.Add(new Status { Id = "222", CreatedAt = DateTime.Today.AddDays(-4) });

            // record
            Expect.Call(_twitterApiClient.FriendsTimeline()).Return(statuses);

            // playback
            _mocks.ReplayAll();
            _tweetsManager.RefreshAll();

            // assert
            Assert.AreEqual(3, _tweetsManager.All.Count);
            Assert.AreEqual("000", _tweetsManager.All[0].Id);
            Assert.AreEqual("111", _tweetsManager.All[1].Id);
            Assert.AreEqual("222", _tweetsManager.All[2].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanRefreshAllReplaceAllTweetsWhenTweetsCollectionIsNotEmpty()
        {
            // setup twitter api mock return values
            Statuses statuses = new Statuses();
            statuses.Add(new Status { Id = "000", CreatedAt = DateTime.Today });
            statuses.Add(new Status { Id = "111", CreatedAt = DateTime.Today.AddDays(-2) });
            statuses.Add(new Status { Id = "222", CreatedAt = DateTime.Today.AddDays(-4) });

            // add some tweets to tweets manager
            _tweetsManager.All.Add(new Status { Id = "999", CreatedAt = DateTime.Today.AddDays(6) });
            _tweetsManager.All.Add(new Status { Id = "888", CreatedAt = DateTime.Today.AddDays(4) });
            _tweetsManager.All.Add(new Status { Id = "777", CreatedAt = DateTime.Today.AddDays(2) });

            // record
            Expect.Call(_twitterApiClient.FriendsTimeline()).Return(statuses);

            // playback
            _mocks.ReplayAll();
            _tweetsManager.RefreshAll();

            // assert
            Assert.AreEqual(3, _tweetsManager.All.Count);
            Assert.AreEqual("000", _tweetsManager.All[0].Id);
            Assert.AreEqual("111", _tweetsManager.All[1].Id);
            Assert.AreEqual("222", _tweetsManager.All[2].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanRefreshReplaceAllTweetsWhenTweetsCollectionIsEmpty()
        {
            // setup twitter api mock return values
            Statuses statuses = new Statuses();
            statuses.Add(new Status { Id = "000", CreatedAt = DateTime.Today });
            statuses.Add(new Status { Id = "111", CreatedAt = DateTime.Today.AddDays(-2) });
            statuses.Add(new Status { Id = "222", CreatedAt = DateTime.Today.AddDays(-4) });

            // record
            Expect.Call(_twitterApiClient.FriendsTimeline()).Return(statuses);

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Refresh();

            // assert
            Assert.AreEqual(3, _tweetsManager.All.Count);
            Assert.AreEqual("000", _tweetsManager.All[0].Id);
            Assert.AreEqual("111", _tweetsManager.All[1].Id);
            Assert.AreEqual("222", _tweetsManager.All[2].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanRefreshAddToExistingTweetsCollection()
        {
            // setup twitter api mock return values
            Statuses statuses = new Statuses();
            statuses.Add(new Status { Id = "000", CreatedAt = DateTime.Today });
            statuses.Add(new Status { Id = "111", CreatedAt = DateTime.Today.AddDays(-2) });
            statuses.Add(new Status { Id = "222", CreatedAt = DateTime.Today.AddDays(-4) });

            // add some tweets to tweet manager
            _tweetsManager.All.Add(new Status { Id = "888", CreatedAt = DateTime.Today.AddDays(-10) });
            _tweetsManager.All.Add(new Status { Id = "999", CreatedAt = DateTime.Today.AddDays(-12) });

            // record
            Expect.Call(_twitterApiClient.FriendsTimelineSince("888")).Return(statuses);

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Refresh();

            // assert
            Assert.AreEqual(5, _tweetsManager.All.Count);
            Assert.AreEqual("000", _tweetsManager.All[0].Id);
            Assert.AreEqual("111", _tweetsManager.All[1].Id);
            Assert.AreEqual("222", _tweetsManager.All[2].Id);
            Assert.AreEqual("888", _tweetsManager.All[3].Id);
            Assert.AreEqual("999", _tweetsManager.All[4].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanUpdate()
        {
            // setup
            string tweet1 = "Hello world";
            string tweet2 = "World Hello";

            // record
            Expect.Call(_twitterApiClient.UpdateWithSource(tweet1, TweetsManager.APP_NAME)).Return(new Status { Text = tweet1 });
            Expect.Call(_twitterApiClient.UpdateWithSource(tweet2, TweetsManager.APP_NAME)).Return(new Status { Text = tweet2 });

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Update(tweet1);
            _tweetsManager.Update(tweet2);

            // assert
            Assert.AreEqual(2, _tweetsManager.All.Count);
            Assert.AreEqual(tweet2, _tweetsManager.All[0].Text);
            Assert.AreEqual(tweet1, _tweetsManager.All[1].Text);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanDelete()
        {
            // setup
            string tweet1 = "Tweet 1"; Status theTweet1 = new Status { Id = tweet1, CreatedAt = DateTime.Today };
            string tweet2 = "Tweet 2"; Status theTweet2 = new Status { Id = tweet2, CreatedAt = DateTime.Today.AddDays(-5) };
            string tweet3 = "Tweet 3"; Status theTweet3 = new Status { Id = tweet3, CreatedAt = DateTime.Today.AddDays(-10) };
            _tweetsManager.All.Add(theTweet1);
            _tweetsManager.All.Add(theTweet2);
            _tweetsManager.All.Add(theTweet3);

            // record
            Expect.Call(_twitterApiClient.DestroyStatus(tweet2)).Return(new Status { Id = tweet2 });

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Delete(theTweet2);

            // assert
            Assert.AreEqual(2, _tweetsManager.All.Count);
            Assert.AreEqual(tweet1, _tweetsManager.All[0].Id);
            Assert.AreEqual(tweet3, _tweetsManager.All[1].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanDeleteAbortOnTwitterApiError()
        {
            // setup
            string tweet1 = "Tweet 1"; Status theTweet1 = new Status { Id = tweet1, CreatedAt = DateTime.Today };
            string tweet2 = "Tweet 2"; Status theTweet2 = new Status { Id = tweet2, CreatedAt = DateTime.Today.AddDays(-5) };
            string tweet3 = "Tweet 3"; Status theTweet3 = new Status { Id = tweet3, CreatedAt = DateTime.Today.AddDays(-10) };
            _tweetsManager.All.Add(theTweet1);
            _tweetsManager.All.Add(theTweet2);
            _tweetsManager.All.Add(theTweet3);

            // record
            Expect.Call(_twitterApiClient.DestroyStatus(null)).IgnoreArguments().Throw(new Exception());

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Delete(theTweet2);

            // assert
            Assert.AreEqual(3, _tweetsManager.All.Count);
            Assert.AreEqual(tweet1, _tweetsManager.All[0].Id);
            Assert.AreEqual(tweet2, _tweetsManager.All[1].Id);
            Assert.AreEqual(tweet3, _tweetsManager.All[2].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanReply()
        {
            // setup
            string tweet1 = "Tweet 1"; Status theTweet1 = new Status { Id = tweet1, CreatedAt = DateTime.Today };
            string tweet2 = "Tweet 2"; Status theTweet2 = new Status { Id = tweet2, CreatedAt = DateTime.Today.AddDays(-5) };
            string tweet3 = "Tweet 3"; Status theTweet3 = new Status { Id = tweet3, CreatedAt = DateTime.Today.AddDays(-10) };
            _tweetsManager.All.Add(theTweet1);
            _tweetsManager.All.Add(theTweet2);
            _tweetsManager.All.Add(theTweet3);

            // record
            string reply = "Reply 1"; Status replyTweet = new Status { Id = reply, CreatedAt = DateTime.Today.AddDays(1) };
            Expect.Call(_twitterApiClient.UpdateWithSource(reply, theTweet3.Id, TweetsManager.APP_NAME)).Return(replyTweet);

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Reply(reply, theTweet3);

            // assert
            Assert.AreEqual(4, _tweetsManager.All.Count);
            Assert.AreEqual(reply, _tweetsManager.All[0].Id);
            Assert.AreEqual(tweet1, _tweetsManager.All[1].Id);
            Assert.AreEqual(tweet2, _tweetsManager.All[2].Id);
            Assert.AreEqual(tweet3, _tweetsManager.All[3].Id);
            _mocks.VerifyAll();
        }

        [TestMethod]
        public void CanReplyAbortOnTwitterApiError()
        {
            // setup
            string tweet1 = "Tweet 1"; Status theTweet1 = new Status { Id = tweet1, CreatedAt = DateTime.Today };
            string tweet2 = "Tweet 2"; Status theTweet2 = new Status { Id = tweet2, CreatedAt = DateTime.Today.AddDays(-5) };
            string tweet3 = "Tweet 3"; Status theTweet3 = new Status { Id = tweet3, CreatedAt = DateTime.Today.AddDays(-10) };
            _tweetsManager.All.Add(theTweet1);
            _tweetsManager.All.Add(theTweet2);
            _tweetsManager.All.Add(theTweet3);

            // record
            string reply = "Reply Me";
            Expect.Call(_twitterApiClient.UpdateWithSource(reply, theTweet1.Id, TweetsManager.APP_NAME)).Throw(new Exception());

            // playback
            _mocks.ReplayAll();
            _tweetsManager.Reply(reply, theTweet1);

            // assert
            Assert.AreEqual(3, _tweetsManager.All.Count);
            Assert.AreEqual(tweet1, _tweetsManager.All[0].Id);
            Assert.AreEqual(tweet2, _tweetsManager.All[1].Id);
            Assert.AreEqual(tweet3, _tweetsManager.All[2].Id);
            _mocks.VerifyAll();
        }
    }
}
