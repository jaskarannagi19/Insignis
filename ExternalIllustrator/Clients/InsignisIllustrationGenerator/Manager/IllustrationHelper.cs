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

        internal async Task<bool> SaveIllustraionAsync(IllustrationDetailViewModel model)
        {
            var illustrationDetail = _mapper.Map<IllustrationDetail>(model);

            //_context.
            var IsSave = false;
            return  IsSave;
        }
    }
}
