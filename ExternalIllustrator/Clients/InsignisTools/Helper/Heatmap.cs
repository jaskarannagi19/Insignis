using Insignis.Asset.Management.Clients.Helper;
using Octavo.Gate.Nabu.Abstraction;
using Octavo.Gate.Nabu.Entities;
using Octavo.Gate.Nabu.Entities.Financial;
using Octavo.Gate.Nabu.Entities.Globalisation;
using Octavo.Gate.Nabu.Entities.Operations;
using Octavo.Gate.Nabu.Entities.Publicity;
using System;
using System.Collections.Generic;
using System.IO;

namespace Insignis.Asset.Management.Tools.Helper
{
    public class Heatmap : BaseType
    {
        public List<HeatmapInstitution> heatmapInstitutions = new List<HeatmapInstitution>();

        private FinancialAbstraction financialAbstraction = null;
        private OperationsAbstraction operationsAbstraction = null;
        private PublicityAbstraction publicityAbstraction = null;
        private Language language = null;

        public Heatmap(BaseAbstraction pBaseAbstraction, Language pLanguage)
        {
            financialAbstraction = new FinancialAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
            operationsAbstraction = new OperationsAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
            publicityAbstraction = new PublicityAbstraction(pBaseAbstraction.ConnectionString, pBaseAbstraction.DBType, pBaseAbstraction.ErrorLogFile);
            language = pLanguage;
        }

        public void Generate(bool pIncludeCorporateProducts, string pMoneyFactsFilterConfig, string pCurrencyCode, bool pIncludePooledProducts)
        {
            this.ErrorsDetected = false;
            this.StackTrace = "";
            this.ErrorDetails.Clear();

            try
            {
                Institution[] institutions = financialAbstraction.ListInstitutions((int)language.LanguageID);
                if (File.Exists(pMoneyFactsFilterConfig))
                {
                    Console.Apps.Data.Loader.Config.DataLoaderConfig dataLoaderConfig = new Console.Apps.Data.Loader.Config.DataLoaderConfig(pMoneyFactsFilterConfig);
                    if (dataLoaderConfig.StagedFilter != null)
                    {
                        foreach (Console.Apps.Data.Loader.Config.FieldDefinition fieldDefinition in dataLoaderConfig.StagedFilter.FieldDefinitions)
                        {
                            if (fieldDefinition.Name.CompareTo("Institution") == 0)
                            {
                                foreach (Institution institution in institutions)
                                {
                                    if (institution.ErrorsDetected == false)
                                    {
                                        string tempInstitutionName = institution.Name;
                                        if (tempInstitutionName.Contains("&amp;"))
                                            tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");

                                        if (fieldDefinition.WithinInClause("'" + tempInstitutionName + "'"))
                                        {
                                            institution.Name = tempInstitutionName;
                                            heatmapInstitutions.Add(new HeatmapInstitution(institution));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (HeatmapInstitution heatmapInstitution in heatmapInstitutions)
                    {
                        if (heatmapInstitution.institution.ErrorsDetected == false)
                        {
                            ProductLine[] productLines = publicityAbstraction.ListProductLinesByOrganisation((int)heatmapInstitution.institution.PartyID, (int)language.LanguageID);
                            if (productLines.Length > 0)
                            {
                                foreach (ProductLine productLine in productLines)
                                {
                                    if (productLine.ErrorsDetected == false)
                                    {
                                        bool showProductLine = false;
                                        if (pIncludeCorporateProducts == true && productLine.Detail.Alias.CompareTo("CORPORATEPRODUCTS" + pCurrencyCode) == 0)
                                            showProductLine = true;
                                        else if (pIncludeCorporateProducts == false && productLine.Detail.Alias.CompareTo("PRODUCTS" + pCurrencyCode) == 0)
                                            showProductLine = true;
                                        if (pIncludePooledProducts == true && productLine.Detail.Alias.CompareTo("POOLEDPRODUCTS" + pCurrencyCode) == 0)
                                            showProductLine = true;
                                        if (showProductLine)
                                        {
                                            Part[] products = operationsAbstraction.ListPartByProductLine((int)productLine.ProductLineID, (int)language.LanguageID);
                                            if (products != null && products.Length > 0)
                                            {
                                                foreach (Part product in products)
                                                {
                                                    if (product.ErrorsDetected == false)
                                                    {
                                                        product.PartFeatures = operationsAbstraction.ListPartFeatures((int)product.PartID, (int)language.LanguageID);

                                                        HeatmapTerm heatmapTerm = new HeatmapTerm();
                                                        heatmapTerm.InvestmentTerm = new InvestmentTerm(product);

                                                        if (heatmapTerm.InvestmentTerm.investmentAccountType != InvestmentAccountType.Unspecified)
                                                        {
                                                            foreach (PartFeature partFeature in product.PartFeatures)
                                                            {
                                                                if (partFeature.ErrorsDetected == false)
                                                                {
                                                                    if (partFeature.PartFeatureType.Detail.Alias.StartsWith("AER"))
                                                                    {
                                                                        try
                                                                        {
                                                                            if (partFeature.PartFeatureType.Detail.Alias.EndsWith("250K"))
                                                                                heatmapTerm.AER250K = Convert.ToDecimal(partFeature.Value);
                                                                            else if (partFeature.PartFeatureType.Detail.Alias.EndsWith("100K"))
                                                                                heatmapTerm.AER100K = Convert.ToDecimal(partFeature.Value);
                                                                            else if (partFeature.PartFeatureType.Detail.Alias.EndsWith("50K"))
                                                                                heatmapTerm.AER50K = Convert.ToDecimal(partFeature.Value);
                                                                        }
                                                                        catch
                                                                        {
                                                                        }
                                                                    }
                                                                    else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("MinimumInvestment") == 0)
                                                                    {
                                                                        try
                                                                        {
                                                                            heatmapTerm.MinimumInvestment = Convert.ToDecimal(partFeature.Value);
                                                                        }
                                                                        catch
                                                                        {
                                                                        }
                                                                    }
                                                                    else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("MaximumInvestment") == 0)
                                                                    {
                                                                        try
                                                                        {
                                                                            heatmapTerm.MaximumInvestment = Convert.ToDecimal(partFeature.Value);
                                                                        }
                                                                        catch
                                                                        {
                                                                        }
                                                                    }
                                                                    else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("InterestPaid") == 0)
                                                                    {
                                                                        heatmapTerm.InterestPaid = partFeature.Value;
                                                                    }
                                                                }
                                                                else
                                                                    break;
                                                            }
                                                            heatmapInstitution.investmentTerms.Add(heatmapTerm);
                                                        }
                                                    }
                                                    else
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
            }
            catch (Exception exc)
            {
                this.ErrorsDetected = true;
                this.StackTrace = exc.StackTrace;
                this.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
            }
        }

        public void Generate(int pAvailableToHubAccountTypeID, string pCurrencyCode, string pPreferencesRootFolder)
        {
            this.ErrorsDetected = false;
            this.StackTrace = "";
            this.ErrorDetails.Clear();

            try
            {
                Institution[] institutions = financialAbstraction.ListInstitutions((int)language.LanguageID);
                foreach (Institution institution in institutions)
                {
                    if (institution.ErrorsDetected == false)
                    {
                        if (Insignis.Asset.Management.Clients.Helper.HideInstitutionFromClient.IsHidden(institution.PartyID.Value, System.Configuration.ConfigurationManager.AppSettings["preferencesRoot"]) == false)
                        {
                            string tempInstitutionName = institution.Name;
                            if (tempInstitutionName.Contains("&amp;"))
                                tempInstitutionName = tempInstitutionName.Replace("&amp;", "&");
                            institution.Name = tempInstitutionName;
                            heatmapInstitutions.Add(new HeatmapInstitution(institution));
                        }
                    }
                }
                institutions = null;

                foreach (HeatmapInstitution heatmapInstitution in heatmapInstitutions)
                {
                    if (heatmapInstitution.institution.ErrorsDetected == false)
                    {
                        ProductLine[] productLines = publicityAbstraction.ListProductLinesByOrganisation((int)heatmapInstitution.institution.PartyID, (int)language.LanguageID);
                        if (productLines.Length > 0)
                        {
                            foreach (ProductLine productLine in productLines)
                            {
                                if (productLine.ErrorsDetected == false)
                                {
                                    if (productLine.Detail.Alias.StartsWith("DIRECTPRODUCTS") || productLine.Detail.Alias.StartsWith("POOLED"))
                                    {
                                        if (productLine.Detail.Alias.EndsWith(pCurrencyCode))
                                        {
                                            Part[] products = operationsAbstraction.ListPartByProductLine((int)productLine.ProductLineID, (int)language.LanguageID);
                                            if (products != null && products.Length > 0)
                                            {
                                                foreach (Part product in products)
                                                {
                                                    if (product.ErrorsDetected == false)
                                                    {
                                                        product.PartFeatures = operationsAbstraction.ListPartFeatures((int)product.PartID, (int)language.LanguageID);

                                                        bool includeProduct = false;
                                                        PartFeature availableToFeature = product.GetPartFeatureByAlias("AvailableTo");
                                                        if (availableToFeature.ErrorsDetected == false && availableToFeature.Value.Trim().Length > 0)
                                                        {
                                                            string pipeSeparator = "|";
                                                            string[] availableToAccountTypes = availableToFeature.Value.Split(pipeSeparator.ToCharArray());
                                                            foreach (string availableToAccountType in availableToAccountTypes)
                                                            {
                                                                if (availableToAccountType.Trim().Length > 0)
                                                                {
                                                                    if (availableToAccountType.CompareTo(pAvailableToHubAccountTypeID.ToString()) == 0)
                                                                    {
                                                                        includeProduct = true;
                                                                        break;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (includeProduct)
                                                        {

                                                            HeatmapTerm heatmapTerm = new HeatmapTerm();
                                                            heatmapTerm.InvestmentTerm = new InvestmentTerm(product);

                                                            if (heatmapTerm.InvestmentTerm.investmentAccountType != InvestmentAccountType.Unspecified)
                                                            {
                                                                foreach (PartFeature partFeature in product.PartFeatures)
                                                                {
                                                                    if (partFeature.ErrorsDetected == false)
                                                                    {
                                                                        if (partFeature.PartFeatureType.Detail.Alias.StartsWith("AER"))
                                                                        {
                                                                            try
                                                                            {
                                                                                if (partFeature.PartFeatureType.Detail.Alias.EndsWith("250K"))
                                                                                    heatmapTerm.AER250K = Convert.ToDecimal(partFeature.Value);
                                                                                else if (partFeature.PartFeatureType.Detail.Alias.EndsWith("100K"))
                                                                                    heatmapTerm.AER100K = Convert.ToDecimal(partFeature.Value);
                                                                                else if (partFeature.PartFeatureType.Detail.Alias.EndsWith("50K"))
                                                                                    heatmapTerm.AER50K = Convert.ToDecimal(partFeature.Value);
                                                                            }
                                                                            catch
                                                                            {
                                                                            }
                                                                        }
                                                                        else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("MinimumInvestment") == 0)
                                                                        {
                                                                            try
                                                                            {
                                                                                heatmapTerm.MinimumInvestment = Convert.ToDecimal(partFeature.Value);
                                                                            }
                                                                            catch
                                                                            {
                                                                            }
                                                                        }
                                                                        else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("MaximumInvestment") == 0)
                                                                        {
                                                                            try
                                                                            {
                                                                                heatmapTerm.MaximumInvestment = Convert.ToDecimal(partFeature.Value);
                                                                            }
                                                                            catch
                                                                            {
                                                                            }
                                                                        }
                                                                        else if (partFeature.PartFeatureType.Detail.Alias.CompareTo("InterestPaid") == 0)
                                                                        {
                                                                            heatmapTerm.InterestPaid = partFeature.Value;
                                                                        }
                                                                    }
                                                                    else
                                                                        break;
                                                                }
                                                                heatmapInstitution.investmentTerms.Add(heatmapTerm);
                                                            }
                                                        }
                                                    }
                                                    else
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    break;
                            }
                        }
                    }
                    else
                        break;
                }
            }
            catch (Exception exc)
            {
                this.ErrorsDetected = true;
                this.StackTrace = exc.StackTrace;
                this.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
            }
        }

        public List<HeatmapInstitution> GetProductsWithinTerm(int pFromDays, int pToDays, decimal? pMaximumDepositInAnyOneInstitution)
        {
            List<HeatmapInstitution> productsWithinTerm = new List<HeatmapInstitution>();
            foreach (HeatmapInstitution heatmapInstitution in heatmapInstitutions)
            {
                HeatmapInstitution potentialProduct = new HeatmapInstitution(heatmapInstitution.institution);
                foreach (HeatmapTerm heatmapTerm in heatmapInstitution.investmentTerms)
                {
                    bool considerProduct = false;
                    if (pMaximumDepositInAnyOneInstitution.HasValue)
                    {
                        if (heatmapTerm.MaximumInvestment.HasValue)
                        {
                            if (pMaximumDepositInAnyOneInstitution.Value <= heatmapTerm.MaximumInvestment.Value)
                                considerProduct = true;
                        }
                        else
                            considerProduct = true;
                    }
                    else
                        considerProduct = true;
                    if (considerProduct)
                    {
                        if (heatmapTerm.InvestmentTerm.investmentAccountType == InvestmentAccountType.InstantAccessAccount)
                        {
                            if (pFromDays == 0)
                                potentialProduct.investmentTerms.Add(heatmapTerm);
                        }
                        else if (heatmapTerm.InvestmentTerm.investmentAccountType == InvestmentAccountType.NoticeAccount)
                        {
                            if (heatmapTerm.InvestmentTerm.NoticeDays.HasValue)
                            {
                                if (heatmapTerm.InvestmentTerm.NoticeDays.Value >= pFromDays && heatmapTerm.InvestmentTerm.NoticeDays.Value <= pToDays)
                                    potentialProduct.investmentTerms.Add(heatmapTerm);
                            }
                        }
                        else if (heatmapTerm.InvestmentTerm.investmentAccountType == InvestmentAccountType.TermAccount)
                        {
                            if (heatmapTerm.InvestmentTerm.TermDays.HasValue)
                            {
                                if (heatmapTerm.InvestmentTerm.TermDays.Value >= pFromDays && heatmapTerm.InvestmentTerm.TermDays.Value <= pToDays)
                                    potentialProduct.investmentTerms.Add(heatmapTerm);
                            }
                        }
                    }
                }
                if (potentialProduct.investmentTerms.Count > 0)
                    productsWithinTerm.Add(potentialProduct);
            }
            return productsWithinTerm;
        }

        public List<Clients.Helper.InvestmentTerm> ListAllTerms()
        {
            List<Clients.Helper.InvestmentTerm> investmentTerms = new List<InvestmentTerm>();

            foreach (HeatmapInstitution heatmapInstitution in heatmapInstitutions)
            {
                foreach (HeatmapTerm heatmapTerm in heatmapInstitution.investmentTerms)
                {
                    bool found = false;
                    foreach (Clients.Helper.InvestmentTerm investmentTerm in investmentTerms)
                    {
                        if (investmentTerm.GetLiquidityDays()==heatmapTerm.InvestmentTerm.GetLiquidityDays())
                        {
                            found = true;
                            break;
                        }
                    }
                    if (found == false)
                    {
                        foreach (Clients.Helper.InvestmentTerm investmentTerm in investmentTerms)
                        {
                            if (investmentTerm.GetText().CompareTo(heatmapTerm.InvestmentTerm.GetText()) == 0)
                            {
                                found = true;
                                break;
                            }
                        }
                        if(found==false)
                            investmentTerms.Add(heatmapTerm.InvestmentTerm);
                    }
                }
            }

            try
            {
                investmentTerms.Sort((x, y) => x.GetLiquidityDays().Value.CompareTo(y.GetLiquidityDays().Value));
                investmentTerms.Reverse();
            }
            catch
            {
            }
            return investmentTerms;
        }
    }
}
