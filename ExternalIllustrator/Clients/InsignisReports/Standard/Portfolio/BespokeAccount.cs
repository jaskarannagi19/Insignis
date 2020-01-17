using System;
using System.Collections.Generic;

namespace Insignis.Asset.Management.Reports.Standard.Portfolio
{
    public class BespokeAccount
    {
        public int? ID = null;
        public DateTime? Opened = null;
        public string Bank;                                         // United Trust Bank
        public List<string> Liquidity = new List<string>();         // 3 Year Bond (Term)
        public string Matures = "";
        public string Rate;                                         // 2.10%
        public List<string> InvestmentAmount = new List<string>();  // £100,000.00
        public string Comment = "";                                 // any comment i.e. notice period
        public int LiquidityDays = 0;
    }
}
