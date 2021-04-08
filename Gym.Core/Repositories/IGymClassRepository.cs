using Gym.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gym.Core.Repositories
{
    public interface IGymClassRepository
    {
        void Add(GymClass gymClass);
        bool Any(int id);
        Task<IEnumerable<GymClass>> GetAllAsync();
        Task<GymClass> GetAsync(int? id);
        Task<IEnumerable<GymClass>> GetHistoryAsync();
        Task<IEnumerable<GymClass>> GetWithBookingsAsync();
        void Remove(GymClass gymClass);
        void Update(GymClass gymClass);
        Task<GymClass> FindAsync(int? id);
    }
}