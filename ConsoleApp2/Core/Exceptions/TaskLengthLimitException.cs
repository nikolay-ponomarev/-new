using System;

namespace ConsoleApp2.Core.Exceptions
{
    public class TaskLengthLimitException : Exception
    {
        public TaskLengthLimitException(string message) : base(message) { }
    }
}