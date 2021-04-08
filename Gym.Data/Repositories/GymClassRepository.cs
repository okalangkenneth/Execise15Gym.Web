using Gym.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Data.Repositories
{
   public class GymClassRepository
    {
        private readonly ApplicationDbContext db;

        public GymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }


    }
}
