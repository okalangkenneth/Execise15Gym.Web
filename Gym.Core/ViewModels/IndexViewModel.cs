﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Core.ViewModels
{
   public class IndexViewModel
    {
        public IEnumerable<GymClassesViewModel> GymClasses { get; set; }
        public bool ShowHistory { get; set; }
    }
}
