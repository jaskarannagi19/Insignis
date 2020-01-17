using System.IO;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Footer
    {
        public static Literal FooterBody()
        {
            Literal footer = new Literal();
            footer.Text = "";
            string templateBody = null;
            if (Directory.Exists(System.Web.HttpRuntime.AppDomainAppPath))
            {
                string templateFile = System.Web.HttpRuntime.AppDomainAppPath;
                if (templateFile.EndsWith("\\") == false)
                    templateFile += "\\";
                templateFile += "Template";
                if (Directory.Exists(templateFile))
                {
                    templateFile += "\\";
                    templateFile += "Menu";
                    if (Directory.Exists(templateFile))
                    {
                        templateFile += "\\";
                        templateFile += "FooterBody.htm";
                        if (File.Exists(templateFile))
                        {
                            using (StreamReader sr = new StreamReader(templateFile))
                            {
                                templateBody = sr.ReadToEnd();
                                sr.Close();
                            }
                        }
                    }
                }
            }
            if (templateBody == null)
            {
                //footer.Text += "<div class=\"row\">\n";
                //footer.Text += "    <div class=\"span3\" id=\"footer-middle-widget-area-1\"><div id=\"text-2\" class=\"widget widget_text\"><div class=\"textwidget\">Insignis Asset Management<br/>Merlin Place<br/>Milton Road<br/>Cambridge<br/>CB4 0DP</div></div></div>\n";
                //footer.Text += "    <div class=\"span3\" id=\"footer-middle-widget-area-2\"><div id=\"text-3\" class=\"widget widget_text\"><div class=\"textwidget\">Follow us on social media<br/><a href=\"https://www.facebook.com/Insignis-Asset-Management-670326009736887/?fref=ts\" target=\"_blank\"><i class=\"fa fa-facebook\"></i></a>&nbsp;&nbsp;<a href=\"https://www.linkedin.com/company/9487040?trk=tyah&amp;trkInfo=clickedVertical%3Acompany%2CclickedEntityId%3A9487040%2Cidx%3A2-1-2%2CtarId%3A1450465075130%2Ctas%3Ainsignis%20asset%20management\" target=\"_blank\"><i class=\"fa fa-linkedin\"></i></a>&nbsp;&nbsp;<a href=\"https://twitter.com/InsignisAM\" target=\"_blank\"><i class=\"fa fa-twitter\"></i></a></div></div></div>\n";
                //footer.Text += "    <div class=\"span3\" id=\"footer-middle-widget-area-3\"><div id=\"text-4\" class=\"widget widget_text\"><div class=\"textwidget\"><a href=\"https://www.insigniscash.com/index.php/privacy-policy/\">Privacy Policy</a><br/><a href=\"https://www.insigniscash.com/index.php/terms-and-conditions/\">Terms and Conditions</a><br/><a href=\"https://www.insigniscash.com/index.php/legal-information/\">Legal Information</a><br/></div></div></div>\n";
                //footer.Text += "    <div class=\"span3\" id=\"footer-middle-widget-area-4\"><div id=\"text-7\" class=\"widget widget_text\"><div class=\"textwidget\">Send us a message<br/><a href=\"mailto:client.services@insigniscash.com\" target=\"_blank\">client.services@insigniscash.com</a></div></div></div>\n";
                //footer.Text += "</div>\n";

                footer.Text += "<div class=\"row\">";
                footer.Text += "  <div class=\"span3\" id=\"footer-middle-widget-area-1\">";
                footer.Text += "    <div id=\"ewf_widget_contact_info-4\" class=\"widget ewf_widget_contact_info\">";
                footer.Text += "      <h4 class=\"widget-title\">INSIGNIS CASH SOLUTIONS</h4>";
                footer.Text += "      <ul><li><span><i class=\"ifc-home\"></i></span>Merlin Place, Milton Road, Cambridge, CB4 0DP</li></ul>";
                footer.Text += "    </div>";
                footer.Text += "  </div>";
                footer.Text += "  <div class=\"span3\" id=\"footer-middle-widget-area-2\">";
                footer.Text += "  </div>";
                footer.Text += "  <div class=\"span3\" id=\"footer-middle-widget-area-3\">";
                footer.Text += "    <div id=\"ewf_widget_social_media-2\" class=\"widget ewf_widget_social_media\">";
                footer.Text += "      <h4 class=\"widget-title\">FOLLOW US</h4>";
                footer.Text += "      <div class=\"fixed\"><a class=\"facebook-icon social-icon\" href=\"https://www.facebook.com/InsignisAM/\"><i class=\"fa fa-facebook\"></i></a><a class=\"twitter-icon social-icon\" href=\"https://twitter.com/InsignisCash\"><i class=\"fa fa-twitter\"></i></a><a class=\"linkedin-icon social-icon\" href=\"https://www.linkedin.com/company-beta/9487040/\"><i class=\"fa fa-linkedin\"></i></a></div>";
                footer.Text += "    </div>";
                footer.Text += "  </div>";
                footer.Text += "  <div class=\"span3\" id=\"footer-middle-widget-area-4\">";
                footer.Text += "  </div>";
                footer.Text += "</div>";
            }
            else
                footer.Text += templateBody;
            return footer;
        }

        public static Literal FooterBottom()
        {
            Literal footerBottom = new Literal();
            string templateBody = null;
            if (Directory.Exists(System.Web.HttpRuntime.AppDomainAppPath))
            {
                string templateFile = System.Web.HttpRuntime.AppDomainAppPath;
                if (templateFile.EndsWith("\\") == false)
                    templateFile += "\\";
                templateFile += "Template";
                if (Directory.Exists(templateFile))
                {
                    templateFile += "\\";
                    templateFile += "Menu";
                    if (Directory.Exists(templateFile))
                    {
                        templateFile += "\\";
                        templateFile += "FooterBottom.htm";
                        if (File.Exists(templateFile))
                        {
                            using (StreamReader sr = new StreamReader(templateFile))
                            {
                                templateBody = sr.ReadToEnd();
                                sr.Close();
                            }
                        }
                    }
                }
            }
            if (templateBody == null)
            {
                footerBottom.Text = "";
                footerBottom.Text += "<div class=\"row\">\n";
                //footerBottom.Text += "    <div class=\"span12\" id=\"footer-bottom-widget-area-1\"><div id=\"text-6\" class=\"widget widget_text\"><div class=\"textwidget\"><small>Insignis Asset Management Limited (Company number 09477376)  is authorised by the Financial Conduct Authority as an appointed representative of Markham Private Clients Limited. Registration number 673532</small></div></div></div>\n";
                footerBottom.Text += "    <div id=\"text-4\" class=\"widget widget_text\"><div class=\"textwidget\"><small>Insignis Cash Solutions is a trading name of Insignis Asset Management Limited (Company number 09477376)<br></small><small>Insignis Asset Management Limited is authorised and regulated by the Financial Conduct Authority (813442).<br></small><a href=\"https://www.insigniscash.com/privacy-policy/\">PRIVACY POLICY</a>  |  <a href=\"https://www.insigniscash.com/terms-and-conditions/\">WEBSITE TERMS</a>  |  <a href=\"https://www.insigniscash.com/wp-content/uploads/2019/02/InsignisClientTermsAndConditions.pdf\" target=\"_blank\" rel=\"noopener\">CLIENT TERMS AND CONDITIONS</a> | <a href=\"https://www.insigniscash.com/legal-information/\">LEGAL INFORMATION</a></div></div>";
                footerBottom.Text += "</div>\n";
            }
            else
                footerBottom.Text += templateBody;
            return footerBottom;
        }
    }
}