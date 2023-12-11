﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pustokk.Core.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public string BirthDate { get; set; }
    }
}
