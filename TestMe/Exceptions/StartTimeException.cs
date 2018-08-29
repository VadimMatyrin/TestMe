using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    [Serializable]
    public class TestTimeException : Exception
    {
        public override string Message => "Test's start time isn't set";
        public TestTimeException() : base()
        {
        }
        public TestTimeException(string message)
        : base($"Test's start time with an identificator:{message} isn't set") { }
    }
}
