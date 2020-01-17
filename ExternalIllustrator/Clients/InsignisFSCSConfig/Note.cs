using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class Note
    {
        public string Text = "";

        public Note(string pText)
        {
            Text = pText;
        }

        public Note(XmlElement pRoot)
        {
            if (pRoot.Name.ToString().CompareTo("note") == 0)
            {
                Text = pRoot.InnerXml.Replace("&amp;","&");
            }
        }

        public string ToXML()
        {
            string xml = "";
            xml += "<note>";
            xml += Text.Replace("&","&amp;");
            xml += "</note>";
            return xml;
        }
    }
}