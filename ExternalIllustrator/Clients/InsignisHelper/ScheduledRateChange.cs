using System;

namespace Insignis.Asset.Management.Clients.Helper
{
    public class ScheduledRateChange
    {
        public int InstitutionID;
        public string ProductCode;
        public DateTime ScheduledDate;
        public decimal Rate;

        public ScheduledRateChange()
        {
        }

        public ScheduledRateChange(int pInstitutionID, string pProductCode, DateTime pScheduledDate, decimal pRate)
        {
            InstitutionID = pInstitutionID;
            ProductCode = pProductCode;
            ScheduledDate = pScheduledDate;
            Rate = pRate;
        }
    }
}
