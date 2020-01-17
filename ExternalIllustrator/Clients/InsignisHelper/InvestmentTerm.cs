using Octavo.Gate.Nabu.Entities.Operations;
using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class InvestmentTerm
    {
        public DateTime? TermEnds = null;
        public int? TermDays = null;
        public string TermText = "";
        public int? TermAge = null;

        public int? NoticeDays = null;
        public string NoticeText = "";

        public InvestmentAccountType investmentAccountType = InvestmentAccountType.Unspecified;

        public InvestmentTerm()
        {
        }

        public InvestmentTerm(Part pProduct)
        {
            string _noticeText = "";
            string _noticeDays = "";
            string _termText = "";
            string _termDays = "";

            foreach (PartFeature feature in pProduct.PartFeatures)
            {
                if (feature.ErrorsDetected == false)
                {
                    if (feature.PartFeatureType.Detail.Alias.CompareTo("TermDays") == 0)
                        _termDays = feature.Value;
                    else if (feature.PartFeatureType.Detail.Alias.CompareTo("Term") == 0)
                        _termText = feature.Value;
                    else if (feature.PartFeatureType.Detail.Alias.CompareTo("NoticeDays") == 0)
                        _noticeDays = feature.Value;
                    else if (feature.PartFeatureType.Detail.Alias.CompareTo("Notice") == 0)
                        _noticeText = feature.Value;
                }
            }

            if (pProduct.PartType.Detail.Alias.ToUpper().CompareTo("FIXED") == 0 || pProduct.PartType.Detail.Alias.ToUpper().CompareTo("VARIABLE") == 0)
            {
                if (_noticeText.CompareTo("Instant") == 0 || _noticeText.CompareTo("None") == 0)
                    investmentAccountType = InvestmentAccountType.InstantAccessAccount;
                else
                {
                    if ((_noticeText.CompareTo("-") == 0 || _noticeText.Trim().Length == 0) && _termText.Length > 0 && _termText.CompareTo("-") != 0)
                    {
                        investmentAccountType = InvestmentAccountType.TermAccount;

                        if (_termText.StartsWith("Age"))
                        {
                            // specific age
                            try
                            {
                                TermAge = Convert.ToInt32(_termText.Substring(3).Trim());
                            }
                            catch
                            {
                            }
                        }
                        else if(_termText.Length == 8 && _termText.Contains("."))
                        {
                            // specific end date
                            try
                            {
                                TermEnds = DateTime.ParseExact(_termText, "yy.MM.dd", System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            TermText = _termText;
                            if (_termDays.Length > 0 && _termDays.CompareTo("-") != 0)
                            {
                                try
                                {
                                    TermDays = Convert.ToInt32(_termDays);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                    else
                    {
                        NoticeText = _noticeText;
                        investmentAccountType = InvestmentAccountType.NoticeAccount;
                        if (_noticeDays.Length > 0 && _noticeDays.CompareTo("-") != 0)
                        {
                            try
                            {
                                NoticeDays = Convert.ToInt32(_noticeDays);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            else
            {
                // for now, we ignore anything which is not Fixed or Variable in the part type column
            }
        }

        public string GetClientReviewReturnDate(DateTime pAccountOpened, DateTime? pAdditionalDateDetails)
        {
            string returnDate = "";

            if (investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                returnDate = "n/a";
            else if (investmentAccountType == InvestmentAccountType.NoticeAccount)
            {
                if (pAdditionalDateDetails.HasValue)
                {
                    if (NoticeDays.HasValue)
                        returnDate = pAdditionalDateDetails.Value.AddDays(NoticeDays.Value).ToString("dd/MM/yyyy");
                }
                else
                {
                    returnDate = "n/a";
                }
            }
            else if (investmentAccountType == InvestmentAccountType.TermAccount)
            {
                if (pAdditionalDateDetails.HasValue)
                {
                    DateTime? expiresOn = GetTermExpiryDate(pAccountOpened, pAdditionalDateDetails);
                    if (expiresOn.HasValue)
                    {
                        returnDate = expiresOn.Value.ToString("dd/MM/yyyy");
                    }
                }
            }
            return returnDate;
        }

        public int? GetLiquidityDays()
        {
            int? liquidityDays = null;

            if (investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                liquidityDays = 0;
            else if (investmentAccountType == InvestmentAccountType.TermAccount)
            {
                if (TermDays.HasValue)
                    liquidityDays = TermDays;
            }
            else if (investmentAccountType == InvestmentAccountType.NoticeAccount)
                liquidityDays = NoticeDays;
            return liquidityDays;
        }

        public DateTime? GetTermExpiryDate(DateTime? pDateOpened, DateTime? pDateInvested)
        {
            DateTime? expiresOn = null;
            if (investmentAccountType == InvestmentAccountType.TermAccount)
            {
                if (TermEnds.HasValue)
                    expiresOn = TermEnds;
                else
                {
                    if (TermDays.HasValue)
                    {
                        if (pDateOpened.HasValue)
                        {
                            expiresOn = pDateOpened.Value.AddDays(TermDays.Value);
                        }
                        else
                        {
                            if (pDateInvested.HasValue)
                            {
                                expiresOn = pDateInvested.Value.AddDays(TermDays.Value);
                            }
                        }
                    }
                    else
                    {
                        // Term Age
                    }
                }
            }
            return expiresOn;
        }

        public string GetText()
        {
            string text = "n/a";
            try
            {
                if (investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                {
                    text = "Instant Access";
                }
                else if (investmentAccountType == InvestmentAccountType.TermAccount)
                {
                    text = TermText + " (Term)";
                }
                else if (investmentAccountType == InvestmentAccountType.NoticeAccount)
                {
                    text = NoticeText + " (Notice)";
                }
            }
            catch
            {
            }
            return text;
        }

        public string GetColorAlias()
        {
            string colourAlias = "unspecified";

            if (investmentAccountType == InvestmentAccountType.InstantAccessAccount)
            {
                colourAlias = "instantAccessAccount";
            }
            else if (investmentAccountType == InvestmentAccountType.TermAccount)
            {
                if (TermDays.HasValue)
                {
                    if (TermDays.Value > (365 * 2))         // two years
                        colourAlias = "twoOrMoreYears";
                    else if (TermDays.Value > (365 * 1.5))  // 18 to 24 months
                        colourAlias = "eighteenTotwentyfourMonths";
                    else if (TermDays.Value > 365)          // 12 to 17 months
                        colourAlias = "twelveToSeventeenMonths";
                    else if (TermDays.Value > 182)          // 6 to 12 months
                        colourAlias = "sixToTwelveMonths";
                    else if (TermDays.Value > 91)           // 3 to 6 months
                        colourAlias = "threeToSixMonths";
                    else if (TermDays.Value > 30)           // > 1 months
                        colourAlias = "oneToThreeMonths";
                    else if (TermDays.Value > 1)           // > 1 days
                        colourAlias = "upToOneMonth";
                    else
                        colourAlias = "oneDay";
                }
                else
                {
                    if (TermEnds.HasValue)
                    {
                        if (DateTime.Now.CompareTo(TermEnds.Value) <= 0)
                        {
                            int daysUntilTermExpires = TermEnds.Value.Subtract(DateTime.Now).Days;
                            if (daysUntilTermExpires > (365 * 2))         // two years
                                colourAlias = "twoOrMoreYears";
                            else if (daysUntilTermExpires > (365 * 1.5))  // 18 to 24 months
                                colourAlias = "eighteenTotwentyfourMonths";
                            else if (daysUntilTermExpires > 365)          // 12 to 17 months
                                colourAlias = "twelveToSeventeenMonths";
                            else if (daysUntilTermExpires > 182)          // 6 to 12 months
                                colourAlias = "sixToTwelveMonths";
                            else if (daysUntilTermExpires > 91)           // 3 to 6 months
                                colourAlias = "threeToSixMonths";
                            else if (daysUntilTermExpires > 30)           // > 1 months
                                colourAlias = "oneToThreeMonths";
                            else if (daysUntilTermExpires > 1)           // > 1 days
                                colourAlias = "upToOneMonth";
                            else
                                colourAlias = "oneDay";
                        }
                    }
                    else if (TermAge.HasValue)
                    {
                        colourAlias = "noDaysSpecified";
                    }
                }
            }
            else if (investmentAccountType == InvestmentAccountType.NoticeAccount)
            {
                if (NoticeDays.HasValue)
                {
                    if (NoticeDays.Value > (365 * 2))         // two years
                        colourAlias = "twoOrMoreYears";
                    else if (NoticeDays.Value > (365 * 1.5))  // 18 to 24 months
                        colourAlias = "eighteenTotwentyfourMonths";
                    else if (NoticeDays.Value > 365)          // 12 to 17 months
                        colourAlias = "twelveToSeventeenMonths";
                    else if (NoticeDays.Value > 182)          // 6 to 12 months
                        colourAlias = "sixToTwelveMonths";
                    else if (NoticeDays.Value > 91)           // 3 to 6 months
                        colourAlias = "threeToSixMonths";
                    else if (NoticeDays.Value > 30)           // > 1 months
                        colourAlias = "oneToThreeMonths";
                    else if (NoticeDays.Value > 1)           // > 1 days
                        colourAlias = "upToOneMonth";
                    else
                        colourAlias = "oneDay";
                }
            }
            return colourAlias;
        }

        public string GetColor()
        {
            string colour = "red";

            if (investmentAccountType == InvestmentAccountType.InstantAccessAccount)
            {
                colour = "#0065B8";
            }
            else if (investmentAccountType == InvestmentAccountType.TermAccount)
            {
                if (TermDays.HasValue)
                {
                    if (TermDays.Value > (365 * 2))         // two years
                        colour = "#3c586d";
                    else if (TermDays.Value > (365 * 1.5))  // 18 to 24 months
                        colour = "#49758e";
                    else if (TermDays.Value > 365)          // 12 to 17 months
                        colour = "#548eab";
                    else if (TermDays.Value > 182)          // 6 to 12 months
                        colour = "#5da4c4";
                    else if (TermDays.Value > 91)           // 3 to 6 months
                        colour = "#65b5d8";
                    else if (TermDays.Value > 30)           // > 1 months
                        colour = "#6ac3e8";
                    else if (TermDays.Value > 1)           // > 1 months
                        colour = "#6fcef5";
                    else
                        colour = "#72d4fc";
                }
                else
                {
                    if (TermEnds.HasValue)
                    {
                        if (DateTime.Now.CompareTo(TermEnds.Value) <= 0)
                        {
                            int daysUntilTermExpires = TermEnds.Value.Subtract(DateTime.Now).Days;
                            if (daysUntilTermExpires > (365 * 2))         // two years
                                colour = "#3c586d";
                            else if (daysUntilTermExpires > (365 * 1.5))  // 18 to 24 months
                                colour = "#49758e";
                            else if (daysUntilTermExpires > 365)          // 12 to 17 months
                                colour = "#548eab";
                            else if (daysUntilTermExpires > 182)          // 6 to 12 months
                                colour = "#5da4c4";
                            else if (daysUntilTermExpires > 91)           // 3 to 6 months
                                colour = "#65b5d8";
                            else if (daysUntilTermExpires > 30)           // > 1 months
                                colour = "#6ac3e8";
                            else if (daysUntilTermExpires > 1)           // > 1 months
                                colour = "#6fcef5";
                            else
                                colour = "#72d4fc";
                        }
                    }
                    else if (TermAge.HasValue)
                    {
                        colour = "yellow";
                    }
                }
            }
            else if (investmentAccountType == InvestmentAccountType.NoticeAccount)
            {
                if (NoticeDays.HasValue)
                {
                    if (NoticeDays.Value > (365 * 2))         // two years
                        colour = "#3c586d";
                    else if (NoticeDays.Value > (365 * 1.5))  // 18 to 24 months
                        colour = "#49758e";
                    else if (NoticeDays.Value > 365)          // 12 to 17 months
                        colour = "#548eab";
                    else if (NoticeDays.Value > 182)          // 6 to 12 months
                        colour = "#5da4c4";
                    else if (NoticeDays.Value > 91)           // 3 to 6 months
                        colour = "#65b5d8";
                    else if (NoticeDays.Value > 30)           // > 1 months
                        colour = "#6ac3e8";
                    else if (NoticeDays.Value > 1)           // > 1 months
                        colour = "#6fcef5";
                    else
                        colour = "#72d4fc";
                }
            }
            return colour;
        }

        public string ConvertToSemiColonSeparatedValues()
        {
            string csv = "";

            if(investmentAccountType != InvestmentAccountType.Unspecified)
            {
                if(investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                    csv += "Instant";
                else if(investmentAccountType == InvestmentAccountType.NoticeAccount)
                    csv += "Notice";
                else if(investmentAccountType == InvestmentAccountType.TermAccount)
                    csv += "Term";
                csv += ";";

                if(TermEnds.HasValue)
                    csv += TermEnds.Value.ToString("dd-MMM-yyyy HH:mm:ss");
                csv += ";";

                if(TermDays.HasValue)
                    csv += TermDays.Value.ToString();
                csv += ";";

                if(TermText != null && TermText.Trim().Length > 0)
                    csv += TermText;
                csv += ";";

                if(TermAge.HasValue)
                    csv += TermAge.ToString();
                csv += ";";

                if(NoticeDays.HasValue)
                    csv += NoticeDays.Value.ToString();
                csv += ";";

                if (NoticeText != null && NoticeText.Trim().Length > 0)
                    csv += NoticeText;
            }
            return csv;
        }

        public void ConvertFromSemiColonSeparatedValues(string pCSV)
        {
            if (pCSV != null && pCSV.Trim().Length > 0)
            {
                try
                {
                    if (pCSV.Contains(";"))
                    {
                        string separator = ";";
                        string[] parts = pCSV.Split(separator.ToCharArray());
                        if (parts.Length == 7)
                        {
                            if (parts[0].Length > 0)
                            {
                                if (parts[0].CompareTo("Instant") == 0)
                                    investmentAccountType = InvestmentAccountType.InstantAccessAccount;
                                else if (parts[0].CompareTo("Notice") == 0)
                                    investmentAccountType = InvestmentAccountType.NoticeAccount;
                                else if (parts[0].CompareTo("Term") == 0)
                                    investmentAccountType = InvestmentAccountType.TermAccount;
                                else
                                    investmentAccountType = InvestmentAccountType.Unspecified;
                            }

                            if (parts[1].Length > 0)
                            {
                                try
                                {
                                    TermEnds = DateTime.ParseExact(parts[1],"dd-MMM-yyyy HH:mm:ss",System.Globalization.CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                }
                            }

                            if (parts[2].Length > 0)
                            {
                                try
                                {
                                    TermDays = Convert.ToInt32(parts[2]);
                                }
                                catch
                                {
                                }
                            }

                            if (parts[3].Length > 0)
                                TermText = parts[3];

                            if (parts[4].Length > 0)
                            {
                                try
                                {
                                    TermAge = Convert.ToInt32(parts[4]);
                                }
                                catch
                                {
                                }
                            }

                            if (parts[5].Length > 0)
                            {
                                try
                                {
                                    NoticeDays = Convert.ToInt32(parts[5]);
                                }
                                catch
                                {
                                }
                            }

                            if (parts[6].Length > 0)
                                NoticeText = parts[6];
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }

    public enum InvestmentAccountType
    {
        TermAccount,
        NoticeAccount,
        InstantAccessAccount,
        Unspecified
    }
}