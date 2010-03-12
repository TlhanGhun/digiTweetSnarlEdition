using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
        [CollectionDataContract(Name = "lists", ItemName = "list", Namespace = "")]
        public class Lists : List<SingleList>
        {
        }
    
}
