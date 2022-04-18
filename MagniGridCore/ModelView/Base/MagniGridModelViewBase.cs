using MagniGrid.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MagniGrid.Core.ModelView.Base
{
    public abstract class MagniGridModelViewBase<T> : INotifyPropertyChanged, IMagniGridModelView
    {
        private ObservableCollection<IMagniModel> data;
        private Action<T> fix_value;
        private Hashtable stored_procedures;

        public event PropertyChangedEventHandler PropertyChanged;

        public MagniGridModelViewBase()
        {
            string def_proc = $"CRUD_{typeof(T).Name}";
            stored_procedures = new Hashtable() 
            {
                { CRUD.Create, def_proc },
                { CRUD.Read, def_proc },
                { CRUD.Update, def_proc },
                { CRUD.Delete, def_proc },
            };
        }

        #region Initializers
        public virtual void InitializeGrid(DataGrid _grid)
        {
            _grid.SetBinding(DataGrid.ItemsSourceProperty, "ModelView.Data");
            _grid.SetBinding(DataGrid.DataContextProperty, "ModelView");
        }
        #endregion

        #region CollectionChanged
        internal void Data_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    // TODO: MOST LICKELY IS GOING TO LOOSE THE REFERENCE.
                    CollectionChangedManager(e.NewItems, (x) =>
                    {
                        //((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += MagniGridModelViewBase_PropertyChanged;
                        //fix_value?.Invoke((T)e.NewItems[0]);

                        ((INotifyPropertyChanged)x).PropertyChanged += ModelView_PropertyChanged;
                        fix_value?.Invoke((T)x);
                    });
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    CollectionChangedManager(e.OldItems, (x) =>
                    {
                        ((INotifyPropertyChanged)x).PropertyChanged -= ModelView_PropertyChanged;
                        fix_value?.Invoke((T)x);
                    });
                    break;
            }
            OnPropertyChanged("Data");
        }

        private void CollectionChangedManager(IList? _values, Action<object> _executor)
        {
            if (_values.Count == 1)
                _executor.Invoke(_values[0]);
            else if (_values.Count > 1)
                foreach (object item in _values)
                    _executor.Invoke(item);
        }
        #endregion

        #region PropertyChanged
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        internal void ModelView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // No need to notify changes for interface properties.
            if (typeof(IMagniModel).GetProperties().Select(x => x.Name).Contains(e.PropertyName)) return;
            switch (((IMagniModel)sender).State)
            {
                case DataState.Natural:
                case DataState.Modified:
                    ((IMagniModel)sender).State = DataState.Modified;
                    break;
                case DataState.Added:
                default:
                    ((IMagniModel)sender).State = DataState.ModifiedLocaly;
                    break;
            }
        }
        #endregion

        #region Filters
        public void FilterData(Func<IMagniModel, bool> _filter)
        {
            foreach (IMagniModel model in data)
                model.Visibility = _filter == null ? true : _filter.Invoke(model);
        }

        public void FilterData(Func<T, bool> _filter)
        {
            foreach (IMagniModel model in data)
                model.Visibility = _filter == null ? true : _filter.Invoke((T)model);
        }
        #endregion

        #region Database Interaction
        public virtual bool TestConnection()
        {
            return true;
        }

        public virtual (Exception, int)[] CommitChanges()
        {
            return null;
        }

        public virtual (Exception, int) CommitAdded()
        {
            return (null, -1);
        }

        public virtual (Exception, int) CommitModified()
        {
            return (null, -1);
        }

        public virtual Exception DeleteAtIndex(int _index)
        {
            return null;
        }

        // Utilities
        public IEnumerable<SqlParameter> HashToParameter(Hashtable _parameters)
        {
            foreach (DictionaryEntry entry in _parameters)
            {
                if (entry.Value is string)
                {
                    ParameterDirection direction = ParameterDirection.Input;
                    string pname = (string)entry.Key;
                    if (pname.Contains("o~"))
                    {
                        pname = pname.Replace("o~", "");
                        direction = ParameterDirection.Output;
                    }
                    else if (pname.Contains("io~"))
                    {
                        pname = pname.Replace("io~", "");
                        direction = ParameterDirection.InputOutput;
                    }
                    else if (pname.Contains("rv~"))
                    {
                        pname = pname.Replace("rv~", "");
                        direction = ParameterDirection.ReturnValue;
                    }

                    yield return new SqlParameter(pname, entry.Value) { Direction = direction };
                }
                else if (entry.Key is SqlParameter)
                {
                    yield return (SqlParameter)entry.Key;
                }
            }
        }
        #endregion

        #region Properties
        public ObservableCollection<IMagniModel> Data
        {
            get => data;
            set 
            { 
                data = value;
                OnPropertyChanged("Data");
            }
        }

        public virtual IEnumerable<IMagniModel> AddedData
        {
            get => data.Where(x => x.State.Equals(DataState.Added) || x.State.Equals(DataState.ModifiedLocaly));
        }

        public virtual IEnumerable<IMagniModel> ModifiedData
        {
            get => data.Where(x => x.State.Equals(DataState.Modified));
        }

        public Action<T> Fix_value { get => fix_value; set => fix_value = value; }

        public Hashtable StoredProcedures { get => stored_procedures; set => stored_procedures = value; }
        #endregion
    }
}
