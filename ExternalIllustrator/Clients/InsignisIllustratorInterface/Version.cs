namespace Insignis.Asset.Management.Illustrator.Interface
{
    public class Version
    {
        public string VersionNumber = "1.00";
        public string ToXML()
        {
            string xml = "<version number=\"" + VersionNumber + "\"/>";
            return xml;
        }
    }
}
