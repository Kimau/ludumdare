using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD17
{
    class StickyBall
    {
        Vector3 m_Force;
        Vector3 m_Position;        
        float   m_Size;
        public List<StickyBall> m_Friends = new List<StickyBall>();

        #region Properties
        public float Radius { get { return m_Size; } set { m_Size = value; } }
        public Vector3 Position { get { return m_Position; } set { m_Position = value; } }
        public Vector3 Force { get { return m_Force; } set { m_Force = value; } }

        public Matrix Transform
        {
            get 
            {
                Matrix trans = Matrix.CreateScale(m_Size * 0.2f);
                trans.Translation = m_Position;
                return trans;
            }
        }
        #endregion

        public StickyBall(Vector3 pos)
        {
            m_Position = pos;
            m_Size = 5.0f;
            m_Force = Vector3.Zero;
        }

        public void Update(GameTime time)
        {
            m_Position += m_Force * (float)time.ElapsedGameTime.TotalSeconds;
            m_Force = Vector3.Zero;
        }

        public void ApplyForce(Vector3 force)
        {
            m_Force += force;
        }

        public bool IsInterecting(StickyBall other, out Vector3 force)
        {
            force = Vector3.Zero;
            if (other == this)
                return false;

            Vector3 relDir = (Position - other.Position);
            float distSqr = (Position - other.Position).LengthSquared();

            // Avoid Stuck
            if (distSqr < 0.01f)
            {
                Position += Vector3.UnitX;
                relDir = (Position - other.Position);
                distSqr = (Position - other.Position).LengthSquared();
            }

            if (distSqr < (Radius * Radius + other.Radius * other.Radius))
            {
                m_Friends.Add(other);
                relDir.Normalize();
                force = relDir * 25.0f;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
