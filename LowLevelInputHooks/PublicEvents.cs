using System.Text.Json;
using System.Text.Json.Serialization;

namespace LowLevelInputHooks
{
    public class InputEvent : EventArgs
    {
        public InputOrigin InputOrigin { get; set; }
        public KeyEvent? KeyEvent { get; set; }
        public MouseEvent? MouseEvent { get; set; }

        internal InputEvent() { }

        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static InputEvent()
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

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }

    public enum InputOrigin
    {
        Mouse,
        Keyboard,
    }

    // Just exist to ease debugging propuses
    public abstract class InputEventBase
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static InputEventBase()
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

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }

    public class KeyEvent : InputEventBase
    {
        public Keys Key { get; private set; }
        public InputAction InputAction { get; private set; }
        public bool Shift { get; private set; }
        public bool Ctrl { get; private set; }
        public bool Alt { get; private set; }

        internal KeyEvent(Keys Key, InputAction InputType, bool Shift, bool Ctrl, bool Alt)
        {
            this.Key = Key;
            this.Shift = Shift;
            this.Ctrl = Ctrl;
            this.Alt = Alt;
            this.InputAction = InputType;
        }
    }

    public enum InputAction
    {
        Down,
        Up,
#warning consider separing move action
        Move,
        None
    }

    public class MouseEvent : InputEventBase
    {
        public MouseButton MouseButton { get; private set; }
        public InputAction InputAction { get; private set; }
        public Point Position { get; private set; }

        internal MouseEvent(MouseMessage MouseMessage, MsllHookStruct extra)
        {
            this.Position = extra.pt;

            switch (MouseMessage)
            {
                case MouseMessage.WM_MOUSEMOVE:
                case MouseMessage.WM_NCMOUSEMOVE:
                    InputAction = InputAction.Move;
                    MouseButton = MouseButton.None;
                    break;
                case MouseMessage.WM_LBUTTONDOWN:
                case MouseMessage.WM_NCLBUTTONDOWN:
                case MouseMessage.WM_LBUTTONDBLCLK:
                case MouseMessage.WM_NCLBUTTONDBLCLK:
                    InputAction = InputAction.Down;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_LBUTTONUP:
                case MouseMessage.WM_NCLBUTTONUP:
                    InputAction = InputAction.Up;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_RBUTTONDOWN:
                case MouseMessage.WM_NCRBUTTONDOWN:
                case MouseMessage.WM_RBUTTONDBLCLK:
                case MouseMessage.WM_NCRBUTTONDBLCLK:
                    InputAction = InputAction.Down;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_RBUTTONUP:
                case MouseMessage.WM_NCRBUTTONUP:
                    InputAction = InputAction.Up;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_MBUTTONDOWN:
                case MouseMessage.WM_NCMBUTTONDOWN:
                case MouseMessage.WM_MBUTTONDBLCLK:
                case MouseMessage.WM_NCMBUTTONDBLCLK:
                    InputAction = InputAction.Down;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MBUTTONUP:
                case MouseMessage.WM_NCMBUTTONUP:
                    InputAction = InputAction.Up;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MOUSEWHEEL:
                    MouseButton = MouseButton.Scroll;
                    if (extra.WheelDelta > 0)
                        InputAction = InputAction.Up;
                    else if (extra.WheelDelta < 0)
                        InputAction = InputAction.Down;
                    break;
                case MouseMessage.WM_MOUSEHWHEEL:
                    InputAction = InputAction.None;
                    MouseButton = MouseButton.WheelB;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
