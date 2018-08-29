using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    public class AnswerNotFoundException : Exception
    {
        public override string Message => "Answer doesn't exist";
        public AnswerNotFoundException() : base()
        {
        }
        public AnswerNotFoundException(string message)
        : base($"Answer identificator:{message} doesn't exist") { }
    }
}
