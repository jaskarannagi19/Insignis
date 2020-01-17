using Octavo.Gate.Nabu.Abstraction;
using System;

namespace Insignis.Asset.Management.External.Illustrator.Helper.UI
{
    public class Currency
    {
        private Octavo.Gate.Nabu.Entities.Financial.Currency currency = null;
        private FinancialAbstraction financialAbstraction = null;

        public Currency(Octavo.Gate.Nabu.Entities.Financial.Currency pCurrency, FinancialAbstraction pFinancialAbstraction)
        {
            currency = pCurrency;
            financialAbstraction = pFinancialAbstraction;

            if (pCurrency != null)
            {
                if (pCurrency.CurrencyID.HasValue)
                {
                    if (pCurrency.CurrencyCode == null || pCurrency.CurrencyCode.Trim().Length == 0)
                        currency = financialAbstraction.GetCurrency((int)pCurrency.CurrencyID);
                }
            }
        }

        public Currency(Octavo.Gate.Nabu.Entities.Financial.Currency pCurrency)
        {
            currency = pCurrency;
        }

        public string DisplayValue(decimal? pValue)
        {
            string result = "";

            if (currency != null)
            {
                if (currency.CurrencyID.HasValue)
                {
                    result += currency.GetCurrencySymbol();
                }
            }
            if (pValue.HasValue)
                result += String.Format("{0:#,##0.00}", pValue.Value);

            return result;
        }

        public string DisplayValueWithoutSymbol(decimal? pValue)
        {
            string result = "";

            if (pValue.HasValue)
                result += String.Format("{0:#,##0.00}", pValue.Value);

            return result;
        }

        public string DisplayValueAsHTML(string pValue)
        {
            string result = "";
            if (pValue != null && pValue.Trim().Length > 0)
            {
                try
                {
                    return DisplayValueAsHTML(Convert.ToDecimal(pValue));
                }
                catch
                {
                }
            }
            return result;
        }

        public string DisplayValueAsHTML(decimal? pValue)
        {
            string result = "";

            if (currency != null)
            {
                if (currency.CurrencyID.HasValue)
                {
                    result += currency.GetCurrencySymbolAsHTML();
                }
            }
            if (pValue.HasValue)
                result += String.Format("{0:#,##0.00}", pValue.Value);

            return result;
        }

        public Octavo.Gate.Nabu.Entities.Financial.Currency GetCurrentCurrency()
        {
            return currency;
        }
    }
}