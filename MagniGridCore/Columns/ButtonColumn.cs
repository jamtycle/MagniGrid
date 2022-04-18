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
    public class ButtonColumn : BaseColumn
    {
        public delegate void OnColumnClick(object sender, RoutedEventArgs e);
        public event OnColumnClick ColumnClick;

        public ButtonColumn(DataGridBoundColumn _column) : base(_column)
        {
            ((Binding)this.Binding).UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new Button();
            control.AddHandler(Button.ClickEvent, ColumnClick);
            control.SetBinding(Button.ContentProperty, Binding);
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return base.GenerateEditingElement(cell, dataItem);
        }
    }
}
