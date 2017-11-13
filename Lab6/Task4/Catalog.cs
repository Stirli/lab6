using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lab6.Task4
{
    public class Catalog
    {
        // Можно в один Hashtable хранить список списков(дисков) песен. Или список спиков(песен) дисков
        // Т.е. мы емеем свзяь многие-ко-многим. И искать нам нужно как по песням (которые могут дублироваться на несколько дисков),
        // так и по дискам. Поэтому делаем подобие реляционной бд на основе Hashtable. 
        // В качестве ключа будет использоваться строка, содержащая название диска. В качестве значения - список песен на этом диске.
        private readonly Hashtable _disks;

        // Список песен
        private readonly ICollection<Song> _songs;

        public Catalog()
        {
            _disks = new Hashtable();
            _songs = new List<Song>();
        }
        #region Disk

        // Добавляет диск в каталог и возвращает id диска.
        // Бросает ArgumentNullException
        public void AddDisk(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Имя диска не может быть пустым");
            if (_disks.Contains(name))
                throw new ArgumentException("Диск уже существует");
            _disks.Add(name, new HashSet<Song>());
        }

        // Удалить диск.
        public void RemoveDisk(string name)
        {
            // Удаление диска не приведет к удалению песен из каталога, если они были только на этом диске.
            _disks.Remove(name);
        }
        // Добавляет песню на диск.
        public void AddSongToDisk(string disk, Song song)
        {
            // Проверяем наличие песни в каталоге.
            if (!_songs.Contains(song))
            {
                throw new ArgumentException("Песня не найдена", "song");
            }

            // Наличие диска в каталоге.
            if (!_disks.Contains(disk))
            {
                throw new ArgumentException("Диск не найден", "disk");
            }

            // Получаем список песен на диске.
            ICollection<Song> diskSongs = _disks[disk] as ICollection<Song>;

            // Если песени нет на диске
            if (diskSongs.Contains(song))
            {
                throw new ArgumentException("Песня" + song + " уже добавлена на этот диск " + disk, "song");
            }

            // Добавляем песню на диск
            diskSongs.Add(song);
        }

        public IEnumerable<string> EnumerateDisks()
        {
            return _disks.Keys.Cast<string>();
        }

        // Список песен на диске
        public IEnumerable<Song> EnumerateDisk(string disk)
        {
            // _disks[disk] вернет object, который мы приводим к IEnumerable<Song>. если это значение будет null, будет брошено исключение
            return _disks[disk] as IEnumerable<Song> ?? throw new ArgumentException("Диск '" + disk + "' не найден", "disk");
        }

        #endregion


        #region Catalog

        // Добавляет песню в каталог.
        public void AddSong(Song song)
        {
            if (_songs.Contains(song))
                throw new ArgumentException("Песня уже существует", "song");
            _songs.Add(song);
        }

        // Удаляет песню из каталога. cascade - песня будет удалена со всех дисков.
        public void RemoveSong(Song song, bool cascade = false)
        {
            _songs.Remove(song);
            if (!cascade)
            {
                return;
            }

            IEnumerable<ICollection<Song>> lists = _disks.Values.Cast<ICollection<Song>>();
            foreach (ICollection<Song> list in lists)
            {
                list.Remove(song);
            }
        }


        // Список песен в каталоге
        public IEnumerable<Song> EnumerateCatalog()
        {
            return _songs;
        }

        public IEnumerable<Song> FindSongs(string artistName)
        {
            return _songs.Where(song => song.Artist.Equals(artistName));
        }
        #endregion
    }
}
