////////////////////////////////////////////////////////////
//
// This software is provided 'as-is', without any express or implied warranty.
// In no event will the authors be held liable for any damages arising from the use of this software.
//
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it freely,
// subject to the following restrictions:
//
// 1. The origin of this software must not be misrepresented;
// you must not claim that you wrote the original software.
// If you use this software in a product, an acknowledgment
// in the product documentation would be appreciated but is not required.
//
// 2. Altered source versions must be plainly marked as such,
// and must not be misrepresented as being the original software.
//
// 3. This notice may not be removed or altered from any source distribution.
//
////////////////////////////////////////////////////////////

////////////////////////////////////////////////////////////
// Headers
////////////////////////////////////////////////////////////
using System;
using SFML.Graphics;
using SFML.System;

namespace sf
{
    ////////////////////////////////////////////////////////////
    public class RoundedRectangleShape : Shape
    {
        public RoundedRectangleShape(Vector2f size, float radius, int cornerPointCount)
        {
            mySize = size;
            myRadius = radius;
            myCornerPointCount = cornerPointCount;
        }

        ////////////////////////////////////////////////////////////
        public void SetSize(Vector2f size)
        {
            mySize = size;
        }

        ////////////////////////////////////////////////////////////
        public Vector2f GetSize()
        {
            return mySize;
        }

        ////////////////////////////////////////////////////////////
        public void SetCornersRadius(float radius)
        {
            myRadius = radius;
        }

        ////////////////////////////////////////////////////////////
        public float GetCornersRadius()
        {
            return myRadius;
        }

        ////////////////////////////////////////////////////////////
        public void SetCornerPointCount(int count)
        {
            myCornerPointCount = count;
        }



        ////////////////////////////////////////////////////////////
        public override Vector2f GetPoint(uint index)
        {
            if (index >= myCornerPointCount * 4)
                return new Vector2f(0, 0);

            float deltaAngle = 90.0f / (myCornerPointCount - 1);
            Vector2f center = new Vector2f();
            int centerIndex = (int)(index / myCornerPointCount);

#warning Chekc if that's worth (maybe it has some impact on performance)
            //const float pi = 3.141592654f;
            const double pi = Math.PI;
            

            switch (centerIndex)
            {
                case 0: center.X = mySize.X - myRadius; center.Y = myRadius; break;
                case 1: center.X = myRadius; center.Y = myRadius; break;
                case 2: center.X = myRadius; center.Y = mySize.Y - myRadius; break;
                case 3: center.X = mySize.X - myRadius; center.Y = mySize.Y - myRadius; break;
            }
            string s = Math.Cos(deltaAngle * (index - centerIndex) * pi / 180).ToString();
            float f1 = Convert.ToSingle(s);
            string s2 = Math.Sin(deltaAngle * (index - centerIndex) * pi / 180).ToString();
            float f2 = Convert.ToSingle(s2);
            return new Vector2f(myRadius * f1 + center.X, -myRadius * f2 + center.Y);
        }

        public override uint GetPointCount()
        {
            return (uint)(myCornerPointCount * 4);
        }

        public float myRadius;
        public int myCornerPointCount;
        public Vector2f mySize;
    }
}
