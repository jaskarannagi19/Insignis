using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InsignisIllustrationGenerator.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        
            modelBuilder.Entity<InvestmentTermMapper>().HasData(
                new InvestmentTermMapper { Id = 1, InvestmentText = "Instant Access", InvestmentTerm = "Instant Access" },

                new InvestmentTermMapper { Id = 2, InvestmentText="35 Day", InvestmentTerm="One Month"},
                new InvestmentTermMapper { Id = 3, InvestmentText = "1 Week", InvestmentTerm = "One Month" },
                new InvestmentTermMapper { Id = 4, InvestmentText = "1 Week Bond", InvestmentTerm = "One Month" },
                new InvestmentTermMapper { Id = 5, InvestmentText = "1 Month Bond", InvestmentTerm = "One Month" },
                new InvestmentTermMapper { Id = 6, InvestmentText = "30 Day", InvestmentTerm = "One Month" },
                new InvestmentTermMapper { Id = 7, InvestmentText = "45 Day", InvestmentTerm = "One Months" },


                new InvestmentTermMapper { Id = 8, InvestmentText = "2 Month Bond", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 9, InvestmentText = "90 Day", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 10, InvestmentText = "3 Month Bond", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 11, InvestmentText = "95 Day", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 12, InvestmentText = "100 Day", InvestmentTerm = "Three Months" },


                new InvestmentTermMapper { Id = 13, InvestmentText = "4 Month Bond", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 14, InvestmentText = "5 Month Bond", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 15, InvestmentText = "6 Month", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 16, InvestmentText = "6 Month Bond", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 17, InvestmentText = "185 Day", InvestmentTerm = "Six Months" },


                new InvestmentTermMapper { Id = 18, InvestmentText = "7 Month Bond", InvestmentTerm = "Nine Months" },
                new InvestmentTermMapper { Id = 19, InvestmentText = "8 Month Bond", InvestmentTerm = "Nine Months" },
                new InvestmentTermMapper { Id = 20, InvestmentText = "270 Day", InvestmentTerm = "Nine Months" },
                new InvestmentTermMapper { Id = 21, InvestmentText = "9 Month Bond", InvestmentTerm = "Nine Months" },


                new InvestmentTermMapper { Id = 22, InvestmentText = "10 Month Bond", InvestmentTerm = "One Year" },
                new InvestmentTermMapper { Id = 23, InvestmentText = "11 Month Bond", InvestmentTerm = "One Year" },
                new InvestmentTermMapper { Id = 24, InvestmentText = "1 Year Bond", InvestmentTerm = "One Year" },

                new InvestmentTermMapper { Id = 25, InvestmentText = "18 Month Bond", InvestmentTerm = "Two Years" },
                new InvestmentTermMapper { Id = 26, InvestmentText = "2 Year Bond", InvestmentTerm = "Two Years" },

                

                new InvestmentTermMapper { Id = 27, InvestmentText = "3 Year Bond", InvestmentTerm = "Three Years" },
                
                
                new InvestmentTermMapper { Id = 28, InvestmentText = "4 Year Bond", InvestmentTerm = "Three Years" },
                new InvestmentTermMapper { Id = 29, InvestmentText = "5 Year Bond", InvestmentTerm = "Three Years" });

            modelBuilder.Entity<IllustrationDetail>().HasData(
                new IllustrationDetail {Id =1,PartnerEmail=" ",PartnerName=" ",ClientName=" ",Currency="GBP",PartnerOrganisation=" ",Status="Invalid" ,ClientType=0,IllustrationUniqueReference= "ICS-20200218-199999", TotalDeposit=0,TwoYears=0,ThreeYearsPlus=0,NineMonths=0,SixMonths=0,ThreeMonths=0,OneMonth=0,OneYear=0,EasyAccess=0 });

            base.OnModelCreating(modelBuilder);
            
        }

        //Database Entities
        public DbSet<IllustrationDetail> IllustrationDetails { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<TempInstitution>TempInstitution { get; set; }
        public DbSet<InvestmentTermMapper> InvestmentTermMapper { get; set; }
        public DbSet<ExcludedInstitute> ExcludedInstitutes { get; set; }
    }
}
