using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagniGrid.Core.Columns.ComboColumn
{
    public class ComboModel
    {
        private object display;
        private object value;
        private DataRow row;
        private object[] row_values;

        public ComboModel(object _display, object _value)
        {
            display = _display;
            value = _value;
        }

        public ComboModel(object _display, object _value, DataRow _row)
        {
            display = _display;
            value = _value;
            row = _row;
            row_values = _row.ItemArray;
        }

        public ComboModel(object _display, object _value, object[] _row)
        {
            display = _display;
            value = _value;
            row_values = _row;
        }

        public static IEnumerable<ComboModel> GenerateCollection(IEnumerable<object[]> _array)
        {
            foreach (object[] row in _array)
                if(row.Length >= 2)
                    yield return new ComboModel(row[0], row[1], row);
        }

        public static IEnumerable<ComboModel> GenerateCollection(string _display, string _value, DataTable _model)
        {
            foreach (DataRow row in _model.Rows)
                yield return new ComboModel(row[_display], row[_value], row);
        }

        public object Display { get => display; set => display = value; }
        public object Value { get => value; set => this.value = value; }
        public object this[string _column]
        {
            get
            {
                if (row == null) return null;

                int column = row.Table.Columns.IndexOf(_column);
                if (column != -1)
                    return row_values[column];
                else
                    return null;
            }
        }
    }
}
