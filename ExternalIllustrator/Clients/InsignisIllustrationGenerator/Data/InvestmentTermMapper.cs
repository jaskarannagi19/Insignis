using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Data
{
    public class InvestmentTermMapper
    {
        ///<summary>
        ///Entity to map investment term with investment text
        /// </summary>
        
        public int Id { get; set; }
        public string InvestmentText { get; set; }
        public string InvestmentTerm { get; set; }

    }
}
