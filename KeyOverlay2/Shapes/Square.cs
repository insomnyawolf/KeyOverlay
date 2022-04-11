using System.Numerics;
using KeyOverlay2.Helpers;
using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal class Rect : Shape
    {
        private readonly Vector2 HalfSize;

        public Rect(BaseWindow Window, Vector2 Position, Vector2 Size, RgbaByte Color, float Rotation = 0) :
            base(Window, new VertexPositionColor[4], new ushort[] { 0, 1, 2, 3 }, Rotation)
        {
            this.HalfSize = Size / 2f;

            // Bottom Left
            Vector2 C = Position;
            // Top Left
            Vector2 A = Vector2.Add(C, new Vector2(0, Size.Y));
            // Top Right
            Vector2 B = Vector2.Add(C, new Vector2(Size.X, Size.Y));
            // Bottom Right
            Vector2 D = Vector2.Add(C, new Vector2(Size.X, 0));

            Vertices[0] = new VertexPositionColor(A, Color);
            Vertices[1] = new VertexPositionColor(B, Color);
            Vertices[2] = new VertexPositionColor(C, Color);
            Vertices[3] = new VertexPositionColor(D, Color);

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
                new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2),
                new VertexElementDescription("Color", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Byte4_Norm));

            var pipelineDescription = new GraphicsPipelineDescription();

            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;

            pipelineDescription.DepthStencilState = new DepthStencilStateDescription(depthTestEnabled: true, depthWriteEnabled: true, comparisonKind: ComparisonKind.LessEqual);

            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,
                frontFace: FrontFace.Clockwise,
                depthClipEnabled: true,
                scissorTestEnabled: false);

            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;

            pipelineDescription.ResourceLayouts = Array.Empty<ResourceLayout>();

            pipelineDescription.ShaderSet = new ShaderSetDescription(vertexLayouts: new VertexLayoutDescription[] { vertexLayout }, shaders: Window.Shaders);

            pipelineDescription.Outputs = Window.GraphicsDevice.SwapchainFramebuffer.OutputDescription;

            Pipeline = Window.ResourceFactory.CreateGraphicsPipeline(pipelineDescription);
        }

        // This is horrible and only work to calculate the center while the rotation is 0 degrees, i'm not clever enough to do it properly
        // But since the center is cached, it works well enough
        // Help me plz, i'm suffering
        protected override Vector2 CalculateCenter()
        {
            return Vector2.Add(Vertices[2].Position, HalfSize);
        }

        public override void Draw()
        {
            base.Draw();
            Window.CommandList.DrawIndexed(indexCount: 4, instanceCount: 1, indexStart: 0, vertexOffset: 0, instanceStart: 0);
        }
    }
}
