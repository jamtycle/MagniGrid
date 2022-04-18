using MagniGrid.Core.Columns.Base;
using MagniGrid.Core.Configuration;
using MagniGrid.Core.Model;
using MagniGrid.Core.ModelView.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace MagniGrid.WPF
{
    public abstract class MagniGridBase : DataGrid
    {
        private IList<Task> task_pool = new List<Task>();
        private bool debug_mode;
        private MagniGridConfig config;
        private IMagniGridModelView model_view;
        private readonly IEnumerable<string> interface_fields = typeof(IMagniModel).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Select(x => x.Name);

        #region Custom Events
        public delegate void MagniGridElementEvent(IMagniModel sender, EventArgs e);
        public delegate void MagniGridEvent(object sender, EventArgs e);

        public event MagniGridElementEvent MagniGridSelectionChanged;
        #endregion

        public MagniGridBase() : base()
        {
            config = new MagniGridConfig(this);
        }

        public MagniGridBase(MagniGridConfig _config) : base()
        {
            if (_config.Grid != this) throw new Exception("The configuration Grid is not the same as this one.");
            config = _config;
        }

        #region Initializers
        public virtual void InitializeGrid(IMagniGridModelView _model_view)
        {
            model_view = _model_view;
            this.AutoGenerateColumns = true;
            var databind = this.SetBinding(DataGrid.ItemsSourceProperty, "ModelView.Data");
            this.SetBinding(DataGrid.DataContextProperty, "ModelView");
            //model_view.InitializeGrid(this);
        }
        #endregion

        #region Column Generation
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);
            if (!debug_mode)
                if (interface_fields.Contains(e.PropertyName))
                {
                    e.Column.Visibility = System.Windows.Visibility.Hidden;
                    return;
                }

            if (Config == null) return;

            var entry = Config[e.PropertyName];
            if (entry == null) entry = Config[e.Column.DisplayIndex];

            e.Column = entry.ColumnType;
            e.Column.Header = entry.ColumnVisualName;
            e.Column.DisplayIndex = entry.ColumnVisualIndex;
            entry.ExtraConfig?.Invoke((BaseColumn)e.Column);
        }
        #endregion

        #region Selection Changes
        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
        {
            base.OnSelectedCellsChanged(e);
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            int row_index = this.Items.IndexOf(this.CurrentItem);
            if (row_index == -1) return;

            var info = model_view.Data[row_index];
            MagniGridSelectionChanged?.Invoke(info, e);
        }
        #endregion

        #region Data Manipulation
        protected override void OnCellEditEnding(DataGridCellEditEndingEventArgs e)
        {
            base.OnCellEditEnding(e);
        }
        #endregion

        #region Clear
        public void ClearGrid()
        {
            this.SetBinding(DataGrid.ItemsSourceProperty, "");
        }
        #endregion 

        #region Properties
        public IEnumerable<string> InterfaceFields { get => interface_fields; }
        public MagniGridConfig Config { get => config; set => config = value; }
        public IMagniGridModelView ModelView { get => model_view; }
        public IList<Task> TaskPool { get => task_pool; }
        public bool DebugMode { get => debug_mode; set => debug_mode = value; }
        #endregion
    }
}
