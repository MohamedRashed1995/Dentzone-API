using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Domain.Models;
using Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Services
{
    public class PharmacyDetailRepository : Repository<PharmacyDetail>, IPharmacyDetailRepository
    {
        public PharmacyDetailRepository(DragzaContext context) : base(context) { }
    }
}
