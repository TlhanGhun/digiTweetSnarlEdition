using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [CollectionDataContract(Name = "direct-messages", ItemName = "direct_message", Namespace = "")]
    public class DirectMessages : List<DirectMessage>
    {
    }
}
