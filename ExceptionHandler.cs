using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public static class ExceptionHandler

    {
        public static void HandleGlobalException(Exception ex)
        {
            Console.WriteLine("Произошла непредвиденная ошибка:");

            Exception? current = ex;
            int level = 0;

            while (current != null)
            {
                Console.WriteLine($"\n--- Уровень {level} ---");
                Console.WriteLine($"Тип: {current.GetType().Name}");
                Console.WriteLine($"Сообщение: {current.Message}");
                Console.WriteLine($"StackTrace: {current.StackTrace}");

                current = current.InnerException; // Безопасное преобразование
                level++;
            }
        }

        public static int GetMaxTasksInput()
        {
            Console.WriteLine("Введите максимально допустимое количество задач");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int result) || result < 1 || result > 100)
            {
                throw new ArgumentException("Введите число от 1 до 100.");
            }

            return result;
        }

        public static int GetMaxTaskLengthInput()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи");
            string input = Console.ReadLine();

            if (!int.TryParse(input, out int result) || result < 1 || result > 100)
            {
                throw new ArgumentException("Введите число от 1 до 100.");
            }

            return result;
        }

    }

}

