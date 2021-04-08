using Gym.Core.Entities;
using Gym.Core.Repositories;
using Gym.Data.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Data.Repositories
{
    public class ApplicationUserGymClassRepository : IApplicationUserGymClassRepository
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserGymClassRepository(ApplicationDbContext db)
        {
            this.db = db;
        }


        public async Task<ApplicationUserGymClass> GetAttending(int? id, string userId)
        {
            return await db.ApplicationUserGyms.FindAsync(userId, id);
        }

        public async Task<IEnumerable<ApplicationUserGymClass>> GetBookingsAsync(string userId)
        {
            return await db.ApplicationUserGyms
                                           .Include(g => g.GymClass)
                                           .IgnoreQueryFilters()
                                           .Where(u => u.ApplicationUserId == userId)
                                           .ToListAsync();

        }

        public void Add(ApplicationUserGymClass attending)
        {
            db.Add(attending);
        }

        public void Remove(ApplicationUserGymClass attending)
        {
            db.Remove(attending);
        }
    }
}
