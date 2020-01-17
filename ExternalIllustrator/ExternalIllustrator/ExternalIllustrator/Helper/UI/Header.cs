using Octavo.Gate.Nabu.Encryption;
using System.IO;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Header
    {
        public static Literal oldHeaderTop(string pLoggedInName, string pLoggedInAsName)
        {
            EncryptorDecryptor encryptorDecryptor = new EncryptorDecryptor();
            string someText = "";
            if (pLoggedInName != null && pLoggedInName.Trim().Length > 0)
                someText += encryptorDecryptor.Decrypt(pLoggedInName);

            if (pLoggedInAsName != null && pLoggedInAsName.Trim().Length > 0)
            {
                if (someText.Trim().Length > 0)
                    someText += " : ";
                someText += encryptorDecryptor.Decrypt(pLoggedInAsName);
            }
            return oldHeaderTop(someText);
        }

        public static Literal oldHeaderTop(string pSomeText)
        {
            Literal headerTop = new Literal();
            headerTop.Text = "";
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
                        templateFile += "HeaderTop.htm";
                        if (File.Exists(templateFile))
                        {
                            using (StreamReader sr = new StreamReader(templateFile))
                            {
                                templateBody = sr.ReadToEnd();
                                sr.Close();
                            }
                            templateBody = templateBody.Replace("#SOMETEXT", pSomeText);
                        }
                    }
                }
            }
            if (templateBody == null)
            {
                headerTop.Text += "<div class=\"row\">\n";
                headerTop.Text += "    <div class=\"span5\" id=\"header-top-widget-area-1\">\n";
                headerTop.Text += "        <div id=\"text-5\" class=\"widget widget_text\">\n";
                headerTop.Text += "            <div class=\"textwidget\">" + pSomeText + "</div>\n";
                headerTop.Text += "        </div>\n";
                headerTop.Text += "    </div>\n";
                headerTop.Text += "    <div class=\"span6\" id=\"header-top-widget-area-2\">\n";
                headerTop.Text += "        <div id=\"ewf_widget_contact_info-3\" class=\"widget ewf_widget_contact_info\">\n";
                headerTop.Text += "            <ul>\n";
                headerTop.Text += "                <li><span><i class=\"ifc-phone2\"></i></span>+44 (0)1223 200 674</li>\n";
                headerTop.Text += "                <li><span><i class=\"ifc-message\"></i></span><a href=\"mailto:client.services@insigniscash.com\">client.services@insigniscash.com</a></li>\n";
                headerTop.Text += "            </ul>\n";
                headerTop.Text += "        </div>\n";
                headerTop.Text += "    </div>\n";
                headerTop.Text += "    <div class=\"span1\"><p><a href=\"#\" style=\"font-size:8px;\" onclick=\"DecreaseScreenFont();\" title=\"Reduce Font Size\">-A</a>&nbsp;<a href=\"#\" style=\"font-size:16px;\" onclick=\"IncreaseScreenFont();\" title=\"Increase Font Size\">A+</a></p></div>\n";
                headerTop.Text += "</div>\n";
            }
            else
                headerTop.Text += templateBody;
            return headerTop;
        }

        public static Literal HeaderBody(Literal menu)
        {
            Literal header = new Literal();
            header.Text = "";
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
                        templateFile += "HeaderBody.htm";
                        if (File.Exists(templateFile))
                        {
                            using (StreamReader sr = new StreamReader(templateFile))
                            {
                                templateBody = sr.ReadToEnd();
                                sr.Close();
                            }
                            templateBody = templateBody.Replace("#MENUBODY", menu.Text);
                        }
                    }
                }
            }
            if (templateBody == null)
            {
                Helper.UI.Image imageHelper = new Image(Helper.DomainRoot.Get());

                header.Text += "<div class=\"row\">\n";
                header.Text += "	<div class=\"span3\">\n";
                header.Text += "		<!-- // Logo // -->\n";
                header.Text += "		<a href=\"https://partner.insigniscash.com\" alt=\"logo\" title=\"Home\"><img class=\"responsive-img\" src=\"https://partner.insigniscash.com/wp-content/uploads/Insignis-logo.jpg\" alt=\"\" style=\"max-height:124px;\"/></a>\n";
                header.Text += "        <!-- end #logo -->\n";
                header.Text += "	</div><!-- end .span3 -->\n";
                header.Text += "	<div class=\"span9\">\n";
                header.Text += "		<a id=\"mobile-menu-trigger\" href=\"#\"><i class=\"fa fa-bars\"></i></a>\n";
                header.Text += "		<!-- // Menu // -->\n";
                header.Text += menu.Text;
                header.Text += "	</div><!-- end .span9 -->\n";
                header.Text += "</div><!-- end .row -->\n";
            }
            else
                header.Text += templateBody;
            return header;
        }
    }
}