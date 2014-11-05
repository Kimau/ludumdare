using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
namespace LD17
{
    /// <summary>
    /// Theta = Elavation [0:PI] = 0 is up (ratation is down)
    /// Phi = Angle [0:2PI] = 0 is right (rotation counter clockwise)
    /// </summary>
    class SphereHelper
    {
        /// <summary>
        /// Gets a cartisian position of a spherical coord
        /// </summary>
        /// <param name="theta">Elavation</param>
        /// <param name="phi">Angle of Rotation</param>
        /// <param name="rad">Radius of Sphere</param>
        public static Vector3 BuildVector(double theta, double phi, double rad)
        {
            Debug.Assert(rad > 0, "Radius must be positive");
            Debug.Assert((theta >= 0) && (theta <= Math.PI), "Theta must be [0:PI]");
            Debug.Assert((phi >= 0) && (phi <= MathHelper.TwoPi), "Phi must be [0:2PI]");
            
            Vector3 pos = new Vector3(
                (float)(rad * Math.Sin(theta) * Math.Cos(phi)),
                (float)(rad * Math.Cos(theta)),
                (float)(rad * Math.Sin(theta) * Math.Sin(phi)));

            return pos;
        }

        public static void DeconVector(Vector3 input, out double theta, out double phi, out double rad)
        {
            theta = 0;
            phi = 0;
            rad = input.Length();

            if (rad > 0)
            {
                phi = Math.Atan2(input.Z, input.X);

                if (input.Y > 0)
                {
                    theta = Math.Acos(input.Y / rad);
                }
            }
        }
    }
}
