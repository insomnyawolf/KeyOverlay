using System.Diagnostics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace InEngine
{
    public abstract class BaseWindow
    {
        private readonly Stopwatch StopWatch = new();
        protected readonly Sdl2Window Window;
        protected internal readonly GraphicsDevice GraphicsDevice;
        protected internal readonly ResourceFactory ResourceFactory;
        // Using a single command list to avoid weird render behaviour in screenshoots
        protected internal readonly CommandList CommandList;
        protected internal Shader[] Shaders;

        internal readonly InEngineConfig InEngineConfig;

        public BaseWindow(InEngineConfig Config)
        {
            InEngineConfig = Config;

            var WindowConfig = InEngineConfig.WindowConfig;

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
            catch (VeldridException ex)
            {
#warning To-Do: only catch needed errors
                // Vulkan explodes while closing it for some reason
                Console.WriteLine(ex.ToString());
            }
        }

        protected abstract void Update(InputSnapshot input, float deltaTime);

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

        // Compute actual value for deltaSeconds so i can normalize the speed of things...

        private float Frametime()
        {
            var frametime = (float)StopWatch.ElapsedTicks / Stopwatch.Frequency;
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
