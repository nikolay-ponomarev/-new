using System;
using System.Collections.Generic;
using ConsoleApp2.Core.Entities;

namespace ConsoleApp2.Core.DataAccess
{
    public interface IToDoRepository
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem? Get(Guid id);
        void Add(ToDoItem item);
        void Update(ToDoItem item);
        void Delete(Guid id);
        bool ExistsByName(Guid userId, string name);
        int CountActive(Guid userId);
        IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate);
    }
}