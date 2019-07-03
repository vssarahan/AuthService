using AuthService.CORE.Models;
using AuthService.DATA.Enteties;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.CORE.Interfaces
{
    public interface IJwtGenerator
    {
        Task<Response<Token>> GenerateJwt(User user);
    }
}
