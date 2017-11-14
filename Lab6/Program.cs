using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lab6.Task4;

namespace Lab6
{
    class Program
    {
        private static int ReadInt(string message = "Введите число", int min = int.MinValue, int max = int.MaxValue)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(message);
                    Console.WriteLine(" Или Ctrl+Z для отмены");
                    string readLine = Console.ReadLine();
                    var readInt = int.Parse(readLine);
                    if (readInt < min || readInt > max)
                    {
                        throw new OverflowException();
                    }

                    return readInt;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Введенная строка не является целым числом.\n Попробуйте еще раз");
                }
                catch (ArgumentNullException)
                {
                    // если ввести ctr+z, а потом enter, то Console.ReadLine вернет null
                    throw new ApplicationException("Ввод был отменен");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Число слшком большое или слишком маленькое");
                    Console.WriteLine("Допустимые значения: {0} - {1}", min, max);
                    Console.WriteLine("Попробуйте еще раз");
                }
            }
        }

        private static Song ReadSong()
        {
            return new Song { Name = ReadString("Название"), Artist = ReadString("Исполнитель") };
        }

        private static string ReadString(string v)
        {
            Console.Write(v);
            Console.WriteLine(" Или CTRL+Z для отмены");
            string val = Console.ReadLine();
            if (val == null)
                throw new ApplicationException("Ввод был отменен.");
            return val;
        }

        static void Print<T>(IEnumerable<T> enumearble)
        {
            Console.WriteLine("---");
            foreach (T obj in enumearble)
            {
                Console.WriteLine(obj);
            }

            Console.WriteLine("---");
        }

        // Выводит список на экран, индексируя, и возвращает введенный пользователем индекс
        static int SelectItem<T>(IEnumerable<T> items, string message)
        {
            Console.WriteLine(message);
            // Счетчик
            int i = 0;
            foreach (T item in items)
            {
                // Сначала выводим индекс
                Console.Write("{0,3}: ", i++);
                // Выводим сам элемент
                Console.WriteLine(item);
            }
            return ReadInt("Введите индекс:", 0, i);
        }

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

        static void Task4()
        {
            // Создаем пустой каталог
            Catalog catalog = new Catalog();

            // Создаем диски
            for (int i = 0; i < 5; i++)
            {
                catalog.AddDisk("Disk " + i);
            }

            // Создаем песни и добавляем их на диски
            // Создаем массив, т.к. если оставить IEnumerable, то получение элемента по индексу каждый раз будет вызывать перечисление
            string[] disks = catalog.EnumerateDisks().OrderBy(s => s).ToArray();
            int songsPerDisk = 5;
            for (int i = 0; i < disks.Length * songsPerDisk; i++)
            {
                Song song = new Song { Artist = "Artist " + i % songsPerDisk, Name = "Song " + i };
                catalog.AddSong(disks[i % songsPerDisk], song);
            }

            while (true)
            {
                // Меню
                int select = SelectItem(
                    new[] { "Добавить диск", "Удалить диск", "Добавить песню", "Удалить песню", "Содержимое каталога"},
                    "Главное меню");
                switch (select)
                {
                    case 0:
                        AddDisk(catalog);
                        break;
                    case 1:
                        DelDisk(catalog);
                        break;
                    case 2:
                        AddSong(catalog);
                        break;
                    case 3:
                        // Все песни
                        List<Song> allsongs = catalog.EnumerateSongs().ToList();
                        // Выбираем песню
                        int songI = SelectItem(allsongs, "Удаление песни");
                        catalog.RemoveSong(allsongs[songI]);
                        break;
                    case 4:
                        IEnumerable<string> sortedDisks = catalog.EnumerateDisks().OrderBy(s => s);
                        foreach (string disk in sortedDisks)
                        {
                            Console.WriteLine(disk);
                            IEnumerable<Song> songs = catalog.EnumerateDisk(disk);
                            foreach (Song song in songs)
                            {
                                Console.WriteLine("\t" + song);
                            }
                        }
                        break;
                }
            }
        }

        private static void DelDisk(Catalog catalog)
        {
            Console.WriteLine("Удаление дисков");
            // Бесконечно создаем и добавляем в каталог диски, покуда не будет брошен ApplicationException (пользователь введет ctrl+z
            try
            {
                while (true)
                {
                    string[] disks = catalog.EnumerateDisks().ToArray();
                    int i = SelectItem(disks, "Можно удалить следущие диски:");
                    catalog.RemoveDisk(disks[i]);
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void AddDisk(Catalog catalog)
        {
            Console.WriteLine("Добавление дисков");
            // Бесконечно создаем и добавляем в каталог диски, покуда не будет брошен ApplicationException (пользователь введет ctrl+z
            try
            {
                while (true)
                {
                    catalog.AddDisk(ReadString("Введите название диска"));
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void AddSong(Catalog catalog)
        {
            // Бесконечно создаем и добавляем в каталог песни, покуда не будет брошен ApplicationException (пользователь введет ctrl+z
            Console.WriteLine("Добавление песен");
            try
            {
                while (true)
                {
                    Song song1 = ReadSong();
                    catalog.AddSong(song1);

                    try
                    {
                        // Перечисляем коллекцию в массив
                        string[] disks = catalog.EnumerateDisks().ToArray();
                        // SelectItem выводит на консоль коллекцию, и вовзращает введенный пользователем индекс
                        int diskNum = SelectItem(disks, "Выберите диск");
                        // Добавляем на выбранный диск добавленную песню
                        catalog.AddSongToDisk(disks[diskNum], song1);
                    }
                    // Добавление песни в диск можно отменить при помощи ctrl+z
                    catch (ApplicationException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            catch (ApplicationException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                Task1();
                Console.ReadKey(true);
                Task23();
                Task4();
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
