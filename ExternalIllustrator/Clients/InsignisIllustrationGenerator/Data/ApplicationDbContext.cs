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
                new InvestmentTermMapper { Id = 1, InvestmentText="35 Day", InvestmentTerm="One Month"},
                new InvestmentTermMapper { Id = 2, InvestmentText = "3 Year Bond", InvestmentTerm = "Three Years" },
                new InvestmentTermMapper { Id = 3, InvestmentText = "2 Year Bond", InvestmentTerm = "Two Years" },
                new InvestmentTermMapper { Id = 4, InvestmentText = "1 Year Bond", InvestmentTerm = "One Year" },
                new InvestmentTermMapper { Id = 5, InvestmentText = "9 Month Bond", InvestmentTerm = "Nine Months" },
                new InvestmentTermMapper { Id = 6, InvestmentText = "6 Month Bond", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 7, InvestmentText = "6 Month", InvestmentTerm = "Six Months" },
                new InvestmentTermMapper { Id = 8, InvestmentText = "95 Day", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 9, InvestmentText = "90 Day", InvestmentTerm = "Three Months" },
                new InvestmentTermMapper { Id = 10, InvestmentText = "Instant Access", InvestmentTerm = "Instant Access" },
                new InvestmentTermMapper { Id = 11, InvestmentText = "100 Day", InvestmentTerm = "Three Months" });

            modelBuilder.Entity<IllustrationDetail>().HasData(
                new IllustrationDetail {Id =1, ClientType=0,IllustrationUniqueReference= "ICS-20200218-199999", TotalDeposit=0,TwoYears=0,ThreeYearsPlus=0,NineMonths=0,SixMonths=0,ThreeMonths=0,OneMonth=0,OneYear=0,EasyAccess=0 });

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
