using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace DigiFlare.DigiTweet.DataAccess
{
    public class TinyUrlClient : ITinyUrlClient, IDisposable
    {
        #region Constants
        public const string ENCODING_NAME = "ASCII";
        public const int REQUEST_TIMEOUT = 5000;
        public const string TINYURL_ADDRESS_TEMPLATE = "http://tinyurl.com/api-create.php?url={0}";
        public const string USER_AGENT = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.9.0.1) Gecko/2008070208 Firefox/3.0.1";
        #endregion

        #region Instance Variables
        private HttpWebRequest request = null;
        private HttpWebResponse response = null;
        #endregion

        #region ITinyUrlClient Members

        public string Create(string url)
        {
            try
            {
                // setup web request to tinyurl
                request = (HttpWebRequest)WebRequest.Create(string.Format(TINYURL_ADDRESS_TEMPLATE, url));
                request.Timeout = REQUEST_TIMEOUT;
                request.UserAgent = USER_AGENT;

                // get response
                response = (HttpWebResponse)request.GetResponse();

                // prase response stream to string
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(ENCODING_NAME));

                // convert the buffer into string and store in content
                StringBuilder sb = new StringBuilder();
                while (reader.Peek() >= 0)
                {
                    sb.Append(reader.ReadLine());
                }
                return sb.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                request.Abort();
                response.Close();
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion
    }
}
