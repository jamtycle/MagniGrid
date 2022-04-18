using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MagniGrid.Core.Columns.Base
{
    public abstract class BaseColumn : DataGridBoundColumn
    {
        public BaseColumn(DataGridBoundColumn _column)
        {
            Header = _column;
            Binding = (Binding)_column.Binding;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return cell;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return cell;
        }
    }
}
