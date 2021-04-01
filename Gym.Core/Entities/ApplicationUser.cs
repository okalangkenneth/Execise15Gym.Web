﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        //Lägg till egna props sen

        public ICollection<ApplicationUserGymClass> AttendedClasses { get; set; }
    }
}
