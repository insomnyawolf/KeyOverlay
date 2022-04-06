using System.Drawing;
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
        public InputType InputType { get; private set; }
        public bool Shift { get; private set; }
        public bool Ctrl { get; private set; }
        public bool Alt { get; private set; }

        internal KeyEvent(Keys Key, InputType InputType, bool Shift, bool Ctrl, bool Alt)
        {
            this.Key = Key;
            this.Shift = Shift;
            this.Ctrl = Ctrl;
            this.Alt = Alt;
            this.InputType = InputType;
        }
    }

    public enum InputType
    {
        Down,
        Up
    }

    public class MouseEvent : InputEventBase
    {
        public MouseMessage MouseMessage { get; private set; }
        public Point Position { get; private set; }

        internal MouseEvent(MouseMessage MouseMessage, Point Position)
        {
            this.MouseMessage = MouseMessage;
            this.Position = Position;
        }
    }
}
