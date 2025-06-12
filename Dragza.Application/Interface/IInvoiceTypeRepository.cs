using Dragza.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Application.Interface
{
    public interface IInvoiceTypeRepository : IRepository<InvoiceType>
    {
        Task<InvoiceType> GetByNameAsync(string Name);
    }
}
