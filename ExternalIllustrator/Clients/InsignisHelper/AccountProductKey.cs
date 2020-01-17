using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class AccountProductKey
    {
        public int? InstitutionID = null;
        public string PartCode = null;
        public decimal? Rate = null;
        public string Rating = null;
        public bool? PaperFreeOption = null;
        public InvestmentTerm investmentTerm = null;
        public DateTime? investedOn = null;
        public bool? IsPooledProduct = null;

        // interest calculation variables
        public decimal? InterestAccruedRate = null;
        public int? InterestCalculatedDayWithinPeriod = null;
        public string InterestFrequencyOfCalculation = null;
        public bool? InterestIsCompounded = null;
        public string InterestRegime = null;
        public string InterestClientViewConfig = null;

        // top ups [Instant and Notice only]
        public bool AccountPermitsTopUps = true;

        public AccountProductKey()
        {
        }

        public AccountProductKey(string pPSV)
        {
            ConvertFromPSV(pPSV);
        }

        public string ConvertToPSV()
        {
            string psv = "";
            try
            {
                if (InstitutionID.HasValue)
                    psv += InstitutionID.Value.ToString();
                psv += "|";
                if (PartCode != null)
                    psv += PartCode;
                psv += "|";
                if (Rate.HasValue)
                    psv += Rate.Value.ToString();
                psv += "|";
                if (Rating != null)
                    psv += Rating;
                psv += "|";
                if (PaperFreeOption.HasValue)
                    psv += ((PaperFreeOption.Value == true) ? "true" : "false");
                psv += "|";
                if (investmentTerm != null)
                    psv += investmentTerm.ConvertToSemiColonSeparatedValues();
                psv += "|";
                if (investedOn.HasValue)
                    psv += investedOn.Value.ToString("dd-MMM-yyyy HH:mm:ss");
                psv += "|";
                if (IsPooledProduct.HasValue)
                    psv += ((IsPooledProduct.Value == true) ? "true" : "false");

                // New interest related properties
                psv += "|";
                if (InterestAccruedRate.HasValue)
                    psv += InterestAccruedRate.ToString();
                psv += "|";
                if (InterestCalculatedDayWithinPeriod.HasValue)
                    psv += InterestCalculatedDayWithinPeriod.ToString();
                psv += "|";
                if (InterestFrequencyOfCalculation != null)
                    psv += InterestFrequencyOfCalculation;
                psv += "|";
                if(InterestIsCompounded != null)
                    psv += ((InterestIsCompounded.Value == true) ? "true" : "false");
                psv += "|";
                if (InterestRegime != null)
                    psv += InterestRegime;
                psv += "|";
                if (InterestClientViewConfig != null)
                    psv += InterestClientViewConfig;
                psv += "|";
                if (investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount || investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                    psv += ((AccountPermitsTopUps == true) ? "true" : "false");
                else
                    psv += "false";
            }
            catch
            {
            }
            return psv;
        }

        public void ConvertFromPSV(string pPSV)
        {
            try
            {
                if(pPSV != null && pPSV.Trim().Length > 0 && pPSV.Contains("|"))
                {
                    string separator = "|";
                    string[] parts = pPSV.Split(separator.ToCharArray());

                    if (parts[0].Length > 0)
                    {
                        try
                        {
                            InstitutionID = Convert.ToInt32(parts[0]);
                        }
                        catch
                        {
                        }
                    }
                    if(parts[1].Length > 0)
                        PartCode = parts[1];
                    if (parts[2].Length > 0)
                    {
                        try
                        {
                            Rate = Convert.ToDecimal(parts[2]);
                        }
                        catch
                        {
                        }
                    }
                    if(parts[3].Length > 0)
                        Rating = parts[3];
                    if (parts[4].Length > 0)
                        PaperFreeOption = ((parts[4].CompareTo("true") == 0) ? true : false);
                    if (parts[5].Length > 0)
                    {
                        investmentTerm = new InvestmentTerm();
                        investmentTerm.ConvertFromSemiColonSeparatedValues(parts[5]);
                    }
                    if (parts[6].Length > 0)
                    {
                        try
                        {
                            investedOn = DateTime.ParseExact(parts[6], "dd-MMM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }
                    if(parts.Length > 7)
                    {
                        if (parts[7].Length > 0)
                            IsPooledProduct = ((parts[7].CompareTo("true") == 0) ? true : false);

                        if (parts.Length > 8)
                        {
                            // interest calculation variables
                            if (parts[8].Length > 0)
                                InterestAccruedRate = Convert.ToDecimal(parts[8]);
                            if (parts[9].Length > 0)
                                InterestCalculatedDayWithinPeriod = Convert.ToInt32(parts[9]);
                            if (parts[10].Length > 0)
                                InterestFrequencyOfCalculation = parts[10];
                            if (parts[11].Length > 0)
                                InterestIsCompounded = ((parts[11].CompareTo("true") == 0) ? true : false);
                            if (parts[12].Length > 0)
                                InterestRegime = parts[12];
                            if (parts[13].Length > 0)
                                InterestClientViewConfig = parts[13];
                            if (parts.Length > 13)
                            {
                                if (investmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount || investmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                                    AccountPermitsTopUps = ((parts[14].CompareTo("true") == 0) ? true : false);
                                else
                                    AccountPermitsTopUps = false;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}