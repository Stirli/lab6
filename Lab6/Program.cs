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
            return new Song { Name = ReadString("Название песни"), Artist = ReadString("Исполнитель") };
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
            int count = 0;
            Console.WriteLine("---");
            foreach (T obj in enumearble)
            {
                if (++count >= Console.WindowHeight)
                {
                    count = 0;
                    Console.WriteLine("Далее -->");
                    Console.ReadKey(true);
                }
                Console.WriteLine(obj);
            }

            Console.WriteLine("---");
        }

        // Выводит список на экран, индексируя, и возвращает введенный пользователем индекс
        static int SelectItem<T>(IEnumerable<T> items)
        {
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

        // Выводит список на экран, индексируя, и возвращает введенный пользователем индекс
        static T SelectItem<T>(T[] items, string message)
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
            return items[ReadInt("Введите индекс:", 0, i)];
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

            int songsPerDisk = 10;
            // Создаем диски
            for (int i = 0; i < 100; i++)
            {
                catalog.AddDisk("Disk " + i);
            }

            // Создаем песни и добавляем их на диски
            // Создаем массив, т.к. если оставить IEnumerable, то получение элемента по индексу каждый раз будет вызывать перечисление
            string[] disks = catalog.EnumerateDisks().OrderBy(s => s).ToArray();
            for (int i = 0; i < disks.Length * songsPerDisk; i++)
            {
                Song song = new Song { Artist = "Artist " + i % 10, Name = "Song " + i };
                catalog.AddSong(song, disks[i / songsPerDisk]);
            }

            while (true)
            {
                // Меню
                string select = SelectItem(new[]
                {
                    "Добавить диск",
                    "Удалить диск",
                    "Добавить песню",
                    "Удалить песню",
                    "Список дисков",
                    "Содержимое диска",
                    "Содержимое каталога",
                    "Поиск по исполнителю"
                }, "Главное меню");
                try
                {
                    switch (select)
                    {
                        case "Добавить диск":
                            {
                                Console.WriteLine("Добавление дисков");
                                // Бесконечно создаем и добавляем в каталог диски, покуда не будет брошен ApplicationException (пользователь введет ctrl+z
                                while (true)
                                {
                                    catalog.AddDisk(ReadString("Введите название диска"));
                                    Console.WriteLine();
                                }
                            }
                        case "Удалить диск":
                            {
                                while (true)
                                {
                                    Console.WriteLine("Удаление дисков");
                                    string searchDisk = ReadString("Введите регулярное выражение для поиска");
                                    Regex reg = new Regex(searchDisk, RegexOptions.IgnoreCase);
                                    // Перечисляем коллекцию дисков в массив
                                    try
                                    {
                                        while (true)
                                        {
                                            string[] disks1 = catalog.EnumerateDisks().Where(s => reg.IsMatch(s)).OrderBy(s => s).ToArray();
                                            if (disks1.Length == 0)
                                            {
                                                Console.WriteLine("Элементов больше не найдено.\nВыход\n");
                                                break;
                                            }

                                            // SelectItem выводит на консоль коллекцию, и вовзращает выбранный пользователем диск
                                            string selDisk = SelectItem(disks1, "Выберите диск");
                                            catalog.RemoveDisk(selDisk);
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (ApplicationException e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    Console.WriteLine();
                                }
                            }
                        case "Добавить песню":
                            {
                                // Бесконечно создаем и добавляем в каталог песни, покуда не будет брошен ApplicationException (пользователь введет ctrl+z
                                while (true)
                                {
                                    Console.WriteLine("Добавление песен");
                                    string selDisk = SelDisk(catalog);
                                    try
                                    {
                                        while (true)
                                        {
                                            // Читаем песню из консоли
                                            Song song1 = ReadSong();
                                            // Добавляем на выбранный диск песню
                                            catalog.AddSong(song1, selDisk);
                                            Console.WriteLine();
                                        }
                                    }
                                    catch (ApplicationException e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                    Console.WriteLine();
                                }
                            }
                        case "Удалить песню":
                            while (true)
                            {
                                try
                                {
                                    Console.WriteLine("Удаление песни. Для выхода в главное меню отмените ввод.");
                                    string selDisk = SelDisk(catalog);
                                    Song[] diskSongs = catalog.EnumerateDisk(selDisk).ToArray();
                                    Song songToRemove = SelectItem(diskSongs, "Выберите песню");

                                    catalog.RemoveSong(songToRemove, selDisk);
                                }
                                catch (ArgumentException e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                                Console.WriteLine();
                            }
                        case "Содержимое каталога":
                        {
                            int count = 0;
                                IEnumerable<string> sortedDisks = catalog.EnumerateDisks().OrderBy(s => s);
                                foreach (string disk in sortedDisks)
                                {
                                    Console.WriteLine(disk);
                                    IEnumerable<Song> songs = catalog.EnumerateDisk(disk);
                                    foreach (Song song in songs)
                                    {
                                        if (count>= Console.WindowHeight)
                                        {
                                            Console.WriteLine("Далее -->");
                                            Console.ReadKey(true);
                                            count = 0;
                                        }
                                        Console.WriteLine("\t" + song);
                                        count++;
                                    }
                                    Console.WriteLine();
                                }
                            }
                            break;
                        case "Содержимое диска":
                            {
                                string selDisk = SelDisk(catalog);
                                Console.WriteLine(selDisk);
                                Print(catalog.EnumerateDisk(selDisk));
                                Console.WriteLine();
                            }

                            break;
                        case "Список дисков":
                            Print(catalog.EnumerateDisks());
                            Console.WriteLine();
                            break;
                        case "Поиск по исполнителю":
                            Console.WriteLine("Поиск по исполнителю");
                            Print(catalog.FindSongs(ReadString("Имя исполнителя (регулярное выражение)")).OrderBy(s=>s.Name));
                            break;

                    }
                }
                catch (ApplicationException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static string SelDisk(Catalog catalog)
        {
            Console.WriteLine("Выбор диска:");
            string searchDisk = ReadString("Введите регулярное выражение для поиска дисков");
            Regex reg = new Regex(searchDisk, RegexOptions.IgnoreCase);
            // Перечисляем коллекцию дисков в массив
            string[] disks1 = catalog.EnumerateDisks().Where(s => reg.IsMatch(s)).OrderBy(s => s).ToArray(); if (disks1.Length == 0)
            {
                throw new ApplicationException("Элементов больше не найдено.\nВыход\n");
            }
            // SelectItem выводит на консоль коллекцию, и вовзращает выбранный пользователем диск
            string selDisk = SelectItem(disks1, "Выберите диск");
            Console.WriteLine("Выбран диск " + selDisk);
            return selDisk;
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
