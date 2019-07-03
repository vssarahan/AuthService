using AuthService.CORE.EF;
using AuthService.CORE.Interfaces;
using AuthService.CORE.Models;
using AuthService.CORE.Services.Models;
using AuthService.DATA.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.CORE.Services
{
    public class JwtGenerator: IJwtGenerator
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AuthServiceContext _context;

        public JwtGenerator(UserManager<User> um,
            IConfiguration conf, AuthServiceContext context)
        {
            _userManager = um;
            _configuration = conf;
            _context = context;
        }

        public async Task<Response<Token>> GenerateJwt(User user)
        {
            try
            {
                var accessToken = await GenerateAccessToken(user);
                if (accessToken == null)
                    return new Response<Token>(500, "Failed to create access token");

                var refreshToken = await GenerateRefreshToken(user.Id);
                if (refreshToken == null)
                    return new Response<Token>(500, "Failed to create refresh token");

                var data = new Token
                {
                    AccessToken = accessToken.Token,
                    RefreshToken = refreshToken,
                    ExpiresIn = accessToken.ExpiresIn
                };

                return new Response<Token>(data);
            }
            catch (Exception)
            {
                return new Response<Token>(520, "Unknown error");
            }
        }

        private async Task<AccessToken> GenerateAccessToken(User user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");


                claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddMinutes(Convert.ToDouble(10));
                var dateTimeOffset = new DateTimeOffset(expires);

                var token = new JwtSecurityToken(
                    _configuration["Issuer"],
                    _configuration["Audience"],
                    claimsIdentity.Claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new AccessToken
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresIn = dateTimeOffset.ToUnixTimeSeconds() - 30
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> GenerateRefreshToken(Guid id)
        {
            try
            {
                var randomNumber = new byte[32];
                string token;
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(randomNumber);
                    token = Convert.ToBase64String(randomNumber);
                }

                await _context.RefreshTokens.AddAsync(new RefreshToken
                {
                    UserId = id,
                    Token = token,
                    ExpiresDate = DateTime.Now.AddDays(Convert.ToDouble(30))
                });

                await _context.SaveChangesAsync();
                return token;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
