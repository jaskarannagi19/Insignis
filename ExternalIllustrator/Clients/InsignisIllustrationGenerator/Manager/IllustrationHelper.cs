using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Helper;
using InsignisIllustrationGenerator.Models;
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
    }
}
