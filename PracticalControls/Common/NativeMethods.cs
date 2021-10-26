using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalControls.Common
{
    public class NativeMethods
    {
        public static System.Windows.Point GetCursorPos()
        {
            WinApi.User32.User32Methods.GetCursorPos(out NetCoreEx.Geometry.Point p);
            return new System.Windows.Point(p.X, p.Y);
        }
    }
}
