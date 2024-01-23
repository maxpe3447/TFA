using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFA.Domain
{
    public interface IMomentProvider
    {
        DateTimeOffset Now { get; }
    }

    public class MomentProvider: IMomentProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
