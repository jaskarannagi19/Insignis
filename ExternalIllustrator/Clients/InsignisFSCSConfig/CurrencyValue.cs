using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Insignis.Asset.Management.Clients.Helper.FSCS
{
    public class CurrencyValue
    {
        public string CurrencyCode = "";
        public decimal Value = 0;
        public bool IsJointInvestment = false;

        public CurrencyValue()
        {
        }

        public CurrencyValue(string pCurrencyCode, decimal pValue)
        {
            CurrencyCode = pCurrencyCode;
            Value = pValue;
        }

        public CurrencyValue(string pCurrencyCode, decimal pValue, bool pIsJointInvestment)
        {
            CurrencyCode = pCurrencyCode;
            Value = pValue;
            IsJointInvestment = pIsJointInvestment;
        }
    }
}