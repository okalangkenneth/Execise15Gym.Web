using Gym.Core.Repositories;
using System.Threading.Tasks;

namespace Gym.Core.Repositories
{
   public interface IUnitOfWork
    {
        IApplicationUserGymClassRepository AppUserRepo { get; }
        IGymClassRepository GymClassRepository { get; }

        Task CompleteAsync();
    }
}