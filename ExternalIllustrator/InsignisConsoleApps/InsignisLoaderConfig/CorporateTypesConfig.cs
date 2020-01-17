using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class CorporateTypesConfig
    {
        public List<CorporateType> Types = new List<CorporateType>();

        public CorporateTypesConfig()
        {
        }

        public CorporateTypesConfig(string pConfigFile)
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
                foreach (XmlNode insignisNode in doc.ChildNodes)
                {
                    if (insignisNode.Name.ToString().CompareTo("insignis") == 0)
                    {
                        foreach (XmlElement corporateTypesNode in insignisNode.ChildNodes)
                        {
                            if (corporateTypesNode.Name.ToString().CompareTo("corporateTypes") == 0)
                            {
                                foreach (XmlElement corporateTypeNode in corporateTypesNode.ChildNodes)
                                {
                                    if (corporateTypeNode.Name.ToString().CompareTo("corporateType") == 0)
                                    {
                                        Types.Add(new CorporateType(corporateTypeNode));
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
            xml += "<insignis>";
            if (Types != null && Types.Count > 0)
            {
                xml += "<corporateTypes>";
                Types.Sort((x, y) => (x.Name).CompareTo(y.Name));
                foreach (CorporateType corporateType in Types)
                    xml += corporateType.ToXML();
                xml += "</corporateTypes>";
            }
            xml += "</insignis>";
            return xml;
        }

        public void RegisterType(string pType)
        {
            if (pType != null && pType.Trim().Length > 0)
            {
                if (Compare(pType) == false)
                    Types.Add(new CorporateType(Format(pType)));
            }
        }

        public bool Compare(string pType)
        {
            bool found = false;
            if (pType != null && pType.Trim().Length > 0)
            {
                string tmpType = Format(pType);
                if (tmpType.Trim().Length > 0)
                {
                    foreach (CorporateType corporateType in Types)
                    {
                        if (corporateType.Name.ToLower().CompareTo(tmpType.ToLower()) == 0)
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }
            return found;
        }

        private string Format(string pType)
        {
            string tmpType = pType;
            tmpType = tmpType.Replace("&#xD;", "");
            tmpType = tmpType.Replace("\r", "");
            tmpType = tmpType.Replace("\n", "");
            tmpType = tmpType.Replace("&amp;", "&");
            tmpType = tmpType.Replace(".", "");
            return tmpType;
        }
    }
}
