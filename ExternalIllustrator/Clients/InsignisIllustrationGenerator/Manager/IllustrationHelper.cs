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

            var illustrationDetail = _mapper.Map<IllustrationDetailViewModel,IllustrationDetail>(model);

            illustrationDetail.IllustrationProposedPortfolio = new List<ProposedPortfolio>();
            ProposedPortfolio folio = new ProposedPortfolio();
            foreach (var item in model.ProposedPortfolio.ProposedInvestments)
            {

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
            return  IsSave;
        }

        internal async Task< IEnumerable<IllustrationListViewModel>> GetIllustrationListAsync(SearchParameterViewModel searchParameter)
        {
            var IllustrationDetails = await _context.IllustrationDetails.Include(x=>x.IllustrationProposedPortfolio).ToListAsync();

            if (searchParameter != null) {
                IllustrationDetails = IllustrationDetails.Where(f =>
                //Advisor Search
                (string.IsNullOrEmpty(searchParameter.AdviserName) | f.AdviserName.Contains(searchParameter.AdviserName))
                //Client Search
                & (string.IsNullOrEmpty(searchParameter.ClientName) | f.ClientName.Contains(searchParameter.ClientName))
                //Company Search
                & (string.IsNullOrEmpty(searchParameter.CompanyName) | f.ClientName.Contains(searchParameter.CompanyName))
                //Company Search
                & ((searchParameter.IllustrationFrom.HasValue | f.GenerateDate > searchParameter.IllustrationFrom)
                & (searchParameter.IllustrationTo.HasValue | f.GenerateDate < searchParameter.IllustrationTo))).ToList();
            }
            List<IllustrationListViewModel> response = new List<IllustrationListViewModel>();
            response = _mapper.Map(IllustrationDetails, response);

            return response;
        }
    }
}
