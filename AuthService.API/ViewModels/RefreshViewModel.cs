﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.API.ViewModels
{
    public class RefreshViewModel
    {
        public string AccessToken { get; set; }
        
        public string RefreshToken { get; set; }
    }
}
