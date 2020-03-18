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
        
            base.OnModelCreating(modelBuilder);
        }

        //Database Entities
        public DbSet<IllustrationDetail> IllustrationDetails { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<TempInstitution>TempInstitution { get; set; }
    }
}
