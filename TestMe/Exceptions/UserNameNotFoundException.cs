using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    public class UserNameNotFoundException : Exception
    {
        public override string Message => "Username doesn't exist";
        public UserNameNotFoundException() : base()
        {
        }
        public UserNameNotFoundException(string message)
        : base($"User identificator:{message} doesn't exist") { }
    }
}
