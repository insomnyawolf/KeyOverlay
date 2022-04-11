//#define INPUTDEBUG
using System.Runtime.InteropServices;
using LowLevelInputHooks.DeviceSpecific;
using LowLevelInputHooks.DeviceSpecific.Windows;

namespace LowLevelInputHooks
{
    // Based on https://stackoverflow.com/questions/46013287/c-sharp-global-keyboard-hook-that-opens-a-form-from-a-console-application
    public class LowLevelInputHook : IDisposable
    {
        private readonly bool Global;

        public delegate void InputEventDelegate(InputEvent @event);
        public event InputEventDelegate OnKeyEvent;

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
        private readonly int KeyboardHookID;
        private readonly CallbackDelegate OnKeyboardHookCallBack;

        private readonly int MouseHookID;
        private readonly CallbackDelegate OnMouseHookCallBack;

        //Start hook
        public LowLevelInputHook(bool Global)
        {
#if INPUTDEBUG
            OnKeyEvent += (InputEvent @event) =>
            {
                Console.WriteLine(@event);
            };
#endif

            this.Global = Global;

            OnKeyboardHookCallBack = new CallbackDelegate(KeybHookProc);
            OnMouseHookCallBack = new CallbackDelegate(MouseHookProc);

            //0 for local hook. eller hwnd til user32 for global
            int hInstance = 0;

            if (Global)
            {
                //0 for global hook. eller thread for hooken
                int threadId = 0;

                // Keyboard
                KeyboardHookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, OnKeyboardHookCallBack, hInstance, threadId);
                //Mouse
                MouseHookID = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnMouseHookCallBack, hInstance, threadId);
            }
            else
            {
                //0 for global hook. eller thread for hooken
                int threadId = GetCurrentThreadId();

                //Keyboard
                KeyboardHookID = SetWindowsHookEx(HookType.WH_KEYBOARD, OnKeyboardHookCallBack, hInstance, threadId);
                //Mouse
                MouseHookID = SetWindowsHookEx(HookType.WH_MOUSE, OnMouseHookCallBack, hInstance, threadId);
            }
        }

        #region keyboard

        private int KeybHookProc(int Code, IntPtr W, IntPtr L)
        {
            if (Code < 0)
            {
                return CallNextHookEx(KeyboardHookID, Code, W, L);
            }
            try
            {
                var input = new InputEvent()
                {
                    InputOrigin = InputOrigin.Keyboard,
                };

                if (!Global)
                {
                    // Overflow if you don't use int64
                    var keyState = L.ToInt64() >> 30;

                    if (keyState == 0)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyboardInputAction.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (keyState == 3)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyboardInputAction.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (keyState == 1)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyboardInputAction.Repeat, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else
                    {
                        throw new NotImplementedException($"keyState => {keyState}");
                    }
                }
                else
                {
                    RawKeyEvents kEvent = (RawKeyEvents)W;

                    // Virtual Key Code
                    var vkCode = Marshal.ReadInt32(L);

                    if (kEvent == RawKeyEvents.KeyDown || kEvent == RawKeyEvents.SKeyDown)
                    {
                        input.KeyEvent = new KeyEvent((Keys)vkCode, KeyboardInputAction.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (kEvent == RawKeyEvents.KeyUp || kEvent == RawKeyEvents.SKeyUp)
                    {
                        input.KeyEvent = new KeyEvent((Keys)vkCode, KeyboardInputAction.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else
                    {
                        throw new NotImplementedException($"keyState => {kEvent}");
                    }
                }

                OnKeyEvent?.Invoke(input);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.ToString());
                throw;
#else
#warning shall we really?
                //Ignore all errors...
#endif
            }
            return CallNextHookEx(KeyboardHookID, Code, W, L);
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
                return true;
            return false;
        }
        public static bool GetCtrlPressed()
        {
            int state = GetKeyState(Keys.ControlKey);
            if (state < -1 || state > 1)
                return true;
            return false;
        }
        public static bool GetAltPressed()
        {
            int state = GetKeyState(Keys.Menu);
            if (state < -1 || state > 1)
                return true;
            return false;
        }

        #endregion keyboard

        #region mouse
        public int MouseHookProc(int Code, IntPtr W, IntPtr L)
        {
            if (Code < 0)
            {
                return CallNextHookEx(MouseHookID, Code, W, L);
            }

            var mouseExtraData = new MsllHookStruct();
            Marshal.PtrToStructure(L, mouseExtraData);

            var input = new InputEvent()
            {
                InputOrigin = InputOrigin.Mouse,
                MouseEvent = new WindowsMouseEvent((MouseMessage)W, mouseExtraData, Global)
            };

            OnKeyEvent?.Invoke(input);


            // Pass the hook information to the next hook procedure in chain
            return CallNextHookEx(MouseHookID, Code, W, L);
        }

        #endregion mouse

        // Destructor
        bool IsFinalized = false;
        ~LowLevelInputHook()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!IsFinalized)
            {
                UnhookWindowsHookEx(KeyboardHookID);
                UnhookWindowsHookEx(MouseHookID);
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}