﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
                
            base.OnModelCreating(modelBuilder);
        }
    }

    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToView(nameof(Account), "dbo");
            builder.HasKey(a => a.Id);
            builder.OwnsMany<Identity>(x => x.Identities, nestedBuilder =>
            {
                nestedBuilder.ToView(nameof(Identity), "dbo");
                nestedBuilder.HasKey(i => i.IdentityId);
                nestedBuilder.WithOwner()
                .HasForeignKey(x => x.AccountId)
                .HasPrincipalKey(a => a.Id);
            });                
        }
    }

   

    public record Account(int Id,string PortalUserName)
    {
        public List<Identity> Identities { get; init; } = default!;
    }

    public record Identity (string IdentityId, int AccountId, string? FirstName, string? LastName, string? Email, bool Enabled);
    
}
