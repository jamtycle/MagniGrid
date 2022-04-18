using MagniGrid.Core.Columns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MagniGrid.Core.Columns
{
    public class EditingButtonColumn : BaseColumn
    {
        public delegate void OnColumnClick(object sender, RoutedEventArgs e);
        public delegate FrameworkElement OnCellGeneration(FrameworkElement control, object dataItem);
        public event OnColumnClick ColumnClick;
        public event OnCellGeneration CellGeneration;
        public event OnCellGeneration EditingCellGeneration;

        public EditingButtonColumn(DataGridBoundColumn _column) : base(_column)
        {

        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            return CellGeneration == null ? control : CellGeneration.Invoke(control, dataItem);
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new Button();
            control.AddHandler(Button.ClickEvent, ColumnClick);
            control.SetValue(Button.ContentProperty, "Click me!");
            return EditingCellGeneration == null ? control : EditingCellGeneration.Invoke(control, dataItem);
        }
    }
}
