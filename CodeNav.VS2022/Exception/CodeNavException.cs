using System;

namespace CodeNav.Exceptions
{
    public class CodeNavException : System.Exception
    {
        public CodeNavException(string message, Exception? innerException = null) : base(message, innerException) { }
    }
}
