using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lab6.Task4;

namespace Lab6_4
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
            Console.WriteLine("---");
            foreach (T obj in enumearble)
            {
                {
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

            try
            {
                while (true)
                {
                    try
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
                                                string[] disks1 = catalog.EnumerateDisks().Where(s => reg.IsMatch(s))
                                                    .OrderBy(s => s).ToArray();
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
                                            if (count >= Console.WindowHeight)
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
                                Print(catalog.FindSongs(ReadString("Имя исполнителя (регулярное выражение)"))
                                    .OrderBy(s => s.Name));
                                break;

                        }
                    }
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("Нажмите любую клавишу");
            Console.ReadKey(true);
        }
    }
}
