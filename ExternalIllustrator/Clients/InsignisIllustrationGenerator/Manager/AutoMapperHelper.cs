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
            CreateMap<IllustrationDetailViewModel, IllustrationDetail>();
        }
    }

}


