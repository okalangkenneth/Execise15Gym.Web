using Gym.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gym.Core.Repositories
{
    public interface IApplicationUserGymClassRepository
    {
        void Add(ApplicationUserGymClass attending);
        Task<ApplicationUserGymClass> GetAttending(int? id, string userId);
        Task<IEnumerable<GymClass>> GetBookingsAsync(string userId);
        void Remove(ApplicationUserGymClass attending);
    }
}