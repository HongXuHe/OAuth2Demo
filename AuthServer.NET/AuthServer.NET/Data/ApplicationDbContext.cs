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
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientAuthCode> ClientsAuthCodes { get; set; }
        public DbSet<ClientImplicit> ClientsImplicits { get; set; }
        public DbSet<ClientROPassword> ClientsRoPasswords { get; set; }
        public DbSet<ClientClientCredentials> ClientsClientCredentialses { get; set; }
    }
}
