using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.NET.Models.Entities;

namespace AuthServer.NET.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientAuthCode> ClientsAuthCodes { get; set; }
        public DbSet<ClientImplicit> ClientsImplicits { get; set; }
        public DbSet<ClientROPassword> ClientsRoPasswords { get; set; }
        public DbSet<ClientClientCredentials> ClientsClientCredentialses { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser[]
            {
                new ApplicationUser()
                {
                 Id =Guid.Parse("935FA90C-B06A-4BB3-B9E8-8C2DDDDCDA78")
                    

                }
});
            modelBuilder.Entity<ClientImplicit>().HasData(new ClientImplicit[]
            {
                new ClientImplicit()
                {
                    ApplicationUserId = Guid.Parse("935FA90C-B06A-4BB3-B9E8-8C2DDDDCDA78"),
                    client_id = "TestClient",
                    client_secret = "TestSecret",
                    RefreshTokenExpirationMin = 15,
                    IssureRefreshTokens = true
                }
            });
        }
    }
}
