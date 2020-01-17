using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class ClientReview
    {
        public string ClientReference = "";
        public DateTime DateOfReview = DateTime.Now;
        //public List<KeyValuePair<string,string>> Recommendations = new List<KeyValuePair<string, string>>();
        public string PotentialImprovementsText = "";

        public ClientReview()
        {
        }

        public ClientReview(string pConfigFile)
        {
            if(File.Exists(pConfigFile))
                Read(pConfigFile);
        }

        private bool Read(string pConfigFile)
        {
            bool result = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pConfigFile);
                foreach (XmlNode insignisNode in doc.ChildNodes)
                {
                    if (insignisNode.Name.ToString().CompareTo("insignis") == 0)
                    {
                        foreach (XmlElement clientNode in insignisNode.ChildNodes)
                        {
                            if (clientNode.Name.ToString().CompareTo("client") == 0)
                            {
                                foreach (XmlAttribute clientAttribute in clientNode.Attributes)
                                {
                                    if (clientAttribute.Name.CompareTo("reference") == 0)
                                        ClientReference = clientAttribute.Value;
                                }

                                foreach (XmlElement reviewNode in clientNode.ChildNodes)
                                {
                                    if (reviewNode.Name.ToString().CompareTo("review") == 0)
                                    {
                                        foreach (XmlAttribute reviewAttribute in reviewNode.Attributes)
                                        {
                                            if (reviewAttribute.Name.CompareTo("dateOfReview") == 0)
                                            {
                                                try
                                                {
                                                    DateOfReview = DateTime.ParseExact(reviewAttribute.Value,"dd-MMM-yyyy",System.Globalization.CultureInfo.InvariantCulture);
                                                }
                                                catch
                                                {
                                                }
                                            }
                                        }

                                        foreach (XmlElement recommendationsNode in reviewNode.ChildNodes)
                                        {
                                            if (recommendationsNode.Name.ToString().CompareTo("potentialImprovements") == 0)
                                            {
                                                PotentialImprovementsText = recommendationsNode.InnerText;
                                            }
                                            else if (recommendationsNode.Name.ToString().CompareTo("recommendations") == 0)
                                            {
                                                /*foreach (XmlElement recommendNode in recommendationsNode.ChildNodes)
                                                {
                                                    if (recommendNode.Name.CompareTo("recommend") == 0)
                                                    {
                                                        string productCode = "";
                                                        string amount = "";
                                                        foreach (XmlAttribute recommendAttribute in recommendNode.Attributes)
                                                        {
                                                            if (recommendAttribute.Name.CompareTo("productCode") == 0)
                                                                productCode = recommendAttribute.Value;
                                                            else if (recommendAttribute.Name.CompareTo("amount") == 0)
                                                                amount = recommendAttribute.Value;
                                                        }
                                                        if (productCode.Trim().Length > 0)
                                                            Recommendations.Add(new KeyValuePair<string, string>(productCode, amount));
                                                    }
                                                }*/
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public bool Write(string pConfigFile)
        {
            bool result = false;
            try
            {
                if (File.Exists(pConfigFile))
                    File.Delete(pConfigFile);

                using (StreamWriter sw = new StreamWriter(pConfigFile, false))
                {
                    sw.Write(ToXML());
                    sw.Close();
                }
                result = true;
            }
            catch
            {
            }
            return result;
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            xml += "<insignis>";
            xml += "<client reference=\"" + ClientReference + "\">";
            xml += "<review dateOfReview=\"" + DateOfReview.ToString("dd-MMM-yyyy") + "\">";
            //xml += "<recommendations>";
            //foreach (KeyValuePair<string, string> recommendation in Recommendations)
            //    xml += "<recommend productCode=\"" + recommendation.Key + "\" amount=\"" + recommendation.Value + "\"/>";
            //xml += "</recommendations>";
            xml += "<potentialImprovements>" + PotentialImprovementsText + "</potentialImprovements>";
            xml += "</review>";
            xml += "</client>";
            xml += "</insignis>";
            return xml;
        }
    }
}
