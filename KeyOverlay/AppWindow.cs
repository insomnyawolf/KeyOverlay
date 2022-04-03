﻿using System;
using System.Collections.Generic;
using System.IO;
using sf;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace KeyOverlay
{
    public class AppWindow
    {
        //Path.GetFullPath()
        public static readonly string ResourcesLocation = Path.Combine(Program.ProgramLocation, "Resources");

        public static Config Configuration => Program.ConfigHelper.Config;

        public bool ShallStart = true;

        private int ActiveKeys;
        private RenderWindow Window;

        private double RatioX;
        private double RatioY;

        private List<Key> KeyList;
        private List<RoundedRectangleShape> SquareList;
        private List<Drawable> StaticDrawables;

        private Sprite Background;
        private Sprite FadingSprite;
        private Font Font;

        private Clock Clock = new();

        // I don't get the implementation of rotation
        // it behaves in a weird way

        public AppWindow()
        {
            Program.ConfigHelper.OnConfigurationChanged += Reload;
        }

        public void Load()
        {
            Window?.Close();

            ActiveKeys = 0;

            KeyList = new();
            //create keys which will be used to create the squares and text
            for (var i = 0; i < Configuration.Keys.Count; i++)
            {
                try
                {
                    var current = Configuration.Keys[i];

                    if (current.IsDisabled)
                    {
                        continue;
                    }

                    ActiveKeys++;
                    var key = new Key(current);
                    KeyList.Add(key);
                }
                catch (InvalidOperationException e)
                {
                    //invalid key
                    Console.WriteLine(e.Message);
                    using var sw = new StreamWriter("keyErrorMessage.txt");
                    sw.WriteLine(e.Message);
                }
            }

            ushort windowWidth = 0;
            ushort windowHeight = 0;

            //get background image if in config
            if (!string.IsNullOrWhiteSpace(Configuration.BackgroundImage))
            {
                var path = Path.Combine(ResourcesLocation, Configuration.BackgroundImage);
                var texture = new Texture(path);
                Background = new Sprite(texture);
            }

            // Create Window
            if (Configuration.Height > windowHeight)
            {
                windowHeight = Configuration.Height;
            }

            var tempWidth = (ushort)((Configuration.KeySize * ActiveKeys) + (Configuration.OutLineThickness * ActiveKeys * 2) + (Configuration.Margin * 2) + (Configuration.KeySpacing * (ActiveKeys - 1)));

            if (tempWidth > windowWidth)
            {
                windowWidth = tempWidth;
            }
            else
            {
                // Adjust margin
                var diff = windowWidth - tempWidth;

                Configuration.Margin += (ushort)(tempWidth / 2);
            }

            Window = new RenderWindow(new VideoMode(windowWidth, windowHeight), "KeyOverlay", Styles.Default);

            //calculate screen ratio relative to original program size for easy resizing
            RatioX = windowWidth / 480d;
            RatioY = windowHeight / 960d;

            Font = new Font(Path.GetFullPath(Path.Combine(ResourcesLocation, Configuration.Font)));

            //create squares
            SquareList = CreateItems.CreateKeys(ActiveKeys, Configuration.OutLineThickness, Configuration.KeySize, Configuration.KeySpacing, RatioX, RatioY, Configuration.Margin, Window, Configuration.KeyBackgroundColor, Configuration.BorderColor);

            StaticDrawables = new();

            // Add stuff to static drawables
            for (int i = 0; i < ActiveKeys; i++)
            {
                // Add Squares
                StaticDrawables.Add(SquareList[i]);

                // Add texts
                var text = CreateItems.CreateText(KeyList[i].KeyMapping.Display(), SquareList[i], Configuration.FontColor, false, Configuration.TextRotation, Font);
                StaticDrawables.Add(text);
            }

            //Creating a sprite for the fading effect
            var fadingList = Fading.GetBackgroundColorFadingTexture(Configuration.BackgroundColor, Window.Size.X, RatioY, Configuration.KeySize);
            var fadingTexture = new RenderTexture(Window.Size.X, (uint)(RatioY * 960 - (Configuration.OutLineThickness * 2 - Configuration.KeySize - Configuration.Margin)));

            fadingTexture.Clear(Color.Transparent);

            if (Configuration.Fade)
            {
                for (int i = 0; i < fadingList.Count; i++)
                {
                    fadingTexture.Draw(fadingList[i]);
                }
            }

            fadingTexture.Display();

            FadingSprite = new Sprite(fadingTexture.Texture);

            Window.Closed += OnClose;
            Window.SetFramerateLimit(Configuration.MaxFps);

            ShallStart = false;
        }

        private void OnClose(object sender, EventArgs e)
        {
            Window.Close();
        }

        private void Reload()
        {
            ShallStart = true;
        }

        public void Run()
        {
            while (Window.IsOpen)
            {
                Window.Clear(Configuration.BackgroundColor);
                Window.DispatchEvents();

                //if no keys are being held fill the square with bg color
                for (int i = 0; i < SquareList.Count; i++)
                {
                    SquareList[i].FillColor = Configuration.KeyBackgroundColor;
                }

                //if a key is being held, change the key bg and increment hold variable of key
                for (int i = 0; i < KeyList.Count; i++)
                {
                    var key = KeyList[i];
                    if (key.isKey && Keyboard.IsKeyPressed(key.KeyboardKey) || !key.isKey && Mouse.IsButtonPressed(key.MouseButton))
                    {
                        key.Hold++;
                        SquareList[i].FillColor = key.KeyMapping.Color;
                    }
                    else
                    {
                        key.Hold = 0;
                    }
                }

                MoveBars(KeyList, SquareList);

                //draw bg from image if not null

                if (Background is not null)
                {
                    Window.Draw(Background);
                }

                for (int i = 0; i < StaticDrawables.Count; i++)
                {
                    Window.Draw(StaticDrawables[i]);
                }

                for (int i = 0; i < KeyList.Count; i++)
                {
                    var key = KeyList[i];

                    if (Configuration.HitCount)
                    {
                        var text = CreateItems.CreateText(key.KeyMapping.Count.ToString(), SquareList[i], Configuration.FontColor, true, Configuration.TextRotation, Font);
                        Window.Draw(text);
                    }

                    for (int j = 0; j < key.BarList.Count; j++)
                    {
                        Window.Draw(key.BarList[j]);
                    }

                }

                Window.Draw(FadingSprite);

                Window.Display();
            }
        }

        /// <summary>
        /// if a key is a new input create a new bar, if it is being held stretch it and move all bars up
        /// </summary>
        private void MoveBars(List<Key> keyList, List<RoundedRectangleShape> squareList)
        {
            // Normalize
            var moveDist = Clock.Restart().AsSeconds() * Configuration.BarSpeed;

            for (int i = 0; i < keyList.Count; i++)
            {
                var key = keyList[i];

                if (key.Hold == 1)
                {
                    var rect = CreateItems.CreateBar(squareList[i], Configuration.OutLineThickness, moveDist);
                    key.BarList.Add(rect);
                    key.KeyMapping.Count++;
                }
                else if (key.Hold > 1)
                {
                    var rect = key.BarList[^1];
                    rect.Size = new Vector2f(rect.Size.X, rect.Size.Y + moveDist);
                }

                for (int j = 0; j < key.BarList.Count; j++)
                {
                    var rect = key.BarList[j];
                    rect.Position = new Vector2f(rect.Position.X, rect.Position.Y - moveDist);
                }

                if (key.BarList.Count > 0)
                {
                    var firstBar = key.BarList[0];
                    if (firstBar.Position.Y + firstBar.Size.Y < 0)
                    {
                        key.BarList.RemoveAt(0);
                    }
                }
            }
        }
    }
}