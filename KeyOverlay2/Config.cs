﻿using GitHelper;
using Veldrid;

namespace KeyOverlay2
{
    public class Config
    {
        public GitHelpersConfig GitHelpersConfig { get; set; } = new()
        {
            GitUrl = "https://github.com/insomnyawolf/KeyOverlay/",
        };

        public AppConfig AppConfig { get; set; } = new();
    }

    public class AppConfig
    {
        public WindowConfig WindowConfig { get; set; } = new();
    }

    public class WindowConfig
    {
        public GraphicsBackend? GraphicsBackend { get; set; } = null;
        public bool Vsync { get; set; } = true;
        public ushort Width { get; set; } = 600;
        public ushort Height { get; set; } = 800;
        public string WindowTitle { get; set; } = "KeyOverlay 2";
    }
}
