using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace KeyOverlay2
{
    public abstract class BaseWindow
    {
        private readonly Stopwatch StopWatch = new();

        internal readonly Sdl2Window Window;
        internal readonly GraphicsDevice GraphicsDevice;

        // Using a single command list to avoid weird render behaviour in screenshoots
        internal CommandList CommandList;

        internal DeviceBuffer VertexBuffer;
        internal DeviceBuffer IndexBuffer;
        internal Shader[] Shaders;
        internal Pipeline Pipeline;

        internal readonly AppConfig AppConfig;

        public BaseWindow(AppConfig Config)
        {
            AppConfig = Config;

            var WindowConfig = AppConfig.WindowConfig;

            var windowCI = new WindowCreateInfo()
            {
                X = 100,
                Y = 100,
                WindowWidth = WindowConfig.Width,
                WindowHeight = WindowConfig.Height,
                WindowTitle = WindowConfig.WindowTitle,
                WindowInitialState = WindowState.Normal
            };

            Window = VeldridStartup.CreateWindow(ref windowCI);

            var options = new GraphicsDeviceOptions()
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                SyncToVerticalBlank = WindowConfig.Vsync,
            };

            var GraphicsBackend = WindowConfig.GraphicsBackend ?? VeldridStartup.GetPlatformDefaultBackend();

            GraphicsDevice = VeldridStartup.CreateGraphicsDevice(Window, options, GraphicsBackend);
        }

        private void DrawInternals(InputSnapshot input, float deltaTime)
        {
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            Update(input, deltaTime);

            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
        }

        internal abstract void Update(InputSnapshot input, float deltaTime);

        public void Start()
        {
            while (Window.Exists)
            {
                var deltaTime = Frametime();
                var input = Window.PumpEvents();
                DrawInternals(input, deltaTime);                
            }

            DisposeResources();
        }

        // Compute actual value for deltaSeconds.
        private float Frametime()
        {
            var frametime = StopWatch.ElapsedMilliseconds / 1000f;
            StopWatch.Restart();
            return frametime;
        } 

        private void DisposeResources()
        {
            Pipeline.Dispose();
            CommandList.Dispose();
            VertexBuffer.Dispose();
            IndexBuffer.Dispose();
            GraphicsDevice.Dispose();
        }
    }
}
