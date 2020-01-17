using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class CurrencyUplift
    {
        public List<KeyValuePair<string,decimal>> rates = new List<KeyValuePair<string,decimal>>();

        public CurrencyUplift()
        {
        }

        public CurrencyUplift(string pConfigFile)
        {
            Read(pConfigFile);
        }

        private bool Read(string pConfigFile)
        {
            bool result = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(pConfigFile);
                foreach (XmlNode currencyUpliftNode in doc.ChildNodes)
                {
                    if (currencyUpliftNode.Name.ToString().CompareTo("currencyUplift") == 0)
                    {
                        foreach (XmlElement upliftNode in currencyUpliftNode.ChildNodes)
                        {
                            if (upliftNode.Name.ToString().CompareTo("uplift") == 0)
                            {
                                string currencyCode = "";
                                decimal? rate = null;

                                foreach (XmlAttribute attribute in upliftNode.Attributes)
                                {
                                    if (attribute.Name.CompareTo("currencyCode") == 0)
                                        currencyCode = attribute.Value;
                                    else if (attribute.Name.CompareTo("rate") == 0)
                                    {
                                        try
                                        {
                                            rate = Convert.ToDecimal(attribute.Value);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                                if(currencyCode != null && currencyCode.Length > 0 && rate.HasValue)
                                    rates.Add(new KeyValuePair<string, decimal>(currencyCode,rate.Value));
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
                if (System.IO.File.Exists(pConfigFile))
                    System.IO.File.Delete(pConfigFile);

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
            xml += "<currencyUplift>";
            if (rates.Count > 0)
            {
                foreach (KeyValuePair<string,decimal> rate in rates)
                    xml += "<uplift currencyCode=\"" + rate.Key + "\" rate=\"" + rate.Value.ToString("0.000") + "\" />";
            }
            xml += "</currencyUplift>";
            return xml;
        }

        public decimal GetRateForCurrencyCode(string pCurrencyCode)
        {
            decimal rate = 0;
            foreach (KeyValuePair<string, decimal> loopRate in rates)
            {
                if (loopRate.Key.CompareTo(pCurrencyCode) == 0)
                {
                    rate = loopRate.Value;
                    break;
                }
            }
            return rate;
        }
    }
}
