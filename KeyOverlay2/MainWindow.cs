using System.Numerics;
using System.Text;
using ImGuiNET;
using KeyOverlay2.Shapes;
using LowLevelInputHooks;
using LowLevelInputHooks.DeviceSpecific;
using Veldrid;
using Veldrid.SPIRV;

namespace KeyOverlay2
{
    internal class MainWindow : BaseWindow
    {
        private readonly ImGuiRenderer ImguiRenderer;

        public MainWindow(AppConfig Config, LowLevelInputHook lowLevelInput) : base(Config)
        {
            ImguiRenderer = new ImGuiRenderer(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            (int)GraphicsDevice.MainSwapchain.Framebuffer.Width, (int)GraphicsDevice.MainSwapchain.Framebuffer.Height);

            //lowLevelInput.OnKeyEvent += HandleInput;

            CreateResources();
        }

        public Rect RectA { get; set; }
        public Rect RectB { get; set; }
        public Rect CentA { get; set; }

        private void CreateResources()
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, KeyOverlay2.Shaders.VertexCode, "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, KeyOverlay2.Shaders.FragmentCode, "main");

            Shaders = ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            RectA = new Rect(this, new Vector2 { X = 0f, Y = 0f }, new Vector2 { X = 0.01f, Y = 0.01f }, RgbaByte.Orange);
            RectB = new Rect(this, new Vector2 { X = -0.1f, Y = -0.1f }, new Vector2 { X = 0.2f, Y = 0.2f }, RgbaByte.Blue);
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

        float curentRotation = 0;
        
        private void Content(InputSnapshot test, float deltaTime)
        {
            curentRotation += 100 * deltaTime;
            RectB.Rotate(curentRotation);

            Vector2 movement = Vector2.Zero;
            
            for (int i = 0; i < test.KeyEvents.Count; i++)
            {
                var current = test.KeyEvents[i];

                if (current.Key == Key.Up && current.Down)
                {
                    movement.Y++;
                }

                if (current.Key == Key.Down && current.Down)
                {
                    movement.Y--;
                }

                if (current.Key == Key.Right && current.Down)
                {
                    movement.X++;
                }

                if (current.Key == Key.Left && current.Down)
                {
                    movement.X--;
                }
            }

            RectB.Translate(movement * deltaTime);

            Console.WriteLine(RectB.Center);

            RectB.Draw();
            RectA.Draw();

            //CentA.Draw();
        }
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
