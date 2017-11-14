using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lab6.Task4
{
    public class Catalog
    {
     
        // Ключ - название диска. 
        private readonly Hashtable _disks;

        public Catalog()
        {
            _disks = new Hashtable();
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
            _disks.Remove(name);
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
        public void AddSong(Song song, string disk)
        {

            // Наличие диска в каталоге.
            if (!_disks.Contains(disk))
            {
                throw new ArgumentException("Диск не найден", "disk");
            }

            // Получаем список песен на диске.
            ICollection<Song> diskSongs = _disks[disk] as ICollection<Song>;

            // Если песеня уже есть на диске
            if (diskSongs.Contains(song))
            {
                throw new ArgumentException("Песня" + song + " уже добавлена на этот диск " + disk, "song");
            }
            // Добавляем песню на диск
            diskSongs.Add(song);
        }

        //todo
        // Удаляет песню из каталога.
        public void RemoveSong(Song song, string disk)
        {
            ICollection<Song> diskSongs = _disks[disk] as ICollection<Song>;
            diskSongs.Remove(song);
        }


        // Список песен в каталоге
        public IEnumerable<Song> EnumerateSongs()
        {
            return _disks.Values.Cast<ICollection<Song>>().SelectMany(songs => songs).Distinct();
        }

        public IEnumerable<Song> FindSongs(string artistName)
        {
            Regex reg = new Regex(artistName);
            return EnumerateSongs().Where(song => reg.IsMatch(song.Artist)).ToList();
        }
        #endregion
    }
}
