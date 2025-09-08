

using Otus.ToDoList.ConsoleBot;
using System;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Создаем сервисы
                var userService = new UserService();
                var toDoService = new ToDoService();

                // Создаем обработчик обновлений с внедренными зависимостями
                var handler = new UpdateHandler(userService, toDoService);

                // Создаем клиент бота
                var botClient = new ConsoleBotClient();

                // Запускаем получение обновлений
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Критическая ошибка: {ex.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey();
            }
        }
    }
}

