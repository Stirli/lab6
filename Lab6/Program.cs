using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Lab6.Task4;

namespace Lab6
{
    class Program
    {

        
        private static void Task1()
        {
            Console.WriteLine("Задача 1");
            // Разбираем текстовый файл на символы.
            IEnumerable<char> chars = File.ReadLines("textfile.txt").SelectMany(ch => ch);
            // Создаем стек
            Stack<char> charStack = new Stack<char>(chars);
            Console.WriteLine("Содержимое файла в обратном порядке:");
            // Создаем и выводим на экран строку на основе стека символов. (читая стек, метод ToArray() вернет символы в обратном порядке)
            Console.WriteLine(new string(charStack.ToArray()));
            Console.WriteLine("Содерживоме файла в обратном порядке (только гласные):");
            // Тоже самое, но предварительно фильтруем негласные символы
            Console.WriteLine(new string(charStack.Where(ch => "ОИАЫЮЯЭЁУЕоиаыюяэёуеAaEeIiOoUuYy".Contains(ch)).ToArray()));
            Console.ReadKey(true);
        }

        private static void Task23()
        {
            Console.WriteLine("Задачи 2 и 3");
            Queue<Employee> older = new Queue<Employee>();
            ArrayList olderArrList = new ArrayList();

            IEnumerable<Employee> employees = File.ReadLines("employes.txt").Select(Employee.Parse);


            foreach (Employee employee in employees)
            {
                if (employee.Age < 30)
                {
                    Console.WriteLine(employee);
                }
                else
                {
                    older.Enqueue(employee);
                    olderArrList.Add(employee);
                }
            }

            Console.WriteLine("Вывод коллекции типа класса Queue<T>");
            foreach (Employee employee in older)
            {
                Console.WriteLine(employee);
            }

            Console.WriteLine("Вывод коллекции типа ArrayList");
            foreach (Employee employee in olderArrList)
            {
                Console.WriteLine(employee);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Task1();
                Console.ReadKey(true);
                Task23();
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey(true);
        }
    }
}
