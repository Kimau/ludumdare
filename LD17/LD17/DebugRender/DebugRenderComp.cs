
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using LD17;
#endregion

namespace DebugRender
{
    /// <summary>
    /// Debug Draw Class
    /// </summary>
    public partial class DebugRenderer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Camera m_cam;
        int m_numWorldLines;
        bool m_bDrawn = false;

        VertexBuffer m_lineVertxBuffer;
        VertexPositionColor[] m_pointDataBuffer;
        VertexDeclaration m_lineVertexDecl;

        Effect m_DebugEffect;
        ContentManager m_Content;

        const int MAX_DEBUG_LINES = 2048;

        /// <summary>
        /// A debug renderer that uses the global graphics service to implement the debug prims
        /// </summary>
        /// <param name="game"></param>
        public DebugRenderer(Game game, Camera cam, ContentManager content)
            : base(game) 
        {
            m_numWorldLines = 0;
            m_cam = cam;

            // Register Component
            Game.Components.Add(this);
            Game.Services.AddService(typeof(DebugRenderer), this);

            // Setup Content Piple
            m_Content = content;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();


            m_lineVertxBuffer = new VertexBuffer(GraphicsDevice,
                                                typeof(VertexPositionColor),
                                                MAX_DEBUG_LINES * 2,
                                                BufferUsage.None);

            m_pointDataBuffer = new VertexPositionColor[MAX_DEBUG_LINES * 2];

            // the vertex format of our debug lines
            m_lineVertexDecl = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            m_bDrawn = false;
        }

        /// <summary>
        /// All drawing for this component occurs here
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            m_bDrawn = true;
            if (m_numWorldLines == 0)
                return;

            // Get Aspect Ratio
            float aspectRatio = GraphicsDevice.Viewport.Width /
                                GraphicsDevice.Viewport.Height;

            // Get Matrix
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1,
                                                                    10000);


            m_DebugEffect.Parameters["Alpha"].SetValue(1.0f);
            m_DebugEffect.Parameters["World"].SetValue(Matrix.Identity);
            m_DebugEffect.Parameters["View"].SetValue(m_cam.ViewMatrix);
            m_DebugEffect.Parameters["Projection"].SetValue(projection);


            // Setup Render Device
            m_lineVertxBuffer.SetData<VertexPositionColor>(m_pointDataBuffer, 0, m_numWorldLines * 2);

            GraphicsDevice.VertexDeclaration = m_lineVertexDecl;
            GraphicsDevice.Vertices[0].SetSource(m_lineVertxBuffer, 0, VertexPositionColor.SizeInBytes);

            m_DebugEffect.Begin();
            foreach (EffectPass pass in m_DebugEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, m_numWorldLines * 2);
                pass.End();
            }
            m_DebugEffect.End();

            m_numWorldLines = 0;
        }

        protected override void LoadContent()
        {
            // load the effect
            m_DebugEffect = m_Content.Load<Effect>("DebugLine");
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            m_Content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// Draw a World Space line
        /// </summary>
        /// <param name="colour">colour to render line in</param>
        /// <param name="a">start position of line</param>
        /// <param name="b">end position of line</param>
        public void WorldLine(Color colour, Vector3 a, Vector3 b)
        {
            if (m_numWorldLines >= MAX_DEBUG_LINES)
                return;

            m_pointDataBuffer[(m_numWorldLines * 2) + 0].Position = a;
            m_pointDataBuffer[(m_numWorldLines * 2) + 1].Position = b;

            m_pointDataBuffer[(m_numWorldLines * 2) + 0].Color = colour;
            m_pointDataBuffer[(m_numWorldLines * 2) + 1].Color = colour;

            m_numWorldLines += 1;
        }

        /// <summary>
        /// World Box
        /// </summary>
        public void WorldBox(Matrix transform )
        {
            Vector3[] points = 
            {
                Vector3.Transform((  Vector3.UnitX + Vector3.UnitY), transform),
                Vector3.Transform((- Vector3.UnitX + Vector3.UnitY), transform),
                Vector3.Transform((  Vector3.UnitZ + Vector3.UnitY), transform),
                Vector3.Transform((- Vector3.UnitZ + Vector3.UnitY), transform),
                Vector3.Transform((  Vector3.UnitX - Vector3.UnitY), transform),
                Vector3.Transform((- Vector3.UnitX - Vector3.UnitY), transform),
                Vector3.Transform((  Vector3.UnitZ - Vector3.UnitY), transform),
                Vector3.Transform((- Vector3.UnitZ - Vector3.UnitY), transform)
            };

            // now draw the lines
            WorldLine(Color.Blue, points[0], points[2]);
            WorldLine(Color.Blue, points[0], points[3]);
            WorldLine(Color.Blue, points[1], points[2]);
            WorldLine(Color.Blue, points[1], points[3]);

            WorldLine(Color.Green, points[4], points[6]);
            WorldLine(Color.Green, points[4], points[7]);
            WorldLine(Color.Green, points[5], points[6]);
            WorldLine(Color.Green, points[5], points[7]);

            WorldLine(Color.Red, points[0], points[4]);
            WorldLine(Color.Red, points[1], points[5]);
            WorldLine(Color.Red, points[2], points[6]);
            WorldLine(Color.Red, points[3], points[7]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height">The relative height to a radius of 1</param>
        public void WorldCylinder(Matrix transform, float height)
        {
	        const int numDivisions = 20;
            Vector3[] points = new Vector3[numDivisions * 2]; 

            // Create Points
            for (int seg = 0; seg < numDivisions; seg++)
			{
                Vector2 flatPos = PolarHelper.GetVector(Math.PI / numDivisions, 1.0f);
                points[seg] = new Vector3(flatPos.X, +(height * 0.5f), flatPos.Y);
                points[seg + numDivisions] = new Vector3(flatPos.X, -(height * 0.5f), flatPos.Y);

                Vector3.Transform(points[seg], transform);
                Vector3.Transform(points[seg + numDivisions], transform);
			}

            // Create Lines
            for (int seg = 0; seg < numDivisions; seg++)
            {
                WorldLine(Color.Blue, points[seg], points[((seg + 1) % numDivisions)]);
                WorldLine(Color.Green, points[seg + numDivisions], points[((seg + 1) % numDivisions) + numDivisions]);

                WorldLine(Color.Red, points[seg], points[seg + numDivisions]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void WorldSphere(Matrix transform, float radius)
        {
            const int numHorLines = 8;
            const int numVerLines = 12;

            Vector3[,] Points = new Vector3[numHorLines, numVerLines];

            // Create Points
            for (int vLine = 0; vLine < numVerLines; vLine++)
            {
                for (int hLine = 0; hLine < numHorLines; hLine++)
                {
                    Points[hLine, vLine] = SphereHelper.BuildVector(hLine * (Math.PI / numHorLines), vLine * (Math.PI * 2.0 / numVerLines), radius);
                }
            }

            // Create Lines
            for (int vLine = 0; vLine < numVerLines; vLine++)
            {
                float colour = ((vLine * 1.1f) / numVerLines);
                Color horColour = new Color(colour, 0, (1.0f - colour));                

                for (int hLine = 0; hLine < numHorLines; hLine++)
                {
                    WorldLine(Color.Green, Points[hLine, vLine], Points[((hLine + 1) % numHorLines), vLine]);
                    WorldLine(horColour, Points[hLine, vLine], Points[hLine, ((vLine + 1) % numVerLines)]);
                }
            }
        }

    }
}


