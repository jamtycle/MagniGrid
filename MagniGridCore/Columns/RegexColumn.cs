using MagniGrid.Core.Columns.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MagniGrid.Core.Columns
{
    public class RegexColumn : BaseColumn
    {
        private Regex reg_exp;

        public RegexColumn(DataGridBoundColumn _column, Regex _regex) : base(_column)
        {
            reg_exp = _regex;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var control = new TextBlock();
            control.SetBinding(TextBlock.TextProperty, Binding);
            control.PreviewTextInput += Control_PreviewTextInput;
            control.Loaded += Control_Loaded;
            return control;
        }

        private void Control_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            TextBox tb = (TextBox)sender;
            tb.SelectAll();
            FocusManager.SetFocusedElement(this, tb);
        }

        private void Control_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            string sub_text = ((TextBox)sender).Text + e.Text;
            e.Handled = reg_exp.IsMatch(sub_text);
        }
    }
}
