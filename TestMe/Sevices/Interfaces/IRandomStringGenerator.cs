using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Sevices.Interfaces
{
    public interface IRandomStringGenerator
    {
        string RandomString(int length);
    }
}
