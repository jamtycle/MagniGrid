using MagniGrid.Core.Model;
using MagniGrid.Core.ModelView.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagniGrid.Core.ModelView
{
    public class ArrayModelView<T> : MagniGridModelViewBase<T>
    {
        private IEnumerable<IEnumerable<object>> raw_array;
        private SqlConnection connection;

        public ArrayModelView(IEnumerable<IEnumerable<object>> _array, SqlConnection _connection, IDictionary<CRUD, IEnumerable<string>> _params_dictionary) : base()
        {
            raw_array = _array;
            connection = _connection;
            if (_array.Count() <= 0) return;

            Data = new ObservableCollection<IMagniModel>(BuildCollection(_array));
            Data.CollectionChanged += Data_CollectionChanged;

            StoredProcedures.Clear();
            foreach (var entry in _params_dictionary)
            {
                StoredProcedures.Add(entry.Key, ($"CRUD_{typeof(T).Name}", entry.Value));
            }

        }

        public IEnumerable<IMagniModel> BuildCollection(IEnumerable<IEnumerable<object>> _array)
        {
            foreach (IEnumerable<object> row in _array)
            {
                IMagniModel info = (IMagniModel)Activator.CreateInstance<T>();
                info.FromArray(row.ToArray());
                yield return info;
            }
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
                var added = AddedData;
                if (AddedData.Count() <= 0) return (null, affected);

                var config = ((string, IEnumerable<string>))StoredProcedures[CRUD.Create];

                foreach (var item in added)
                {
                    // CONNECT DB
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = config.Item1;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(config.Item2.Select(x => new SqlParameter(x, item.GetType().GetProperty(x))).ToArray());
                    // EXECUTE NON QUERY
                    affected += cmd.ExecuteNonQuery();
                    // CLOSE DB
                    connection.Close();
                }

                foreach (var item in added)
                    item.State = DataState.Natural;

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
                var modified = ModifiedData;
                if (modified.Count() <= 0) return (null, affected);

                var config = ((string, IEnumerable<string>))StoredProcedures[CRUD.Update];

                foreach (var item in modified)
                {
                    // CONNECT DB
                    connection.Open();
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = config.Item1;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(config.Item2.Select(x => new SqlParameter(x, item.GetType().GetProperty(x))).ToArray());
                    // EXECUTE NON QUERY
                    affected += cmd.ExecuteNonQuery();
                    // CLOSE DB
                    connection.Close();
                }


                foreach (var item in modified)
                    item.State = DataState.Natural;

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
                var delete = Data[_index];

                // CONNECT DB
                connection.Open();

                var config = ((string, IEnumerable<string>))StoredProcedures[CRUD.Delete];

                SqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = config.Item1;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(config.Item2.Select(x => new SqlParameter(x, delete.GetType().GetProperty(x))).ToArray());
                // EXECUTE NON QUERY
                cmd.ExecuteNonQuery();

                Data.Remove(Data[_index]);

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

        #region Properties
        public IEnumerable<IEnumerable<object>> RawArray { get => raw_array; }
        #endregion
    }
}
