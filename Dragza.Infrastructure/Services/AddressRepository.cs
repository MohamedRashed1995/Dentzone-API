using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DragzaContext _context;

        public AddressRepository(DragzaContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetAllAsync()
        {
            return _context.Addresses.ToList();
        }

        public async Task<Address> GetByIdAsync(Guid id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task AddAsync(Address address)
        {
            await _context.Addresses.AddAsync(address);
        }

        public void Update(Address address)
        {
            _context.Addresses.Update(address);
        }

        public void Delete(Address address)
        {
            _context.Addresses.Remove(address);
        }
    }
}
