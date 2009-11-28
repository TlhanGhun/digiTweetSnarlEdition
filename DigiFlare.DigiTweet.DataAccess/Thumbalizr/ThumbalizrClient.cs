using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IO;

namespace DigiFlare.DigiTweet.DataAccess
{
    public class ThumbalizrClient : ClientBase<IThumbalizrClient>, IThumbalizrClient
    {
        #region Constructor

        public ThumbalizrClient()
        {
        }

        #endregion

        #region Public Methods

        public Stream GetPreview(string apiKey, string url)
        {
            return base.Channel.GetPreview(apiKey, url);
        }

        #endregion
    }
}
