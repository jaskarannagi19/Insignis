using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Insignis.Console.Apps.Data.Loader.Config
{
    public class DataLoaderConfig
    {
        public StagedFilterDefinition StagedFilter = null;
        public List<BaseExcludeInstitution> ExclusionList = new List<BaseExcludeInstitution>();

        public DataLoaderConfig()
        {
        }

        public DataLoaderConfig(string pConfigFile)
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
                        foreach (XmlElement dataLoaderNode in insignisNode.ChildNodes)
                        {
                            if (dataLoaderNode.Name.ToString().CompareTo("dataLoader") == 0)
                            {
                                foreach (XmlElement configNode in dataLoaderNode.ChildNodes)
                                {
                                    if (configNode.Name.ToString().CompareTo("config") == 0)
                                    {
                                        foreach (XmlElement stagedFilterNode in configNode.ChildNodes)
                                        {
                                            if (stagedFilterNode.Name.ToString().CompareTo("stagedFilterDefinition") == 0)
                                            {
                                                StagedFilter = new StagedFilterDefinition(stagedFilterNode);
                                            }
                                        }
                                    }
                                    else if (configNode.Name.ToString().CompareTo("exclusionList") == 0)
                                    {
                                        foreach (XmlElement excludeInstitutionNode in configNode.ChildNodes)
                                        {
                                            if (excludeInstitutionNode.Name.ToString().CompareTo("excludeInstitution") == 0)
                                            {
                                                ExclusionList.Add(new BaseExcludeInstitution(excludeInstitutionNode));
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
            catch (Exception exc)
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
            xml += "<dataLoader>";
            if (StagedFilter != null)
            {
                xml += "<config>";
                xml += StagedFilter.ToXML();
                xml += "</config>";
            }
            if (ExclusionList != null && ExclusionList.Count > 0)
            {
                xml += "<exclusionList>";
                foreach (BaseExcludeInstitution excludeInstitution in ExclusionList)
                    xml += excludeInstitution.ToXML();
                xml += "</exclusionList>";
            }
            xml += "</dataLoader>";
            xml += "</insignis>";
            return xml;
        }
    }
}
