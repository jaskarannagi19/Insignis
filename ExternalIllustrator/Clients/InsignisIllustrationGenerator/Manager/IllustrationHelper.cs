
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

            var illustrationDetail = _mapper.Map<IllustrationDetailViewModel, IllustrationDetail>(model);

            illustrationDetail.Status = "CREATED";
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

            //illustrationDetail.IllustrationProposedPortfolio = model.ProposedPortfolio.ProposedInvestments.;

            _context.IllustrationDetails.Add(illustrationDetail);
            _context.SaveChanges();
            var IsSave = false;
            return IsSave;
        }







        internal IEnumerable<IllustrationListViewModel> GetIllustrationList(SearchParameterViewModel searchParameter)
        {
            var IllustrationDetails = _context.IllustrationDetails.Include(x => x.IllustrationProposedPortfolio).OrderByDescending(x=>x.GenerateDate).ToList();

            //DateTime? IllustrationFrom = Convert.ToDateTime(searchParameter.IllustrationFrom);
            //DateTime? IllustrationTo = Convert.ToDateTime(searchParameter.IllustrationTo);

            //searchParameter.ClientName = string.IsNullOrEmpty(searchParameter.ClientName) ? "" : searchParameter.ClientName.ToLower();
            //searchParameter.CompanyName = string.IsNullOrEmpty(searchParameter.CompanyName) ? "" : searchParameter.CompanyName.ToLower();
            //searchParameter.AdvisorName = string.IsNullOrEmpty(searchParameter.AdvisorName) ? "" : searchParameter.AdvisorName.ToLower();
            searchParameter.IllustrationUniqueReference = string.IsNullOrEmpty(searchParameter.IllustrationUniqueReference) ? "" : searchParameter.IllustrationUniqueReference.ToLower();


            if (searchParameter != null)
            {
                IllustrationDetails = IllustrationDetails.Where(f =>

                //Advisor Search
                (string.IsNullOrEmpty(searchParameter.AdvisorName) || (!string.IsNullOrEmpty(f.AdviserName) && f.AdviserName.ToLower().Contains(searchParameter.AdvisorName)))
                //Client Search
                & (string.IsNullOrEmpty(searchParameter.ClientName) || f.ClientName.ToLower().Contains(searchParameter.ClientName.ToLower()))
                //Company Search
                & (string.IsNullOrEmpty(searchParameter.CompanyName) || f.PartnerName.ToLower().Contains(searchParameter.CompanyName.ToLower()))
                //Company Search
                & (string.IsNullOrEmpty(searchParameter.IllustrationUniqueReference) || f.IllustrationUniqueReference.ToLower().Contains(searchParameter.IllustrationUniqueReference.ToLower()))

                & ((!searchParameter.IllustrationFrom.HasValue || f.GenerateDate > searchParameter.IllustrationFrom.Value)
                & (!searchParameter.IllustrationTo.HasValue || f.GenerateDate < searchParameter.IllustrationTo.Value))).ToList();
            }
            List<IllustrationListViewModel> response = new List<IllustrationListViewModel>();
            response = _mapper.Map(IllustrationDetails, response);

            return response;
        }

        internal IllustrationDetailViewModel GetIllustrationByByUniqueReference(string uniqueReferenceID)
        {
            /*
             Get illustration from unique reference id from db
            Arguments:-
                Unique Reference ID
            Return:-
                Illustration DetailViewModel
             */
            IllustrationDetail dbIllustration = _context.IllustrationDetails.Include(x => x.IllustrationProposedPortfolio).SingleOrDefault(x => x.IllustrationUniqueReference == uniqueReferenceID);
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

            return result;
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
