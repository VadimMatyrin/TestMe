using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMe.Exceptions
{
    public class QuestionNotFoundException : Exception
    {
        public override string Message => "Question doesn't exist";
        public QuestionNotFoundException() : base()
        {

        }
        public QuestionNotFoundException(string message)
        : base($"Question identificator: {message} doesn't exist") { }
    }
}
