using System.Numerics;
using System.Text;
using ImGuiNET;
using KeyOverlay2.Helpers;
using KeyOverlay2.Shapes;
using LowLevelInputHooks;
using Veldrid;
using Veldrid.SPIRV;
using Point = KeyOverlay2.Helpers.Point;

namespace KeyOverlay2
{
    internal class MainWindow : BaseWindow
    {
        private readonly ImGuiRenderer ImguiRenderer;

        public MainWindow(AppConfig Config, LowLevelInputHook lowLevelInput) : base(Config)
        {
            ImguiRenderer = new ImGuiRenderer(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            (int)GraphicsDevice.MainSwapchain.Framebuffer.Width, (int)GraphicsDevice.MainSwapchain.Framebuffer.Height);

            lowLevelInput.OnKeyEvent += HandleInput;

            CreateResources();
        }

        private List<VeldridViewportItem> veldridViewportItems = new List<VeldridViewportItem>();
        private void CreateResources()
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, KeyOverlay2.Shaders.VertexCode, "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, KeyOverlay2.Shaders.FragmentCode, "main");

            Shaders = ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            veldridViewportItems.Add(new Square(this, new Point { X = 0, Y = 0 }, new Size { X = 95, Y = 95 }, RgbaByte.Orange));

            veldridViewportItems.Add(new Square(this, new Point { X = 25, Y = 25 }, new Size { X = 15, Y = 15 }, RgbaByte.Blue));
        }

        private void HandleInput(InputEvent @event)
        {
            if (@event.InputOrigin == InputOrigin.Keyboard)
            {

            }
            else if (@event.InputOrigin == InputOrigin.Mouse)
            {

            }
        }

        internal override void Update(InputSnapshot input, float deltaTime)
        {
            Content(input, deltaTime);
            ImGuiMenus(input, deltaTime);
        }

        private void ImGuiMenus(InputSnapshot input, float deltaTime)
        {
            ImguiRenderer.Update(deltaTime, input);

            // Draw stuff
            ImGui.Text("Hello World");
            ImGui.Text($"Backend => {GraphicsDevice.BackendType}");
            ImGui.Text($"Frametime => {deltaTime}");

            ImguiRenderer.Render(GraphicsDevice, CommandList);
        }

        private void Content(InputSnapshot _, float deltaTime)
        {
            for (int i = 0; i < veldridViewportItems.Count; i++)
            {
                veldridViewportItems[i].Draw();
            }
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
