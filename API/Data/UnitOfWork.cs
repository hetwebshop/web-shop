using API.Entities;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        //public IRoleRepository RoleRepository => new RoleRepository(_context, _mapper, _photoService, _userManager);

        public UnitOfWork(DataContext dataContext, IMapper mapper, UserManager<User> userManager)
        {
            _context = dataContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
