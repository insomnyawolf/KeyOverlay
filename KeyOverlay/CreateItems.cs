using System.Collections.Generic;
using sf;
using SFML.Graphics;
using SFML.System;

namespace KeyOverlay
{
    public static class CreateItems
    {
        public static RectangleShape CreateBar(RoundedRectangleShape square, int outlineThickness, float barSpeed)
        {
            var rect = new RectangleShape(new Vector2f(square.mySize.X + outlineThickness * 2, barSpeed));

            rect.Position = new Vector2f(square.Position.X - outlineThickness, square.Position.Y - (square.mySize.Y / 2) - square.OutlineThickness);

            rect.FillColor = square.FillColor;

            return rect;
        }

        public static List<RoundedRectangleShape> CreateKeys(int keyAmount, int outlineThickness, float size, float spacing, double ratioX, double ratioY,
            int margin, RenderWindow window, Color backgroundColor, Color outlineColor)
        {
            var yPos = 900 * ratioY;
            var width = size + outlineThickness * 2;
            var keyList = new List<RoundedRectangleShape>();
            for (int i = 0; i < keyAmount; i++)
            {
                var square = new RoundedRectangleShape(new Vector2f(size, size), 5, 100);
                square.FillColor = backgroundColor;
                square.OutlineColor = outlineColor;
                square.OutlineThickness = outlineThickness;
                square.Origin = new Vector2f(0, size);
                square.Position = new Vector2f(margin + (outlineThickness + width + spacing) * i , (float)yPos); 
                keyList.Add(square);
            }
            return keyList;
        }

        public static Text CreateText(string key, RoundedRectangleShape square, Color color, bool counter, int rotation, Font font)
        {
            var text = new Text(key, font);
            text.CharacterSize = (uint)(50 * square.mySize.X / 140);
            text.Style = Text.Styles.Bold;
            text.FillColor = color;
            text.Rotation = rotation;
            text.Origin = new Vector2f(text.GetLocalBounds().Width / 2f, 32 * square.mySize.X / 140f);
            float extraWidth = 0;
            float extraHeight = 0;
            if (rotation < 0)
            {
                extraWidth = square.mySize.X;
            }
            else if (rotation == 0)
            {
                extraWidth = square.mySize.X / 2f;
                extraHeight = square.mySize.X / 2f + text.CharacterSize/2f;
            }
              
            if (counter)
            {
                text.Position = new Vector2f(square.GetGlobalBounds().Left + square.OutlineThickness - text.CharacterSize /3 * 2 * rotation/90f + extraWidth,
                    square.GetGlobalBounds().Top + square.OutlineThickness + text.CharacterSize * 3 / 2 + extraHeight);
            }
            else
            {
                text.Position = new Vector2f(square.GetGlobalBounds().Left + square.OutlineThickness + square.mySize.X / 2f,
                    square.GetGlobalBounds().Top + square.OutlineThickness + square.mySize.Y / 2f);
            }
            
            return text;
        }
    }
}