namespace Insignis.Asset.Management.Illustrator.Interface
{
    public class Product
    {
        public int ProductID = -1;
        public string ProductCode = "";
        public string ProductName = "";
        public string ProductType = "";

        public string LiquidityType = "";
        public string NoticeText = "";
        public string NoticeDays = "";
        public string TermText = "";
        public string TermDays = "";
        public string RateFor50KDeposit = "";
        public string RateFor100KDeposit = "";
        public string RateFor250KDeposit = "";
        public string InterestPaid = "";
        public string MinimumDeposit = "";
        public string MaximumDeposit="";
        public bool IsAvailableToPersonalHubAccounts = false;
        public bool IsAvailableToJointHubAccounts = false;
        public bool IsAvailableToSMEHubAccounts = false;
        public bool IsAvailableToTrustHubAccounts = false;
        public bool IsAvailableToIncorporatedCharityHubAccounts = false;
        public bool IsAvailableToPowerOfAttorneyHubAccounts = false;
        public bool IsAvailableToPersonalTrustHubAccounts = false;
        public bool IsAvailableToLocalAuthorityHubAccounts = false;
        public bool IsAvailableToSSASHubAccounts = false;
        public bool IsAvailableToSIPPHubAccounts = false;
        public bool IsAvailableToLargeCorporateHubAccounts = false;
        public bool IsAvailableToUnincorporatedCharityHubAccounts = false;
        public bool IsAvailableToCourtOfProtectionHubAccounts = false;

        public string ToXML()
        {
            string xml = "<product id=\"" + ProductID + "\" code=\"" + ProductCode + "\" name=\"" + ProductName + "\">";
            xml += "<productType>" + ProductType + "</productType>";
            xml += "<liquidityType>" + LiquidityType + "</liquidityType>";
            xml += "<noticeText>" + NoticeText + "</noticeText>";
            xml += "<noticeDays>" + NoticeDays + "</noticeDays>";
            xml += "<termText>" + TermText + "</termText>";
            xml += "<termDays>" + TermDays + "</termDays>";
            xml += "<rateFor50KDeposit>" + RateFor50KDeposit + "</rateFor50KDeposit>";
            xml += "<rateFor100KDeposit>" + RateFor100KDeposit + "</rateFor100KDeposit>";
            xml += "<rateFor250KDeposit>" + RateFor250KDeposit + "</rateFor250KDeposit>";
            xml += "<interestPaid>" + InterestPaid + "</interestPaid>";
            xml += "<minimumDeposit>" + MinimumDeposit + "</minimumDeposit>";
            xml += "<maximumDeposit>" + MaximumDeposit + "</maximumDeposit>";
            xml += "<isAvailableToPersonalHubAccounts>" + IsAvailableToPersonalHubAccounts + "</isAvailableToPersonalHubAccounts>";
            xml += "<isAvailableToJointHubAccounts>" + IsAvailableToJointHubAccounts + "</isAvailableToJointHubAccounts>";
            xml += "<isAvailableToSMEHubAccounts>" + IsAvailableToSMEHubAccounts + "</isAvailableToSMEHubAccounts>";
            xml += "<isAvailableToTrustHubAccounts>" + IsAvailableToTrustHubAccounts + "</isAvailableToTrustHubAccounts>";
            xml += "<isAvailableToIncorporatedCharityHubAccounts>" + IsAvailableToIncorporatedCharityHubAccounts + "</isAvailableToIncorporatedCharityHubAccounts>";
            xml += "<isAvailableToPowerOfAttorneyHubAccounts>" + IsAvailableToPowerOfAttorneyHubAccounts + "</isAvailableToPowerOfAttorneyHubAccounts>";
            xml += "<isAvailableToPersonalTrustHubAccounts>" + IsAvailableToPersonalTrustHubAccounts + "</isAvailableToPersonalTrustHubAccounts>";
            xml += "<isAvailableToLocalAuthorityHubAccounts>" + IsAvailableToLocalAuthorityHubAccounts + "</isAvailableToLocalAuthorityHubAccounts>";
            xml += "<isAvailableToSSASHubAccounts>" + IsAvailableToSSASHubAccounts + "</isAvailableToSSASHubAccounts>";
            xml += "<isAvailableToSIPPHubAccounts>" + IsAvailableToSIPPHubAccounts + "</isAvailableToSIPPHubAccounts>";
            xml += "<isAvailableToLargeCorporateHubAccounts>" + IsAvailableToLargeCorporateHubAccounts + "</isAvailableToLargeCorporateHubAccounts>";
            xml += "<isAvailableToUnincorporatedCharityHubAccounts>" + IsAvailableToUnincorporatedCharityHubAccounts + "</isAvailableToUnincorporatedCharityHubAccounts>";
            xml += "<isAvailableToCourtOfProtectionHubAccounts>" + IsAvailableToCourtOfProtectionHubAccounts + "</isAvailableToCourtOfProtectionHubAccounts>";
            xml += "</product>";
            return xml;
        }
    }
}
