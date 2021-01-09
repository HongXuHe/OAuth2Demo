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
    }
}
