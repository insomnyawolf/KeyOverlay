using System.Numerics;
using Veldrid;

namespace InEngine.Helpers
{
    public static class PublicHelpers
    {
        private const float OnePercent = 2f / 100;

        public static float PercentToVeldrid(float value)
        {
            return value * OnePercent;
        }

        public static Vector2 GetPointAtDefinedPercentages(float X, float Y)
        {
            return new Vector2(PercentToVeldrid(X) - 1, PercentToVeldrid(Y) - 1);
        }
    }


    public struct VertexPositionColor
    {
        // This is the position, in normalized device coordinates.
        // Tldr where will be the things be relative to the window
        public Vector2 Position { get; set; }
        // This is the color of the vertex.
        public RgbaByte Color { get; set; }
        public VertexPositionColor(Vector2 position, RgbaByte color)
        {
            Position = position;
            Color = color;
        }

        //Don't ask me how this is calculated, i'm confused as well
        public const uint SizeInBytes = 12;
    }
}
