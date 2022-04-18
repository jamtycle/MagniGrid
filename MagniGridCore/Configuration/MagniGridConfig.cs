using MagniGrid.Core.Columns.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace MagniGrid.Core.Configuration
{
    public class MagniGridConfig : IList<MagniGridConfigEntry>
    {
        private DataGrid grid;
        private MagniGridConfigEntry[] config_array;
        private Hashtable config_table;
        private bool read_only = true;

        public MagniGridConfig(DataGrid _grid)
        {
            grid = _grid;
        }

        public MagniGridConfig(DataGrid _grid, Hashtable _config)
        {
            grid = _grid;
            config_table = _config;

            config_array = new MagniGridConfigEntry[_config.Count];
            int i = 0;
            foreach (DictionaryEntry entry in _config)
            {
                int cindex = _grid.Columns.FirstOrDefault(x => x.Header.Equals(entry.Key))?.DisplayIndex ?? -1;
                if (cindex != -1)
                    config_array[cindex] = (MagniGridConfigEntry)entry.Value;
            }
        }

        #region Applying Config
        public void ApplyConfig()
        {
            foreach (MagniGridConfigEntry entry in config_array)
                ApplyConfig(entry);
        }

        public void ApplyConfig(int _index)
        {
            MagniGridConfigEntry entry = config_array[_index];
            ApplyConfig(entry);
        }

        private void ApplyConfig(MagniGridConfigEntry _entry)
        {
            var gcolumn = grid.Columns.FirstOrDefault(x => x.Header.Equals(_entry.ColumnVisualName) && x.DisplayIndex.Equals(_entry.ColumnVisualIndex));
            if (gcolumn == null) return;
            grid.Columns[_entry.ColumnVisualIndex] = _entry.ColumnType;
            _entry.ExtraConfig?.Invoke((BaseColumn)grid.Columns[_entry.ColumnVisualIndex]);
        }
        #endregion

        #region IList Implementation
        public void Add(MagniGridConfigEntry item)
        {
            if (read_only) return;

            // TODO: most likely this line will add 2 spaces in the array instead of 1.
            Array.Resize(ref config_array, config_array.Length + 1);
            config_array[config_array.Length - 1] = item;

            config_table.Add(item.ColumnName, item);

            ApplyConfig(config_array.Length - 1);
        }

        public void Clear()
        {
            if (read_only) return;

            config_array = new MagniGridConfigEntry[0];
            config_table.Clear();
        }

        public bool Contains(MagniGridConfigEntry item)
        {
            return config_array.Contains(item);
        }

        public void CopyTo(MagniGridConfigEntry[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(MagniGridConfigEntry item)
        {
            int index = -1;
            for (int i = 0; i < config_array.Length; i++)
                if (config_array[i].Equals(item))
                    index = i;
            return index;
        }

        public void Insert(int index, MagniGridConfigEntry item)
        {
            if (read_only) return;
            throw new NotImplementedException();
        }

        public bool Remove(MagniGridConfigEntry item)
        {
            if (read_only) return false;
            int item_index = this.IndexOf(item);
            if (item_index == -1) return false;

            config_array = config_array.Where(x => !x.Equals(config_array[item_index])).ToArray();
            config_table.Remove(item.ColumnName);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (read_only) return;
            if (index > config_array.Length - 1 || index < 0) return;

            Remove(config_array[index]);
        }

        // Enumerators
        IEnumerator IEnumerable.GetEnumerator()
        {
            return config_array.GetEnumerator();
        }

        public IEnumerator<MagniGridConfigEntry> GetEnumerator()
        {
            return (IEnumerator<MagniGridConfigEntry>)config_array.GetEnumerator();
        }
        #endregion

        #region Indexer
        public MagniGridConfigEntry this[int index]
        {
            get => config_array[index];
            set
            {
                if (read_only) return;
                config_array[index] = value;
                ApplyConfig(index);
            }
        }

        public MagniGridConfigEntry this[string column_name]
        {
            get => config_array.FirstOrDefault(x => x.ColumnName.Equals(column_name));
        }
        #endregion

        #region Properties
        public int Count => config_array.Length;

        public bool IsReadOnly => read_only;

        public DataGrid Grid { get => grid; }
        #endregion
    }
}
