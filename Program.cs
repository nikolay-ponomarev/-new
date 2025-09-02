

using System;
using System.Security.Principal;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp2
{
    public class Program
    {
        public static void Main(string[] args)
        {
           

                const string Start = "/start";
                const string Help = "/help";
                const string Info = "/info";
                const string Exit = "/exit";
                const string Echo = "/echo";
                const string Addtask = "/addtask";
                const string Showtask = "/showtask";
                const string Removetask = "/removetask";
                int maxTasks = 0;
                int maxTaskLength = 0;

            try
            {
                // Ввод максимального количества задач
                try
                {
                    maxTasks = ExceptionHandler.GetMaxTasksInput();
                    Console.WriteLine($"Максимальное количество задач установлено: {maxTasks}");
                }

                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // Ввод максимальной длины задачи
                try
                {
                    maxTaskLength = ExceptionHandler.GetMaxTaskLengthInput();
                    Console.WriteLine($"Максимальная длина задачи установлена: {maxTaskLength}");
                }

                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                string[] allKnownCommands = { Start, Help, Info, Exit, Echo, Addtask, Showtask, Removetask };  // Все известные команды (включая Echo для проверки)

                Console.WriteLine("Добрый день! Доступные команды:");
                Console.WriteLine($"{Start}, {Help}, {Info}, {Exit}, {Echo}, {Addtask}, {Showtask}, {Removetask}");
                Console.WriteLine("Примечание: команда Echo доступна после ввода имени через команду /start");


                string command;
                do
                {
                    Console.WriteLine("Введите команду:");
                    command = Console.ReadLine()?.Trim().ToLower();

                    bool isEchoAvailable = !string.IsNullOrWhiteSpace(CommandHandler.Name);

                    // Проверка на пустой ввод или неизвестную команду
                    if (string.IsNullOrWhiteSpace(command) || !allKnownCommands.Any(cmd => command.StartsWith(cmd)))
                    {
                        Console.WriteLine("Вы ввели неверную команду");
                        continue;
                    }

                    try
                    {
                        switch (command.Split(' ')[0])
                        {
                            case Start:
                                CommandHandler.HandleStart();
                                break;
                            case Help:
                                CommandHandler.HandleHelp();
                                break;
                            case Info:
                                CommandHandler.HandleInfo();
                                break;
                            case Addtask:
                                CommandHandler.HandleAddtask(maxTasks, maxTaskLength); // Передаем maxTasks
                                break;
                            case Showtask:
                                CommandHandler.HandleShowtask();
                                break;
                            case Removetask:
                                CommandHandler.HandleRemovetask();
                                break;
                            case Exit:
                                Environment.Exit(0);
                                break;
                            case Echo:
                                if (isEchoAvailable)
                                {
                                    CommandHandler.HandleEcho(command);
                                }
                                else
                                {
                                    Console.WriteLine($"Уважаемый {CommandHandler.Name}! /Echo недоступна,без ввода имени в команде /start");
                                }
                                break;
                            default:
                                Console.WriteLine("Неизвестная команда");
                                break;
                        }
                    }
                    catch (TaskCountLimitException ex)
                    {
                    Console.WriteLine(ex.Message); // Обработка TaskCountLimitException
                    }

                    catch (TaskLengthLimitException ex)
                    {
                        Console.WriteLine(ex.Message); // Выводим только сообщение, приложение продолжает работу
                    }
                    catch (DuplicateTaskException ex) // Добавлен новый обработчик
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                    Console.WriteLine($"Ошибка при выполнении команды: {ex.Message}");
                    }

                } while (command != Exit);
            }

            catch (Exception ex) 
            {
                ExceptionHandler.HandleGlobalException(ex); 
            }
            finally
            {
                Console.WriteLine("Нажмите любую клавишу для выхода...");
                Console.ReadKey(); // Логика завершения в finally
            }
        }
    }
}
