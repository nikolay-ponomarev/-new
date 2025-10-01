using System;
using ConsoleApp2.Core.Services;
using ConsoleApp2.Infrastructure.DataAccess;
using ConsoleApp2.TelegramBot;
using Otus.ToDoList.ConsoleBot;

namespace ConsoleApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var userRepository = new InMemoryUserRepository();
                var toDoRepository = new InMemoryToDoRepository();

                var userService = new UserService(userRepository);
                var toDoService = new ToDoService(toDoRepository);
                var toDoReportService = new ToDoReportService(toDoRepository);

                var handler = new UpdateHandler(userService, toDoService, toDoReportService);
                var botClient = new ConsoleBotClient();

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

