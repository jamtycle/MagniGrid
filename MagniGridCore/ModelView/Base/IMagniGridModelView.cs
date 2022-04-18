using MagniGrid.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MagniGrid.Core.ModelView.Base
{
    public interface IMagniGridModelView
    {
        event PropertyChangedEventHandler PropertyChanged;

        public void InitializeGrid(DataGrid _grid);

        public void FilterData(Func<IMagniModel, bool> _filter);

        public bool TestConnection() => true;
        public (Exception, int)[] CommitChanges();
        public (Exception, int) CommitAdded();
        public (Exception, int) CommitModified();
        public Exception DeleteAtIndex(int _index);
        public IEnumerable<SqlParameter> HashToParameter(Hashtable _parameters);

        public ObservableCollection<IMagniModel> Data { get; set; }
        public virtual IEnumerable<IMagniModel> AddedData { get => Data.Where(x => x.State.Equals(DataState.Added) || x.State.Equals(DataState.ModifiedLocaly)); }
        public virtual IEnumerable<IMagniModel> ModifiedData { get => Data.Where(x => x.State.Equals(DataState.Modified)); }
        public Hashtable StoredProcedures { get; set; }
    }
}
