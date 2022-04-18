using MagniGrid.Core.Columns.Base;
using MagniGrid.Core.Columns.ComboColumn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MagniGrid.Core.Columns
{
    public class ComboBoxColumn : BaseColumn
    {
        public delegate void OnIndexChanged(object _sender, SelectionChangedEventArgs _e);
        public event OnIndexChanged IndexChanged;

        private ObservableCollection<ComboModel> original_source;
        private ObservableCollection<ComboModel> filter_source;
        private bool filtered;

        public ComboBoxColumn(DataGridBoundColumn _column, string _display_member, string _value_member, DataTable _source) : base(_column)
        {
            ((Binding)Binding).UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            original_source = new ObservableCollection<ComboModel>(ComboModel.GenerateCollection(_display_member, _value_member, _source));
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            control.SetBinding(TextBlock.TextProperty, new Binding($"{((Binding)this.Binding).Path.Path}.Display"));
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new ComboBox();
            control.SelectionChanged += Control_SelectionChanged;
            control.SetBinding(ComboBox.SelectedValueProperty, this.Binding);
            control.SetValue(ComboBox.IsTextSearchEnabledProperty, true);
            control.SetValue(ComboBox.ItemsSourceProperty, !filtered ? original_source : filter_source);
            control.SetValue(ComboBox.DisplayMemberPathProperty, "Display");
            return control;
        }

        private void Control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IndexChanged?.Invoke(sender, e);
        }

        public void FilterColumnCombo(Func<ComboModel, bool> _filter)
        {
            if (_filter == null) return;

            filter_source = new ObservableCollection<ComboModel>(original_source.Where(x => _filter(x)));
            filtered = filter_source.Count > 0;
        }

        public bool Filtered { get => filtered; set => filtered = value; }
    }
}
