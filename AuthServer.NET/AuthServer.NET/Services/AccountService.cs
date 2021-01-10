using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthServer.NET.Data;
using AuthServer.NET.Models.Entities;
using AuthServer.NET.Models.Exceptions;
using AuthServer.NET.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.NET.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<TokenResponse> GenerateTokenAsync(Guid applicationUserId,
            string clientId,
            string ipAddress,
            bool generateRefreshToken = true)
        {
            var client = _context.Clients.FirstOrDefault(x => x.client_id == clientId && x.Active);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(client.client_secret);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", applicationUserId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(client.RefreshTokenExpirationMin),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            //   var jwt = GenerateJWTToken(client.client_secret, applicationUser.Id);
            var refreshTokenStr = string.Empty;
            if (generateRefreshToken)
            {
                var refreshToken = GenerateRefreshToken(ipAddress, applicationUserId);
                _context.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();
                refreshTokenStr = refreshToken.Token;
            }

            return new TokenResponse()
            {
                access_token = accessToken,
                refresh_token = refreshTokenStr,
                expire_in = (int)TimeSpan.FromMinutes(client.RefreshTokenExpirationMin).TotalSeconds,
                token_type = "Bearer"
            };
        }
        protected virtual string GenerateJWTToken(string clientSecret, Guid accountId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(clientSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", accountId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        protected virtual RefreshToken GenerateRefreshToken(string ipAddress, Guid applicationUserId)
        {
            return new RefreshToken()
            {
                Token = GenerateRandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                ApplicationUserId = applicationUserId
            };
        }

        protected virtual string GenerateRandomTokenString()
        {
            using var cryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            cryptoServiceProvider.GetBytes(randomBytes);
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        public async Task<Guid> ValidateTokenAsync(string clientId, string token)
        {
            try
            {
                var client = _context.Clients.FirstOrDefault(x => x.client_id == clientId && x.Active);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(client.client_secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);


            }
            catch (Exception e)
            {

            }
            return Guid.Empty;
        }

        public virtual async Task<RefreshToken> GetRefreshTokenAsync(string refreshToken)
        {
            var refreshTokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken && x.Active);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
            {
                throw new AppException("Invalid Token");
            }


            var applicationUser =
                await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == refreshTokenEntity.ApplicationUserId && x.Active);
            return refreshTokenEntity;
        }
        public virtual async Task<bool> RevokeToken(string refreshToken, string ipAdderss)
        {
            var refreshTokenEntity = await GetRefreshTokenAsync(refreshToken);
            refreshTokenEntity.Revoked = DateTime.UtcNow;
            refreshTokenEntity.RevokedByIp = ipAdderss;
            _context.Update(refreshTokenEntity);
            return await _context.SaveChangesAsync() > 0;

        }
    }
}
