using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [CollectionDataContract(Name = "statuses", ItemName = "status", Namespace="")]
    public class Statuses : List<Status>
    {
    }
}
