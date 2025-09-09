using System;

namespace ConsoleApp2.Core.Exceptions
{
    public class DuplicateTaskException : Exception
    {
        public DuplicateTaskException(string message) : base(message) { }
    }
}