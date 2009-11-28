using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;

namespace DigiFlare.DigiTweet.DataAccess
{
    [ServiceContract]
    public interface ITwitterSearchClient
    {
        [OperationContract(Name="Search")]
        [WebGet(UriTemplate = "search.atom?q={keyword}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SearchResults Search(string keyword);

        [OperationContract(Name="SearchPage")]
        [WebGet(UriTemplate = "search.atom?q={keyword}&page={page}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SearchResults Search(string keyword, int page);

        [OperationContract(Name = "SearchSince")]
        [WebGet(UriTemplate = "search.atom?q={keyword}&since_id={sinceId}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        SearchResults Search(string keyword, string sinceId);
    }
}
