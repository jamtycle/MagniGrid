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
    public class SecretColumn : BaseColumn
    {
        public SecretColumn(DataGridBoundColumn _column) : base(_column)
        {
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var source = dataItem.GetType().GetProperty(((Binding)Binding).Path.Path)?.GetValue(dataItem, null)?.ToString();
            if (source == null) return new TextBlock();
            return new TextBlock() { Text = $"{string.Concat("".PadLeft(source.Length, '●'))}" };
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var source = dataItem.GetType().GetProperty(((Binding)Binding).Path.Path).GetValue(dataItem, null);
            if (((Binding)Binding).Converter == null) ((Binding)Binding).Converter = new Converters.PasswordConverter();
            if (source != null) { var txt = new TextBox(); txt.SetBinding(TextBox.TextProperty, Binding); return txt; }
            else
            {
                var txt = new TextBox();
                txt.SetBinding(TextBox.TextProperty, Binding);
                return txt;
            }
        }
    }
}
