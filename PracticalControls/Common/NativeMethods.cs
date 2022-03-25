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
            Win32.User32.GetCursorPos(out Win32.POINT p);
            return new System.Windows.Point(p.x, p.y);
        }
    }
}
