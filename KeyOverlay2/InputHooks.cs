using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KeyOverlay2
{
    // Based on https://stackoverflow.com/questions/46013287/c-sharp-global-keyboard-hook-that-opens-a-form-from-a-console-application
    public class KeyPressedArgs : EventArgs
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static KeyPressedArgs()
        {
            JsonSerializerOptions = new()
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                IncludeFields = true,
            };

            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public Keys Key { get; private set; }
        public KeyEvents EventType { get; private set; }
        public bool Shift { get; private set; }
        public bool Ctrl { get; private set; }
        public bool Alt { get; private set; }

        internal KeyPressedArgs(Keys key, KeyEvents EventType, bool Shift, bool Ctrl, bool Alt)
        {
            Key = key;
            this.Shift = Shift;
            this.Ctrl = Ctrl;
            this.Alt = Alt;
            this.EventType = EventType;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }

    public enum KeyEvents
    {
        Down,
        Up
    }

    public class LowLevelInputHook : IDisposable
    {
        private readonly bool Global;

        public delegate void KeyEvent(KeyPressedArgs @event);
        public event KeyEvent OnKeyEvent;

        private delegate int CallbackDelegate(int Code, IntPtr W, IntPtr L);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(HookType idHook, CallbackDelegate lpfn, int hInstance, int threadId);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern short GetKeyState(Keys nVirtKey);

        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        private readonly int HookID;
        private readonly CallbackDelegate OnHookCallBack;

        //Start hook
        public LowLevelInputHook(bool Global)
        {
#if DEBUG
            OnKeyEvent += (KeyPressedArgs @event) =>
            {
                Console.WriteLine(@event);
            };
#endif

            this.Global = Global;
            OnHookCallBack = new CallbackDelegate(KeybHookProc);
            if (Global)
            {
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, OnHookCallBack,
                    0, //0 for local hook. eller hwnd til user32 for global
                    0); //0 for global hook. eller thread for hooken
            }
            else
            {
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD, OnHookCallBack,
                    0, //0 for local hook. or hwnd to user32 for global
                    GetCurrentThreadId()); //0 for global hook. or thread for the hook
            }
        }

        //The listener that will trigger events
        [STAThread]
        private int KeybHookProc(int Code, IntPtr W, IntPtr L)
        {
            if (Code < 0)
            {
                return CallNextHookEx(HookID, Code, W, L);
            }
            try
            {
                if (!Global)
                {
                    var keydownup = L.ToInt64() >> 30;

                    if (keydownup == 0)
                    {
                        OnKeyEvent?.Invoke(new KeyPressedArgs((Keys)W, KeyEvents.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed()));
                    }
                    if (keydownup == 3)
                    {
                        OnKeyEvent?.Invoke(new KeyPressedArgs((Keys)W, KeyEvents.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed()));
                    }
                }
                else
                {
                    RawKeyEvents kEvent = (RawKeyEvents)W;
                    var vkCode = Marshal.ReadInt32(L); //Leser vkCode som er de første 32 bits hvor L peker.
                    if (kEvent == RawKeyEvents.KeyDown || kEvent == RawKeyEvents.SKeyDown)
                    {
                        OnKeyEvent?.Invoke(new KeyPressedArgs((Keys)vkCode, KeyEvents.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed()));
                    }
                    if (kEvent == RawKeyEvents.KeyUp || kEvent == RawKeyEvents.SKeyUp)
                    {
                        OnKeyEvent?.Invoke(new KeyPressedArgs((Keys)vkCode, KeyEvents.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed()));
                    }
                }
            }
            catch (Exception ex)
            {
                //Ignore all errors...
#warning shall we really?
#if DEBUG
                throw;
#endif
            }
            return CallNextHookEx(HookID, Code, W, L);
        }

        private enum RawKeyEvents
        {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SKeyDown = 0x0104,
            SKeyUp = 0x0105
        }

        public static bool GetCapslock()
        {
            return Convert.ToBoolean(GetKeyState(Keys.CapsLock)) & true;
        }
        public static bool GetNumlock()
        {
            return Convert.ToBoolean(GetKeyState(Keys.NumLock)) & true;
        }
        public static bool GetScrollLock()
        {
            return Convert.ToBoolean(GetKeyState(Keys.Scroll)) & true;
        }
        public static bool GetShiftPressed()
        {
            int state = GetKeyState(Keys.ShiftKey);
            if (state < -1 || state > 1)
            {
                return true;
            }

            return false;
        }
        public static bool GetCtrlPressed()
        {
            int state = GetKeyState(Keys.ControlKey);
            if (state < -1 || state > 1)
            {
                return true;
            }

            return false;
        }
        public static bool GetAltPressed()
        {
            int state = GetKeyState(Keys.Menu);
            if (state < -1 || state > 1)
            {
                return true;
            }

            return false;
        }

        // Destructor
        bool IsFinalized = false;
        ~LowLevelInputHook()
        {
            if (!IsFinalized)
            {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }
        public void Dispose()
        {
            if (!IsFinalized)
            {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }
    }

    // Referenced from https://referencesource.microsoft.com/#System.Windows.Forms/winforms/Managed/System/WinForms/Keys.cs to avoid dependences on winforms

    //------------------------------------------------------------------------------
    // <copyright file="Keys.cs" company="Microsoft">
    //     Copyright (c) Microsoft Corporation.  All rights reserved.
    // </copyright>                                                                
    //------------------------------------------------------------------------------

    // Certain memberd of Keys enum are actually meant to be OR'ed.
    [Flags]
    public enum Keys
    {
        KeyCode = 0x0000FFFF,
        Modifiers = unchecked((int)0xFFFF0000),
        None = 0x00,
        LButton = 0x01,
        RButton = 0x02,
        Cancel = 0x03,
        MButton = 0x04,
        XButton1 = 0x05,
        XButton2 = 0x06,
        Back = 0x08,
        Tab = 0x09,
        LineFeed = 0x0A,
        Clear = 0x0C,
        Return = 0x0D,
        Enter = Return,
        ShiftKey = 0x10,
        ControlKey = 0x11,
        Menu = 0x12,
        Pause = 0x13,
        Capital = 0x14,
        CapsLock = 0x14,
        KanaMode = 0x15,
        HanguelMode = 0x15,
        HangulMode = 0x15,
        JunjaMode = 0x17,
        FinalMode = 0x18,
        HanjaMode = 0x19,
        KanjiMode = 0x19,
        Escape = 0x1B,
        IMEConvert = 0x1C,
        IMENonconvert = 0x1D,
        IMEAccept = 0x1E,
        IMEAceept = IMEAccept,
        IMEModeChange = 0x1F,
        Space = 0x20,
        Prior = 0x21,
        PageUp = Prior,
        Next = 0x22,
        PageDown = Next,
        End = 0x23,
        Home = 0x24,
        Left = 0x25,
        Up = 0x26,
        Right = 0x27,
        Down = 0x28,
        Select = 0x29,
        Print = 0x2A,
        Execute = 0x2B,
        Snapshot = 0x2C,
        PrintScreen = Snapshot,
        Insert = 0x2D,
        Delete = 0x2E,
        Help = 0x2F,
        // 0
        D0 = 0x30,
        // 1
        D1 = 0x31,
        // 2
        D2 = 0x32,
        // 3
        D3 = 0x33,
        // 4
        D4 = 0x34,
        // 5
        D5 = 0x35,
        // 6
        D6 = 0x36,
        // 7
        D7 = 0x37,
        // 8
        D8 = 0x38,
        // 9
        D9 = 0x39,
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
        LWin = 0x5B,
        RWin = 0x5C,
        Apps = 0x5D,
        Sleep = 0x5F,
        NumPad0 = 0x60,
        NumPad1 = 0x61,
        NumPad2 = 0x62,
        NumPad3 = 0x63,
        NumPad4 = 0x64,
        NumPad5 = 0x65,
        NumPad6 = 0x66,
        NumPad7 = 0x67,
        NumPad8 = 0x68,
        NumPad9 = 0x69,
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
        NumLock = 0x90,
        Scroll = 0x91,
        LShiftKey = 0xA0,
        RShiftKey = 0xA1,
        LControlKey = 0xA2,
        RControlKey = 0xA3,
        LMenu = 0xA4,
        RMenu = 0xA5,
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
        MediaPreviousTrack = 0xB1,
        MediaStop = 0xB2,
        MediaPlayPause = 0xB3,
        LaunchMail = 0xB4,
        SelectMedia = 0xB5,
        LaunchApplication1 = 0xB6,
        LaunchApplication2 = 0xB7,
        OemSemicolon = 0xBA,
        Oem1 = OemSemicolon,
        Oemplus = 0xBB,
        Oemcomma = 0xBC,
        OemMinus = 0xBD,
        OemPeriod = 0xBE,
        OemQuestion = 0xBF,
        Oem2 = OemQuestion,
        Oemtilde = 0xC0,
        Oem3 = Oemtilde,
        OemOpenBrackets = 0xDB,
        Oem4 = OemOpenBrackets,
        OemPipe = 0xDC,
        Oem5 = OemPipe,
        OemCloseBrackets = 0xDD,
        Oem6 = OemCloseBrackets,
        OemQuotes = 0xDE,
        Oem7 = OemQuotes,
        Oem8 = 0xDF,
        OemBackslash = 0xE2,
        Oem102 = OemBackslash,
        ProcessKey = 0xE5,
        Packet = 0xE7,
        Attn = 0xF6,
        Crsel = 0xF7,
        Exsel = 0xF8,
        EraseEof = 0xF9,
        Play = 0xFA,
        Zoom = 0xFB,
        //A constant reserved for future use.
        NoName = 0xFC,
        Pa1 = 0xFD,
        OemClear = 0xFE,
        Shift = 0x00010000,
        Control = 0x00020000,
        Alt = 0x00040000,
    }
}
