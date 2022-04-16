using Veldrid;

namespace InEngine
{
    public class InEngineConfig
    {
        public WindowConfig WindowConfig { get; set; } = new();
    }

    public class WindowConfig
    {
        //public GraphicsBackend? GraphicsBackend { get; set; } = null;

        // Personal preference for development
        public GraphicsBackend? GraphicsBackend { get; set; } = null;
        public bool Vsync { get; set; } = true;
        public ushort Width { get; set; } = 800;
        public ushort Height { get; set; } = 600;
        public short XPosition { get; set; } = -800;
        public short YPosition { get; set; } = 30;
        public string WindowTitle { get; set; } = "KeyOverlay 2";
    }
}
