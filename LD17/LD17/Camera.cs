using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LD17
{
    public abstract class Camera
    {
        #region Fields
        protected Matrix m_viewMatrix;
        #endregion

        #region Properties
        public Matrix ViewMatrix { get { return m_viewMatrix; } }
        #endregion

        public Camera()
        {
        }

        public abstract void UpdateViewMatrix();
    }

    /// <summary>
    /// Targeted Camera
    /// </summary>
    public class TargetCam : Camera
    {
       #region Fields
        Vector3 m_camPos = new Vector3(50, 50, 50);
        Vector3 m_tarPos = new Vector3(0, 0, 0);
        Vector3 m_upVec = new Vector3(0, 1, 0);
        #endregion

        #region Properties
        public Vector3 Position { get { return m_camPos; } set { m_camPos = value; UpdateViewMatrix(); } }
        public Vector3 Target { get { return m_tarPos; } set { m_tarPos = value; UpdateViewMatrix(); } }
        public Vector3 Up { get { return m_upVec; } set { m_upVec = value; UpdateViewMatrix(); } }
        public float Distance
        {
            get
            {
                Vector3 rel = m_camPos - m_tarPos;
                return rel.Length();
            }

            set
            {
                Vector3 rel = m_camPos - m_tarPos;
                rel.Normalize();
                Position = m_tarPos + (rel * value);
            }
        }
        #endregion

        public TargetCam() : base()
        {
            UpdateViewMatrix();
        }

        public override void UpdateViewMatrix()
        {
            m_viewMatrix = Matrix.CreateLookAt(m_camPos, m_tarPos, m_upVec);
        }
    }

    /// <summary>
    /// Orbiting Camera
    /// </summary>
    public class OrbitCam : Camera
    {
        #region Fields
        double m_radius = 1.0;
        double m_theata = 1.0;
        double m_phi = 1.0;
        Vector3 m_targetPos = Vector3.Zero;
        #endregion

        #region Properties
        public Vector3 Target
        {
            get { return m_targetPos; }
            set { m_targetPos = value; }
        }
        public double Distance
        {
            get { return m_radius; }
            set 
            {
                Debug.Assert(value > 0, "Radius must be positive");

                m_radius = value; 
                UpdateViewMatrix(); 
            }
        }

        /// <summary>
        /// The angle from the Zenith [0:PI]
        /// </summary>
        public double Theta
        {
            get { return m_theata; }
            set 
            {
                Debug.Assert((value >= 0) && (value <= Math.PI), "Theta must be [0:PI]");
                m_theata = value; 
                UpdateViewMatrix(); 
            }
        }

        /// <summary>
        /// The angle on the clock [0:2PI]
        /// Counter Clockwise rotation
        /// </summary>
        public double Phi
        {
            get { return m_phi; }
            set 
            {
                Debug.Assert((value >= 0) && (value <= MathHelper.TwoPi), "Phi must be [0:2PI]"); 
                m_phi = value; 
                UpdateViewMatrix();
            }
        }

        #endregion

        public OrbitCam() : base()
        {
            UpdateViewMatrix();
        }

        public override void UpdateViewMatrix()
        {
            Vector3 rel = SphereHelper.BuildVector(m_theata, m_phi, m_radius);
            Vector3 camPos = m_targetPos + rel;

            // Get Up Vector
            Vector3 up;

            if (m_theata < 0.1)
            {
                double alteredPhi = m_phi + MathHelper.Pi;
                if (alteredPhi > MathHelper.TwoPi)
                {
                    alteredPhi -= MathHelper.TwoPi;
                }

                up = SphereHelper.BuildVector(m_theata, alteredPhi, m_radius);
            }
            else
            {
                double alteredTheate = m_theata - 0.1f;
                up = SphereHelper.BuildVector(alteredTheate, m_phi, m_radius);
            }

            up -= rel;
            up.Normalize();

            m_viewMatrix = Matrix.CreateLookAt(camPos, m_targetPos, up);
        }
    }
}
