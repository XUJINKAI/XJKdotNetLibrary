using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJK.PInvoke
{
    public static class VirtualKeysExtension
    {
        public static char ToChar(this VirtualKeys vk) => (char)vk;
        public static int ToInt(this VirtualKeys vk) => (int)vk;

        public static bool IsModifiers(this VirtualKeys vk)
        {
            return vk == VirtualKeys.Control || vk == VirtualKeys.LeftControl || vk == VirtualKeys.RightControl
                || vk == VirtualKeys.Shift || vk == VirtualKeys.LeftShift || vk == VirtualKeys.RightShift
                || vk == VirtualKeys.Menu || vk == VirtualKeys.LeftMenu || vk == VirtualKeys.RightMenu
                || vk == VirtualKeys.LeftWindows || vk == VirtualKeys.RightWindows;
        }

        public static bool IsMouse(this VirtualKeys vk)
        {
            return vk == VirtualKeys.LeftButton || vk == VirtualKeys.Right || vk == VirtualKeys.MiddleButton
                || vk == VirtualKeys.ExtraButton1 || vk == VirtualKeys.ExtraButton2;
        }
    }

    public enum VirtualKeys : int
    {
        /// <summary>
        /// Nothing be pressed
        /// </summary>
        None = 0x00,
        LeftButton = 0x01,
        RightButton = 0x02,
        /// <summary>
        /// Control-break processing
        /// </summary>
        Cancel = 0x03,
        MiddleButton = 0x04,
        ExtraButton1 = 0x05,
        ExtraButton2 = 0x06,
        //0x07
        /// <summary>
        /// BACKSPACE key
        /// </summary>
        Back = 0x08,
        Tab = 0x09,
        //0x0A-0B Reserved
        Clear = 0x0C,
        Return = 0x0D,
        //0x0E-0F
        Shift = 0x10,
        Control = 0x11,
        Menu = 0x12,
        Pause = 0x13,
        CapsLock = 0x14,
        /// <summary>
        /// IME Kana mode
        /// </summary>
        Kana = 0x15,
        /// <summary>
        /// IME Hanguel mode (maintained for compatibility; use VK_HANGUL)
        /// </summary>
        Hangeul = 0x15,
        /// <summary>
        /// IME Hangul mode
        /// </summary>
        Hangul = 0x15,
        //0x16
        /// <summary>
        /// IME Junja mode
        /// </summary>
        Junja = 0x17,
        /// <summary>
        /// IME final mode
        /// </summary>
        Final = 0x18,
        /// <summary>
        /// IME Hanja mode
        /// </summary>
        Hanja = 0x19,
        /// <summary>
        /// IME Kanji mode
        /// </summary>
        Kanji = 0x19,
        //0x1A
        Escape = 0x1B,
        /// <summary>
        /// IME convert
        /// </summary>
        Convert = 0x1C,
        /// <summary>
        /// IME nonconvert
        /// </summary>
        NonConvert = 0x1D,
        /// <summary>
        /// IME accept
        /// </summary>
        Accept = 0x1E,
        /// <summary>
        /// IME mode change request
        /// </summary>
        ModeChange = 0x1F,
        Space = 0x20,
        /// <summary>
        /// PAGE UP key
        /// </summary>
        Prior = 0x21,
        /// <summary>
        /// PAGE DOWN key
        /// </summary>
        Next = 0x22,
        End = 0x23,
        Home = 0x24,
        Left = 0x25,
        Up = 0x26,
        Right = 0x27,
        Down = 0x28,
        Select = 0x29,
        Print = 0x2A,
        Execute = 0x2B,
        /// <summary>
        /// PRINT SCREEN key
        /// </summary>
        Snapshot = 0x2C,
        Insert = 0x2D,
        Delete = 0x2E,
        Help = 0x2F,
        N0 = 0x30,
        N1 = 0x31,
        N2 = 0x32,
        N3 = 0x33,
        N4 = 0x34,
        N5 = 0x35,
        N6 = 0x36,
        N7 = 0x37,
        N8 = 0x38,
        N9 = 0x39,
        //0x3A-40
        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,
        LeftWindows = 0x5B,
        RightWindows = 0x5C,
        Application = 0x5D,
        //0x5E
        /// <summary>
        /// Computer Sleep key
        /// </summary>
        Sleep = 0x5F,
        Numpad0 = 0x60,
        Numpad1 = 0x61,
        Numpad2 = 0x62,
        Numpad3 = 0x63,
        Numpad4 = 0x64,
        Numpad5 = 0x65,
        Numpad6 = 0x66,
        Numpad7 = 0x67,
        Numpad8 = 0x68,
        Numpad9 = 0x69,
        Multiply = 0x6A,
        Add = 0x6B,
        Separator = 0x6C,
        Subtract = 0x6D,
        Decimal = 0x6E,
        Divide = 0x6F,
        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        F13 = 0x7C,
        F14 = 0x7D,
        F15 = 0x7E,
        F16 = 0x7F,
        F17 = 0x80,
        F18 = 0x81,
        F19 = 0x82,
        F20 = 0x83,
        F21 = 0x84,
        F22 = 0x85,
        F23 = 0x86,
        F24 = 0x87,
        //0x88-8F Unassigned
        NumLock = 0x90,
        ScrollLock = 0x91,
        //0x92-96 OEM specific
        //0x97-9F Unassigned
        LeftShift = 0xA0,
        RightShift = 0xA1,
        LeftControl = 0xA2,
        RightControl = 0xA3,
        LeftMenu = 0xA4,
        RightMenu = 0xA5,
        BrowserBack = 0xA6,
        BrowserForward = 0xA7,
        BrowserRefresh = 0xA8,
        BrowserStop = 0xA9,
        BrowserSearch = 0xAA,
        BrowserFavorites = 0xAB,
        BrowserHome = 0xAC,
        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUp = 0xAF,
        MediaNextTrack = 0xB0,
        MediaPrevTrack = 0xB1,
        MediaStop = 0xB2,
        MediaPlayPause = 0xB3,
        LaunchMail = 0xB4,
        LaunchMediaSelect = 0xB5,
        LaunchApplication1 = 0xB6,
        LaunchApplication2 = 0xB7,
        //0xB8-B9 Reserved
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key
        /// </summary>
        OEM1 = 0xBA,
        OEMPlus = 0xBB,
        OEMComma = 0xBC,
        OEMMinus = 0xBD,
        OEMPeriod = 0xBE,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key
        /// </summary>
        OEM2 = 0xBF,
        /// <summary>
        /// Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key
        /// </summary>
        OEM3 = 0xC0,
        //0xC1-D7 Reserved
        //0xD8-DA Unassigned
        OEM4 = 0xDB,
        OemOpenBrackets = OEM4,
        OEM5 = 0xDC,
        OEM6 = 0xDD,
        OemCloseBrackets = OEM6,
        OEM7 = 0xDE,
        OEM8 = 0xDF,
        //0xE0 Reserved
        //0xE1 OEM specific
        /// <summary>
        /// Either the angle bracket key or the backslash key on the RT 102-key keyboard
        /// </summary>
        OEM102 = 0xE2,
        //0xE3-E4 OEM specific
        /// <summary>
        /// IME PROCESS key
        /// </summary>
        ProcessKey = 0xE5,
        //0xE6 OEM specific
        /// <summary>
        /// Used to pass Unicode characters as if they were keystrokes. 
        /// The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. 
        /// For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP
        /// </summary>
        Packet = 0xE7,
        //0xE8 Unassigned
        //0xE9-F5 OEM specific
        ATTN = 0xF6,
        CrSel = 0xF7,
        ExSel = 0xF8,
        /// <summary>
        /// Erase EOF key
        /// </summary>
        EREOF = 0xF9,
        Play = 0xFA,
        Zoom = 0xFB,
        /// <summary>
        /// Reserved
        /// </summary>
        Noname = 0xFC,
        PA1 = 0xFD,
        OEMClear = 0xFE,
        Undefined0xFF = 0xFF,
    }
}
