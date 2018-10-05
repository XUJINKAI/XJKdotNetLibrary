using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace XJK.Network.Socket
{
    public enum SocketType : int
    {
        None = 0,
        Request = 1,
        Response = 2,
        Shutdown = 10,
    }

    [StructLayout(LayoutKind.Sequential, Size = 128, CharSet = CharSet.Ansi, Pack = 1)]
    public struct SocketHeader
    {
        public const int Size = 128;
        public int ID;
        public int Length;
        public SocketType Type;

        public byte[] ToBytesArray()
        {
            byte[] Bytes = new byte[Size];
            IntPtr ptr = Marshal.AllocHGlobal(Size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, Bytes, 0, Size);
            Marshal.FreeHGlobal(ptr);
            return Bytes;
        }

        public static SocketHeader FromBytes(byte[] bytes)
        {
            SocketHeader result;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                result = (SocketHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(SocketHeader));
            }
            finally
            {
                handle.Free();
            }
            return result;
        }

        public override string ToString()
        {
            return $"[{Type}][{ID}][{Length}]";
        }
    }

}
