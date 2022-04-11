using System.Numerics;
using KeyOverlay2.Helpers;
using Veldrid;

namespace KeyOverlay2.Shapes
{
    internal abstract class Shape : VeldridViewportItem
    {
        private float CurrentRotation { get; set; }
        private const float RadianConversionFactor = MathF.PI / 180;

        protected readonly ushort[] Indexes;
        protected readonly VertexPositionColor[] Vertices;
        protected DeviceBuffer VertexBuffer;
        protected DeviceBuffer IndexBuffer;
        private bool NeedVertexBufferUpdate = true;

        internal uint VerticesCount => (uint)Vertices.Length;

        public Shape(BaseWindow Window, VertexPositionColor[] Vertices, ushort[] Indexes, float Rotation = 0) : base(Window)
        {
            this.Indexes = Indexes;
            this.Vertices = Vertices;
            this.CurrentRotation = CurrentRotation;

            IndexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesCount * sizeof(ushort), BufferUsage.IndexBuffer));
            Window.GraphicsDevice.UpdateBuffer(IndexBuffer, 0, Indexes);

            VertexBuffer = Window.ResourceFactory.CreateBuffer(new BufferDescription(VerticesCount * VertexPositionColor.SizeInBytes, BufferUsage.VertexBuffer));
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

        internal void RotateInternals()
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

            var currentCenter = GetCenter();

            var temp = RadianConversionFactor * CurrentRotation;

            var cosA = MathF.Cos(temp);
            var sinA = MathF.Sin(temp);

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = Vector2.Subtract(Vertices[i].Position, currentCenter);
            }

            //var temp2 = GetCenter();

            //for (int i = 0; i < Vertices.Length; i++)
            //{
            //    Vertices[i].Position = RotatePoint(Vertices[i].Position);
            //}

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = Vector2.Add(Vertices[i].Position, currentCenter);
            }

            CalculateVertexBuffer();

            // Cry count to make worj on things that arent at 0,0
            // Counter: 8
            Vector2 RotatePoint(Vector2 origin)
            {
                return new Vector2()
                {
                    X = origin.X * cosA - origin.Y * sinA,
                    Y = origin.X * sinA + origin.Y * cosA,
                };
            }

            NeedVertexBufferUpdate = true;
        }

        internal abstract void CalculateVertexBuffer();

        public abstract Vector2 GetCenter();

        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = Vector2.Add(Vertices[i].Position, translation);
            }

            NeedVertexBufferUpdate = true;
        }

        internal override void Draw()
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
    }
}
