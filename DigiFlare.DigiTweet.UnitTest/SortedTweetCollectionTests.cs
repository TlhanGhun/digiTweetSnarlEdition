using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.UnitTest
{
    /// <summary>
    /// Summary description for SortedTweetCollection
    /// </summary>
    [TestClass]
    public class SortedTweetCollectionTests
    {
        private SortedTweetCollection<Status> _tweets = null;

        [TestInitialize]
        public void Setup()
        {
            _tweets = new SortedTweetCollection<Status>();
        }

        [TestMethod]
        public void CanAddToEmptyCollection()
        {
            _tweets.Add(new Status { Id = "499" });

            Assert.AreEqual(1, _tweets.Count);
            Assert.AreEqual("499", _tweets[0].Id);
        }

        [TestMethod]
        public void CanAddToEndOfCollection()
        {
            Status tweet0 = new Status { Id = "0", CreatedAt = DateTime.Today };
            Status tweet1 = new Status { Id = "1", CreatedAt = DateTime.Today.AddDays(-1) };
            Status tweet2 = new Status { Id = "2", CreatedAt = DateTime.Today.AddDays(-2) };
            _tweets.Add(tweet0);
            _tweets.Add(tweet1);
            _tweets.Add(tweet2);

            Status addMe = new Status { Id = "999", CreatedAt = DateTime.Today.AddDays(-5) };
            _tweets.Add(addMe);

            Assert.AreEqual(4, _tweets.Count);
            Assert.AreEqual(tweet0, _tweets[0]);
            Assert.AreEqual(tweet1, _tweets[1]);
            Assert.AreEqual(tweet2, _tweets[2]);
            Assert.AreEqual(addMe, _tweets[3]);
        }

        [TestMethod]
        public void CanAddToStartOfCollection()
        {
            Status tweet0 = new Status { Id = "0", CreatedAt = DateTime.Today };
            Status tweet1 = new Status { Id = "1", CreatedAt = DateTime.Today.AddDays(-1) };
            Status tweet2 = new Status { Id = "2", CreatedAt = DateTime.Today.AddDays(-2) };
            _tweets.Add(tweet0);
            _tweets.Add(tweet1);
            _tweets.Add(tweet2);

            Status addMe = new Status { Id = "999", CreatedAt = DateTime.Today.AddDays(1) };
            _tweets.Add(addMe);

            Assert.AreEqual(4, _tweets.Count);
            Assert.AreEqual(addMe, _tweets[0]);
            Assert.AreEqual(tweet0, _tweets[1]);
            Assert.AreEqual(tweet1, _tweets[2]);
            Assert.AreEqual(tweet2, _tweets[3]);
        }

        [TestMethod]
        public void CanAddToMiddleOfCollection()
        {
            Status tweet0 = new Status { Id = "0", CreatedAt = DateTime.Today };
            Status tweet1 = new Status { Id = "1", CreatedAt = DateTime.Today.AddDays(-2) };
            Status tweet2 = new Status { Id = "2", CreatedAt = DateTime.Today.AddDays(-4) };
            _tweets.Add(tweet0);
            _tweets.Add(tweet1);
            _tweets.Add(tweet2);

            Status addMe0 = new Status { Id = "888", CreatedAt = DateTime.Today.AddDays(-1) };
            Status addMe1 = new Status { Id = "999", CreatedAt = DateTime.Today.AddDays(-3) };
            _tweets.Add(addMe0);
            _tweets.Add(addMe1);

            Assert.AreEqual(5, _tweets.Count);
            Assert.AreEqual(tweet0, _tweets[0]);
            Assert.AreEqual(addMe0, _tweets[1]);
            Assert.AreEqual(tweet1, _tweets[2]);
            Assert.AreEqual(addMe1, _tweets[3]);
            Assert.AreEqual(tweet2, _tweets[4]);
        }
    }
}
