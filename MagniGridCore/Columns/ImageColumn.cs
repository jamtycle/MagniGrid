using MagniGrid.Core.Columns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MagniGrid.Core.Columns
{
    public class ImageColumn : BaseColumn
    {
        public ImageColumn(System.Windows.Controls.DataGridBoundColumn _column) : base(_column)
        {
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new Image();
            control.SetBinding(Image.SourceProperty, Binding);
            control.SetValue(Image.StretchProperty, System.Windows.Media.Stretch.Fill);
            return control;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new Image();
            control.SetBinding(Image.SourceProperty, Binding);
            return control;
        }
    }
}
