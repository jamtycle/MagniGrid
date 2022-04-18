using MagniGrid.Core.Columns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MagniGrid.Core.Columns
{
    public class ReactiveTextColumn : BaseColumn
    {
        public delegate Color OnElementGeneration(object sender, RoutedEventArgs e);
        public event OnElementGeneration CellGeneration;

        public ReactiveTextColumn(DataGridBoundColumn _column) : base(_column)
        {
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Padding = new Thickness(5, 0, 0, 0),
                FontWeight = FontWeights.Bold
            };
            control.SetBinding(TextBlock.TextProperty, Binding);
            if (!string.IsNullOrEmpty(control.Text))
            {
                Color bgcolor = CellGeneration == null ? Colors.Transparent : CellGeneration.Invoke(dataItem, new RoutedEventArgs() { Source = this });
                control.SetValue(TextBlock.BackgroundProperty, bgcolor);
                control.SetValue(TextBlock.ForegroundProperty, PerceivedBrightness(bgcolor) > 130 ? Brushes.Black : Brushes.White);
            }
            return control;
        }

        private int PerceivedBrightness(System.Windows.Media.Color c)
        {
            return (int)Math.Sqrt(c.R * c.R * .241 + c.G * c.G * .691 + c.B * c.B * .068);
        }
    }
}
