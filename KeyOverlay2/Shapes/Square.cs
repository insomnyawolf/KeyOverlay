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
            //var sizeDelta = new Vector2(VeldridHelpers.PercentToVeldrid(Size.X), VeldridHelpers.PercentToVeldrid(Size.Y));
            // Bottom Left
            //Vector2 C = Position.Vector2();

            this.HalfSize = Size / 2;
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

        internal override void CalculateVertexBuffer()
        {
            Window.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
        }

        public override Vector2 GetCenter()
        {
            // Move so it's relative to the item
            var movedToItem = Vector2.Add(Vertices[2].Position, HalfSize);
            return movedToItem;
        }

        internal override void Draw()
        {
            base.Draw();
            Window.CommandList.DrawIndexed(indexCount: 4, instanceCount: 1, indexStart: 0, vertexOffset: 0, instanceStart: 0);
        }
    }
}
