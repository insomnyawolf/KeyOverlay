using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace KeyOverlay
{
    public class Key
    {
        public KeyMapping KeyMapping;

        public int Hold { get; set; }
        public List<RectangleShape> BarList = new();
        public readonly Keyboard.Key KeyboardKey;
        public readonly Mouse.Button MouseButton;
        public readonly bool isKey;

        public Key(KeyMapping key)
        {
            KeyMapping = key;
            KeyMapping.Key = KeyMapping.Key.ToUpper();

            switch (KeyMapping.KeyType)
            {
                case KeyType.Keyboard:
                    isKey = true;
                    if (!Enum.TryParse(KeyMapping.Key, out KeyboardKey))
                    {
                        ThrowExc(KeyMapping);
                    }
                    break;
                case KeyType.Mouse:
                    
                    if (!Enum.TryParse(KeyMapping.Key, out MouseButton))
                    {
                        ThrowExc(KeyMapping);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void ThrowExc(KeyMapping key)
        {
            string exceptName = "Invalid key " + key.Key;
            throw new InvalidOperationException(exceptName);
        }
    }

}