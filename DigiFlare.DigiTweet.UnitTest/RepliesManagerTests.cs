using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DigiFlare.DigiTweet.DataAccess;

namespace DigiFlare.DigiTweet.UnitTest
{
    /// <summary>
    /// Summary description for RepliesManager
    /// </summary>
    [TestClass]
    public class RepliesManagerTests
    {
        private RepliesManager _repliesManager;

        [TestInitialize]
        public void Setup()
        {
            _repliesManager = new RepliesManager(null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void CanThrowExceptionOnUpdate()
        {
            _repliesManager.Update("hello world");
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void CanThrowExceptionOnDelete()
        {
            Status deleteMe = new Status { Id = "111" };
            _repliesManager.Delete(deleteMe);
        }
    }
}
