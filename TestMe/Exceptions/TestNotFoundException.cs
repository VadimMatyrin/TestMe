using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    [Serializable]
    public class TestNotFoundException : Exception
    {
        public override string Message => "Test doesn't exist";
        public TestNotFoundException() : base()
        {
        }
        public TestNotFoundException(string message)
        : base($"Test identificator:{message} doesn't exist") { }
    }
}
