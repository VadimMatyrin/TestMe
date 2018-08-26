using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    public class UserAnswersException : Exception
    {
        public override string Message => "Invalid user answers";
        public UserAnswersException() : base()
        {
        }
    }
}
