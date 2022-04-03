using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace KeyOverlay {
    public static class Fading
    {
        public static List<Sprite> GetBackgroundColorFadingTexture(Color backgroundColor, uint windowWidth, double ratioY, int keysize) 
        {
            var sprites = new List<Sprite>();
            var alpha = 255;
            var color = backgroundColor;
            var n = ratioY * 960 - 100 - keysize;
            for (int i = 0; i < n; i++)
            {
                Image img = ratioY>=.5f ? new Image(windowWidth, (uint)(2 * ratioY), color) : new Image(windowWidth, 1, color);
                var sprite = new Sprite(new Texture(img));
                sprite.Position = new Vector2f(0, img.Size.Y * i);
                sprites.Add(sprite);
                alpha -= (int)(255 / n);
                color.A = (byte)alpha;
            }
            return sprites;
        }
    }
}
