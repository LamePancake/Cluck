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

namespace Cluck
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Cluck : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Model ground;
        private Model fence;
        private Model leftArm;
        private Model rightArm;
        private SpriteFont timerFont;
        private TimeSpan timer;
        private Boolean timeStart;
        private string time;
        private Texture2D armsDiffuse;
        private KeyboardState oldKeyState;
        private KeyboardState curKeyState;

        private const float CAMERA_FOVX = 85.0f;
        private const float CAMERA_ZNEAR = 0.01f;
        private const float CAMERA_ZFAR = 2048.0f * 2.0f;
        private const float CAMERA_PLAYER_EYE_HEIGHT = 110.0f;
        private const float CAMERA_ACCELERATION_X = 900.0f;
        private const float CAMERA_ACCELERATION_Y = 900.0f;
        private const float CAMERA_ACCELERATION_Z = 900.0f;
        private const float CAMERA_VELOCITY_X = 300.0f;
        private const float CAMERA_VELOCITY_Y = 300.0f;
        private const float CAMERA_VELOCITY_Z = 300.0f;
        private const float CAMERA_RUNNING_MULTIPLIER = 2.0f;
        private const float CAMERA_RUNNING_JUMP_MULTIPLIER = 2.0f;

        private FirstPersonCamera camera;
        private PlayerComponent playerComponent;

        private int windowWidth;
        private int windowHeight;

        public Cluck()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            camera = new FirstPersonCamera(this);
            Components.Add(camera);
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

            windowWidth = GraphicsDevice.DisplayMode.Width / 2;
            windowHeight = GraphicsDevice.DisplayMode.Height / 2;

            timer = new TimeSpan(0, 2, 0);
            timeStart = false;

            camera.EyeHeightStanding = CAMERA_PLAYER_EYE_HEIGHT;
            camera.Acceleration = new Vector3(
                CAMERA_ACCELERATION_X,
                CAMERA_ACCELERATION_Y,
                CAMERA_ACCELERATION_Z);
            camera.VelocityWalking = new Vector3(
                CAMERA_VELOCITY_X,
                CAMERA_VELOCITY_Y,
                CAMERA_VELOCITY_Z);
            camera.VelocityRunning = new Vector3(
                camera.VelocityWalking.X * CAMERA_RUNNING_MULTIPLIER,
                camera.VelocityWalking.Y * CAMERA_RUNNING_JUMP_MULTIPLIER,
                camera.VelocityWalking.Z * CAMERA_RUNNING_MULTIPLIER);
            camera.Perspective(
                CAMERA_FOVX,
                (float)windowWidth / (float)windowHeight,
                CAMERA_ZNEAR, CAMERA_ZFAR);

            //Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f,
            //        (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 1f, 10f);
            //effect.Projection = projection;
            //Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);
            //effect.View = V;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            timerFont = Content.Load<SpriteFont>("MessageFont");
            // TODO: use this.Content to load your game content here
            armsDiffuse = Content.Load<Texture2D>(@"Textures\arms_diffuse");

            leftArm = Content.Load<Model>(@"Models\arm_left");
            rightArm = Content.Load<Model>(@"Models\arm_right");

            fence = Content.Load<Model>(@"Models\fence_bounds");
            ground = Content.Load<Model>(@"Models\ground");

            time = timer.ToString();

            playerComponent = new PlayerComponent(camera, rightArm, leftArm, armsDiffuse);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //
        // Adds time on to TimeSpan timer
        //
        private void AddTime(TimeSpan addition)
        {
            timer += addition;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            curKeyState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && oldKeyState != curKeyState)
            {
                if(!timeStart)
                {
                    timeStart = true;
                }
                else
                {
                    timeStart = false;
                }
            }

            bool isCatch = false;
            if ((Keyboard.GetState().IsKeyDown(Keys.F) && oldKeyState != curKeyState) || GamePad.GetState(PlayerIndex.One).Buttons.X == ButtonState.Pressed)
            {
                isCatch = true;
            }
            playerComponent.Update(gameTime, isCatch);

            if (timer > TimeSpan.Zero && timeStart)
            {
                timer -= gameTime.ElapsedGameTime;
            }

            time = timer.ToString();

            KeepCameraInBounds();

            oldKeyState = curKeyState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // TODO: Add your drawing code here
            //leftArm.Draw(GraphicsDevice, effect, "diffuseMapTexture", armsDiffuse);

            Matrix[] groundMatrix = new Matrix[ground.Bones.Count];
            ground.CopyAbsoluteBoneTransformsTo(groundMatrix);
            foreach (ModelMesh mm in ground.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    //be.TextureEnabled = true;
                    be.EnableDefaultLighting();
                    be.World = groundMatrix[mm.ParentBone.Index] * Matrix.CreateTranslation(0, -1, 0);
                    be.View = camera.ViewMatrix;
                    be.Projection = camera.ProjectionMatrix;
                    //be.Texture = armsDiffuse;
                }
                mm.Draw();
            }

            Matrix[] fenceMatrix = new Matrix[fence.Bones.Count];
            fence.CopyAbsoluteBoneTransformsTo(fenceMatrix);
            foreach (ModelMesh mm in fence.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    //be.TextureEnabled = true;
                    be.EnableDefaultLighting();
                    be.World = fenceMatrix[mm.ParentBone.Index];
                    be.View = camera.ViewMatrix;
                    be.Projection = camera.ProjectionMatrix;
                    //be.Texture = armsDiffuse;
                }
                mm.Draw();
            }

            playerComponent.Draw(gameTime);

            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, time, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawTimer()
        {

        }

        private void KeepCameraInBounds()
        {
            Vector3 newPos = camera.Position;

            if (camera.Position.X < -1024.0f)
                newPos.X = -1024.0f;

            if (camera.Position.X > 1024.0f)
                newPos.X = 1024.0f;

            if (camera.Position.Y > 1024.0f)
                newPos.Y = 1024.0f;

            if (camera.Position.Y < -1.0f)
                newPos.Y = -1.0f;

            if (camera.Position.Z < -1024.0f)
                newPos.Z = -1024.0f;

            if (camera.Position.Z > 1024.0f)
                newPos.Z = 1024.0f;

            camera.Position = newPos;
        }
    }
}
