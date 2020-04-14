
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Manager
{
    public class IllustrationHelper
    {
        private readonly AutoMapper.IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public IllustrationHelper(AutoMapper.IMapper mapper, ApplicationDbContext context)
        {

            _mapper = mapper;
            _context = context;
        }

        internal bool SaveIllustraionAsync(IllustrationDetailViewModel model)
        {

            //check for existence
            bool exits = _context.IllustrationDetails.Any(x => x.IllustrationUniqueReference == model.IllustrationUniqueReference);
            var illustrationDetail = new IllustrationDetail();
            if (exits)
            {
                illustrationDetail = _context.IllustrationDetails.FirstOrDefault(x => x.IllustrationUniqueReference == model.IllustrationUniqueReference);




                //illustrationDetail.AdviserName = model.AdviserName;
                illustrationDetail.ClientName = model.ClientName;
                illustrationDetail.ClientType = model.ClientType;

                illustrationDetail.Currency = model.Currency;
                illustrationDetail.EasyAccess = Convert.ToDouble(model.EasyAccess);
                illustrationDetail.GenerateDate = model.GenerateDate;



                illustrationDetail.NineMonths = Convert.ToDouble(model.NineMonths);
                illustrationDetail.OneMonth = Convert.ToDouble(model.OneMonth);
                illustrationDetail.OneYear = Convert.ToDouble(model.OneYear);
                illustrationDetail.PartnerEmail = model.PartnerEmail;
                illustrationDetail.PartnerName = model.PartnerName;
                illustrationDetail.PartnerOrganisation = model.PartnerOrganisation;
                illustrationDetail.ReferredBy = model.ReferredBy;
                illustrationDetail.SixMonths = Convert.ToDouble(model.SixMonths);
                illustrationDetail.Status = model.Status;
                illustrationDetail.ThreeMonths = Convert.ToDouble(model.ThreeMonths);
                illustrationDetail.ThreeYearsPlus = Convert.ToDouble(model.ThreeYearsPlus);
                illustrationDetail.TotalDeposit = Convert.ToDouble(model.TotalDeposit);
                illustrationDetail.TwoYears = Convert.ToDouble(model.TwoYears);




                illustrationDetail.Status = "Created";
                //Save interests
                illustrationDetail.AnnualGrossInterestEarned = model.ProposedPortfolio.AnnualGrossInterestEarned;
                illustrationDetail.AnnualNetInterestEarned = model.ProposedPortfolio.AnnualNetInterestEarned;
                illustrationDetail.GrossAverageYield = model.ProposedPortfolio.GrossAverageYield;
                illustrationDetail.NetAverageYield = model.ProposedPortfolio.GrossAverageYield;
                illustrationDetail.IllustrationProposedPortfolio = new List<ProposedPortfolio>();
                foreach (var item in model.ProposedPortfolio.ProposedInvestments)
                {
                    ProposedPortfolio folio = new ProposedPortfolio();
                    folio.InvestmentTerm = item.InvestmentTerm.TermText;
                    folio.AnnualInterest = item.AnnualInterest;
                    folio.DepositSize = item.DepositSize;
                    folio.IllustrationID = model.Id;
                    folio.InstitutionID = item.InstitutionID;
                    folio.InstitutionName = item.InstitutionName;
                    folio.InstitutionShortName = item.InstitutionShortName;
                    folio.Rate = item.Rate;
                    illustrationDetail.IllustrationProposedPortfolio.Add(folio);

                }
            }
            else
            {
                illustrationDetail = _mapper.Map<IllustrationDetailViewModel, IllustrationDetail>(model);
                illustrationDetail.Status = "Created";
                //Save interests
                illustrationDetail.AnnualGrossInterestEarned = model.ProposedPortfolio.AnnualGrossInterestEarned;
                illustrationDetail.AnnualNetInterestEarned = model.ProposedPortfolio.AnnualNetInterestEarned;
                illustrationDetail.GrossAverageYield = model.ProposedPortfolio.GrossAverageYield;
                illustrationDetail.NetAverageYield = model.ProposedPortfolio.GrossAverageYield;
                illustrationDetail.IllustrationProposedPortfolio = new List<ProposedPortfolio>();
                foreach (var item in model.ProposedPortfolio.ProposedInvestments)
                {
                    ProposedPortfolio folio = new ProposedPortfolio();
                    folio.InvestmentTerm = item.InvestmentTerm.TermText;
                    folio.AnnualInterest = item.AnnualInterest;
                    folio.DepositSize = item.DepositSize;
                    folio.IllustrationID = model.Id;
                    folio.InstitutionID = item.InstitutionID;
                    folio.InstitutionName = item.InstitutionName;
                    folio.InstitutionShortName = item.InstitutionShortName;
                    folio.Rate = item.Rate;
                    illustrationDetail.IllustrationProposedPortfolio.Add(folio);

                }
                _context.IllustrationDetails.Add(illustrationDetail);
            }

            _context.SaveChanges();
            var IsSave = true;
            return IsSave;
        }







        internal IEnumerable<IllustrationListViewModel> GetIllustrationList(SearchParameterViewModel searchParameter, bool isSuperUser)
        {

            var IllustrationDetails = new List<IllustrationDetail>();


            if (!isSuperUser)
                IllustrationDetails = _context.IllustrationDetails.Where(x => x.PartnerEmail == searchParameter.PartnerEmail && x.PartnerOrganisation == searchParameter.PartnerOrganisation).Include(x => x.IllustrationProposedPortfolio).OrderByDescending(x => x.GenerateDate).ToList();
            else
                IllustrationDetails = _context.IllustrationDetails.Include(x => x.IllustrationProposedPortfolio).OrderByDescending(x => x.GenerateDate).ToList();

            searchParameter.IllustrationUniqueReference = string.IsNullOrEmpty(searchParameter.IllustrationUniqueReference) ? "" : searchParameter.IllustrationUniqueReference.ToLower();


            if (searchParameter.IllustrationFrom != null && searchParameter.IllustrationTo != null) {
                searchParameter.IllustrationTo = searchParameter.IllustrationTo.Value.AddHours(23).AddMinutes(59).AddSeconds(59);
            }

            if (!string.IsNullOrEmpty(searchParameter.ClientName) || !string.IsNullOrEmpty(searchParameter.PartnerName) || !string.IsNullOrEmpty(searchParameter.CompanyName) || !string.IsNullOrEmpty(searchParameter.IllustrationUniqueReference) || searchParameter.IllustrationTo != null || searchParameter.IllustrationFrom != null)
            {
                IllustrationDetails = IllustrationDetails.Where(f =>

                //Advisor Search
                (string.IsNullOrEmpty(searchParameter.PartnerName) || (!string.IsNullOrEmpty(f.PartnerName) && f.PartnerName.ToLower().Contains(searchParameter.PartnerName.ToLower())))
                //Client Search
                & (string.IsNullOrEmpty(searchParameter.ClientName) || f.ClientName.ToLower().Contains(searchParameter.ClientName.ToLower()))
                //Company Search
                & (string.IsNullOrEmpty(searchParameter.CompanyName) || f.PartnerOrganisation.ToLower().Contains(searchParameter.CompanyName.ToLower()))
                //Company Search
                & (string.IsNullOrEmpty(searchParameter.IllustrationUniqueReference) || f.IllustrationUniqueReference.ToLower().Contains(searchParameter.IllustrationUniqueReference.ToLower()))

                & ((!searchParameter.IllustrationFrom.HasValue || f.GenerateDate >= searchParameter.IllustrationFrom.Value)
                & (!searchParameter.IllustrationTo.HasValue || f.GenerateDate <= searchParameter.IllustrationTo.Value))).ToList();
            }
            List<IllustrationListViewModel> response = new List<IllustrationListViewModel>();
            response = _mapper.Map(IllustrationDetails, response);

            return response;
        }

        internal IllustrationDetailViewModel GetIllustrationByUniqueReferenceId(string uniqueReferenceID)
        {
            /*
             Get illustration from unique reference id from db
            Arguments:-
                Unique Reference ID
            Return:-
                Illustration DetailViewModel
             */
            IllustrationDetail dbIllustration = _context.IllustrationDetails.Include(x => x.IllustrationProposedPortfolio).FirstOrDefault(x => x.IllustrationUniqueReference == uniqueReferenceID);
            //map db entity to view model ProposedPortfolio. Investment count is 0 after Mapping
            IllustrationDetailViewModel result = _mapper.Map<IllustrationDetailViewModel>(dbIllustration);
            result.ProposedPortfolio = new Insignis.Asset.Management.Tools.Sales.SCurveOutput();
            //result.ProposedPortfolio.ProposedInvestments = new List<Insignis.Asset.Management.Tools.Sales.SCurveOutputRow>();


            foreach (var item in dbIllustration.IllustrationProposedPortfolio)
            {
                Insignis.Asset.Management.Tools.Sales.SCurveOutputRow row = new Insignis.Asset.Management.Tools.Sales.SCurveOutputRow();
                Insignis.Asset.Management.Clients.Helper.InvestmentTerm term = new Insignis.Asset.Management.Clients.Helper.InvestmentTerm();
                row.AnnualInterest = item.AnnualInterest;
                row.DepositSize = item.DepositSize;
                row.InstitutionName = item.InstitutionName;
                row.Rate = item.Rate;
                row.InvestmentTerm = term;
                row.InvestmentTerm.TermText = item.InvestmentTerm;
                row.InstitutionShortName = item.InstitutionShortName;

                result.ProposedPortfolio.ProposedInvestments.Add(row);
            }




            result.AnnualGrossInterestEarned = 0;

            foreach (var investment in result.ProposedPortfolio.ProposedInvestments)
            {
                result.AnnualGrossInterestEarned += investment.AnnualInterest;
            }

            result.ProposedPortfolio.AnnualGrossInterestEarned = result.AnnualGrossInterestEarned;

            result.GrossAverageYield = (result.ProposedPortfolio.AnnualGrossInterestEarned / Convert.ToDecimal(result.TotalDeposit)) * 100;

            if (result.TotalDeposit.Value >= 50000 && result.TotalDeposit <= 299999)
                result.ProposedPortfolio.FeePercentage = 0.25M;

            if (result.TotalDeposit.Value >= 300000 && result.TotalDeposit <= 999999)
                result.ProposedPortfolio.FeePercentage = 0.20M;

            result.NetAverageYield = (result.GrossAverageYield - result.ProposedPortfolio.FeePercentage);


            result.ProposedPortfolio.TotalDeposited =Convert.ToDecimal(result.TotalDeposit);
            result.ProposedPortfolio.Fee = (result.ProposedPortfolio.TotalDeposited * (decimal)(result.ProposedPortfolio.FeePercentage / 100));

            result.AnnualNetInterestEarned = (result.ProposedPortfolio.AnnualGrossInterestEarned - result.ProposedPortfolio.Fee);






            return result;
        }

        internal Guid GetSessionIdForIllustration(string uniqueReferenceId)
        {
            return _context.ExcludedInstitutes.Where(x=>x.UniqueReferenceId == uniqueReferenceId).ToList().Last().SessionId;
        }


        internal int GetNextIllustrationRefernce()
        {
            /*
             Get next illustration reference number
             Arguments:-
                None
            Return:-
                String reference number
             */
            string lastReference = _context.IllustrationDetails.OrderByDescending(x => x.IllustrationUniqueReference).First().IllustrationUniqueReference;
            int number = Convert.ToInt32(lastReference.Split("-")[2]) + 1;
            return number;
        }



        internal bool UpdateIllustrationStatus(UpdateStatusViewModel model)
        {
            IllustrationDetail illustration = _context.IllustrationDetails.First(x => x.IllustrationUniqueReference == model.UniqueReferenceId);

            if (illustration.Status.ToLower() == "chased" & (model.Status == "Accepted" || model.Status == "Deleted"))
            {
                illustration.Status = model.Status;
            }

            if (illustration.Status.ToLower() == "created" & (model.Status == "Deleted"||model.Status=="Chased"||model.Status=="Accepted"))
            {
                illustration.Status = model.Status;
            }
            if(illustration.Status.ToLower() == "accepted" & model.Status == "Deleted")
            {
                illustration.Status = model.Status;
            }
                
            illustration.Comment = model.Comment;
            illustration.ReferredBy = model.ReferredBy;
            bool isSaved = false;
            try { 
            _context.SaveChanges();
                isSaved = true;
            }
            catch
            {

            }
            return isSaved;
        }
    }
}
