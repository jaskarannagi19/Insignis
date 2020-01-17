namespace Insignis.Asset.Management.Reports.Helper
{
    public class Escape
    {
        public static string ToXML(string pSource)
        {
            string xml = "";

            xml = pSource.Replace("\"", "&quot;");
            xml = xml.Replace("'", "&apos;");
            xml = xml.Replace("<", "&lt;");
            xml = xml.Replace(">", "&gt;");
            xml = xml.Replace("&", "&amp;");

            return xml;
        }

        public static string FromXML(string pSource)
        {
            string xml = "";
            xml = pSource.Replace("&quot;", "\"");
            xml = xml.Replace("&apos;", "'");
            xml = xml.Replace("&lt;", "<");
            xml = xml.Replace("&gt;", ">");
            xml = xml.Replace("&amp;", "&");
            return xml;
        }
    }
}
