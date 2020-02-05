using System.Collections.Generic;

namespace Insignis.Asset.Management.Illustrator.Interface
{
    public class Bank
    {
        public int BankID = -1;
        public string BankName = "";
        public string FitchRating = "";

        public List<Product> Products = new List<Product>();

        public string ToXML()
        {
            string xml = "<bank id=\"" + BankID + "\" name=\"" + BankName + "\" fitchRating=\"" + FitchRating + "\">";
            xml += "<products>";
            foreach (Product product in Products)
                xml += product.ToXML();
            xml += "</products>";
            xml += "</bank>";
            return xml;
        }
    }
}
