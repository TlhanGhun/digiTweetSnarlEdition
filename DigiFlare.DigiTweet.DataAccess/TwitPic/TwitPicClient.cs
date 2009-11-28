using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace DigiFlare.DigiTweet.DataAccess
{
    public class TwitPicClient : ITwitPicClient
    {
        #region Constants
        private const string TWITPIC_UPLOAD_API_URL = "http://twitpic.com/api/upload";
        #endregion

        #region Instance Variables
        private string _username;
        private string _password;
        #endregion

        #region Constructor
        public TwitPicClient(string username, string password)
        {
            _username = username;
            _password = password;
        }
        #endregion

        #region ITwitPicClient Members

        public string Upload(byte[] image, string fileName)
        {
            string boundary = Guid.NewGuid().ToString();
            string requestUrl = TWITPIC_UPLOAD_API_URL;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
            string encoding = "iso-8859-1";

            // init http web request object
            request.PreAuthenticate = true;
            request.AllowWriteStreamBuffering = true;
            request.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
            request.Method = "POST";

            // build the http request
            string header = string.Format("--{0}", boundary);
            string footer = string.Format("--{0}--", boundary);

            StringBuilder contents = new StringBuilder();
            contents.AppendLine(header);

            string fileContentType = GetImageType(fileName);
            string fileHeader = String.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", "media", fileName);
            string fileData = Encoding.GetEncoding(encoding).GetString(image);

            contents.AppendLine(fileHeader);
            contents.AppendLine(String.Format("Content-Type: {0}", fileContentType));
            contents.AppendLine();
            contents.AppendLine(fileData);

            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "username"));
            contents.AppendLine();
            contents.AppendLine(_username);

            contents.AppendLine(header);
            contents.AppendLine(String.Format("Content-Disposition: form-data; name=\"{0}\"", "password"));
            contents.AppendLine();
            contents.AppendLine(_password);

            contents.AppendLine(footer);

            byte[] bytes = Encoding.GetEncoding(encoding).GetBytes(contents.ToString());
            request.ContentLength = bytes.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                // make server request
                requestStream.Write(bytes, 0, bytes.Length);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    // read server response
                    string result = reader.ReadToEnd();
                    XDocument doc = XDocument.Parse(result);

                    // if status is "ok", then return pic url
                    // otherwise, return null
                    if (doc.Descendants("rsp").Attributes("stat").Single().Value.Equals("ok", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string url = doc.Descendants("mediaurl").Single().Value;
                        return url;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private string GetImageType(string filename)
        {
            string imageType = string.Empty;
            switch (System.IO.Path.GetExtension(filename).ToLower())
            {
                case ".bm":
                    imageType = "image/bmp";
                    break;
                case ".bmp":
                    imageType = "image/bmp";
                    break;
                case ".gif":
                    imageType = "image/gif";
                    break;
                case ".ico":
                    imageType = "image/x-icon";
                    break;
                case ".jpe":
                    imageType = "image/jpeg";
                    break;
                case ".jpeg":
                    imageType = "image/jpeg";
                    break;
                case ".jpg":
                    imageType = "image/jpeg";
                    break;
                case ".png":
                    imageType = "image/png";
                    break;
                case ".rgb":
                    imageType = "image/x-rgb";
                    break;
                case ".tif":
                    imageType = "image/tiff";
                    break;
                case ".tiff":
                    imageType = "image/tiff";
                    break;
                case ".x-png":
                    imageType = "image/png";
                    break;
            }
            return imageType;
        }

        #endregion
    }
}
