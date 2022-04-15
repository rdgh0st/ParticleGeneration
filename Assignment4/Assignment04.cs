﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CPI411.SimpleEngine;

namespace Assignment4
{
    public class Assignment04 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont font;
        Model model; // **** FBX file
        Effect effect;
        Texture2D texture;
        Texture2D fire, smoke, water;

        Matrix world = Matrix.Identity;
        Matrix view = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix view_default = Matrix.CreateLookAt(new Vector3(20, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 600f, 0.1f, 100f);
        Matrix lightView = Matrix.CreateLookAt(new Vector3(0, 0, 10), -Vector3.UnitZ, Vector3.UnitY);
        Matrix lightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 1f, 100f);

        Vector3 cameraPosition, cameraTarget, lightPosition;
        float angle, angle2, angleL, angleL2;
        float angle_default, angle2_default, angleL_default, angleL2_default;
        float distance = 20f;
        float distance_default = 20f;
        MouseState preMouse;
        KeyboardState preKey;

        int currentTechnique = 0;
        int particleStyle = 0;
        float particleAge = 10;
        float particleResilience = 0.5f;
        float particleFriction = 0.5f;
        Vector3 windDirection = new Vector3(0.01f, 0, 0);

        bool showParams = true;
        int shape = 0;

        Vector3 particlePosition;
        System.Random random;
        ParticleManager particleManager;

        public Assignment04()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            model = Content.Load<Model>("Plane(1)");
            effect = Content.Load<Effect>("ParticleShader");
            fire = Content.Load<Texture2D>("fire");
            smoke = Content.Load<Texture2D>("smoke");
            water = Content.Load<Texture2D>("water");
            font = Content.Load<SpriteFont>("font"); // create spritefont in mgcb editor
            texture = water;

            random = new System.Random();
            particleManager = new ParticleManager(GraphicsDevice, 100);
            particlePosition = new Vector3(0, 0, 0);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) angleL += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) angleL -= 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) angleL2 += 0.02f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) angleL2 -= 0.02f;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                angle -= (Mouse.GetState().X - preMouse.X) / 100f;
                angle2 += (Mouse.GetState().Y - preMouse.Y) / 100f;
            }
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                distance += (Mouse.GetState().X - preMouse.X) / 100f;
            }
            if (Mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                Vector3 ViewRight = Vector3.Transform(Vector3.UnitX,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                Vector3 ViewUp = Vector3.Transform(Vector3.UnitY,
                    Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
                cameraTarget -= ViewRight * (Mouse.GetState().X - preMouse.X) / 10f;
                cameraTarget += ViewUp * (Mouse.GetState().Y - preMouse.Y) / 10f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                angle = angle_default;
                angle2 = angle2_default;
                angleL = angleL_default;
                angleL2 = angleL2_default;
                distance = distance_default;
                view = view_default;
                cameraTarget = new Vector3(0f, 0f, 0f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D1))
            {
                currentTechnique = 0;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D2))
            {
                currentTechnique = 1;
                texture = smoke;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D3))
            {
                currentTechnique = 1;
                texture = water;
            } else if (Keyboard.GetState().IsKeyDown(Keys.D4))
            {
                currentTechnique = 1;
                texture = fire;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                particleStyle = 0;
            } else if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                particleStyle = 1;
            } else if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                particleStyle = 2;
            } else if (Keyboard.GetState().IsKeyDown(Keys.F4) && !preKey.IsKeyDown(Keys.F4))
            {  
                shape++;
                if (shape > 2)
                {
                    shape = 0;
                }
                particleStyle = 3;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemQuestion) && !preKey.IsKeyDown(Keys.OemQuestion)) // GET RIGHT KEYCODE
            {
                showParams = !showParams;   
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F) && !preKey.IsKeyDown(Keys.F))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    particleFriction -= 0.1f;
                }
                else
                {
                    particleFriction += 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.R) && !preKey.IsKeyDown(Keys.R))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    particleResilience -= 0.1f;
                }
                else
                {
                    particleResilience += 0.1f;
                }
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.A) && !preKey.IsKeyDown(Keys.A))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                {
                    particleAge -= 1;
                }
                else
                {
                    particleAge += 1;
                }
            }

            preKey = Keyboard.GetState();
            preMouse = Mouse.GetState();
            // Update Camera
            cameraPosition = Vector3.Transform(new Vector3(0, 0, distance),
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle));
            view = Matrix.CreateLookAt(cameraPosition, cameraTarget, Vector3.Transform(Vector3.UnitY,
                Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle)));
            // Update Light
            lightPosition = Vector3.Transform(new Vector3(0, 0, 10), Matrix.CreateRotationX(angleL2) * Matrix.CreateRotationY(angleL));
            // Update LightMatrix
            lightView = Matrix.CreateLookAt(
                lightPosition,
                -Vector3.Normalize(lightPosition),
                Vector3.Up);

            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {

                if (particleStyle == 0)
                {
                    Particle particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.Velocity = new Vector3(0, 5, 0);
                    particle.MaxAge = particleAge;
                    particle.Init();
                }
                else if (particleStyle == 1)
                {
                    Particle particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.Velocity = new Vector3(random.Next(-5, 5), 5, random.Next(-5, 5));
                    particle.Acceleration = new Vector3(0, -5, 0); // gravity in y
                    particle.MaxAge = particleAge;
                    particle.Init();
                }
                else if (particleStyle == 2)
                {
                    Particle particle = particleManager.getNext();
                    particle.Position = particlePosition;
                    particle.Bounce = true;
                    particle.Resilience = particleResilience;
                    particle.Friction = particleFriction;
                    particle.WindForce = windDirection;
                    particle.Velocity = new Vector3(random.Next(-2, 2), 5, random.Next(-2, 2));
                    particle.Acceleration = new Vector3(0, -5, 0); // gravity in y
                    particle.MaxAge = particleAge;
                    particle.Init();
                }
                else if (particleStyle == 3)
                {
                    if (shape == 0)
                    {
                        Particle particle = particleManager.getNext();
                        particle.Position = particlePosition;
                        particle.Velocity = new Vector3(0, 5, 0);
                        particle.MaxAge = particleAge;
                        particle.Init();
                    } else if (shape == 1)
                    {
                        for (int i = 0; i < 60; i++)
                        {
                            Particle particle = particleManager.getNext();
                            particle.Position = particlePosition;
                            // particle.Velocity = new Vector3(random.Next(-5, 5), 0, 0);
                            double a = System.Math.PI * (i * 6) / 180.0;
                            particle.Velocity = new Vector3(
                                5.0f * (float)System.Math.Sin(a), 0, 5.0f * (float)System.Math.Cos(a)); // random in first
                            particle.Acceleration = new Vector3(0, 0, 0); // gravity in y
                            particle.MaxAge = particleAge;
                            particle.Init();
                        }
                    }
                    else
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Particle particle = particleManager.getNext();
                            particle.Position = particlePosition;
                            // particle.Velocity = new Vector3(random.Next(-5, 5), 0, 0);
                            double a = System.Math.PI * (i * 6) / 180.0;
                            particle.Velocity = new Vector3(
                                5.0f * (float)System.Math.Sin(a), 0, 5.0f * (float)System.Math.Cos(a)); // random in first
                            particle.Acceleration = new Vector3(0, 0, 0); // gravity in y
                            particle.MaxAge = particleAge;
                            particle.Init();
                        }
                    }
                }

            }
            particleManager.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = new DepthStencilState();

            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            model.Draw(world, view, projection);

            effect.CurrentTechnique = effect.Techniques[currentTechnique];
            effect.CurrentTechnique.Passes[0].Apply();
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(view);
            effect.Parameters["Projection"].SetValue(projection);
            effect.Parameters["Texture"].SetValue(texture);
            effect.Parameters["AmbientColor"].SetValue(new Vector4(0, 1, 0, 1));
            effect.Parameters["AmbientIntensity"].SetValue(1f);
            effect.Parameters["DiffuseColor"].SetValue(new Vector4(0, 1, 0, 1));
            effect.Parameters["DiffuseIntensity"].SetValue(1f);
            effect.Parameters["CameraPosition"].SetValue(cameraPosition);
            effect.Parameters["LightPosition"].SetValue(lightPosition);
            effect.Parameters["Shininess"].SetValue(20f);
            effect.Parameters["SpecularColor"].SetValue(new Vector4(1, 0, 0, 1));

            Matrix invertCamera = Matrix.CreateRotationX(angle2) * Matrix.CreateRotationY(angle);
            effect.Parameters["InverseCamera"].SetValue(invertCamera);

            particleManager.Draw(GraphicsDevice);
            
            spriteBatch.Begin();
            if (showParams)
            {
                spriteBatch.DrawString(font, "Angle the camera: Drag Left Mouse", Vector2.UnitX + Vector2.UnitY * 12, Color.White);
                spriteBatch.DrawString(font, "Zoom the camera: Drag Right Mouse", Vector2.UnitX + Vector2.UnitY * 30, Color.White);
                spriteBatch.DrawString(font, "Pan the camera: Drag Middle Mouse", Vector2.UnitX + Vector2.UnitY * 48, Color.White);
                spriteBatch.DrawString(font, "Rotate Light: Arrow Keys", Vector2.UnitX + Vector2.UnitY * 66, Color.White);
                spriteBatch.DrawString(font, "Reset: S", Vector2.UnitX + Vector2.UnitY * 84, Color.White);
                spriteBatch.DrawString(font, "Emit Particles: P", Vector2.UnitX + Vector2.UnitY * 102, Color.White);
                spriteBatch.DrawString(font, "Change Particle: 1/2/3/4", Vector2.UnitX + Vector2.UnitY * 120, Color.White);
                spriteBatch.DrawString(font, "Change Emission: F1/F2/F3, F4 to toggle shape", Vector2.UnitX + Vector2.UnitY * 138, Color.White);
                spriteBatch.DrawString(font, "Friction: F / Shift and F", Vector2.UnitX + Vector2.UnitY * 156, Color.White);
                spriteBatch.DrawString(font, "Resilience: R / Shift and R", Vector2.UnitX + Vector2.UnitY * 174, Color.White);
                spriteBatch.DrawString(font, "Age: A / Shift and A", Vector2.UnitX + Vector2.UnitY * 192, Color.White);
                spriteBatch.DrawString(font, "Help: ?", Vector2.UnitX + Vector2.UnitY * 210, Color.White);
                
                switch (particleStyle)
                {
                    case 0:
                        spriteBatch.DrawString(font, "Particle Style: Fountain no Gravity", Vector2.UnitX + Vector2.UnitY * 228, Color.White);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "Particle Style: Fountain with Gravity", Vector2.UnitX + Vector2.UnitY * 228, Color.White);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "Particle Style: Fountain with Gravity/Bounce/Wind", Vector2.UnitX + Vector2.UnitY * 228, Color.White);
                        break;
                    case 3:
                        spriteBatch.DrawString(font, "Particle Style: Circle/Curve/Square Fountain", Vector2.UnitX + Vector2.UnitY * 228, Color.White);
                        break;
                }

                switch (currentTechnique)
                {
                    case 0:
                        spriteBatch.DrawString(font, "Particle Texture: None", Vector2.UnitX + Vector2.UnitY * 246, Color.White);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "Particle Texture: Smoke", Vector2.UnitX + Vector2.UnitY * 246, Color.White);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "Particle Texture: Water", Vector2.UnitX + Vector2.UnitY * 246, Color.White);
                        break;
                    case 3:
                        spriteBatch.DrawString(font, "Particle Texture: Fire", Vector2.UnitX + Vector2.UnitY * 246, Color.White);
                        break;
                }
                spriteBatch.DrawString(font, "Particle Age: " + particleAge, Vector2.UnitX + Vector2.UnitY * 264, Color.White);
                spriteBatch.DrawString(font, "Particle Resilience: " + particleResilience, Vector2.UnitX + Vector2.UnitY * 282, Color.White);
                spriteBatch.DrawString(font, "Particle Friction: " + particleFriction, Vector2.UnitX + Vector2.UnitY * 300, Color.White);
                spriteBatch.DrawString(font, "Wind Velocity: " + windDirection, Vector2.UnitX + Vector2.UnitY * 318, Color.White);
                switch (shape)
                {
                    case 0:
                        spriteBatch.DrawString(font, "Shape: Square", Vector2.UnitX + Vector2.UnitY * 336, Color.White);
                        break;
                    case 1:
                        spriteBatch.DrawString(font, "Shape: Circle", Vector2.UnitX + Vector2.UnitY * 336, Color.White);
                        break;
                    case 2:
                        spriteBatch.DrawString(font, "Shape: Curve", Vector2.UnitX + Vector2.UnitY * 336, Color.White);
                        break;
                }
                // friction, resilience, age
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
