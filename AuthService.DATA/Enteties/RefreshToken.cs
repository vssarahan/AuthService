using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.DATA.Enteties
{
    public class RefreshToken
    {
        public string Token { get; set; }

        public Guid UserId { get; set; }

        public DateTime ExpiresDate { get; set; }
    }
}
