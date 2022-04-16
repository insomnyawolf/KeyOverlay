using System.Numerics;
using ImGuiNET;
using InEngine;
using InEngine.Shapes;
using LowLevelInputHooks;
using LowLevelInputHooks.OsSpecific.Windows;
using Veldrid;
using Veldrid.SPIRV;

namespace InEngineTestProject
{
    internal class MainWindow : BaseWindow
    {
        protected readonly ImGuiRenderer ImguiRenderer;

        public MainWindow(InEngineConfig Config, InputHook InputHook) : base(Config)
        {
            ImguiRenderer = new ImGuiRenderer(GraphicsDevice, GraphicsDevice.MainSwapchain.Framebuffer.OutputDescription,
            (int)GraphicsDevice.MainSwapchain.Framebuffer.Width, (int)GraphicsDevice.MainSwapchain.Framebuffer.Height);

            //lowLevelInput.OnKeyEvent += HandleInput;

            CreateResources();
        }

        protected Rect RectA { get; set; }
        protected Rect RectB { get; set; }
        protected Rect CentA { get; set; }

        private void CreateResources()
        {
            ShaderDescription vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, InEngine.Helpers.Shaders.VertexCode, "main");
            ShaderDescription fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, InEngine.Helpers.Shaders.FragmentCode, "main");

            Shaders = ResourceFactory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            RectA = new Rect(this, new Vector2 { X = 0f, Y = 0f }, new Vector2 { X = 0.01f, Y = 0.01f }, RgbaByte.Orange);
            RectB = new Rect(this, new Vector2 { X = -0.1f, Y = -0.1f }, new Vector2 { X = 0.2f, Y = 0.2f }, RgbaByte.Blue);
        } 

        protected override void Update(InputSnapshot input, float deltaTime)
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

            RectB.Draw();
            RectA.Draw();

            //CentA.Draw();
        }
    }
}
