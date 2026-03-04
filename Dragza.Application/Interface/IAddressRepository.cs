using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetAllAsync();
        Task<Address> GetByIdAsync(Guid id);
        Task AddAsync(Address address);
        void Update(Address address);
        void Delete(Address address);
    }
}
