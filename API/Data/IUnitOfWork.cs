using System.Threading.Tasks;

namespace API.Data
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        //ISearchRepository SearchRepository { get; }
        //IRoleRepository RoleRepository { get; }
        Task<bool> SaveChanges();
        bool HasChanges();
    }
}
