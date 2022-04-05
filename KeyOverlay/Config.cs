using System.Collections.Generic;
using System.Text.Json.Serialization;
using SFML.Graphics;

namespace KeyOverlay
{
    // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/integral-numeric-types

    public class Config
    {
        // Replaced by key width * keycount + borders
        //public int Width { get; set; }
        public ushort Height { get; set; } = 700;
        public byte KeySize { get; set; } = 125;
        public byte KeySpacing { get; set; } = 1;
        public ushort Margin { get; set; } = 30;
        public ushort MaxFps { get; set; } = 60;
        public int BarSpeed { get; set; } = 600;
        public int OutLineThickness { get; set; } = 5;
        public string BackgroundImage { get; set; } = null;
#if RELEASE
        [JsonIgnore]
        // It's broken and crash the program, idk why
        public bool Fade { get; set; } = false;

#else
        public bool Fade { get; set; } = true;
#endif
        public bool HitCount { get; set; } = true;
        public byte TextRotation { get; set; } = 0;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color KeyBackgroundColor { get; set; } = Color.Black;
        public Color BorderColor { get; set; } = Color.White;
        public string Font { get; set; } = "consolab.ttf";
        public Color FontColor { get; set; } = Color.White;

        public List<KeyMapping> Keys { get; set; } = new List<KeyMapping>()
        {
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "z",
                DisplayName = null,
                Count = 0,
                Color = Color.Blue,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "x",
                DisplayName = null,
                Count = 0,
                Color = Color.Blue,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "C",
                DisplayName = null,
                Count = 0,
                Color = Color.Blue,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "Numpad0",
                DisplayName = "Np0",
                Count = 0,
                Color = Color.Blue,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "Num3",
                DisplayName = null,
                Count = 0,
                Color = Color.Blue,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Keyboard,
                Key = "q",
                DisplayName = null,
                Count = 0,
                Color = Color.Blue,
                IsDisabled = true,
            },
            //https://www.sfml-dev.org/documentation/2.5.1/Mouse_8hpp_source.php
            new KeyMapping()
            {
                KeyType = KeyType.Mouse,
                Key = "0",
                DisplayName = "LMB",
                Count = 0,
                Color = Color.Magenta,
                IsDisabled = false,
            },
            new KeyMapping()
            {
                KeyType = KeyType.Mouse,
                Key = "1",
                DisplayName = "RMB",
                Count = 0,
                Color = Color.Magenta,
                IsDisabled = false,
            },
        };
    }

    public class KeyMapping
    {
        public KeyType KeyType { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public ulong Count { get; set; }
        public Color Color { get; set; }
        public bool IsDisabled { get; set; }

        public string Display()
        {
            return DisplayName ?? Key;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum KeyType
    {
        Keyboard,
        Mouse
    }
}
