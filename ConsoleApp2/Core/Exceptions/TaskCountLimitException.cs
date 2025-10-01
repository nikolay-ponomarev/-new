using System;

namespace ConsoleApp2.Core.Exceptions
{
    public class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(string message) : base(message) { }
    }
}
