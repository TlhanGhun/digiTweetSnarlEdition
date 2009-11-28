using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Xml.Linq;

namespace DigiFlare.DigiTweet.DataAccess
{
    public interface ITwitPicClient
    {
        string Upload(byte[] image, string fileName);
    }
}
