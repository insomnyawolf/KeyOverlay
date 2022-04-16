using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace InEngine.Helpers
{
    public static class GeometryHelpers
    {
        public const float RadianConversionFactor = MathF.PI / 180;

        public static float DegreeToRadian(float value)
        {
            return value * RadianConversionFactor;
        }

        public static float RadianToDegree(float value)
        {
            return value / RadianConversionFactor;
        }

        public static Vector2 RotatePoint(Vector2 origin, float cosA, float sinA)
        {
            return new Vector2()
            {
                X = origin.X * cosA - origin.Y * sinA,
                Y = origin.X * sinA + origin.Y * cosA,
            };
        }
    }
}
