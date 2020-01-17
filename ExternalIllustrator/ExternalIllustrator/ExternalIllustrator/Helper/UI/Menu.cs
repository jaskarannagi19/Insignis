using System.IO;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Menu
    {
        string domainRoot = "";

        public Menu(string pDomainRoot)
        {
            domainRoot = pDomainRoot;
            if (domainRoot.EndsWith("/") == false)
                domainRoot += "/";
        }

        public Literal LandingMenu(string pCurrentPage)
        {
            Literal menu = new Literal();
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
                        templateFile += "Landing.htm";
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
            menu.Text = "\n";
            menu.Text += "<ul id=\"menu\" class=\"sf-menu fixed\">\n";
            if (templateBody == null)
            {
                menu.Text += "   <li  class=\"menu-item menu-item-type-post_type menu-item-object-page page_item page-item-9 menu-item-21\">\n";
                menu.Text += "       <a title=\"Home\" href=\"" + Helper.DomainRoot.GetPublic() + "\"><span>Home</span></a>\n";
                menu.Text += "   </li>\n";
            }
            else
                menu.Text += templateBody;
            menu.Text += "</ul>\n";
            return menu;
        }

        public Literal LoggedOutMenu(string pCurrentPage)
        {
            Literal menu = new Literal();
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
                        templateFile += "LoggedOut.htm";
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
            menu.Text = "\n";
            menu.Text += "<ul id=\"menu\" class=\"sf-menu fixed\">\n";
            if (templateBody == null)
            {
                menu.Text += "   <li  class=\"menu-item menu-item-type-post_type menu-item-object-page page_item page-item-9 menu-item-21\">\n";
                menu.Text += "       <a title=\"Home\" href=\"" + Helper.DomainRoot.GetPublic() + "\"><span>Home</span></a>\n";
                menu.Text += "   </li>\n";
            }
            else
                menu.Text += templateBody;
            menu.Text += "   <li  class=\"menu-item menu-item-type-post_type menu-item-object-page " + ((pCurrentPage.CompareTo("Default.aspx") == 0) ? "current-menu-item current_page_item current " : "") + "menu-item-20\">\n";
            menu.Text += "      <a title=\"Partner Login\" href=\"" + domainRoot + "\"><span>Partner Login</span></a>\n";
            menu.Text += "   </li>\n";
            menu.Text += "</ul>\n";
            return menu;
        }

        public Literal CloseWindowMenu()
        {
            Literal menu = new Literal();
            menu.Text = "\n";
            menu.Text += "<ul id=\"menu\" class=\"sf-menu fixed\">\n";
            menu.Text += "   <li  class=\"menu-item menu-item-type-post_type menu-item-object-page menu-item-20\">\n";
            menu.Text += "      <a title=\"Close Report Preview\" href=\"javascript:window.close();" + "\"><span>Close Preview</span></a>\n";
            menu.Text += "   </li>\n";
            menu.Text += "</ul>\n";
            return menu;
        }
    }
}