using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PracticalControls.Extensions
{
    public static class DataGridExension
    {
        public static void FinishingEditing(this DataGrid dg)
        {
            dg.CommitEdit();
            dg.CommitEdit();
        }
    }
}
