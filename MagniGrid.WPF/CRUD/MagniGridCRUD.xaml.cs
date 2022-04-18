using MagniGrid.Core.Columns.Base;
using MagniGrid.Core.Configuration;
using MagniGrid.Core.ModelView;
using MagniGrid.Core.ModelView.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagniGrid.WPF.CRUD
{
    /// <summary>
    /// Interaction logic for MagniGridCRUD.xaml
    /// </summary>
    public partial class MagniGridCRUD : MagniGridBase
    {
        #region Custom Evetns
        public event MagniGridEvent MagniGridModified;
        public event MagniGridElementEvent MagniGridRowDoubleClick;
        #endregion

        public MagniGridCRUD()
        {
            InitializeComponent();
        }

        public MagniGridCRUD(MagniGridConfig _config) : base(_config)
        {
            InitializeComponent();
        }

        public override void InitializeGrid(IMagniGridModelView _model_view)
        {
            base.InitializeGrid(_model_view);
        }

        private void CRUD_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
                MagniGridModified?.Invoke(sender, e);

            // Text Input Validation
            if (e.EditingElement is TextBox)
            {
                if (e.EditAction == DataGridEditAction.Cancel)
                    ToolTipManager(e.EditingElement, "");

                string text = ((TextBox)e.EditingElement).Text;
                MethodInfo method;
                Type typo;

                try
                {
                    string property = ((Binding)((BaseColumn)e.Column).Binding).Path.Path;
                    typo = this.DataContext.GetType().GetGenericArguments().Single();
                    if (typo == typeof(string)) return;
                    method = typo.GetProperty(property).PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                }
                catch { return; }

                try
                {
                    method?.Invoke(typo, new object[] { text });
                    ToolTipManager(e.EditingElement, "");
                }
                catch(Exception ex)
                {
                    ToolTipManager(e.EditingElement, WrapExceptions(ex));
                }
            }
        }

        private void CRUD_RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int row_index = this.Items.IndexOf(this.CurrentItem);
            if (row_index == -1) return;

            var info = ModelView.Data[row_index];
            MagniGridRowDoubleClick?.Invoke(info, e);
        }

        private string WrapExceptions(Exception _ex)
        {
            string message = _ex.Message;
            Exception inner = _ex.InnerException;
            while (inner != null)
            {
                message += $"\n{inner.Message}";
                inner = inner?.InnerException;
            }
            return message;
        }

        private void ToolTipManager(FrameworkElement _element, string _message)
        {
            if (((ToolTip)_element.ToolTip) != null)
                ((ToolTip)_element.ToolTip).IsOpen = false;

            _element.ToolTip = new ToolTip()
            {
                Content = _message,
                IsOpen = !string.IsNullOrEmpty(_message)
            };
        }
    }
}
