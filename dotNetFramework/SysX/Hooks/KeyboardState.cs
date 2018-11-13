using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XJK.PInvoke;

namespace XJK.SysX.Hooks
{
    [Serializable]
    public class KeyboardState
    {
        public const int LENGTH = 256;

        [XmlElement("KeyboardState")]
        public string Value
        {
            get => ConvertByteArrayToIntString(Bytes);
            set => Bytes = ConvertIntSringToByteArray(value);
        }

        [XmlIgnore]
        public BigInteger BigInteger
        {
            get => ConvertByteArrayToBigInteger(Bytes);
            set => Bytes = ConvertBigIntegerToByteArray(value);
        }

        [XmlIgnore]
        public byte[] Bytes
        {
            get => _keyStates;
            set
            {
                if (value.Length != LENGTH) throw new ArgumentException("Length of array must be 256.");
                for (int i = 0; i < LENGTH; i++)
                {
                    _keyStates[i] = value[i];
                }
            }
        }

        [XmlIgnore]
        public readonly byte[] _keyStates = new byte[LENGTH];

        public int PressedCount => _keyStates.Count(b => IsPressed(b));
        public bool ModifiersPressed => AltPressed || CtrlPressed || ShiftPressed || WinPressed;
        public bool AltPressed => this[VirtualKeys.Menu] || this[VirtualKeys.LeftMenu] || this[VirtualKeys.RightMenu];
        public bool CtrlPressed => this[VirtualKeys.Control] || this[VirtualKeys.LeftControl] || this[VirtualKeys.RightControl];
        public bool ShiftPressed => this[VirtualKeys.Shift] || this[VirtualKeys.LeftShift] || this[VirtualKeys.RightShift];
        public bool WinPressed => this[VirtualKeys.LeftWindows] || this[VirtualKeys.RightWindows];
        public bool CapsLockToggled => IsToggled((int)VirtualKeys.CapsLock);
        public bool NumLockToggled => IsToggled((int)VirtualKeys.NumLock);
        public bool ScrollLockToggled => IsToggled((int)VirtualKeys.ScrollLock);

        public void PressDown(int code) => _keyStates[code] |= 0b1000_0000;
        public void PressUp(int code) => _keyStates[code] &= 0b0000_1111;
        public bool IsPressed(int code) => _keyStates[code] >> 4 > 0;
        public bool IsToggled(int code) => _keyStates[code] << 4 > 0;
        public bool IsPressOneOrZero(VirtualKeys vk) => PressedCount == 0 || (PressedCount == 1 && IsPressed((int)vk));

        public bool this[int key]
        {
            get => IsPressed(key);
            set
            {
                if (value) PressDown(key);
                else PressUp(key);
            }
        }
        public bool this[VirtualKeys key]
        {
            get => this[(int)key];
            set => this[(int)key] = value;
        }

        public KeyboardState() { }

        public KeyboardState(byte[] bytes)
        {
            Bytes = bytes;
        }

        public KeyboardState(BigInteger bigInteger)
        {
            BigInteger = bigInteger;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < LENGTH; i++)
            {
                if (this[i])
                {
                    s.Append((VirtualKeys)i).Append(',');
                }
            }
            if (s.Length > 0) s.Remove(s.Length - 1, 1);
            return s.ToString();
        }


        public KeyboardState SetByCurrentState()
        {
            for (int i = 0; i < LENGTH; i++)
            {
                _keyStates[i] = (byte)User32.GetKeyState(i);
            }
            return this;
        }

        public static KeyboardState FromCurrentState()
        {
            return new KeyboardState().SetByCurrentState();
        }


        public static string ConvertByteArrayToIntString(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            byte b;
            for (int i = 0; i < 256; i++)
            {
                b = bytes[i];
                if (b >> 4 > 0)
                {
                    s.Append(i).Append(',');
                }
            }
            if (s.Length > 0) s.Remove(s.Length - 1, 1);
            return s.ToString();
        }

        public static byte[] ConvertIntSringToByteArray(string s)
        {
            byte[] bytes = new byte[256];
            foreach (var i in s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(c => int.Parse(c)))
            {
                bytes[i] = 1 << 7;
            }
            return bytes;
        }

        public static BigInteger ConvertByteArrayToBigInteger(byte[] bytes)
        {
            byte[] array = new byte[LENGTH / 8];
            for (int i = 0; i < LENGTH / 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (bytes[i * 8 + j] >> 4 > 0)
                    {
                        array[i] |= (byte)(1 << j);
                    }
                    else
                    {
                        array[i] &= (byte)~(1 << j);
                    }
                }
            }
            return new BigInteger(array);
        }

        public static byte[] ConvertBigIntegerToByteArray(BigInteger bigInteger)
        {
            byte[] bytes = new byte[LENGTH];
            byte[] array = bigInteger.ToByteArray();
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    bytes[i * 8 + j] = (byte)(((array[i] >> j) & 1) << 7);
                }
            }
            return bytes;
        }
    }
}
