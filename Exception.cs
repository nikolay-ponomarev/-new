using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class TaskCountLimitException : Exception
    {
        public int TaskCountLimit { get; }

        public TaskCountLimitException(int taskCountLimit)
            : base($"Превышено максимальное количество задач равное {taskCountLimit}")
        {
            TaskCountLimit = taskCountLimit;
        }
    }
    public class TaskLengthLimitException : Exception
    {
        public int TaskLength { get; }
        public int TaskLengthLimit { get; }

        public TaskLengthLimitException(int taskLength, int taskLengthLimit)
            : base($"Длина задачи '{taskLength}' превышает максимально допустимое значение {taskLengthLimit}")
        {
            TaskLength = taskLength;
            TaskLengthLimit = taskLengthLimit;
        }
    }

    public class DuplicateTaskException : Exception
    {
        public string Task { get; }

        public DuplicateTaskException(string task)
            : base($"Задача '{task}' уже существует")
        {
            Task = task;
        }
    }

}
