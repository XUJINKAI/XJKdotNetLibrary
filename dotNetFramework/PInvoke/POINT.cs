using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJK.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public override string ToString()
        {
            return $"{X}, {Y}";
        }

        public static implicit operator Point(POINT point) => new Point(point.X, point.Y);
    }
}
