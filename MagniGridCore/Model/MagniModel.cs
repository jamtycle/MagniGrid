using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagniGrid.Core.Model
{
    public class MagniModel : IMagniModel, INotifyPropertyChanged
    {
        private bool visibility = true;
        private DataState state = DataState.Natural;

        public event PropertyChangedEventHandler PropertyChanged;

        #region IMagniModel
        public IMagniModel FromArray(object[] _array)
        {
            var fields = ClassFields();
            int i = 0;
            foreach (var fld in fields)
            {
                if (i > _array.Length - 1) continue;

                // TODO: REVIEW THIS LINE
                if(_array[i].GetType().Equals(fld.FieldType)) fld.SetValue(this, _array[i]);
                i++;
            }
            return this;
        }

        public IMagniModel FromDataRow(DataRow _row)
        {
            var fields = ClassFields();
            foreach (var fld in fields)
            {
                if (!_row.Table.Columns.Contains(fld.Name)) continue;
                if (fld.FieldType != _row[fld.Name].GetType()) continue;
                fld.SetValue(this, _row[fld.Name] is DBNull ? null : _row[fld.Name]);
            }
            return this;
        }

        public object[] ToArray()
        {
            var fields = ClassFields();
            return fields.Select(x => x.GetValue(this)).ToArray();
        }

        /// <summary>
        /// Creates a new row from the _skeleton DataTable parameter.
        /// This new row DOES NOT will be inserted into the _skeleton.
        /// </summary>
        /// <param name="_skeleton"></param>
        /// <returns></returns>
        public DataRow ToDataRow(DataTable _skeleton)
        {
            DataRow row = _skeleton.NewRow();
            var fields = ClassFields();
            foreach (var fld in fields)
            {
                if (!_skeleton.Columns.Contains(fld.Name)) continue;
                row[fld.Name] = fld.GetValue(this) ?? DBNull.Value;
            }
            return row;
        }

        private IEnumerable<FieldInfo> ClassFields()
        {
            var interface_fields = typeof(IMagniModel).GetFields().Select(x => x.Name);
            return this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => !interface_fields.Contains(x.Name));
        }
        #endregion

        #region IPropertyChanged
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        public bool Visibility
        {
            get => visibility; 
            set
            {
                visibility = value;
                OnPropertyChanged("Visibility");
            }
        }

        public DataState State 
        { 
            get => state; 
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
    }
}
