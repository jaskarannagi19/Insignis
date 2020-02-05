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


            _context.Bank.AddRange(_bankList);

            //var dbBank = _context.Bank.ToList();

            //var saveList = new List<Bank>();

            //foreach (var bb in _bankList)
            //{
            //    //CHeck if bank exit in dbList
            //    if (!dbBank.Any(b => b.BankID== bb.BankID)) {
            //        saveList.Add(bb);
            //    }

            //}


            //var productList = new List<Product>();
            //foreach (var item in _bankList)
            //{
            //    foreach (var product in item.Products)
            //    {
            //        //product.Bank = item;
            //        productList.Add(product);
            //    }
                
            //}

            //_context.Bank.AddRange(saveList);


            //_context.Product.AddRange(productList);


            _context.SaveChanges();

            return true;

        }

        public bool SaveProduct(List<Insignis.Asset.Management.Illustrator.Interface.Bank> bankProducts)
        {
            List<Product> _productList = new List<Product>();
            _productList = _mapper.Map(bankProducts, _productList);

            _context.Product.AddRange(_productList);
            _context.SaveChanges();

            return true;
        }

    }
}
