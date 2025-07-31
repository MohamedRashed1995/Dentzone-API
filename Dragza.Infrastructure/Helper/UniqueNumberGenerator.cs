using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragza.Infrastructure.Helper
{
    public class UniqueNumberGenerator
    {
        private static long _counter = DateTime.UtcNow.Ticks;

        public static long GenerateUniqueNumber()
        {
            return Interlocked.Increment(ref _counter);
        }
    }
}
