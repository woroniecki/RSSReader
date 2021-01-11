﻿using AutoMapper;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RSSReader.Dtos;
using RSSReader.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSSReader.Data
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;

        public AuthService(IConfiguration config, DataContext context)
        {
            this._config = config;
            this._context = context;
        }
        public string CreateAuthToken(string id, string name, out DateTime expiresTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            
            expiresTime = DateTime.UtcNow.AddHours(3);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[]{
                                new Claim(ClaimTypes.NameIdentifier, id),
                                new Claim(ClaimTypes.Name, name)
                    }
                ),
                //Issuer = "http://localhost:5000/",//TODO
                //Audience = "http://localhost:5000/",//TODO
                Expires = expiresTime,
                SigningCredentials = creds,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<RefreshToken> CreateRefreshToken(ApiUser user)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                var refreshToken = new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddMinutes(20),
                    Created = DateTime.UtcNow
                };

                user.RefreshTokens.Add(refreshToken);
                
                if (await _context.SaveChangesAsync() > 0)
                    return refreshToken;
            }
            return null;
        }

        public string GetUserIdFromToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_config.GetSection("AppSettings:Token").Value)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            var id_claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return id_claim != null ? id_claim.Value : null;
        }
    }

    public interface IAuthService
    {
        public Task<RefreshToken> CreateRefreshToken(ApiUser user);
        public string CreateAuthToken(string id, string name, out DateTime expiresTime);
        public string GetUserIdFromToken(string token);
    }
}
