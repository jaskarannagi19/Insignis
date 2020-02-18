using AutoMapper;
using InsignisIllustrationGenerator.Data;
using InsignisIllustrationGenerator.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InsignisIllustrationGenerator.Manager
{
    public class AutoMapperHelper : Profile
    {
        public AutoMapperHelper()
        {

            CreateMap<Insignis.Asset.Management.Tools.Sales.SCurveOutput, SCurveOutput>();
            //to database entity
            CreateMap<IllustrationDetailViewModel, IllustrationDetail>().ForMember(f=>f.IllustrationProposedPortfolio, t=>t.Ignore());

            CreateMap<Insignis.Asset.Management.Illustrator.Interface.Bank, Bank>();
            CreateMap<Insignis.Asset.Management.Illustrator.Interface.Product, Product>();
            CreateMap<Product,Insignis.Asset.Management.Illustrator.Interface.Product>();


          //  CreateMap<Insignis.Asset.Management.Tools.Sales.SCurveOutput, SCurveOutput>();
            CreateMap<InsignisIllustrationGenerator.Models.SCurveOutput,Insignis.Asset.Management.Tools.Sales.SCurveOutput>();

            //Map to view model
            CreateMap<IllustrationDetail, IllustrationListViewModel>();
            CreateMap<IllustrationDetail, IllustrationDetailViewModel>();



        }
    }

}


