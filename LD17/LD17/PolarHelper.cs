using System;
using Microsoft.Xna.Framework;

namespace LD17
{
    /// <summary>
    /// Polar Co-ords Helper
    /// 
    /// Theta 0 = X(0) Y(+1)
    /// </summary>
    class PolarHelper
    {
        static public Vector2 GetVector(double theta, float radius)
        {
            Vector2 position = new Vector2((float)Math.Sin(theta), (float)Math.Cos(theta));
            position *= radius;
            return position;
        }

        static public void DeconVector(Vector2 rel, out double theta, out double radius)
        {
            radius = rel.Length();

            if (radius < 0.0001)
            {
                theta = 0;
                return;
            }

            rel.Normalize();
            theta = Math.Atan2(rel.X, rel.Y);
        }
    }
}
