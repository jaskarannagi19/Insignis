using InsignisIllustrationGenerator.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Manager
{
    public class BankHelper
    {
        //Helper for API
        private readonly AutoMapper.IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public BankHelper(AutoMapper.IMapper mapper, ApplicationDbContext context)
        {
            
            _mapper = mapper;
            _context = context;
        }

        public bool SaveBank(List<Insignis.Asset.Management.Illustrator.Interface.Bank> bank)
        {
            /*
             Saves Bank as well as Product Data into API.
             */
            List<Bank> _bankList = new List<Bank>();
            _bankList = _mapper.Map(bank, _bankList);


            

            //_context.Bank.AddRange(_bankList);
            _context.AttachRange(_bankList);
            _context.SaveChanges();

            return true;

        }
    }
}
