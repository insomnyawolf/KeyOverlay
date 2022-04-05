using System.Diagnostics;
using System.Numerics;
using System.Text;
using ImGuiNET;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.SPIRV;
using Veldrid.StartupUtilities;

namespace KeyOverlay2
{
    internal class MainWindow
    {
        private readonly Stopwatch StopWatch = new Stopwatch();

        private readonly Sdl2Window Window;
        private readonly GraphicsDevice GraphicsDevice;

        // Using a single command list to avoid weird render behaviour in screenshoots
        private CommandList CommandList;

        private DeviceBuffer VertexBuffer;
        private DeviceBuffer IndexBuffer;
        private Shader[] Shaders;
        private Pipeline Pipeline;

        private readonly ImGuiRenderer ImguiRenderer;

        private readonly AppConfig AppConfig;

        public MainWindow(AppConfig Config)
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

            ImguiRenderer = new ImGuiRenderer(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            (int)GraphicsDevice.MainSwapchain.Framebuffer.Width, (int)GraphicsDevice.MainSwapchain.Framebuffer.Height);

            CreateResources();
        }

        private void CreateResources()
        {
            ResourceFactory factory = GraphicsDevice.ResourceFactory;

            var red = RgbaByte.Red;
            var fadingRed = new RgbaByte(0, 0, 255, 0);

            VertexPositionColor[] quadVertices =
            {
                new VertexPositionColor(new Vector2(-0.75f, 0.75f), fadingRed),
                new VertexPositionColor(new Vector2(0.75f, 0.75f), fadingRed),
                new VertexPositionColor(new Vector2(-0.75f, -0.75f), red),
                new VertexPositionColor(new Vector2(0.75f, -0.75f), red)
            };

            ushort[] quadIndices = { 0, 1, 2, 3 };

            VertexBuffer = factory.CreateBuffer(new BufferDescription(4 * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            IndexBuffer = factory.CreateBuffer(new BufferDescription(4 * sizeof(ushort), BufferUsage.IndexBuffer));

            GraphicsDevice.UpdateBuffer(VertexBuffer, 0, quadVertices);
            GraphicsDevice.UpdateBuffer(IndexBuffer, 0, quadIndices);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4_Norm));

            ShaderDescription vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, KeyOverlay2.Shaders.VertexCode, "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, KeyOverlay2.Shaders.FragmentCode, "main");

            Shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleAdditiveBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(depthTestEnabled: true, depthWriteEnabled: true, comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(vertexLayouts: new VertexLayoutDescription[] { vertexLayout }, shaders: Shaders);

            pipelineDescription.Outputs = GraphicsDevice.SwapchainFramebuffer.OutputDescription;

            Pipeline = factory.CreateGraphicsPipeline(pipelineDescription);

            CommandList = factory.CreateCommandList();
        }

        private void DrawInternals(InputSnapshot input, float frameTime)
        {
            CommandList.Begin();
            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, RgbaFloat.Black);

            Draw(input, frameTime);

            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);
            GraphicsDevice.SwapBuffers(GraphicsDevice.MainSwapchain);
        }

        private void Draw(InputSnapshot input, float frameTime)
        {
            Settings(input, frameTime);
            Content(input, frameTime);
        }

        private void Settings(InputSnapshot input, float frameTime)
        {
            ImguiRenderer.Update(frameTime, input);

            // Draw stuff
            ImGui.Text("Hello World");
            ImGui.Text($"Backend => {GraphicsDevice.BackendType}");
            ImGui.Text($"Frametime => {frameTime}");

            ImguiRenderer.Render(GraphicsDevice, CommandList);
        }

        private void Content(InputSnapshot input, float frameTime)
        {
            CommandList.SetVertexBuffer(0, VertexBuffer);
            CommandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            CommandList.SetPipeline(Pipeline);
            CommandList.DrawIndexed(
                indexCount: 4,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);
        }

        public void Start()
        {
            while (Window.Exists)
            {
                var frameTime = Frametime();
                var input = Window.PumpEvents();

                DrawInternals(input, frameTime);                
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

    struct VertexPositionColor
    {
        // This is the position, in normalized device coordinates.
        // Tldr where will be the things be relative to the window
        public Vector2 Position;

        // This is the color of the vertex.
        public RgbaByte Color; 
        public VertexPositionColor(Vector2 position, RgbaByte color)
        {
            Position = position;
            Color = color;
        }

        //Don't ask me how this is calculated, i'm confused as well
        public const uint SizeInBytes = 12;
    }

#warning check if readonly struct fits better
    public static class Shaders
    {
        private static byte[] GetBytes(string input) => Encoding.UTF8.GetBytes(input);

        private const string VertexCodeString = @"
            #version 450

            layout(location = 0) in vec2 Position;
            layout(location = 1) in vec4 Color;

            layout(location = 0) out vec4 fsin_Color;

            void main()
            {
                gl_Position = vec4(Position, 0, 1);
                fsin_Color = Color;
            }";

        public static byte[] VertexCode => GetBytes(VertexCodeString);

        private const string FragmentCodeString = @"
            #version 450

            layout(location = 0) in vec4 fsin_Color;
            layout(location = 0) out vec4 fsout_Color;

            void main()
            {
                fsout_Color = fsin_Color;
            }";
        public static byte[] FragmentCode => GetBytes(FragmentCodeString);
    }
}
