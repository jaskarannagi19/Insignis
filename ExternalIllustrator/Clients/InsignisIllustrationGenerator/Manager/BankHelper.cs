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

        

        public bool SaveBankAndProducts(List<Insignis.Asset.Management.Illustrator.Interface.Bank> bank)
        {
            /*
             Saves Bank as well as Product Data into API.
             */

            List<Bank> _bankList = new List<Bank>();
            _bankList = _mapper.Map(bank, _bankList);
            Bank dbBank = null;
            foreach (var _bank in _bankList)
            {
                dbBank = _context.Bank.SingleOrDefault(x => x.BankID == _bank.BankID);
                //get bank from db
                if (dbBank == null) { 
                    _context.Add(_bank);
                }
                else
                {
                    //update fitch rating

                    dbBank.FitchRating = _bank.FitchRating;
                    _context.SaveChanges();


                    //check product
                    Product dbProduct = null;

                    foreach (var product in _bank.Products)
                    {
                        dbProduct = _context.Product.SingleOrDefault(x => x.ProductID == product.ProductID);
                        if (dbProduct == null) {
                            _context.Product.Add(product);
                        }
                        else
                        {
                            dbProduct= _mapper.Map(dbProduct, product);
                            _context.SaveChanges();

                        }
                        _context.SaveChanges();
                    }

                }
            }


            

            return true;
        }
    }
}
