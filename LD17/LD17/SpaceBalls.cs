using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using DebugRender;

namespace LD17
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class SpaceBalls : Microsoft.Xna.Framework.Game
    {
        enum eGameState
        {
            GS_Setup,
            GS_Playing,
            GS_Done,
            NOOF_GAME_STATES
        };

        const Int16 BALL_COUNT = 400;
        const float CRUST_RADIUS = 75;

        GraphicsDeviceManager m_GraphicsDev;
        SpriteBatch m_SpriteBatch;
        BasicEffect m_baseFx;
        Model m_BallModel;
        List<StickyBall> m_BallList = new List<StickyBall>(BALL_COUNT);
        OrbitCam m_cam = new OrbitCam();
        Random m_random = new Random();

        KeyboardState[] m_keyStates = new KeyboardState[2];

        DebugRenderer m_DebugRenderComp;

        eGameState m_eGameState;

        public SpaceBalls()
        {
            m_GraphicsDev = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            m_eGameState = eGameState.GS_Setup;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            m_cam.Target = Vector3.Zero;
            m_cam.Distance = CRUST_RADIUS * 2;

            m_DebugRenderComp = new DebugRenderer(this, m_cam, Content);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            m_SpriteBatch = new SpriteBatch(GraphicsDevice);
            m_BallModel = Content.Load<Model>("ball");

            m_baseFx = new BasicEffect(GraphicsDevice, null);
            m_baseFx.DiffuseColor = new Vector3(1.0f, 0.5f, 0.5f);
            m_baseFx.Alpha = 0.8f;
            m_baseFx.EmissiveColor = Vector3.Zero;
            m_baseFx.LightingEnabled = true;
            m_baseFx.PreferPerPixelLighting = true;

            m_baseFx.AmbientLightColor = new Vector3(0.2f);

            m_baseFx.DirectionalLight0.Enabled = true;
            m_baseFx.DirectionalLight0.Direction = Vector3.One;
            m_baseFx.DirectionalLight0.DiffuseColor = Vector3.One;
            m_baseFx.DirectionalLight0.SpecularColor = Vector3.UnitX;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (m_eGameState)
            {
                case eGameState.GS_Setup:
                    UpdateSetup(gameTime);
                    break;
                case eGameState.GS_Playing:
                    UpdatePlay(gameTime);
                    break;
                case eGameState.GS_Done:
                    break;
            }

            UpdateControls(gameTime);

            base.Update(gameTime);
        }

        bool IsKeyDown(Keys testKey) { return m_keyStates[1].IsKeyDown(testKey); }
        bool IsKeyUp(Keys testKey) { return m_keyStates[1].IsKeyUp(testKey); }
        bool IsKeyPressed(Keys testKey)
        {
            return ((m_keyStates[0].IsKeyUp(testKey)) &&
                    (m_keyStates[1].IsKeyDown(testKey)));
        }
        bool IsKeyReleased(int index, Keys testKey)
        {
            return ((m_keyStates[0].IsKeyDown(testKey)) &&
                    (m_keyStates[1].IsKeyUp(testKey)));
        }


        private void UpdatePlay(GameTime gameTime)
        {
            // Update Game
            yScan = (int)(gameTime.TotalGameTime.TotalSeconds * 4.0f) % 40;
        }

        private void UpdateControls(GameTime gameTime)
        {
            // Update Controls
            m_keyStates[0] = m_keyStates[1];
            m_keyStates[1] = Keyboard.GetState(PlayerIndex.One);

            if (IsKeyDown(Keys.W))
            {
                double newTheta = m_cam.Theta - (1 * gameTime.ElapsedGameTime.TotalSeconds);
                m_cam.Theta = Math.Max(0.0001, newTheta);
            }

            if (IsKeyDown(Keys.S))
            {
                double newTheta = m_cam.Theta + (1 * gameTime.ElapsedGameTime.TotalSeconds);
                m_cam.Theta = Math.Min(Math.PI, newTheta);
            }

            if (IsKeyDown(Keys.A))
            {
                double newPhi = m_cam.Phi - (1 * gameTime.ElapsedGameTime.TotalSeconds);
                if (newPhi < 0)
                {
                    newPhi += Math.PI * 2.0;
                }
                m_cam.Phi = newPhi;
            }

            if (IsKeyDown(Keys.D))
            {
                double newPhi = m_cam.Phi + (1 * gameTime.ElapsedGameTime.TotalSeconds);
                if (newPhi > (Math.PI * 2.0))
                {
                    newPhi -= Math.PI * 2.0;
                }
                m_cam.Phi = newPhi;
            }

            if (IsKeyDown(Keys.Q))
            {
                double newDist = m_cam.Distance - (m_cam.Distance * 0.3f * gameTime.ElapsedGameTime.TotalSeconds);
                m_cam.Distance = Math.Max(1, newDist);
            }

            if (IsKeyDown(Keys.E))
            {
                double newDist = m_cam.Distance + (m_cam.Distance * 0.3f * gameTime.ElapsedGameTime.TotalSeconds);
                m_cam.Distance = Math.Min(1000000, newDist);
            }

            if (IsKeyPressed(Keys.Escape))
            {
                this.Exit();
            }

            if (IsKeyDown(Keys.Enter))
            {
                m_eGameState = eGameState.GS_Playing;
            }

            if (IsKeyPressed(Keys.Z))
            {
                UpdateIslandConfig(gameTime, 10);
            }

            if (IsKeyPressed(Keys.Space))
            {
                yScan = (yScan + 1) % 40;
            }
        }

        int attempts = 0;
        Vector3[,,] m_GravityPoints = new Vector3[40,40,40];

        private void UpdateSetup(GameTime gameTime)
        {
            attempts++;

            // Spawn Balls
            while (m_BallList.Count < BALL_COUNT)
            {
                // Get Radius
                double rad = m_random.NextDouble() + 0.6;
                rad = CRUST_RADIUS; // Math.Sin(rad * rad) * CRUST_RADIUS;

                // Get Theta (3 o clock)
                double theta = m_random.NextDouble() * MathHelper.Pi;
                double phi = m_random.NextDouble() * MathHelper.TwoPi;

                m_BallList.Add(new StickyBall(SphereHelper.BuildVector(theta, phi, rad)));
            }

            if (UpdateIslandConfig(gameTime, 1) == false)
            {
                return;
            }

            float minForce = -1;
            float maxForce = -1;

            for (int x = 0; x < 40; x++)
            {
                for (int y = 0; y < 40; y++)
                {
                    for (int z = 0; z < 40; z++)
                    {
                        Vector3 tPoint = new Vector3(
                            x*5.0f - 100.0f,
                            y*5.0f - 100.0f,
                            z*5.0f - 100.0f);

                        m_GravityPoints[x, y, z] = CalcGravity(tPoint);

                        float strength = m_GravityPoints[x, y, z].Length();
                        m_GravityPoints[x, y, z].Normalize();

                        if (maxForce < 0)
                        {
                            minForce = strength;
                            maxForce = strength;
                        }
                        else
                        {
                            if (minForce > strength) { minForce = strength; }
                            if (maxForce < strength) { maxForce = strength; }
                        }
                    }
                }
            }

            // UpdateIslandConfig(gameTime, 10);

            m_eGameState = eGameState.GS_Playing;
        }

        Vector3 CalcGravity(Vector3 tPoint)
        {
            Vector3 totalForce = new Vector3(0);

            foreach (StickyBall ball in m_BallList)
            {
                Vector3 newForce = ball.Position - tPoint;
                float distSqr = newForce.LengthSquared();
                if (distSqr < 200000)
                {
                    newForce.Normalize();
                    newForce = newForce * (2000.0f / distSqr);

                    totalForce += newForce;
                }
            }

            return totalForce;
        }

        private bool UpdateIslandConfig(GameTime gameTime, int numIters)
        {
            for (int iterations = 0; iterations < numIters; iterations++)
            {
                int numAlive = 0;

                foreach (StickyBall ball in m_BallList)
                {
                    // Intersections
                    foreach (StickyBall other in m_BallList)
                    {
                        Vector3 newForce;

                        if (ball.IsInterecting(other, out newForce))
                        {
                            ball.ApplyForce(newForce);
                        }
                    }

                    // Gravity
                    if (ball.m_Friends.Count < 2)
                    {
                        numAlive += 1;

                        foreach (StickyBall other in m_BallList)
                        {
                            if (ball != other)
                            {
                                Vector3 newForce = other.Position - ball.Position;
                                float distSqr = newForce.LengthSquared();
                                if (distSqr < 200000)
                                {
                                    newForce.Normalize();
                                    newForce = newForce * (2000.0f / distSqr);

                                    ball.ApplyForce(newForce);
                                }
                            }
                        }
                    }
                }

                foreach (StickyBall ball in m_BallList)
                {
                    ball.Update(gameTime);
                }

                // Wasiting our time now
                if (numAlive < 8)
                {
                    List<StickyBall> deadList = new List<StickyBall>(8);

                    foreach (StickyBall ball in m_BallList)
                    {
                        if (ball.m_Friends.Count < 2)
                        {
                            deadList.Add(ball);
                        }
                    }

                    foreach (StickyBall zombie in deadList)
                    {
                        m_BallList.Remove(zombie);
                    }

                    return true; 
                }
            }

            return false;
        }

        int yScan = 0;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw Balls in Space
            // Get Aspect Ratio
            float aspectRatio = m_GraphicsDev.PreferredBackBufferWidth /
                                m_GraphicsDev.PreferredBackBufferHeight;

            // Get Matrix
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1,
                                                                    10000);

            m_baseFx.View = m_cam.ViewMatrix;
            m_baseFx.Projection = projection;

            foreach (StickyBall ball in m_BallList)
            {
                m_baseFx.World = ball.Transform;
                Vector3 dir = ball.Position;
                dir.Normalize();
                m_baseFx.DirectionalLight0.Direction = dir;

                foreach (ModelMesh mesh in m_BallModel.Meshes)
                {
                    foreach (ModelMeshPart mp in mesh.MeshParts)
                    {
                        m_baseFx.GraphicsDevice.VertexDeclaration = mp.VertexDeclaration;

                        m_baseFx.Begin();
                        foreach (EffectPass pass in m_baseFx.CurrentTechnique.Passes)
                        {
                            pass.Begin();

                            m_baseFx.GraphicsDevice.Indices = mesh.IndexBuffer;

                            m_baseFx.GraphicsDevice.Vertices[0].SetSource(
                                mesh.VertexBuffer, 
                                mp.StreamOffset, 
                                mp.VertexStride);                            

                            m_baseFx.GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList, 
                                mp.BaseVertex, 0,
                                mp.NumVertices, 
                                mp.StartIndex, 
                                mp.PrimitiveCount);

                            pass.End();
                        }
                        m_baseFx.End();
                    }
                }
            }

            for (int xP = 0; xP < 40; xP++)
            {
                for (int yP = 0; yP < 1; yP++)
                {
                    for (int zP = 0; zP < 40; zP++)
                    {
                        int alterYPoint = (yP + yScan) % 40;

                        Vector3 gravPoint = new Vector3(
                                xP * 5.0f - 100.0f,
                                alterYPoint * 5.0f - 100.0f,
                                zP * 5.0f - 100.0f);

                        m_DebugRenderComp.WorldLine(
                            new Color(1,0,0,(yP + 0.5f) / 5.5f), 
                            gravPoint,
                            gravPoint + (m_GravityPoints[xP, alterYPoint, zP] * 5.0f));                       
                        
                    }
                    
                }
                
            }

            base.Draw(gameTime);
        }
    }
}
