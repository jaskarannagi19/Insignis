using Octavo.Gate.Nabu.Abstraction;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Illustrator
{
    public partial class GenerateTemplatedIllustration : System.Web.UI.Page
    {
        private Helper.MultiLingual multiLingual;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["_partnerOrganisation"] != null && Session["_partnerOrganisation"].ToString().Length > 0 &&
               Session["_partnerName"] != null && Session["_partnerName"].ToString().Length > 0 &&
               Session["_partnerEmailAddress"] != null && Session["_partnerEmailAddress"].ToString().Length > 0 &&
               Session["_partnerTelephone"] != null && Session["_partnerTelephone"].ToString().Length > 0)
            {
                Helper.UI.Notify notifications = new Helper.UI.Notify(_panelNotify);
                multiLingual = new Helper.MultiLingual("English");
                Helper.UI.Menu menuHelper = new Helper.UI.Menu(Helper.DomainRoot.Get().ToString());
                this.Master.FindControl("ContentHeader").Controls.Add(Helper.UI.Header.HeaderBody(menuHelper.LandingMenu("Landing.aspx")));
                this.Master.FindControl("ContentFooterBottom").Controls.Add(Helper.UI.Footer.FooterBottom());

                try
                {
                    if (Request.QueryString["T"] != null && Request.QueryString["T"].Trim().Length > 0)
                    {
                        Octavo.Gate.Nabu.Encryption.EncryptorDecryptor decryptor = new Octavo.Gate.Nabu.Encryption.EncryptorDecryptor();
                        string qsTemplateFile = decryptor.Decrypt(decryptor.UrlDecode(Request.QueryString["T"]));
                        if (System.IO.File.Exists(qsTemplateFile))
                        {
                            System.IO.FileInfo templateFile = new System.IO.FileInfo(qsTemplateFile);

                            Insignis.Asset.Management.PowerPoint.Generator.RenderAbstraction powerpointRenderAbstraction = new PowerPoint.Generator.RenderAbstraction(ConfigurationManager.AppSettings["illustrationOutputInternalFolder"], ConfigurationManager.AppSettings["illustrationOutputPublicFacingFolder"]);

                            //Insignis.Asset.Management.Action.Logging.Entities.ActionLog actionLog = new Insignis.Asset.Management.Action.Logging.Entities.ActionLog();
                            //actionLog.Reference = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss");
                            //actionLog.System = "ADMIN";
                            //actionLog.Module = "GenerateTemplatedIllustration";
                            //actionLog.Method = "Page_Load";
                            //actionLog.Action = "PPTX-MERGE-AND-PDF";
                            //actionLog.ActionInitiatedByOrganisationName = "Insignis";
                            //actionLog.ActionInitiatedByContactName = decryptor.Decrypt(Session["LoggedInName"].ToString());
                            //actionLog.ActionInitiatedByContactEmail = decryptor.Decrypt(authentication.userAccountSession.UserDetail.AccountName);
                            //actionLog.ActionInitatedOn = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                            //actionLog.ActionInitiatedAt = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));

                            // now generate the replacements
                            List<KeyValuePair<string, string>> textReplacements = new List<KeyValuePair<string, string>>();

                            Octavo.Gate.Nabu.Preferences.Manager preferencesManager = new Octavo.Gate.Nabu.Preferences.Manager(ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                            Octavo.Gate.Nabu.Preferences.Preference scurveSettings = preferencesManager.GetPreference("Sales.Tools.SCurve.Settings", 1, "Settings");
                            int availableToHubAccountID = -1;
                            if (scurveSettings.GetChildPreference("AvailableTo") != null && scurveSettings.GetChildPreference("AvailableTo").Value.Trim().Length > 0)
                            {
                                try
                                {
                                    availableToHubAccountID = Convert.ToInt32(scurveSettings.GetChildPreference("AvailableTo").Value);
                                }
                                catch
                                {
                                }
                            }
                            FinancialAbstraction financialAbstraction = new FinancialAbstraction(multiLingual.GetAbstraction().ConnectionString, multiLingual.GetAbstraction().DBType, multiLingual.GetAbstraction().ErrorLogFile);
                            Helper.UI.Currency currencyHelper = new Helper.UI.Currency(financialAbstraction.GetCurrencyByCode(scurveSettings.GetChildPreference("CurrencyCode").Value), financialAbstraction);

                            //foreach (Octavo.Gate.Nabu.Preferences.Preference childPreference in scurveSettings.Children)
                            //    actionLog.Settings.Add(new Management.Action.Logging.Entities.ActionSetting("Settings", childPreference.Name, childPreference.Value));

                            Octavo.Gate.Nabu.Preferences.Preference institutionInclusion = preferencesManager.GetPreference("Sales.Tools.SCurve.Institutions", 1, "Institutions");
                            Octavo.Gate.Nabu.Preferences.Preference savedDeposits = preferencesManager.GetPreference("Sales.Tools.SCurve.Builder." + availableToHubAccountID, 1, "Deposits");
                            Octavo.Gate.Nabu.Preferences.Preference scurveProperties = preferencesManager.GetPreference("Sales.Tools.SCurve.Properties." + availableToHubAccountID, 1, "Properties");

                            textReplacements.Add(new KeyValuePair<string, string>("REFERENCE", "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss")));
                            textReplacements.Add(new KeyValuePair<string, string>("DATE", DateTime.Now.ToString("dd/MM/yyyy")));
                            textReplacements.Add(new KeyValuePair<string, string>("CLIENTNAME", scurveSettings.GetChildPreference("OptionalClientName").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("CLIENTTYPE", financialAbstraction.GetAccountType(availableToHubAccountID,(int)multiLingual.language.LanguageID).Detail.Name));
                            textReplacements.Add(new KeyValuePair<string, string>("INTROORG", scurveSettings.GetChildPreference("OptionalIntroducerOrganisationName").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("FEEDISCOUNT", scurveSettings.GetChildPreference("IntroducerDiscount").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("FEE", scurveProperties.GetChildPreference("FEE").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("CHARGE", scurveProperties.GetChildPreference("CHARGE").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("TOTAL", currencyHelper.GetCurrentCurrency().GetCurrencySymbol() + scurveProperties.GetChildPreference("TOTAL").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("PROTECTION", scurveProperties.GetChildPreference("FSCSELIGIBILITY").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("GROSSYIELD", scurveProperties.GetChildPreference("GROSSYIELD").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("GROSSINTEREST", currencyHelper.GetCurrentCurrency().GetCurrencySymbol() + scurveProperties.GetChildPreference("GROSSINTEREST").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("NETYIELD", scurveProperties.GetChildPreference("NETYIELD").Value));
                            textReplacements.Add(new KeyValuePair<string, string>("NETINTEREST", currencyHelper.GetCurrentCurrency().GetCurrencySymbol() + scurveProperties.GetChildPreference("NETINTEREST").Value));

                            // transform the saved deposits into SCurveOutputRows
                            List<Tools.Sales.SCurveOutputRow> proposedPortfolio = new List<Tools.Sales.SCurveOutputRow>();
                            int savedDepositIndex = 0;
                            for (int i = 1; i <= 30; i++)
                            {
                                try
                                {
                                    if (savedDeposits != null && savedDeposits.Children.Count > 0)
                                    {
                                        if ((savedDepositIndex) < savedDeposits.Children.Count)
                                        {
                                            Octavo.Gate.Nabu.Preferences.Preference savedDepositPreference = savedDeposits.Children[savedDepositIndex];

                                            // Now add any saved deposits to the proposed portfolio
                                            Tools.Sales.SCurveOutputRow proposedInvestment = new Tools.Sales.SCurveOutputRow();
                                            proposedInvestment.ID = savedDepositPreference.ID;
                                            proposedInvestment.DepositSize = Convert.ToDecimal(savedDepositPreference.GetChildPreference("DepositAmount").Value);
                                            proposedInvestment.InstitutionID = Convert.ToInt32(savedDepositPreference.GetChildPreference("InstitutionID").Value);
                                            if (scurveSettings.GetChildPreference("AnonymiseDeposits") != null && scurveSettings.GetChildPreference("AnonymiseDeposits").Value != null && scurveSettings.GetChildPreference("AnonymiseDeposits").Value.ToLower().CompareTo("true") == 0)
                                                proposedInvestment.InstitutionName = "Bank " + i.ToString();
                                            else
                                                proposedInvestment.InstitutionName = savedDepositPreference.GetChildPreference("InstitutionName").Value;

                                            if (scurveSettings.GetChildPreference("ShowFitchRating").Value.ToLower().CompareTo("true") == 0)
                                            {
                                                Management.Clients.Helper.InstitutionProperties institutionProperties = new Management.Clients.Helper.InstitutionProperties(proposedInvestment.InstitutionID.Value, ConfigurationManager.AppSettings["preferencesRoot"].ToString() + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerOrganisation"].ToString()) + "\\" + Helper.TextFormatter.RemoveNonAlphaNumericCharacters(Session["_partnerEmailAddress"].ToString()));
                                                Octavo.Gate.Nabu.Preferences.Preference fitchRating = institutionProperties.preferences.GetChildPreference("FitchRating");
                                                if (fitchRating != null && fitchRating.Value != null && fitchRating.Value.Trim().Length > 0)
                                                    proposedInvestment.InstitutionName += " (FIR: " + fitchRating.Value + ")";
                                            }
                                            proposedInvestment.InstitutionShortName = savedDepositPreference.GetChildPreference("InstitutionShortName").Value;

                                            proposedInvestment.heatmapTerm = new Tools.Helper.HeatmapTerm();
                                            proposedInvestment.heatmapTerm.InvestmentTerm = new Management.Clients.Helper.InvestmentTerm();
                                            proposedInvestment.heatmapTerm.InvestmentTerm.ConvertFromSemiColonSeparatedValues(savedDepositPreference.GetChildPreference("InvestmentTerm").Value);
                                            try
                                            {
                                                proposedInvestment.heatmapTerm.MinimumInvestment = Convert.ToDecimal(savedDepositPreference.GetChildPreference("MinimumInvestment").Value);
                                            }
                                            catch
                                            {
                                            }
                                            try
                                            {
                                                proposedInvestment.heatmapTerm.MaximumInvestment = Convert.ToDecimal(savedDepositPreference.GetChildPreference("MaximumInvestment").Value);
                                            }
                                            catch
                                            {
                                            }
                                            proposedInvestment.heatmapTerm.AER50K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER50K").Value);
                                            proposedInvestment.heatmapTerm.AER100K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER100K").Value);
                                            proposedInvestment.heatmapTerm.AER250K = Convert.ToDecimal(savedDepositPreference.GetChildPreference("AER250K").Value);
                                            proposedInvestment.heatmapTerm.InterestPaid = savedDepositPreference.GetChildPreference("InterestPaid").Value;

                                            proposedInvestment.InvestmentTerm = proposedInvestment.heatmapTerm.InvestmentTerm;
                                            proposedInvestment.Rate = proposedInvestment.heatmapTerm.GetBestRate();
                                            proposedInvestment.CalculateAnnualInterest();

                                            proposedPortfolio.Add(proposedInvestment);

                                            savedDepositIndex++;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                            }

                            // sort into the required order
                            // requirement to have a fixed output sort order in liqidity (shortest first) followed by rate (highest first)
                            proposedPortfolio.Sort(delegate (Tools.Sales.SCurveOutputRow x, Tools.Sales.SCurveOutputRow y)
                            {
                                if (x.InvestmentTerm.GetLiquidityDays() == y.InvestmentTerm.GetLiquidityDays())
                                {
                                    if (x.Rate == y.Rate)
                                        return 0;
                                    else if (y.Rate < x.Rate)
                                        return -1;
                                    else
                                        return 1;
                                }
                                else if (x.InvestmentTerm.GetLiquidityDays() < y.InvestmentTerm.GetLiquidityDays())
                                    return -1;
                                else
                                    return 1;
                            });
                            savedDepositIndex = 0;
                            for (int i = 1; i <= 30; i++)
                            {
                                string institutionName = " ";
                                string termDescription = " ";
                                string rate = " ";
                                string deposit = " ";
                                string interest = " ";

                                try
                                {
                                    if (savedDeposits != null && savedDeposits.Children.Count > 0)
                                    {
                                        if (savedDepositIndex < savedDeposits.Children.Count)
                                        {
                                            // Now add any saved deposits to the proposed portfolio
                                            Tools.Sales.SCurveOutputRow proposedInvestment = proposedPortfolio[savedDepositIndex];

                                            institutionName = proposedInvestment.InstitutionName;
                                            termDescription = proposedInvestment.heatmapTerm.InvestmentTerm.GetText();
                                            rate = proposedInvestment.Rate.ToString("0.00") + "%";
                                            deposit = currencyHelper.GetCurrentCurrency().GetCurrencySymbol() + proposedInvestment.DepositSize.ToString("#,##0.00");
                                            interest = currencyHelper.GetCurrentCurrency().GetCurrencySymbol() + proposedInvestment.AnnualInterest.ToString("#,##0.00");
                                            savedDepositIndex++;
                                        }
                                    }
                                }
                                catch
                                {
                                }

                                textReplacements.Add(new KeyValuePair<string, string>("INSTITUTION" + i.ToString("00"), institutionName));
                                textReplacements.Add(new KeyValuePair<string, string>("TERM" + i.ToString("00"), termDescription));
                                textReplacements.Add(new KeyValuePair<string, string>("RATE" + i.ToString("00"), rate));
                                textReplacements.Add(new KeyValuePair<string, string>("DEPOSIT" + i.ToString("00"), deposit));
                                textReplacements.Add(new KeyValuePair<string, string>("INTEREST" + i.ToString("00"), interest));
                            }

                            //actionLog.Settings.Add(new Insignis.Asset.Management.Action.Logging.Entities.ActionSetting("Output-Merge", "TemplateName", templateFile.Name));

                            string requiredOutputNameWithoutExtension = "ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss") + "_CashIllustration";
                            Reports.Helper.ExtendedReportContent extendedReportContent = powerpointRenderAbstraction.MergeDataWithPowerPointTemplate("ICS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + DateTime.Now.ToString("HHmmss"), textReplacements, templateFile.FullName, requiredOutputNameWithoutExtension, true);
                            if (extendedReportContent.ErrorsDetected == false)
                            {
                                //actionLog.Documents.Add(new Insignis.Asset.Management.Action.Logging.Entities.ActionOutputDocument("Public", powerpointRenderAbstraction.PublicFacingOutputFolder + "/" + actionLog.Reference, "", requiredOutputNameWithoutExtension + ".pdf"));
                                //foreach (string otherFile in extendedReportContent.OtherFiles)
                                //{
                                //    if (otherFile.ToLower().StartsWith("http://") || otherFile.ToLower().StartsWith("https://"))
                                //    {
                                //    }
                                //    else
                                //    {
                                //        System.IO.FileInfo ofi = new System.IO.FileInfo(otherFile);
                                //        actionLog.Documents.Add(new Insignis.Asset.Management.Action.Logging.Entities.ActionOutputDocument(ofi.Extension.Replace(".", "").ToUpper(), "", ofi.DirectoryName, ofi.Name));
                                //    }
                                //}
                                //actionLog.ActionStatus = "SUCCESS";

                                Literal someHTML = new Literal();
                                someHTML.Text = "<div><object data=\"" + extendedReportContent.URI + "\" type=\"application/pdf\" width=\"950\" height=\"750\" style=\"margin-left:00px;\">alt : <a href=\"" + extendedReportContent.URI + "\" target=\"_blank\">View Generated Illustration</a></object></div>";
                                someHTML.Text += "<p>";
                                foreach (string otherFile in extendedReportContent.OtherFiles)
                                {
                                    // public facing file
                                    if (otherFile.ToLower().StartsWith("http://") || otherFile.ToLower().StartsWith("https://"))
                                    {
                                        someHTML.Text += "<a href='" + otherFile + "' class='btn' target='_blank'>";
                                        if (otherFile.ToLower().EndsWith(".pdf"))
                                            someHTML.Text += "<i class=\"fa fa-file-pdf-o\" style=\"margin: 0px 0px 0px 0px;\"></i>&nbsp;PDF Download";
                                        else if (otherFile.ToLower().EndsWith(".pptx"))
                                            someHTML.Text += "<i class=\"fa fa-file-powerpoint-o\" style=\"margin: 0px 0px 0px 0px;\"></i>&nbsp;PowerPoint Download";
                                        else
                                            someHTML.Text += "Download";
                                        someHTML.Text += "</a>";
                                    }
                                }
                                someHTML.Text += "</p>";

                                _panelBody.Controls.Add(someHTML);
                            }
                            else
                            {
                                //actionLog.ActionStatus = "FAILED";
                                notifications.Warning("Unable to perform illustration generation.");
                            }
                            //actionLog.ActionCompletedOn = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                            //actionLog.ActionCompletedAt = Convert.ToInt32(DateTime.Now.ToString("HHmmss"));

                            // Update the log
                            //Insignis.Asset.Management.Action.Logging.Abstraction.ActionLoggerAbstraction actionLoggerAbstraction = new Insignis.Asset.Management.Action.Logging.Abstraction.ActionLoggerAbstraction(ConfigurationManager.ConnectionStrings["Octavo.Gate.Nabu.Data.Source.Insignis.Logger"].ConnectionString, Octavo.Gate.Nabu.Entities.DatabaseType.MSSQL, ConfigurationManager.AppSettings["loggerErrorLog"]);
                            //actionLog = actionLoggerAbstraction.InsertActionLog(actionLog);
                            //if (actionLog.ErrorsDetected == false && actionLog.ActionLogID.HasValue)
                            //{
                            //    if (actionLog.Settings != null && actionLog.Settings.Count > 0)
                            //    {
                            //        foreach (Insignis.Asset.Management.Action.Logging.Entities.ActionSetting setting in actionLog.Settings)
                            //            actionLoggerAbstraction.InsertActionSetting(setting, actionLog.ActionLogID.Value);
                            //    }
                            //    if (actionLog.Values != null && actionLog.Values.Count > 0)
                            //    {
                            //        foreach (Insignis.Asset.Management.Action.Logging.Entities.ActionOutputValue value in actionLog.Values)
                            //            actionLoggerAbstraction.InsertActionOutputValue(value, actionLog.ActionLogID.Value);
                            //    }
                            //    if (actionLog.Tables != null && actionLog.Tables.Count > 0)
                            //    {
                            //        foreach (Insignis.Asset.Management.Action.Logging.Entities.ActionOutputTable tableCell in actionLog.Tables)
                            //            actionLoggerAbstraction.InsertActionOutputTable(tableCell, actionLog.ActionLogID.Value);
                            //    }
                            //    if (actionLog.Documents != null && actionLog.Documents.Count > 0)
                            //    {
                            //        foreach (Insignis.Asset.Management.Action.Logging.Entities.ActionOutputDocument document in actionLog.Documents)
                            //            actionLoggerAbstraction.InsertActionOutputDocument(document, actionLog.ActionLogID.Value);
                            //    }
                            //}

                            //actionLoggerAbstraction = null;
                            powerpointRenderAbstraction = null;
                        }
                        else
                            notifications.Error("The selected template does not exist");
                    }
                    else
                        notifications.Error("Invalid or missing query string");
                }
                catch
                {
                    notifications.Error("Error caught!!!");
                }
            }
        }
    }
}