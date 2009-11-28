using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [CollectionDataContract(Name = "feed", ItemName = "entry", Namespace = "http://www.w3.org/2005/Atom")]
    public class SearchResults : List<SearchResult>
    {
    }
}
