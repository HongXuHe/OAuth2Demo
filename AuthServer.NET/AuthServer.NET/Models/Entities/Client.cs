using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.NET.Models.Entities
{
    [Table("Clients")]
    public abstract class Client : BaseEntity
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public bool IssureRefreshTokens { get; set; }
        public int RefreshTokenExpirationMin { get; set; } = 15;
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
