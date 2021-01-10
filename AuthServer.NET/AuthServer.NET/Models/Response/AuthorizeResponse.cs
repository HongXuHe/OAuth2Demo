using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.NET.Models.Response
{
    public class AuthorizeResponse
    {
        public string redirect_uri { get; set; }
        public string state { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
        public string error_uri { get; set; }
    }
}
