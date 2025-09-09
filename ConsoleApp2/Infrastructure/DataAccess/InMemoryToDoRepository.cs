using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp2.Core.DataAccess;
using ConsoleApp2.Core.Entities;

namespace ConsoleApp2.Infrastructure.DataAccess
{
    public class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _items = new List<ToDoItem>();

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _items.Where(i => i.User.UserId == userId).ToList().AsReadOnly();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _items.Where(i => i.User.UserId == userId && i.State == ToDoItemState.Active).ToList().AsReadOnly();
        }

        public ToDoItem? Get(Guid id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public void Add(ToDoItem item)
        {
            _items.Add(item);
        }

        public void Update(ToDoItem item)
        {
            var existingItem = Get(item.Id);
            if (existingItem != null)
            {
                _items.Remove(existingItem);
                _items.Add(item);
            }
        }

        public void Delete(Guid id)
        {
            var item = Get(id);
            if (item != null)
            {
                _items.Remove(item);
            }
        }

        public bool ExistsByName(Guid userId, string name)
        {
            return _items.Any(i => i.User.UserId == userId &&
                                  i.Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                                  i.State == ToDoItemState.Active);
        }

        public int CountActive(Guid userId)
        {
            return _items.Count(i => i.User.UserId == userId && i.State == ToDoItemState.Active);
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            return _items.Where(i => i.User.UserId == userId).Where(predicate).ToList().AsReadOnly();
        }
    }
}