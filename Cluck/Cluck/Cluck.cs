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
using Cluck.AI;

namespace Cluck
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Cluck : Microsoft.Xna.Framework.Game
    {
        public const Int32 INIT_WORLD_SIZE = 1024;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Boolean collided;
        private Model ground;
        private Model fence;
        private Model leftArm;
        private Model rightArm;
        private Model chicken;
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

        private List<GameEntity> world;
        private AISystem aiSystem;
        private RenderSystem renderSystem;
        private PhysicsSystem physicsSystem;

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
            // Create the world
            world = new List<GameEntity>(INIT_WORLD_SIZE);

            aiSystem = new AISystem();
            renderSystem = new RenderSystem(camera);
            physicsSystem = new PhysicsSystem();

            collided = false;

            world = new List<GameEntity>();
            aiSystem = new AISystem();
            renderSystem = new RenderSystem(camera);

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

            chicken = Content.Load<Model>(@"Models\chicken");

            time = timer.ToString();
            

            playerComponent = new PlayerComponent(camera, rightArm, leftArm, armsDiffuse);
            GameEntity fenceEntity = new GameEntity();
            GameEntity groundEntity = new GameEntity();
            GameEntity chickenEntity = new GameEntity();
            GameEntity chickenEntity2 = new GameEntity();

            KinematicComponent chickinematics = new KinematicComponent(0.05f, 1f, (float)Math.PI/4, 0.1f);
            KinematicComponent chickinematics2 = new KinematicComponent(0.05f, 0.5f, (float)Math.PI/4, 0.1f);
            PositionComponent chicken1pos = new PositionComponent(new Vector3(0, 0, 0), (float)Math.PI/2);
            PositionComponent chicken2pos = new PositionComponent(new Vector3(-20, 0, -20), (float)Math.PI);

            SteeringComponent chickenSteering2 = new SteeringComponent(chicken1pos);
            SteeringComponent chickenSteering = new SteeringComponent(chicken2pos);

            chickenEntity.AddComponent(new Renderable(chicken));
            chickenEntity.AddComponent(chickinematics);
            chickenEntity.AddComponent(chickenSteering);
            chickenEntity.AddComponent(chicken1pos);

            chickenEntity2.AddComponent(new Renderable(chicken));
            chickenEntity2.AddComponent(chickinematics2);
            chickenEntity2.AddComponent(chickenSteering2);
            chickenEntity2.AddComponent(chicken2pos);
            chickenEntity2.AddComponent(new CollidableComponent());

            fenceEntity.AddComponent(new Renderable(fence));
            groundEntity.AddComponent(new Renderable(ground));

            world.Add(fenceEntity);
            world.Add(groundEntity);
            world.Add(chickenEntity);
            world.Add(chickenEntity2);
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

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && oldKeyState != curKeyState)
            {
                if (!timeStart)
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

            aiSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
            physicsSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
            oldKeyState = curKeyState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!collided)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }
            else
            {
                GraphicsDevice.Clear(Color.Red);
            }
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // TODO: Add your drawing code here

            renderSystem.Update(world, gameTime);
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
