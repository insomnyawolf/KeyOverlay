using System.Numerics;
using Veldrid;

namespace KeyOverlay2.Helpers
{
    internal static class VeldridHelpers
    {
        internal const float OnePercent = 2f / 100;
        internal static float PercentToVeldrid(byte value)
        {
            return (float)(value * OnePercent);
        }
    }

    internal class Point : Debug
    {
        public byte X { get; set; }
        public byte Y { get; set; }

        internal Vector2 Vector2()
        {
            return new Vector2(VeldridHelpers.PercentToVeldrid(X) - 1, VeldridHelpers.PercentToVeldrid(Y) - 1);
        }
    }

    internal class Size : Debug
    {
        public byte X { get; set; }
        public byte Y { get; set; }
    }
}
