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
    public class DateTimeColumn : BaseColumn
    {
        private string date_format;

        public DateTimeColumn(DataGridBoundColumn _column, string _date_format) : base(_column)
        {
            Binding.StringFormat = _date_format;
            date_format = _date_format;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            BindingOperations.SetBinding(control, TextBlock.TextProperty, Binding);
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new DatePicker();
            control.PreviewKeyDown += Control_PreviewKeyDown;
            BindingOperations.SetBinding(control, DatePicker.SelectedDateProperty, Binding);
            BindingOperations.SetBinding(control, DatePicker.DisplayDateProperty, Binding);
            return control;
        }

        private void Control_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Return:
                    DataGridOwner.CommitEdit();
                    break;
            }
        }
    }
}
