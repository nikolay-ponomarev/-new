using System.Collections.Generic;
using ConsoleApp2.Core.Entities;

namespace ConsoleApp2.Core.Services
{
    public interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
        void SetMaxTasks(int maxTasks);
        void SetMaxTaskLength(int maxTaskLength);
        IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix);
    }
}