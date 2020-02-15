using InsignisIllustrationGenerator.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Manager
{
    public class SuperUserHelper
    {
        private readonly ApplicationDbContext _context;
        public SuperUserHelper(ApplicationDbContext context)
        {
            _context = context;
        }
        internal List<IllustrationDetail> GetIllustrationList()
        {
            return _context.IllustrationDetails.Take(25).ToList();
        }
    }
}
