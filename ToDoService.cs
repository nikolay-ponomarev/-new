using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class ToDoService : IToDoService
    {
        private readonly Dictionary<Guid, List<ToDoItem>> _userTasks = new Dictionary<Guid, List<ToDoItem>>();
        private int _maxTasks = 10;
        private int _maxTaskLength = 50;

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            if (_userTasks.TryGetValue(userId, out var tasks))
            {
                return tasks.AsReadOnly();
            }
            return new List<ToDoItem>().AsReadOnly();
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            if (_userTasks.TryGetValue(userId, out var tasks))
            {
                return tasks.Where(t => t.State == ToDoItemState.Active).ToList().AsReadOnly();
            }
            return new List<ToDoItem>().AsReadOnly();
        }

        public ToDoItem Add(ToDoUser user, string name)
        {
            // Проверяем максимальное количество задач
            var activeTasks = GetActiveByUserId(user.UserId);
            if (activeTasks.Count >= _maxTasks)
            {
                throw new InvalidOperationException($"Достигнуто максимальное количество активных задач: {_maxTasks}");
            }

            // Проверяем максимальную длину задачи
            if (name.Length > _maxTaskLength)
            {
                throw new InvalidOperationException($"Превышена максимальная длина задачи. Максимум: {_maxTaskLength} символов");
            }

            // Проверяем на дубликаты активных задач
            if (activeTasks.Any(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException("Такая активная задача уже существует");
            }

            // Создаем новую задачу
            var newTask = new ToDoItem(user, name);

            // Добавляем задачу в список пользователя
            if (!_userTasks.ContainsKey(user.UserId))
            {
                _userTasks[user.UserId] = new List<ToDoItem>();
            }

            _userTasks[user.UserId].Add(newTask);
            return newTask;
        }

        public void MarkCompleted(Guid id)
        {
            var task = FindTaskById(id);
            if (task != null)
            {
                task.State = ToDoItemState.Completed;
                task.StateChangedAt = DateTime.Now;
            }
        }

        public void Delete(Guid id)
        {
            foreach (var userTasks in _userTasks.Values)
            {
                var task = userTasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    userTasks.Remove(task);
                    return;
                }
            }

            throw new ArgumentException("Задача с указанным ID не найдена");
        }

        private ToDoItem? FindTaskById(Guid id)
        {
            return _userTasks.Values
                .SelectMany(tasks => tasks)
                .FirstOrDefault(task => task.Id == id);
        }

        public void SetMaxTasks(int maxTasks)
        {
            _maxTasks = maxTasks;
        }

        public void SetMaxTaskLength(int maxTaskLength)
        {
            _maxTaskLength = maxTaskLength;
        }
    }
}
