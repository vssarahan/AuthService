using AuthService.CORE.EF;
using AuthService.CORE.Interfaces;
using AuthService.CORE.Models;
using AuthService.DATA.Converters;
using AuthService.DATA.Dto;
using AuthService.DATA.Enteties;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.CORE.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtGenerator _jwt;
        private readonly AuthServiceContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(SignInManager<User> sim, UserManager<User> um, IJwtGenerator jwt, AuthServiceContext context,
            IConfiguration configuration)
        {
            _signInManager = sim;
            _userManager = um;
            _jwt = jwt;
            _context = context;
            _configuration = configuration;
        }

        public async Task<Response<Token>> Login(string email, string password)
        {
            try
            {
                if (email == null || password == null)
                    return new Response<Token>(400, "Invalid email or password");

                var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

                if (result.Succeeded)
                {
                    var appUser = await _userManager.FindByEmailAsync(email);
                    return await _jwt.GenerateJwt(appUser);
                }
                return new Response<Token>(400, "Invalid email or password");
            }
            catch (Exception)
            {
                return new Response<Token>(520, "Unknown error");
            }
        }

        public async Task<Response<Token>> RefreshToken(string token, string refreshToken)
        {
            try
            {
                var principal = GetPrincipalFromExpiredToken(token);
                if (principal == null)
                    return new Response<Token>(400, "Invalid access token");
                var email = principal.Identity.Name;
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return new Response<Token>(404, "User not found");
                var dbToken = _context.RefreshTokens
                    .FirstOrDefault(rt => rt.UserId == user.Id && rt.Token == refreshToken);
                if (dbToken == null)
                    return new Response<Token>(400, "Invalid refresh token");
                if (dbToken.ExpiresDate < DateTime.Now)
                {
                    _context.RefreshTokens.Remove(dbToken);
                    await _context.SaveChangesAsync();
                    return new Response<Token>(400, "Expired refresh token");
                }
                var data = await _jwt.GenerateJwt(user);
                if (data.Succeeded())
                {
                    _context.RefreshTokens.Remove(dbToken);
                    await _context.SaveChangesAsync();
                }
                return data;
            }
            catch (Exception ex)
            {
                return new Response<Token>(520, "Unknown error");
            }
        }

        public async Task<Response<Token>> Register(UserDto item)
        {
            try
            {
                User user = UserConverter.Convert(item);
                if (user == null)
                    return new Response<Token>(400, "Invalid email or password");

                var result = await _userManager.CreateAsync(user, item.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return await _jwt.GenerateJwt(user);
                }

                return new Response<Token>(400, "Invalid data");
            }
            catch (Exception)
            {
                return new Response<Token>(520, "Unknown error");
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _configuration["Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Key"])),
                    ValidateLifetime = false
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                            StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException();

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
