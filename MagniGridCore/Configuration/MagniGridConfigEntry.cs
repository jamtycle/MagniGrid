using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagniGrid.Core.Columns;
using MagniGrid.Core.Columns.Base;

namespace MagniGrid.Core.Configuration
{
    public class MagniGridConfigEntry
    {
        private int column_index;
        private int column_visual_index;
        private string column_name;
        private string column_visual_name;
        private BaseColumn column_type;
        private Action<BaseColumn> extra_config;

        public MagniGridConfigEntry(DataColumn _column)
        {
            column_index = _column.Ordinal;
            column_name = _column.ColumnName;
            column_visual_index = _column.Ordinal;
            column_visual_name = _column.ColumnName;

            if(_column.DataType == typeof(string))
            {
                //column_type = new TextColumn();
            }
        }

        public MagniGridConfigEntry(int _column_index, string _column_name)
        {
            column_index = _column_index;
            column_visual_index = _column_index;
            column_name = _column_name;
            column_visual_name = _column_name;
        }

        public MagniGridConfigEntry(int _column_index, string _column_name, BaseColumn _column_type)
        {
            column_index = _column_index;
            column_visual_index = _column_index;
            column_name = _column_name;
            column_visual_name = _column_name;
            column_type = _column_type;
        }

        public MagniGridConfigEntry(int _column_index, int _column_visual_index, string _column_name, string _column_visual_name, BaseColumn _column_type)
        {
            column_index = _column_index;
            column_visual_index = _column_visual_index;
            column_name = _column_name;
            column_visual_name = _column_visual_name;
            column_type = _column_type;
        }

        public int ColumnIndex { get => column_index; }
        public int ColumnVisualIndex { get => column_visual_index; set => column_visual_index = value; }
        public string ColumnName { get => column_name; }
        public string ColumnVisualName { get => column_visual_name; set => column_visual_name = value; }
        public BaseColumn ColumnType { get => column_type; set => column_type = value; }
        public Action<BaseColumn> ExtraConfig { get => extra_config; set => extra_config = value; }
    }
}
