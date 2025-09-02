using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;

namespace ConsoleApp2
{


    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;

        // Конструктор с внедрением зависимостей
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }

        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                if (update.Message?.Text == null || update.Message.From == null) return;

                var messageText = update.Message.Text.Trim().ToLower();
                var chat = update.Message.Chat;
                var telegramUser = update.Message.From;

                // Получаем пользователя
                var todoUser = _userService.GetUser(telegramUser.Id);

                // Определяем доступные команды
                string[] unregisteredCommands = { "/start", "/help", "/info" };
                string[] allCommands = { "/start", "/help", "/info", "/exit", "/addtask", "/showtask", "/removetask", "/completetask", "/setmaxtasks", "/setmaxtasklength" };

                // Проверяем, зарегистрирован ли пользователь
                bool isRegistered = todoUser != null;
                string[] availableCommands = isRegistered ? allCommands : unregisteredCommands;

                // Проверка на пустой ввод или неизвестную команду
                if (string.IsNullOrWhiteSpace(messageText) || !availableCommands.Any(cmd => messageText.StartsWith(cmd)))
                {
                    if (!isRegistered)
                    {
                        botClient.SendMessage(chat, "Для использования бота необходимо зарегистрироваться. Введите команду /start");
                    }
                    else
                    {
                        botClient.SendMessage(chat, "Вы ввели неверную команду");
                    }
                    return;
                }

                // Обработка команд
                switch (messageText.Split(' ')[0])
                {
                    case "/start":
                        HandleStart(botClient, chat, telegramUser);
                        break;
                    case "/help":
                        HandleHelp(botClient, chat, isRegistered);
                        break;
                    case "/info":
                        HandleInfo(botClient, chat, todoUser);
                        break;
                    case "/exit":
                        Environment.Exit(0);
                        break;
                    default:
                        if (isRegistered)
                        {
                            HandleRegisteredCommands(botClient, chat, messageText, todoUser);
                        }
                        else
                        {
                            botClient.SendMessage(chat, "Для использования этой команды необходимо зарегистрироваться. Введите команду /start");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                botClient.SendMessage(update.Message.Chat, $"Ошибка: {ex.Message}");
            }
        }

        private void HandleStart(ITelegramBotClient botClient, Chat chat, User telegramUser)
        {
            var user = _userService.RegisterUser(telegramUser.Id, telegramUser.Username);

            string welcomeMessage = string.IsNullOrEmpty(user.TelegramUserName)
                ? $"Привет, пользователь #{user.TelegramUserId}!"
                : $"Привет, {user.TelegramUserName}!";

            botClient.SendMessage(chat, welcomeMessage);
            botClient.SendMessage(chat, "Я бот для управления задачами. Используйте /help для списка команд.");
        }

        private void HandleHelp(ITelegramBotClient botClient, Chat chat, bool isRegistered)
        {
            string helpText;

            if (isRegistered)
            {
                helpText = "Доступные команды:\n" +
                           "/start - начать работу\n" +
                           "/help - показать справку\n" +
                           "/info - информация о боте\n" +
                           "/addtask [описание] - добавить задачу\n" +
                           "/showtask - показать активные задачи\n" +
                           "/completetask [номер] - завершить задачу\n" +
                           "/removetask [номер] - удалить задачу\n" +
                           "/setmaxtasks [число] - установить макс. количество задач\n" +
                           "/setmaxtasklength [число] - установить макс. длину задачи\n" +
                           "/exit - выйти";
            }
            else
            {
                helpText = "Доступные команды для незарегистрированных пользователей:\n" +
                           "/start - зарегистрироваться и начать работу\n" +
                           "/help - показать справку\n" +
                           "/info - информация о боте";
            }

            botClient.SendMessage(chat, helpText);
        }

        private void HandleInfo(ITelegramBotClient botClient, Chat chat, ToDoUser user)
        {
            string infoText;

            if (user != null)
            {
                var activeTasks = _toDoService.GetActiveByUserId(user.UserId);
                var allTasks = _toDoService.GetAllByUserId(user.UserId);

                infoText = $"Информация о текущем состоянии:\n" +
                           $"ID пользователя: {user.UserId}\n" +
                           $"Telegram ID: {user.TelegramUserId}\n" +
                           $"Имя пользователя: {user.TelegramUserName ?? "не указано"}\n" +
                           $"Зарегистрирован: {user.RegisteredAt}\n" +
                           $"Активных задач: {activeTasks.Count}\n" +
                           $"Всего задач: {allTasks.Count}";
            }
            else
            {
                infoText = "Информация о боте:\n" +
                           "Это бот для управления задачами. Для начала работы введите команду /start";
            }

            botClient.SendMessage(chat, infoText);
        }

        private void HandleRegisteredCommands(ITelegramBotClient botClient, Chat chat, string messageText, ToDoUser user)
        {
            switch (messageText.Split(' ')[0])
            {
                case "/addtask":
                    HandleAddtask(botClient, chat, messageText, user);
                    break;
                case "/showtask":
                    HandleShowtask(botClient, chat, user);
                    break;
                case "/completetask":
                    HandleCompletetask(botClient, chat, messageText, user);
                    break;
                case "/removetask":
                    HandleRemovetask(botClient, chat, messageText, user);
                    break;
                case "/setmaxtasks":
                    HandleSetMaxTasks(botClient, chat, messageText);
                    break;
                case "/setmaxtasklength":
                    HandleSetMaxTaskLength(botClient, chat, messageText);
                    break;
            }
        }

        private void HandleAddtask(ITelegramBotClient botClient, Chat chat, string messageText, ToDoUser user)
        {
            var taskDescription = messageText.Substring("/addtask".Length).Trim();

            if (string.IsNullOrWhiteSpace(taskDescription))
            {
                botClient.SendMessage(chat, "Пожалуйста, укажите описание задачи");
                return;
            }

            try
            {
                var newTask = _toDoService.Add(user, taskDescription);
                var activeTasks = _toDoService.GetActiveByUserId(user.UserId);

                botClient.SendMessage(chat, $"Задача добавлена. ID: {newTask.Id}. Всего активных задач: {activeTasks.Count}");
            }
            catch (InvalidOperationException ex)
            {
                botClient.SendMessage(chat, ex.Message);
            }
        }

        private void HandleShowtask(ITelegramBotClient botClient, Chat chat, ToDoUser user)
        {
            var activeTasks = _toDoService.GetActiveByUserId(user.UserId).ToList();

            if (activeTasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список активных задач пуст");
                return;
            }

            // Формируем список задач с ID и датой создания
            var tasksList = string.Join("\n", activeTasks.Select((task, index) =>
                $"{index + 1}. {task.Name} - {task.CreatedAt:dd.MM.yyyy HH:mm:ss} - {task.Id}"));

            botClient.SendMessage(chat, $"Активные задачи:\n{tasksList}");
        }

        private void HandleCompletetask(ITelegramBotClient botClient, Chat chat, string messageText, ToDoUser user)
        {
            var activeTasks = _toDoService.GetActiveByUserId(user.UserId).ToList();

            if (activeTasks.Count == 0)
            {
                botClient.SendMessage(chat, "Нет активных задач для завершения");
                return;
            }

            var parts = messageText.Split(' ');
            if (parts.Length < 2 || !int.TryParse(parts[1], out int taskNumber) || taskNumber < 1 || taskNumber > activeTasks.Count)
            {
                botClient.SendMessage(chat, "Пожалуйста, укажите корректный номер активной задачи");
                return;
            }

            // Завершаем задачу
            var taskToComplete = activeTasks[taskNumber - 1];
            _toDoService.MarkCompleted(taskToComplete.Id);

            botClient.SendMessage(chat, $"Задача '{taskToComplete.Name}' завершена. Осталось активных задач: {activeTasks.Count - 1}");
        }

        private void HandleRemovetask(ITelegramBotClient botClient, Chat chat, string messageText, ToDoUser user)
        {
            var allTasks = _toDoService.GetAllByUserId(user.UserId).ToList();

            if (allTasks.Count == 0)
            {
                botClient.SendMessage(chat, "Список задач пуст");
                return;
            }

            var parts = messageText.Split(' ');
            if (parts.Length < 2 || !int.TryParse(parts[1], out int taskNumber) || taskNumber < 1 || taskNumber > allTasks.Count)
            {
                botClient.SendMessage(chat, "Пожалуйста, укажите корректный номер задачи");
                return;
            }

            try
            {
                var taskToRemove = allTasks[taskNumber - 1];
                _toDoService.Delete(taskToRemove.Id);
                botClient.SendMessage(chat, $"Задача '{taskToRemove.Name}' удалена. ID: {taskToRemove.Id}");
            }
            catch (ArgumentException ex)
            {
                botClient.SendMessage(chat, ex.Message);
            }
        }

        private void HandleSetMaxTasks(ITelegramBotClient botClient, Chat chat, string messageText)
        {
            var parts = messageText.Split(' ');
            if (parts.Length < 2 || !int.TryParse(parts[1], out int maxTasks) || maxTasks <= 0)
            {
                botClient.SendMessage(chat, "Пожалуйста, укажите корректное число для максимального количества задач");
                return;
            }

            if (_toDoService is ToDoService todoService)
            {
                todoService.SetMaxTasks(maxTasks);
                botClient.SendMessage(chat, $"Максимальное количество активных задач установлено: {maxTasks}");
            }
            else
            {
                botClient.SendMessage(chat, "Невозможно изменить максимальное количество задач");
            }
        }

        private void HandleSetMaxTaskLength(ITelegramBotClient botClient, Chat chat, string messageText)
        {
            var parts = messageText.Split(' ');
            if (parts.Length < 2 || !int.TryParse(parts[1], out int maxTaskLength) || maxTaskLength <= 0)
            {
                botClient.SendMessage(chat, "Пожалуйста, укажите корректное число для максимальной длины задачи");
                return;
            }

            if (_toDoService is ToDoService todoService)
            {
                todoService.SetMaxTaskLength(maxTaskLength);
                botClient.SendMessage(chat, $"Максимальная длина задачи установлена: {maxTaskLength}");
            }
            else
            {
                botClient.SendMessage(chat, "Невозможно изменить максимальную длину задачи");
            }
        }
    }
}


        



     
    

