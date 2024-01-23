using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFA.Domain
{
    public interface IGuidFactory
    {
        public Guid Create();
    }
    public class GuidFactory : IGuidFactory
    {
        public Guid Create() => Guid.NewGuid();
    }
}
