using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class FeeMatrix
    {
        public List<FeeMatrixItem> items = new List<FeeMatrixItem>();

        public FeeMatrix()
        {
        }

        public FeeMatrix(string pConfigFile)
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
                foreach (XmlNode feeMatrixNode in doc.ChildNodes)
                {
                    if (feeMatrixNode.Name.ToString().CompareTo("feeMatrix") == 0)
                    {
                        foreach (XmlElement feeMatrixItemNode in feeMatrixNode.ChildNodes)
                        {
                            if (feeMatrixItemNode.Name.ToString().CompareTo("feeMatrixItem") == 0)
                                items.Add(new FeeMatrixItem(feeMatrixItemNode));
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
            xml += "<feeMatrix>";
            if (items.Count > 0)
            {
                foreach (FeeMatrixItem item in items)
                    xml += item.ToXML();
            }
            xml += "</feeMatrix>";
            return xml;
        }

        public decimal GetRateForDeposit(decimal pDeposit)
        {
            return GetRateForDeposit(pDeposit, (decimal)0.15);
        }

        public decimal GetRateForDeposit(decimal pDeposit, decimal pDefaultRate)
        {
            decimal rate = pDefaultRate;

            if (pDeposit > 0)
            {
                foreach (FeeMatrixItem item in items)
                {
                    if (item.Rate.HasValue)
                    {
                        if (item.From.HasValue)
                        {
                            if (pDeposit >= item.From)
                            {
                                if (item.To.HasValue)
                                {
                                    if(pDeposit <= item.To)
                                    {
                                        rate = item.Rate.Value;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item.To.HasValue)
                            {
                                if (pDeposit <= item.To)
                                {
                                    rate = item.Rate.Value;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return rate;
        }
    }
}