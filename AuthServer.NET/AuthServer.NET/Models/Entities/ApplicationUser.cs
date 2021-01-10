using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.NET.Models.Entities
{
    [Table("ApplicationUsers")]
    public class ApplicationUser:BaseEntity
    {
        public IEnumerable<Client> Clients { get; set; } = new List<Client>();
        public IEnumerable<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
