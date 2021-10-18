using PracticalControls.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Common.Helpers
{
    public class TreeDataGridHelper
    {
        public static void ResetRelationShip(IEnumerable<TreeDataGridItemBase> lstData)
        {
            if (!lstData.Any())
                return;

            Stack<TreeDataGridItemBase> stack = new Stack<TreeDataGridItemBase>();

            int startLevel = lstData.FirstOrDefault().Level;

            foreach (var data in lstData)
            {
                if (data.Level == startLevel)
                {
                    stack.Push(data);
                    continue;
                }

                var peek = stack.Peek();
                while (peek != null && data.Level <= peek.Level)
                {
                    stack.Pop();
                    if (stack.Count > 0)
                        peek = stack.Peek();
                    else
                        peek = null;
                }

                if (peek != null)
                {
                    peek.Children.Add(data);
                    data.Parent = peek;
                }

                stack.Push(data);//进栈
            }
        }
    }
}
