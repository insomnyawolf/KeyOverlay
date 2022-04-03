using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;

namespace KeyOverlay {
    public static class Fading
    {
#warning fails after around 1 minute for some reason
        public static List<Sprite> GetBackgroundColorFadingTexture(Color backgroundColor, uint windowWidth, double ratioY) 
        {
            var sprites = new List<Sprite>();
            var alpha = 255d;
            var color = backgroundColor;
            var n = ratioY * 960;
            for (int i = 0; i < n; i++)
            {
                uint verticalSize = 1;
                var sprite = new Sprite(new Texture(new Image(windowWidth, verticalSize, color)));
                sprite.Position = new Vector2f(0, verticalSize * i);
                sprites.Add(sprite);
                alpha -= 255 / n;
                color.A = (byte)alpha;
            }
            return sprites;
        }
    }
}
