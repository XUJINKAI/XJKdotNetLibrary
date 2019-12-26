using System.Drawing;
using System.Runtime.InteropServices;

namespace XJK.Win32.PInvoke
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT : System.IEquatable<POINT>
    {
        public int X;
        public int Y;

        public override string ToString() => $"POINT({X}, {Y})";


        public bool Equals(POINT other) => X == other.X && Y == other.Y;

        public override bool Equals(object? obj) => obj is POINT p && p.Equals(this);

        public static bool operator ==(POINT left, POINT right) => left.Equals(right);

        public static bool operator !=(POINT left, POINT right) => !left.Equals(right);


        public Point ToPoint() => new Point(X, Y);

        public static implicit operator Point(POINT point) => point.ToPoint();


        public override int GetHashCode() => this.ToString().GetHashCode(System.StringComparison.Ordinal);
    }
}
