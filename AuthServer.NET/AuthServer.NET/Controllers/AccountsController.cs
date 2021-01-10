using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuthServer.NET.Requests;
using AuthServer.NET.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AuthServer.NET.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountService _accountService;
        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet("Authorize")]
        public async Task<IActionResult> Authotize()
        {
            var responseCode = Request.Query["response_type"].ToString();
            var redirectUrl = Request.Query["redirect_uri"].ToString();
            var state = Request.Query["state"].ToString();
            if (string.IsNullOrEmpty(responseCode))
            {
                //  return
            }

            var splitResponseCodes = responseCode.Split(' ');
            if (splitResponseCodes.Length == 0 || splitResponseCodes.Any(x => x != "code" && x != "token"))
            {
                var url = $"{redirectUrl ?? string.Empty}?error=invalid_request&state={state ?? string.Empty}";
                Response.Headers.Add("Location", url);
                return new StatusCodeResult((int)HttpStatusCode.Found);
            }

            Debug.Write("test");
            return Ok();
        }

        [HttpPost("Token")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> Token([FromForm] TokenRequest tokenRequest)
        {
            var authorize = Request.Headers["Authorization"].ToString();
            var clientId = string.Empty;
            var clientSecret = string.Empty;
            if (!string.IsNullOrEmpty(authorize))
            {
                var authorizeSplit = authorize.Split(" ");
                if (authorizeSplit.Length != 2)
                {
                    return BadRequest();
                }

                var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(authorizeSplit.Last()));
                var decodedCredentialsSplit = decodedCredentials.Split(":");
                clientId = decodedCredentialsSplit.First();
                clientSecret = decodedCredentialsSplit.Last();
            }
            else
            {
                clientId = tokenRequest.client_id;
                clientSecret = tokenRequest.client_secret;
            }

            return Ok(await _accountService.GenerateTokenAsync(Guid.Parse("935FA90C-B06A-4BB3-B9E8-8C2DDDDCDA78"), clientId, IpAddress));
            // return Ok(_accountService.GenerateToken());
        }

        private string IpAddress
        {
            get
            {
                if (Request.Headers.ContainsKey("X-Forwarded-For"))
                {
                    return Request.Headers["X-Forwarded-For"];
                }

                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }
    }
}
