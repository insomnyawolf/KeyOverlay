using System.Numerics;
using KeyOverlay2.Helpers;

namespace KeyOverlay2.Shapes
{
    internal abstract class Shape : VeldridViewportItem
    {
        private float CurrentRotation { get; set; }
        private const float RadianConversionFactor = MathF.PI / 180;

        internal readonly ushort[] Indexes;
        internal readonly VertexPositionColor[] Vertices;

        internal uint VerticesCount => (uint)Vertices.Length;

        public Shape(BaseWindow Window, VertexPositionColor[] Vertices, ushort[] Indexes, float Rotation = 0) : base(Window)
        {
            this.Indexes = Indexes;
            this.Vertices = Vertices;
            this.CurrentRotation = CurrentRotation;
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
                Vertices[i].Position = RotatePoint(Vertices[i].Position);
            }

            CalculateVertexBuffer();

            // Cry count to make worj on things that arent at 0,0
            // Counter: 1
            Vector2 RotatePoint(Vector2 origin)
            {
                return new Vector2()
                {
                    X = origin.X * cosA - origin.Y * sinA,
                    Y = origin.X * sinA + origin.Y * cosA,
                };
            }
        }

        internal abstract void CalculateVertexBuffer();

        public abstract Vector2 GetCenter();
        public void Translate(Vector2 translation)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Position = Vector2.Add(Vertices[i].Position, translation);
            }

            CalculateVertexBuffer();
        }
    }
}
