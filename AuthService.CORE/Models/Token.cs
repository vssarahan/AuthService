using System;
using System.Collections.Generic;
using System.Text;

namespace AuthService.CORE.Models
{
    public class Token
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public long ExpiresIn { get; set; }
    }
}
