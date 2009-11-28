using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DigiFlare.DigiTweet.DataAccess
{
    public class SearchResult : IXmlSerializable
    {
        public const string TWITTER_NAMESPACE = @"http://api.twitter.com/";
        public const string TEXT_CONTENT_TYPE = @"text/";
        public const string IMAGE_CONTENT_TYPE = @"image/";

        #region Properties

        // tweet fields
        public string Id { get; set; }
        public DateTime Published { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Updated { get; set; }
        public string Source { get; set; }
        public string TweetUrl { get; set; }

        // author fields
        public string AuthorName { get; set; }
        public string AuthorUrl { get; set; }
        public string AuthorAvatarUrl { get; set; }

        // unused fields
        public string PublishedString { get; set; }
        public string UpdatedString { get; set; }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // read search result into XElement
            XElement element = XElement.ReadFrom(reader) as XElement;
            XNamespace ns = element.Name.Namespace;
            XNamespace twitterNS = TWITTER_NAMESPACE;
            string dateTimeFormat = "yyyy-MM-ddTHH:mm:ssZ";

            // retrieve tweet fields
            Id = element.Descendants(ns + "id").SingleOrDefault().Value;
            PublishedString = element.Descendants(ns + "published").SingleOrDefault().Value;
            Published = DateTime.ParseExact(PublishedString, dateTimeFormat, CultureInfo.InvariantCulture);
            Title = element.Descendants(ns + "title").SingleOrDefault().Value;
            Content = element.Descendants(ns + "content").SingleOrDefault().Value;
            UpdatedString = element.Descendants(ns + "updated").SingleOrDefault().Value;
            Updated = DateTime.ParseExact(UpdatedString, dateTimeFormat, CultureInfo.InvariantCulture);
            Source = element.Descendants(twitterNS + "source").SingleOrDefault().Value;

            // retrieve author
            var author = element.Descendants(ns + "author").SingleOrDefault();
            AuthorName = author.Descendants(ns + "name").SingleOrDefault().Value;
            AuthorUrl = author.Descendants(ns + "uri").SingleOrDefault().Value;

            // retrieve links
            var links = element.Descendants(ns + "link");
            TweetUrl = links
                .Where(link => link.FirstAttribute.Value.Contains(TEXT_CONTENT_TYPE))
                .Attributes("href")
                .SingleOrDefault().Value;
            AuthorAvatarUrl = links
                .Where(link => link.FirstAttribute.Value.Contains(IMAGE_CONTENT_TYPE))
                .Attributes("href")
                .SingleOrDefault()
                .Value;
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        public string GetAuthorScreenName()
        {
            return Regex.Replace(AuthorName, @"\(.*?\)", string.Empty).Trim();
        }

        #endregion
    }
}
