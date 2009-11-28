using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace DigiFlare.DigiTweet.DataAccess
{
    public interface ITinyUrlClient
    {
        string Create(string url);
    }
}
