using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keycloak.Migration
{
    public  class CC3DbContext : DbContext
    {
        public CC3DbContext(DbContextOptions<CC3DbContext> options) : base(options)
        {
            
        }

        public DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .ToTable(nameof(Account), "dbo");
                
            base.OnModelCreating(modelBuilder);
        }
    }

    public record Account(string PortalUserName);
    
}
