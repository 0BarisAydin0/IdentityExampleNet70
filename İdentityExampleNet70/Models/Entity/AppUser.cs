﻿using Microsoft.AspNetCore.Identity;

namespace İdentityExampleNet70.Models.Entity
{
    public class AppUser : IdentityUser<int>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? CompanyName { get; set; }

    }
}
