using System.Numerics;
using InEngine.Helpers;
using Veldrid;

namespace InEngine.Shapes
{
    public abstract class Shape : VeldridViewportItem
    {
        public float CurrentRotation { get; private set; }
        public Vector2 Center { get; private set; }


        protected readonly ushort[] Indexes;
        protected readonly VertexPositionColor[] Vertices;
        protected readonly Vector2[] UnRotatedVertices;
        protected DeviceBuffer VertexBuffer;
        protected DeviceBuffer IndexBuffer;
        private bool NeedVertexBufferUpdate = true;

        protected uint VerticesCount => (uint)Vertices.Length;

        public Shape(BaseWindow Window, VertexPositionColor[] Vertices, ushort[] Indexes, float Rotation = 0) : base(Window)
        {
            this.Indexes = Indexes;
            this.Vertices = Vertices;
            this.UnRotatedVertices = new Vector2[VerticesCount];

            for (int i = 0; i < VerticesCount; i++)
            {
                var current = Vertices[i].Position;
                UnRotatedVertices[i] = new Vector2(current.X, current.Y);
            }

            this.CurrentRotation = CurrentRotation;

            IndexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesCount * sizeof(ushort), BufferUsage.IndexBuffer));
            Window.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indexes);

            VertexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesCount * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));

            Center = CalculateCenter();
        }

        public void Rotate(float newRotation)
        {
            if (newRotation == CurrentRotation)
            {
                return;
            }

            // Calculate rotation difference
            CurrentRotation = newRotation - CurrentRotation;

            RotateInternals();

            CurrentRotation = newRotation;
        }

        private void RotateInternals()
        {
            if (CurrentRotation < 0)
            {
                while (CurrentRotation < -360)
                    CurrentRotation += 360;
            }
            else
            {
                while (CurrentRotation > 360)
                    CurrentRotation -= 360;
            }

            var radianAngle = GeometryHelpers.DegreeToRadian(CurrentRotation);

            var cosA = MathF.Cos(radianAngle);
            var sinA = MathF.Sin(radianAngle);

#warning todo in the optimal way whith a arbitrary rotation matrix ;-; if i get to find it
            // Cry count to make work on things that arent at 0,0
            // Counter: 10

            for (int i = 0; i < VerticesCount; i++)
            {
                Vertices[i].Position -= Center;
                Vertices[i].Position = GeometryHelpers.RotatePoint(Vertices[i].Position, cosA, sinA);
                Vertices[i].Position += Center;
            }

            NeedVertexBufferUpdate = true;
        }

        protected void CalculateVertexBuffer()
        {
            Window.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
        }

        protected abstract Vector2 CalculateCenter();

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < VerticesCount; i++)
            {
                Vertices[i].Position += translation;
                UnRotatedVertices[i] += translation;
            }

            Center += translation;

            NeedVertexBufferUpdate = true;
        }

        public override void Draw()
        {
            if (NeedVertexBufferUpdate)
            {
                CalculateVertexBuffer();
                NeedVertexBufferUpdate = false;
            }

            Window.CommandList.SetVertexBuffer(0, VertexBuffer);
            Window.CommandList.SetIndexBuffer(IndexBuffer, IndexFormat.UInt16);
            Window.CommandList.SetPipeline(Pipeline);
        }

        // Destructor
        bool IsFinalized = false;

        ~Shape()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsFinalized)
            {
                VertexBuffer.Dispose();
                IndexBuffer.Dispose();
                base.Dispose();
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
