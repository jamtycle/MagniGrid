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
    public class ColorfullColumn : BaseColumn
    {
        public ColorfullColumn(DataGridBoundColumn _column) : base(_column)
        {
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            if (((Binding)Binding).Converter == null) ((Binding)Binding).Converter = new Converters.ColorConverter();
            control.SetBinding(TextBlock.BackgroundProperty, Binding);
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            if (((Binding)Binding).Converter == null) ((Binding)Binding).Converter = new Converters.ColorConverter();
            control.SetBinding(TextBlock.TextProperty, Binding);
            control.SetBinding(TextBlock.BackgroundProperty, Binding);
            return control;
        }
    }
}
