using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class UserRepository : IRepository<User>, IUserRepository
    {
        private readonly DragzaContext _context;

        public UserRepository(DragzaContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users
               .Include(s=>s.SubArea)
                .Include(a=>a.Region)
                .Include(u => u.BalanceAccounts)
                .Include(u => u.UserRoles)
                 .ThenInclude(ur => ur.Role)
                .Include(u=>u.PharmacyDetailUsers)
                
               
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> FindByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<User>> GetAllAsync(
          Expression<Func<User, bool>> filter = null,
          Func<IQueryable<User>, IIncludableQueryable<User, object>> include = null)
        {
            IQueryable<User> query = _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role);

            if (include != null)
            {
                query = include(query);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
        {
            return await _context.Users
                        .Where(predicate)
                        .Include(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                        .ToListAsync();
        }

        public async Task AddAsync(User entity)
        {
            await _context.Users.AddAsync(entity);
        }

        public void Update(User entity)
        {
            _context.Users.Update(entity);
        }

        public void Delete(User entity)
        {
            _context.Users.Remove(entity);
        }
        // Implement other IRepository methods...

        public async Task<List<User>> GetUsersByRoleWithPharmacyAsync(Guid roleId)
        {
            return await _context.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId))
                 .Include(u => u.PharmacyDetailUsers)  // Ensure related data is loaded
                 .Include(u => u.Region)
                 .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }
    }
}
