using AuthService.CORE.Models;
using AuthService.DATA.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.CORE.Interfaces
{
    public interface IAuthService
    {
        Task<Response<Token>> Login(string email, string password);

        Task<Response<Token>> Register(UserDto item);

        Task<Response<Token>> RefreshToken(string token, string refreshToken);
    }
}
