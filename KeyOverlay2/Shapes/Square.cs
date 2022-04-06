using System.Numerics;
using KeyOverlay2.Helpers;
using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal class Square : VeldridViewportItem
    {
        public ushort[] Indices => new ushort[] { 0, 1, 2, 3 };
        public VertexPositionColor[] Vertices { get; private set; } = new VertexPositionColor[4];
        public uint VerticesLength => (uint)Vertices.Length;

        private DeviceBuffer VertexBuffer;
        private DeviceBuffer IndexBuffer;

        public Square(BaseWindow Window, Helpers.Point Position, Size Size, RgbaByte Color) : base(Window)
        {
            var sizeDelta = new Vector2(VeldridHelpers.PercentToVeldrid(Size.X), VeldridHelpers.PercentToVeldrid(Size.Y));

            // Bottom Left
            Vector2 C = Position.Vector2();
            // Top Left
            Vector2 A = Vector2.Add(C, new Vector2(0, sizeDelta.Y));
            // Top Right
            Vector2 B = Vector2.Add(C, sizeDelta);
            // Bottom Right
            Vector2 D = Vector2.Add(C, new Vector2(sizeDelta.X, 0));

            Vertices[0] = new VertexPositionColor(A, Color);
            Vertices[1] = new VertexPositionColor(B, Color);
            Vertices[2] = new VertexPositionColor(C, Color);
            Vertices[3] = new VertexPositionColor(D, Color);

            VertexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesLength * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
            IndexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesLength * sizeof(ushort), BufferUsage.IndexBuffer));

            Window.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
            Window.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indices);

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

        internal override void Draw()
        {
            Window.CommandList.SetVertexBuffer(0, VertexBuffer);
            Window.CommandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            Window.CommandList.SetPipeline(Pipeline);
            Window.CommandList.DrawIndexed(indexCount: 4, instanceCount: 1, indexStart: 0, vertexOffset: 0, instanceStart: 0);
        }
    }
}
