using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace BabyInABag.Models.VMs
{
    public class GetPayPalData
    {

        public String GetPayPalResponse(string tx)
        {
            try
            {
                string authToken = WebConfigurationManager.AppSettings["Token"];
                string txToken = tx;
                string query = string.Format("cmd=_notify-synch&tx={0}&at={1}", txToken, authToken);
                string url = WebConfigurationManager.AppSettings["urlSubmitPayment"];

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = query.Length;
                StreamWriter outStreamWriter = new StreamWriter(req.GetRequestStream(), Encoding.ASCII);
                outStreamWriter.Write(query);
                outStreamWriter.Close();

                StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream());
                string strResponse = reader.ReadToEnd();
                reader.Close();

                if (strResponse.StartsWith("SUCCESS"))
                {
                    return strResponse;
                }
                else
                {
                    return "FAIL";
                }



            }
            catch (Exception e)
            {

                return e.Message;
            }
        }
    }
}