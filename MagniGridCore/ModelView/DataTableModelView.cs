using MagniGrid.Core.Model;
using MagniGrid.Core.ModelView.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MagniGrid.Core.ModelView
{
    public class DataTableModelView<T> : MagniGridModelViewBase<T>
    {
        private DataTable raw_data;
        private SqlConnection connection;

        public DataTableModelView(DataTable _table, SqlConnection _connection) : base()
        {
            raw_data = _table;
            connection = _connection;
            if (_table.Rows.Count <= 0) return;

            Data = new ObservableCollection<IMagniModel>(BuildCollection(_table));
            Data.CollectionChanged += Data_CollectionChanged;
        }

        public IEnumerable<IMagniModel> BuildCollection(DataTable _table)
        {
            foreach (DataRow row in _table.Rows)
            {
                object info = Activator.CreateInstance<T>();
                ((IMagniModel)info).FromDataRow(row);
                ((INotifyPropertyChanged)info).PropertyChanged += ModelView_PropertyChanged;
                yield return (IMagniModel)info;
            }
        }

        public override void InitializeGrid(DataGrid _grid)
        {
            _grid.SetBinding(DataGrid.ItemsSourceProperty, "ModelView.RawData");
            _grid.SetBinding(DataGrid.DataContextProperty, "ModelView");
        }

        #region Database Interaction
        public override bool TestConnection()
        {
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (SqlException ex) { Console.WriteLine(ex.Message); return false; }
        }

        public override (Exception, int)[] CommitChanges()
        {
            if (Data.Count <= 0) return null;

            return new (Exception, int)[] { CommitAdded(), CommitModified() };
        }

        public override (Exception, int) CommitAdded()
        {
            int affected = -1;
            try
            {
                if (AddedData.Count() <= 0) return (null, affected);

                var added = EnumerableToDataTable(AddedData);

                // CONNECT DB
                connection.Open();

                if (added != null && added.Rows.Count > 0)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = (string)StoredProcedures[CRUD.Create];
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(this.HashToParameter(new Hashtable()
                    {
                        { "@table", added },
                        { "@type", CRUD.Create }
                    }).ToArray());
                    // EXECUTE NON QUERY
                    affected = cmd.ExecuteNonQuery();

                    foreach (var item in AddedData)
                        item.State = DataState.Natural;
                }

                // CLOSE DB
                connection.Close();
                return (null, affected);
            }
            catch (SqlException ex)
            {
                return (ex, affected);
            }
        }

        public override (Exception, int) CommitModified()
        {
            int affected = -1;
            try
            {
                if (ModifiedData.Count() <= 0) return (null, affected);

                var modified = EnumerableToDataTable(ModifiedData);

                // CONNECT DB
                connection.Open();

                if (modified != null && modified.Rows.Count > 0)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = (string)StoredProcedures[CRUD.Update];
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(this.HashToParameter(new Hashtable()
                    {
                        { "@table", modified },
                        { "@type", CRUD.Update }
                    }).ToArray());
                    // EXECUTE NON QUERY
                    affected = cmd.ExecuteNonQuery();

                    foreach (var item in ModifiedData)
                        item.State = DataState.Natural;
                }

                // CLOSE DB
                connection.Close();

                return (null, affected);
            }
            catch (SqlException ex)
            {
                return (ex, affected);
            }
        }

        public override Exception DeleteAtIndex(int _index)
        {
            try
            {
                var delete = DataRowToDataTable(Data[_index].ToDataRow(raw_data));

                // CONNECT DB
                connection.Open();

                if (delete != null && delete.Rows.Count > 0)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = (string)StoredProcedures[CRUD.Delete];
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(this.HashToParameter(new Hashtable()
                    {
                        { "@table", delete },
                        { "@type", CRUD.Delete }
                    }).ToArray());
                    // EXECUTE NON QUERY
                    cmd.ExecuteNonQuery();

                    Data.Remove(Data[_index]);
                }

                // CLOSE DB
                connection.Close();

                return null;
            }
            catch (SqlException ex)
            {
                return ex;
            }
        }
        #endregion

        #region Data Manipulation
        private DataTable EnumerableToDataTable(IEnumerable<IMagniModel> _info)
        {
            if (_info.Count() == 0) return null;

            var table = raw_data.Clone();
            return _info.Select(x => x.ToDataRow(table)).CopyToDataTable();
        }

        private DataTable DataRowToDataTable(DataRow _row)
        {
            var ntable = raw_data.Clone();
            ntable.Rows.Add(_row.ItemArray);
            return ntable;
        }
        #endregion

        #region Properties
        public DataTable RawData { get => raw_data; }
        #endregion
    }
}
