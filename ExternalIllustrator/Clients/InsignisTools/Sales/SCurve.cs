using Insignis.Asset.Management.Tools.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities.Globalisation;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Insignis.Asset.Management.Tools.Sales
{
    public class SCurve
    {
        public Heatmap heatmap = null;
        private Clients.Helper.FSCS.Config fscsProtectionConfig = null;

        public SCurve(BaseAbstraction pBaseAbstraction, Language pLanguage)
        {
            heatmap = new Heatmap(pBaseAbstraction, pLanguage);
        }

        public void LoadHeatmap(bool pIncludeCorporateProducts, string pMoneyFactsFilterConfig, string pCurrencyCode)
        {
            heatmap.Generate(pIncludeCorporateProducts, pMoneyFactsFilterConfig, pCurrencyCode, true);
        }
        public void LoadHeatmap(bool pIncludeCorporateProducts, string pMoneyFactsFilterConfig, string pCurrencyCode, bool pIncludePooledProducts)
        {
            heatmap.Generate(pIncludeCorporateProducts, pMoneyFactsFilterConfig, pCurrencyCode, pIncludePooledProducts);
        }
        public void LoadHeatmap(int pAvailableToHubAccountTypeID, string pCurrencyCode, string pPreferencesRootFolder)
        {
            heatmap.Generate(pAvailableToHubAccountTypeID, pCurrencyCode, pPreferencesRootFolder);
        }

        public SCurveOutput Process(SCurveSettings pSettings, string pFSCSConfig)
        {
            return Process(pSettings, pFSCSConfig, null);
        }
        public SCurveOutput Process(SCurveSettings pSettings, string pFSCSConfig, Octavo.Gate.Nabu.Preferences.Preference pInstitutionInclusion)
        {
            SCurveOutput output = new SCurveOutput();

            // be sure to start from zero days, which normally means instant access
            int fromLiquidityDays = 0;
            decimal leftOverFromPreviousLiquidityBand = 0;
            int index = 0;

            // iteracte through each defined liquidity need
            foreach (KeyValuePair<int, decimal> liquidityNeed in pSettings.LiquidityNeedsDaysAndAmounts)
            {
                if (index < (pSettings.LiquidityNeedsDaysAndAmounts.Count - 1))
                    fromLiquidityDays = pSettings.LiquidityNeedsDaysAndAmounts[++index].Key;
                else
                    fromLiquidityDays = 0;

                // extract the TO days from the need
                int toLiquidityDays = liquidityNeed.Key;
                // extract the total deposit amount within this range of FROM and TO days
                decimal liquidityTotalDepositAmount = (liquidityNeed.Value + leftOverFromPreviousLiquidityBand);

                // now use the heatmap to only return those products which are within the DAYS period and whose Maximum deposit is less than or equal to the maximum we want to invest in any one institution
                List<HeatmapInstitution> potentialProducts = heatmap.GetProductsWithinTerm(fromLiquidityDays, toLiquidityDays, pSettings.MaximumDepositInAnyOneInstitution);

                // now sort that product list into best rate order
                potentialProducts.Sort((x, y) => x.GetBestRate().CompareTo(y.GetBestRate()));
                // now reverse the list so the best rates are at the top
                potentialProducts.Reverse();

                decimal totalInvestedForLiquidityRange = 0;
                foreach (HeatmapInstitution potentialProduct in potentialProducts)
                {
                    bool includeInstitution = true;
                    if (pSettings.ShowFitchRating)
                    {
                        Management.Clients.Helper.InstitutionProperties institutionProperties = new Management.Clients.Helper.InstitutionProperties(potentialProduct.institution.PartyID.Value, ConfigurationManager.AppSettings["preferencesRoot"]);
                        Octavo.Gate.Nabu.Preferences.Preference fitchRating = institutionProperties.preferences.GetChildPreference("FitchRating");
                        if (fitchRating != null && fitchRating.Value != null && fitchRating.Value.Trim().Length > 0)
                        {
                            // if the filter panel has defined a miniumum rating
                            if (pSettings.MinimumFitchRating.CompareTo("All") != 0)
                            {
                                includeInstitution = false;
                                if (Insignis.Asset.Management.Clients.Helper.FitchRatings.IsRatingLessThanOrEqualTo(fitchRating.Value, pSettings.MinimumFitchRating))
                                    includeInstitution = true;
                            }
                        }
                    }

                    if (includeInstitution)
                    {
                        if (pInstitutionInclusion != null)
                        {
                            if (pInstitutionInclusion.GetChildPreference(potentialProduct.institution.PartyID.ToString()) != null && pInstitutionInclusion.GetChildPreference(potentialProduct.institution.PartyID.ToString()).Value != null && pInstitutionInclusion.GetChildPreference(potentialProduct.institution.PartyID.ToString()).Value.ToLower().CompareTo("true") == 0)
                                includeInstitution = true;
                            else
                                includeInstitution = false;
                        }
                    }

                    if (includeInstitution)
                    {
                        HeatmapTerm best = potentialProduct.GetBest();

                        // calculate how much we have already invested in this institution
                        decimal totalInvestedInInstitutionSoFar = 0;
                        foreach (SCurveOutputRow proposedInvestment in output.ProposedInvestments)
                        {
                            if (proposedInvestment.InstitutionID == potentialProduct.institution.PartyID)
                                totalInvestedInInstitutionSoFar += proposedInvestment.DepositSize;
                        }

                        // now calculate how much we is left for us to invest
                        decimal availableToInvestInInstitution = 0;
                        if (pSettings.MaximumDepositInAnyOneInstitution.HasValue)
                            availableToInvestInInstitution = (pSettings.MaximumDepositInAnyOneInstitution.Value - totalInvestedInInstitutionSoFar);

                        if (best.MaximumInvestment.HasValue)
                        {
                            if (best.MaximumInvestment < availableToInvestInInstitution)
                                availableToInvestInInstitution = best.MaximumInvestment.Value;
                        }

                        // if we can invest in the institution
                        if (availableToInvestInInstitution > 0)
                        {
                            if ((totalInvestedForLiquidityRange + availableToInvestInInstitution) <= liquidityTotalDepositAmount)
                            {
                                // we are good
                            }
                            else
                            {
                                // this will take us over our range
                                availableToInvestInInstitution = (liquidityTotalDepositAmount - totalInvestedForLiquidityRange);
                            }
                        }
                        if (availableToInvestInInstitution > 0)
                        {
                            bool makeInvestment = false;
                            if (best.MinimumInvestment.HasValue)
                            {
                                if (availableToInvestInInstitution >= best.MinimumInvestment)
                                    makeInvestment = true;
                            }
                            else
                                makeInvestment = true;

                            if (makeInvestment)
                            {
                                makeInvestment = false;
                                if (best.MaximumInvestment.HasValue)
                                {
                                    if (availableToInvestInInstitution <= best.MaximumInvestment)
                                        makeInvestment = true;
                                }
                                else
                                    makeInvestment = true;

                                // OK so our investment is within max and minimum range for the proposed product so lets proceed
                                if (makeInvestment)
                                {
                                    SCurveOutputRow proposedInvestment = new SCurveOutputRow();
                                    proposedInvestment.DepositSize = availableToInvestInInstitution;
                                    proposedInvestment.InstitutionID = potentialProduct.institution.PartyID;
                                    proposedInvestment.InstitutionName = potentialProduct.institution.Name;
                                    proposedInvestment.InstitutionShortName = potentialProduct.institution.ShortName;
                                    proposedInvestment.InvestmentTerm = best.InvestmentTerm;
                                    proposedInvestment.Rate = best.GetBestRate();
                                    proposedInvestment.CalculateAnnualInterest();

                                    if (best.AER100K.HasValue)
                                        proposedInvestment.AER100K = best.AER100K.Value;
                                    if (best.AER250K.HasValue)
                                        proposedInvestment.AER250K = best.AER250K.Value;
                                    if (best.AER50K.HasValue)
                                        proposedInvestment.AER50K = best.AER50K.Value;
                                    proposedInvestment.InterestPaid = best.InterestPaid;
                                    if (best.MinimumInvestment.HasValue)
                                        proposedInvestment.MinimumInvestment = best.MinimumInvestment.Value;
                                    if (best.MaximumInvestment.HasValue)
                                        proposedInvestment.MaximumInvestment = best.MaximumInvestment.Value;

                                    output.ProposedInvestments.Add(proposedInvestment);

                                    totalInvestedForLiquidityRange += proposedInvestment.DepositSize;
                                }
                            }
                        }

                        if (totalInvestedForLiquidityRange == liquidityTotalDepositAmount)
                            break;
                    }
                }

                try
                {
                    leftOverFromPreviousLiquidityBand = (liquidityTotalDepositAmount - totalInvestedForLiquidityRange);
                }
                catch
                {
                    leftOverFromPreviousLiquidityBand = 0;
                }
                // now move the from pointer on
                //fromLiquidityDays = toLiquidityDays;
            }

            Calculate(output, pSettings);
            //oldCalculate(output, pSettings, pFSCSConfig);

            return output;
        }

        public void Calculate(SCurveOutput pOutput, SCurveSettings pSettings)
        {
            foreach (SCurveOutputRow proposedInvestment in pOutput.ProposedInvestments)
            {
                pOutput.TotalDeposited += proposedInvestment.DepositSize;
                pOutput.AnnualGrossInterestEarned += proposedInvestment.AnnualInterest;
            }

            try
            {
                pOutput.GrossAverageYield = (pOutput.AnnualGrossInterestEarned / pOutput.TotalDeposited) * 100;
            }
            catch
            {
                pOutput.GrossAverageYield = 0;
                pOutput.NetAverageYield = 0;
            }

            pOutput.FSCSPercentProtected = 0;            // percentage protected under FSCS

            // first build a list of all the unique institutions within the portfolio
            List<int> uniqueInstitutionIDs = new List<int>();
            foreach (SCurveOutputRow proposedInvestment in pOutput.ProposedInvestments)
            {
                bool found = false;
                foreach (int loopInstitutionID in uniqueInstitutionIDs)
                {
                    if (loopInstitutionID == proposedInvestment.InstitutionID)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    uniqueInstitutionIDs.Add(proposedInvestment.InstitutionID.Value);
            }

            // now loop through the investments, bank at a time to calculate the total invested for that institution
            decimal sumOfProtectedAmounts = 0;
            foreach (int loopInstitutionID in uniqueInstitutionIDs)
            {
                decimal totalInvestedInInstitution = 0;
                bool isNSAndI = false;
                foreach (SCurveOutputRow proposedInvestment in pOutput.ProposedInvestments)
                {
                    if (loopInstitutionID == proposedInvestment.InstitutionID)
                    {
                        totalInvestedInInstitution += proposedInvestment.DepositSize;

                        if (proposedInvestment.InstitutionShortName.CompareTo("NationalSavingsInvestments") == 0)
                            isNSAndI = true;
                    }
                }

                decimal protectedAmount = 0;
                if (pSettings.ClientType == SCurveClientType.Joint)
                {
                    decimal totalAllowed = 0;
                    if (pSettings.CurrencyCode.CompareTo("GBP")==0)
                         totalAllowed = ((isNSAndI == false) ? Convert.ToDecimal("170000.00") : Convert.ToDecimal("2000000.00"));
                    else if (pSettings.CurrencyCode.CompareTo("EUR") == 0)
                        totalAllowed = Convert.ToDecimal("170000.00");
                    else if (pSettings.CurrencyCode.CompareTo("USD") == 0)
                        totalAllowed = Convert.ToDecimal("200000.00");

                    if (totalInvestedInInstitution <= totalAllowed)
                        protectedAmount = totalInvestedInInstitution;
                    else
                        protectedAmount = totalAllowed;
                }
                else
                {
                    decimal totalAllowed = 0;
                    if (pSettings.CurrencyCode.CompareTo("GBP") == 0)
                        totalAllowed = ((isNSAndI == false) ? Convert.ToDecimal("85000.00") : Convert.ToDecimal("1000000.00"));
                    else if (pSettings.CurrencyCode.CompareTo("EUR") == 0)
                        totalAllowed = Convert.ToDecimal("85000.00");
                    else if (pSettings.CurrencyCode.CompareTo("USD") == 0)
                        totalAllowed = Convert.ToDecimal("100000.00");

                    if (totalInvestedInInstitution <= totalAllowed)
                        protectedAmount = totalInvestedInInstitution;
                    else
                        protectedAmount = totalAllowed;
                }
                sumOfProtectedAmounts += protectedAmount;
            }

            // now figure out the percentage
            try
            {
                if (sumOfProtectedAmounts > 0 && pOutput.TotalDeposited > 0)
                {
                    pOutput.FSCSPercentProtected = ((sumOfProtectedAmounts / pOutput.TotalDeposited) * 100);
                }
            }
            catch
            {
            }
        }

        public void Calculate(SCurveOutput pOutput, SCurveSettings pSettings, string pFSCSConfig)
        {
            fscsProtectionConfig = new Clients.Helper.FSCS.Config(pFSCSConfig);

            foreach (SCurveOutputRow proposedInvestment in pOutput.ProposedInvestments)
            {
                pOutput.TotalDeposited += proposedInvestment.DepositSize;
                pOutput.AnnualGrossInterestEarned += proposedInvestment.AnnualInterest;
            }

            try
            {
                pOutput.GrossAverageYield = (pOutput.AnnualGrossInterestEarned / pOutput.TotalDeposited) * 100;
            }
            catch
            {
                pOutput.GrossAverageYield = 0;
                pOutput.NetAverageYield = 0;
            }

            pOutput.FSCSPercentProtected = 0;            // percentage protected under FSCS
            foreach (SCurveOutputRow proposedInvestment in pOutput.ProposedInvestments)
                fscsProtectionConfig.RegisterInvestment(pSettings.CurrencyCode, proposedInvestment.DepositSize, proposedInvestment.InstitutionShortName, ((pSettings.ClientType == SCurveClientType.Joint) ? true : false));
            pOutput.FSCSAmountProtected = fscsProtectionConfig.CalculateAmountProtected(pSettings.CurrencyCode, ((pSettings.ClientType == SCurveClientType.Joint) ? true : false));
            pOutput.FSCSPercentProtected = fscsProtectionConfig.CalculatePercentageProtected(pSettings.CurrencyCode, ((pSettings.ClientType == SCurveClientType.Joint) ? true : false));
            //if (output.FSCSAmountProtected > 0 && output.TotalDeposited > 0)
            //{
            //    output.FSCSPercentProtected = ((output.FSCSAmountProtected / output.TotalDeposited) * 100);
            //}
        }

        public List<Clients.Helper.InvestmentTerm> ListAllTerms()
        {
            return heatmap.ListAllTerms();
        }
    }
}
