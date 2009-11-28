using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace DigiFlare.DigiTweet.DataAccess
{
    [ServiceContract]
    public interface IThumbalizrClient
    {
        [OperationContract]
        [WebGet(UriTemplate = "/?api_key={apiKey}&url={url}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetPreview(string apiKey, string url);
    }
}
