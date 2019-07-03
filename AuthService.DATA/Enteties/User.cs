using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.DATA.Enteties
{
    public class User: IdentityUser<Guid>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public List<RefreshToken> Tokens { get; set; }
    }
}
