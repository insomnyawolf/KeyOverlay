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
        internal readonly ResourceFactory ResourceFactory;

        // Using a single command list to avoid weird render behaviour in screenshoots
        internal readonly CommandList CommandList;

        internal Shader[] Shaders;

        internal readonly AppConfig AppConfig;

        public BaseWindow(AppConfig Config)
        {
            AppConfig = Config;

            var WindowConfig = AppConfig.WindowConfig;

            var windowCI = new WindowCreateInfo()
            {
                X = WindowConfig.XPosition,
                Y = WindowConfig.YPosition,
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

            ResourceFactory = GraphicsDevice.ResourceFactory;

            CommandList = ResourceFactory.CreateCommandList();
        }

        private void DrawInternals(InputSnapshot input, float deltaTime)
        {
            CommandList.Begin();
            
            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            Update(input, deltaTime);

            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            try
            {
                GraphicsDevice.SwapBuffers(/*GraphicsDevice.MainSwapchain*/);
            }
            catch(VeldridException ex)
            {

            }
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
            CommandList.Dispose();
            GraphicsDevice.Dispose();
        }
    }
}
