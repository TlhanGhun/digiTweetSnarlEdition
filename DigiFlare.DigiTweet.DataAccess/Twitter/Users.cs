using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace DigiFlare.DigiTweet.DataAccess
{
    [CollectionDataContract(Name = "users", ItemName = "user", Namespace = "")]
    public class Users : List<User>
    {
    }
}
