using System.Numerics;
using Veldrid;

namespace KeyOverlay2.Helpers
{
    internal static class InternalHelpers
    {
        internal const float OnePercent = 2f / 100;
        internal static float PercentToVeldrid(float value)
        {
            return (float)(value * OnePercent);
        }
    }

    public static class PublicHelpers
    {
        public static Vector2 GetPointAtDefinedPercentages(float X, float Y)
        {
            return new Vector2(InternalHelpers.PercentToVeldrid(X) - 1, InternalHelpers.PercentToVeldrid(Y) - 1);
        }
    }


    struct VertexPositionColor
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
