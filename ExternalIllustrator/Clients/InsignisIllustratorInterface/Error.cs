namespace Insignis.Asset.Management.Illustrator.Interface
{
    public class Error
    {
        public string ErrorMessage = "";

        public string ToXML()
        {
            string xml = "<error message=\"" + ErrorMessage + "\"/>";
            return xml;
        }
    }
}
