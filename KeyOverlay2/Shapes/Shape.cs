using System.Numerics;
using KeyOverlay2.Helpers;
using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal abstract class Shape : VeldridViewportItem
    {
        private const float RadianConversionFactor = MathF.PI / 180;


        public float CurrentRotation { get; private set; }
        public Vector2 Center { get; private set; }


        protected readonly ushort[] Indexes;
        protected readonly VertexPositionColor[] Vertices;
        protected readonly Vector2[] UnRotatedVertices;
        protected DeviceBuffer VertexBuffer;
        protected DeviceBuffer IndexBuffer;
        private bool NeedVertexBufferUpdate = true;

        internal uint VerticesCount => (uint)Vertices.Length;

        public Shape(BaseWindow Window, VertexPositionColor[] Vertices, ushort[] Indexes, float Rotation = 0) : base(Window)
        {
            this.Indexes = Indexes;
            this.Vertices = Vertices;
            this.UnRotatedVertices = new Vector2[Vertices.Length];

            for (int i = 0; i < Vertices.Length; i++)
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

            var temp = RadianConversionFactor * CurrentRotation;

            var cosA = MathF.Cos(temp);
            var sinA = MathF.Sin(temp);

            if (Center != Vector2.Zero)
            {
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i].Position = Vector2.Subtract(Vertices[i].Position, Center);
                }
            }

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = RotatePoint(Vertices[i].Position, cosA, sinA);
            }

            if (Center != Vector2.Zero)
            {
                for (int i = 0; i < Vertices.Length; i++)
                {
                    Vertices[i].Position = Vector2.Add(Vertices[i].Position, Center);
                }
            }

#warning todo in the optimal way whith a arbitrary rotation matrix ;-;
            // Cry count to make work on things that arent at 0,0
            // Counter: 10
            

            NeedVertexBufferUpdate = true;
        }

        internal Vector2 RotatePoint(Vector2 origin, float cosA, float sinA)
        {
            return new Vector2()
            {
                X = origin.X * cosA - origin.Y * sinA,
                Y = origin.X * sinA + origin.Y * cosA,
            };
        }

        protected void CalculateVertexBuffer()
        {
            Window.GraphicsDevice.UpdateBuffer(VertexBuffer, 0, Vertices);
        }

        protected abstract Vector2 CalculateCenter();

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < Vertices.Length; i++)
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
