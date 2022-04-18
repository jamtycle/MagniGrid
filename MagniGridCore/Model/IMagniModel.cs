using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagniGrid.Core.Model
{
    public interface IMagniModel
    {
        IMagniModel FromDataRow(DataRow _row);
        DataRow ToDataRow(DataTable _skeleton);
        IMagniModel FromArray(object[] _array);
        object[] ToArray();

        bool Visibility { get; set; }
        DataState State { get; set; }
    }
}
