using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AuthServer.NET.Controllers
{
    public class AccountsController : Controller
    {
        [HttpGet("Authorize")]
        public async Task Authotize()
        {
            var responseCode = Request.Query["response_type"].ToString();
            if (string.IsNullOrEmpty(responseCode))
            {
              //  return
            }

            var splitResponseCodes = responseCode.Split(' ');
            if (splitResponseCodes.Length == 0)
            {

            }

        }

        [HttpPost("Token")]
        public async Task Token()
        {

        }
    }
}
