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
        /// <summary>
        /// 结束编辑 需要两次扩展
        /// </summary>
        /// <param name="dg"></param>
        public static void FinishingEditing(this DataGrid dg)
        {
            dg.CommitEdit();
            dg.CommitEdit();
        }
    }
}
