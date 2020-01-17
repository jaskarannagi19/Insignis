using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Operations;
using System;
using System.Web.UI.WebControls;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI.Controls
{
    public class RateGuage
    {
        private Part product = null;

        public RateGuage()
        {
        }

        public RateGuage(string pPartCode, OperationsAbstraction pOperationsAbstraction, int pLanguageID)
        {
            product = pOperationsAbstraction.GetPartByCode(pPartCode, pLanguageID);
            if (product.ErrorsDetected == false && product.PartID.HasValue)
                product.PartFeatures = pOperationsAbstraction.ListPartFeatures((int)product.PartID, pLanguageID);
        }

        public Literal Render(int pAccountID)
        {
            Literal html = new Literal();
            html.Text = "";

            html.Text += "<div style=\"position: relative; width: 185px; height: 50px; line-height: normal; margin: auto;\">";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 50px; font-size: 10px;\">50K</div>";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 100px; font-size: 10px;\">100K</div>";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 150px; font-size: 10px;\">250K</div>";
            html.Text += "<div style=\"position: absolute; top: 15px; left: 5px; width: 170px; height: 15px; border: 1px solid black;\">";
            html.Text += "<div style=\"position: absolute: top: 1px; left: 1px; height: 15px; background-color: rgb(255, 166, 77); width:0px;\" id=\"_panelColourFill" + pAccountID.ToString() + "\"></div>";
            html.Text += "</div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 50px; font-size:10px;\">" + RenderRate("AERGBP50K") + "</div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 100px; font-size:10px;\">" + RenderRate("AERGBP100K") + "</div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 150px; font-size:10px;\">" + RenderRate("AERGBP250K") + "</div>";
            html.Text += "</div>";

            return html;
        }

        public Literal Render(string pAlias)
        {
            Literal html = new Literal();
            html.Text = "";

            html.Text += "<div style=\"position: relative; width: 185px; height: 50px; line-height: normal; margin: auto;\">";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 50px; font-size: 10px;\">50K</div>";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 100px; font-size: 10px;\">100K</div>";
            html.Text += "<div style=\"position: absolute; top: 1px; left: 150px; font-size: 10px;\">250K</div>";
            html.Text += "<div style=\"position: absolute; top: 15px; left: 5px; width: 170px; height: 15px; border: 1px solid black;\">";
            html.Text += "<div style=\"position: absolute: top: 1px; left: 1px; height: 15px; background-color: rgb(255, 166, 77); width:0px;\" id=\"_panel" + pAlias + "ColourFill\"></div>";
            html.Text += "</div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 50px; font-size:10px;\" id=\"_" + pAlias + "RateGuageAER50K\"></div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 100px; font-size:10px;\" id=\"_" + pAlias + "RateGuageAER100K\"></div>";
            html.Text += "<div style=\"position: absolute; top: 36px; left: 150px; font-size:10px;\" id=\"_" + pAlias + "RateGuageAER250K\"></div>";
            html.Text += "</div>";

            return html;
        }

        private string RenderRate(string pFeatureAlias)
        {
            decimal rate = 0;
            try
            {
                foreach (PartFeature feature in product.PartFeatures)
                {
                    if (feature.ErrorsDetected == false)
                    {
                        if (feature.PartFeatureType.Detail.Alias.CompareTo(pFeatureAlias) == 0)
                            rate = Convert.ToDecimal(feature.Value);
                    }
                }
            }
            catch
            {
            }
            return rate.ToString("0.00") + "%";
        }

        public decimal GetRateForInvestment(decimal pInvestmentAmount)
        {
            decimal rate = 0;
            try
            {
                foreach (PartFeature feature in product.PartFeatures)
                {
                    if (feature.ErrorsDetected == false)
                    {
                        if (feature.PartFeatureType.Detail.Alias.StartsWith("AERGBP"))
                        {
                            if (feature.PartFeatureType.Detail.Alias.EndsWith("50K"))
                            {
                                if (pInvestmentAmount <= 100000)
                                {
                                    rate = Convert.ToDecimal(feature.Value);
                                    break;
                                }
                            }
                            else if (feature.PartFeatureType.Detail.Alias.EndsWith("100K"))
                            {
                                if (pInvestmentAmount <= 250000)
                                {
                                    rate = Convert.ToDecimal(feature.Value);
                                    break;
                                }
                            }
                            else if (feature.PartFeatureType.Detail.Alias.EndsWith("250K"))
                            {
                                rate = Convert.ToDecimal(feature.Value);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return rate;
        }
    }
}