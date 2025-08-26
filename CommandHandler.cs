using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApp2
{
    public static class CommandHandler
    {
        public static string Name;
        static readonly List<string> tasks = [];
        //private static int taskCount = 0;

        internal static string HandleStart() // команда /start
        {
           string NameVvod;

            {
                do
                {
                    Console.WriteLine("Введите имя пользователя (не пустое):");
                    NameVvod = Console.ReadLine();
                }
                while (string.IsNullOrWhiteSpace(NameVvod));  // проверка введенного имени на пустое или нулевое значение
                Name = NameVvod;
                Console.WriteLine($"Веедено имя пользователя: {Name}");
                return Name;
                
            }
        }

        internal static void HandleHelp() // команда /help
        {
            Console.WriteLine($"Уважаемый {Name}! Для использования программы введите нужную команду. Для выхода введите exit");
            Console.WriteLine($"Уважаемый {Name}! Команды Addtask,Showtask,Removetask служат для работы со списком задач");
        }


        internal static void HandleInfo() // команда /info
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            var buildDate = GetBuildDate(assembly);

            Console.WriteLine($"Уважаемый {Name}! Информация о сборке:");
            Console.WriteLine($"• Версия: {version}");
            Console.WriteLine($"• Дата сборки: {buildDate:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"• Название сборки: {assembly.GetName().Name}");
        }


        private static DateTime GetBuildDate(Assembly assembly)
        {
            return File.GetLastWriteTime(assembly.Location);
        }


        internal static void HandleEcho(string text) // команда /echo
        {
            const string commandecho = "/echo ";
            // Если строка равна команде или короче — ничего не делать
            if (text.Length <= commandecho.Length || !text.StartsWith(commandecho))
            {
                Console.WriteLine($"Уважаемый {Name}! Команда не содержит текста или неверный формат.");
                return;
            }
            // Извлекаем текст после команды
            string textvvod = text.Substring(commandecho.Length);
            Console.WriteLine($"Уважаемый {Name}! Введен аргумент {textvvod}");
        }

        internal static void HandleAddtask(int maxTasks, int maxTaskLength) // команда /addtask с проверкой лимита
        {
            // Проверка на превышение максимального количества задач
            if (tasks.Count >= maxTasks)
            {
                throw new TaskCountLimitException(maxTasks);
            }

            string task;
            do
            {
                Console.WriteLine($"Уважаемый {Name}! Введите задачу для добавления (не пустое):");
                task = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(task));  // проверка введенного имени на пустое или нулевое значение

            // Проверка длины задачи
            if (task.Length > maxTaskLength)
            {
                throw new TaskLengthLimitException(task.Length, maxTaskLength);
            }

            // Проверка на дубликат задачи (без учета регистра)
            if (tasks.Any(t => string.Equals(t, task, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateTaskException(task);
            }

            tasks.Add(task);
            Console.WriteLine($"Задача добавлена: {task}");
            Console.WriteLine($"Текущее количество задач: {tasks.Count}/{maxTasks}");

        }
        internal static void HandleShowtask() // команда /Showtask
        {
            if (tasks is null or { Count: 0})
            {
                Console.WriteLine($"Уважаемый {Name}! Нет задач для отображения");
                return;
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {tasks[i]}"); // +1 чтобы нумерация начиналась с 1
            }
        }
        
        internal static void HandleRemovetask() // команда /Removetask
        {
            HandleShowtask();
            if (tasks is null or { Count: 0 })
                return;


            string userResponse;
            do
            {
                Console.WriteLine($"Уважаемый {Name}! Удалить эелементы из списка (да/нет)?");
                userResponse = Console.ReadLine()?.Trim().ToLower();
            }
            while (userResponse != "да" && userResponse != "нет"); // проверка введенного имени на пустое или нулевое значение

            if (userResponse == "нет")
            {
                Console.WriteLine("Удаление отменено");
                return;
            }

            bool success;
            do
            {
                Console.Write("Введите номер задачи для удаления: ");
                string input = Console.ReadLine();

                // Проверка корректности ввода
                success = int.TryParse(input, out int taskNumber) && taskNumber > 0 && taskNumber <= tasks.Count;

                if (success)
                {
                    // Удаление и подтверждение
                    string removedTask = tasks[taskNumber - 1];
                    tasks.RemoveAt(taskNumber - 1);
                    Console.WriteLine($"Задача \"{removedTask}\" удалена");
                }
                else
                {
                    Console.WriteLine("Некорректный номер! Пожалуйста, введите число из списка");
                }    
            }
            while (!success);    
        }
    } 
}
