using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp2.Core.DataAccess;
using ConsoleApp2.Core.Entities;
using ConsoleApp2.Core.Exceptions;

namespace ConsoleApp2.Core.Services
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        private int _maxTasks = 10;
        private int _maxTaskLength = 50;

        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _toDoRepository.GetAllByUserId(userId);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _toDoRepository.GetActiveByUserId(userId);
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            if (_toDoRepository.CountActive(user.UserId) >= _maxTasks)
            {
                throw new TaskCountLimitException($"Достигнуто максимальное количество активных задач: {_maxTasks}");
            }

            if (name.Length > _maxTaskLength)
            {
                throw new TaskLengthLimitException($"Превышена максимальная длина задачи. Максимум: {_maxTaskLength} символов");
            }

            if (_toDoRepository.ExistsByName(user.UserId, name))
            {
                throw new DuplicateTaskException("Такая активная задача уже существует");
            }

            var newTask = new ToDoItem(user, name);
            _toDoRepository.Add(newTask);
            return newTask;
        }

        public void MarkCompleted(Guid id)
        {
            var task = _toDoRepository.Get(id);
            if (task != null)
            {
                task.State = ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;
                _toDoRepository.Update(task);
            }
        }

        public void Delete(Guid id)
        {
            _toDoRepository.Delete(id);
        }

        public void SetMaxTasks(int maxTasks)
        {
            _maxTasks = maxTasks;
        }

        public void SetMaxTaskLength(int maxTaskLength)
        {
            _maxTaskLength = maxTaskLength;
        }

        public IReadOnlyList<ToDoItem> Find(ToDoUser user, string namePrefix)
        {
            return _toDoRepository.Find(user.UserId, item =>
                item.Name.StartsWith(namePrefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}