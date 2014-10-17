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

        // Initialize an array of indices for the box. 12 lines require 24 indices
        short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };

        private Model ground;
        private Model fence;
        private Model leftArm;
        private Model rightArm;
        private Model chicken;
        private Model penBase;
        private Model chickenPen;
        private Model testFence;
        private Song testSong;
        private Matrix boundingSphereSize;
        private int boundingSize;
        private SpriteFont timerFont;
        private static TimeSpan timer;
        private Boolean timeStart;
        private string time;
        private Texture2D armsDiffuse;
        private Texture2D chickenDiffuse;
        private KeyboardState oldKeyState;
        private KeyboardState curKeyState;
        private GamePadState oldGPState;
        private GamePadState curGPState;
        private static int winState;
        private BoundingBox testBox;

        private const float CAMERA_FOVX = 85f;
        private const float CAMERA_ZNEAR = 0.01f;
        private const float CAMERA_ZFAR = 2048.0f * 2.0f;
        private const float CAMERA_PLAYER_EYE_HEIGHT = 60f;
        private const float CAMERA_ACCELERATION_X = 900.0f;
        private const float CAMERA_ACCELERATION_Y = 900.0f;
        private const float CAMERA_ACCELERATION_Z = 900.0f;
        private const float CAMERA_VELOCITY_X = 300.0f;
        private const float CAMERA_VELOCITY_Y = 300.0f;
        private const float CAMERA_VELOCITY_Z = 300.0f;
        private const float CAMERA_RUNNING_MULTIPLIER = 2.0f;
        private const float CAMERA_RUNNING_JUMP_MULTIPLIER = 1.5f;

        private FirstPersonCamera camera;

        private int windowWidth;
        private int windowHeight;

        private List<GameEntity> world;
        private AISystem aiSystem;
        private RenderSystem renderSystem;
        private PhysicsSystem physicsSystem;

        Model SkySphere;
        Effect SkySphereEffect;

        public const int TOTAL_NUM_OF_CHICKENS = 13;
        public static int remainingChickens;

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
            //set boundingsphere scale
            boundingSize = 50;
            SetBoundingSphereSize(boundingSize);
            // Create the world
            world = new List<GameEntity>(INIT_WORLD_SIZE);
            winState = 0;

            aiSystem = new AISystem();
            renderSystem = new RenderSystem(camera, graphics.GraphicsDevice);
            physicsSystem = new PhysicsSystem(camera);

            //world = new List<GameEntity>();
            //aiSystem = new AISystem();
            //renderSystem = new RenderSystem(camera, graphics.GraphicsDevice);

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

            remainingChickens = TOTAL_NUM_OF_CHICKENS;
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
            chickenDiffuse = Content.Load<Texture2D>(@"Textures\chicken_diffuse");

            leftArm = Content.Load<Model>(@"Models\arm_left");
            rightArm = Content.Load<Model>(@"Models\arm_right");

            fence = Content.Load<Model>(@"Models\fence_bounds");
            ground = Content.Load<Model>(@"Models\ground");

            chicken = Content.Load<Model>(@"Models\chicken");            

            penBase = Content.Load<Model>(@"Models\pen_base");
            chickenPen = Content.Load<Model>(@"Models\chicken_pen");

            testFence = Content.Load<Model>(@"Models\fence");
            //testSong = Content.Load<Song>(@"Audio\Lacrimosa Dominae");

            time = timer.ToString();

            GameEntity playerEntitiy = new GameEntity();
            playerEntitiy.AddComponent(new CameraComponent(camera));
            playerEntitiy.AddComponent(new PositionComponent(camera.Position, camera.Orientation.W));
            world.Add(playerEntitiy);

            GameEntity fenceEntity = new GameEntity();
            GameEntity groundEntity = new GameEntity();
            
            GameEntity leftArmEntity = new GameEntity();
            GameEntity rightArmEntity = new GameEntity();

            GameEntity penBaseEntity = new GameEntity();
            GameEntity chickenPenEntity = new GameEntity();

            GameEntity testFenceEntity = new GameEntity();

            int i = 0;

            for (i = 0; i < TOTAL_NUM_OF_CHICKENS; ++i)
            {
                // new chicken entity
                GameEntity chickenEntity = new GameEntity();

                // create chicken components
                KinematicComponent chickinematics = new KinematicComponent(0.08f, 2f, (float)Math.PI / 4, 0.1f);

                Vector3 randomPos = new Vector3((float)(Util.RandomClamped() * INIT_WORLD_SIZE), 0, (float)(Util.RandomClamped() * INIT_WORLD_SIZE));

                PositionComponent chickenPos = new PositionComponent(randomPos, (float)(Util.RandomClamped() * Math.PI));
                SteeringComponent chickenSteering = new SteeringComponent(chickenPos);
                chickenSteering.SetScaryEntity(playerEntitiy);
                SensoryMemoryComponent chickenSensory = new SensoryMemoryComponent(chickenPos, chickinematics);
                AIThinking chickenThink = new AIThinking(chickenEntity, Meander.Instance);
                Renderable chickenRenderable = new Renderable(chicken, chickenDiffuse, calBoundingSphere(chicken));

                // add chicken components to chicken
                chickenEntity.AddComponent(chickenRenderable);
                chickenEntity.AddComponent(chickinematics);
                chickenEntity.AddComponent(chickenSteering);
                chickenEntity.AddComponent(chickenPos);
                chickenEntity.AddComponent(chickenSensory);
                chickenEntity.AddComponent(chickenThink);
                chickenEntity.AddComponent(new CollidableComponent());

                world.Add(chickenEntity);
            }

            Vector3 fencePos = new Vector3(-500, 0, -500);
            testFenceEntity.AddComponent(new PositionComponent(fencePos, 0.0f));
            Renderable fenceRenderable = new Renderable(testFence, null, calBoundingBox(testFence, fencePos));
            testFenceEntity.AddComponent(fenceRenderable);
            testFenceEntity.AddComponent(new FenceComponent());

            leftArmEntity.AddComponent(new CollidableComponent());
            leftArmEntity.AddComponent(new Renderable(leftArm, armsDiffuse, calBoundingSphere(leftArm)));
            leftArmEntity.AddComponent(new ArmComponent(false));

            rightArmEntity.AddComponent(new CollidableComponent());
            rightArmEntity.AddComponent(new Renderable(rightArm, armsDiffuse, calBoundingSphere(rightArm)));
            rightArmEntity.AddComponent(new ArmComponent(true));

            fenceEntity.AddComponent(new Renderable(fence, null, fence.Meshes[0].BoundingSphere));
            //fenceEntity.AddComponent(new PositionComponent(new Vector3(0, 30, 0), 0.0f));
            groundEntity.AddComponent(new Renderable(ground, null, ground.Meshes[0].BoundingSphere));
            //groundEntity.AddComponent(new PositionComponent(new Vector3(0, 30, 0), 0.0f));

            penBaseEntity.AddComponent(new Renderable(penBase, null, calBoundingSphere(penBase)));
            penBaseEntity.AddComponent(new CaptureComponent());
            penBaseEntity.AddComponent(new CollidableComponent());
            penBaseEntity.AddComponent(new PositionComponent(new Vector3(500, 0, 500), 0.0f));
            chickenPenEntity.AddComponent(new Renderable(chickenPen, null, chickenPen.Meshes[0].BoundingSphere));
            chickenPenEntity.AddComponent(new PositionComponent(new Vector3(500, 0, 500), 0.0f));
            chickenPenEntity.AddComponent(new CollidableComponent());

            world.Add(fenceEntity);
            world.Add(groundEntity);

            world.Add(leftArmEntity);
            world.Add(rightArmEntity);
            world.Add(penBaseEntity);
            world.Add(chickenPenEntity);

            world.Add(testFenceEntity);
            
            Vector3[] testFenceCorners = testFenceEntity.GetComponent<Renderable>().GetBoundingBox().GetCorners();
            foreach (Vector3 v3 in testFenceCorners)
            {
                Console.Write("Boundingbox Corner: " + v3.X + ", " + v3.Y + ", " + v3.Z + "\n");
            }

            SkySphereEffect = Content.Load<Effect>("SkySphere");
            TextureCube SkyboxTexture =
                Content.Load<TextureCube>(@"Textures\sky");
            SkySphere = Content.Load<Model>(@"Models\SphereHighPoly");

            // Set the parameters of the effect
            SkySphereEffect.Parameters["ViewMatrix"].SetValue(
                camera.ViewMatrix);
            SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(
                camera.ProjectionMatrix);
            SkySphereEffect.Parameters["SkyboxTexture"].SetValue(
                SkyboxTexture);
            // Set the Skysphere Effect to each part of the Skysphere model
            foreach (ModelMesh mesh in SkySphere.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = SkySphereEffect;
                }
            }
            // Plays  Lacrimosa Dominae
            //MediaPlayer.Play(testSong);
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
        public static void AddTime(TimeSpan addition)
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
            timeStart = true;

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            if (timer > TimeSpan.Zero && remainingChickens > 0)
            {
                winState = 0;
                curKeyState = Keyboard.GetState();



                // TODO: Add your update logic here
                //if (Keyboard.GetState().IsKeyDown(Keys.F1) && oldKeyState != curKeyState)
                //{
                //    if (!timeStart)
                //    {
                //        timeStart = true;
                //    }
                //    else
                //    {
                //        timeStart = false;
                //    }
                //}

                if (timer > TimeSpan.Zero && timeStart)
                {
                    timer -= gameTime.ElapsedGameTime;
                }

                //if (Keyboard.GetState().IsKeyDown(Keys.F) && oldKeyState != curKeyState)
                //{
                //    physicsSystem.CatchChicken();
                //}

                time = String.Format("{0,2:D2}", timer.Hours) + ":" + String.Format("{0,2:D2}", timer.Minutes) + ":" + String.Format("{0,2:D2}", timer.Seconds);

                KeepCameraInBounds();

                aiSystem.Update(world, gameTime, camera.Position);
                physicsSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
                oldKeyState = curKeyState;
            }
            else if (timer <= TimeSpan.Zero && remainingChickens > 0)
            {
                winState = -1;
            }
            else
            {
                winState = 1;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            SkySphereEffect.Parameters["ViewMatrix"].SetValue(
                camera.ViewMatrix);
            SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(
                camera.ProjectionMatrix);
            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in SkySphere.Meshes)
            {
                mesh.Draw();
            }

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;           
            // TODO: Add your drawing code here

            renderSystem.Update(world, gameTime);
            drawGUI();
            drawWinState(winState);
            //RenderBox(testBox);
            
            base.Draw(gameTime);
        }

        private void drawWinState(int state)
        {
            if (state == 1)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(timerFont, "You've prevented Cluck's wrath!", new Vector2(windowWidth / 2, windowHeight / 2), Color.White);
                spriteBatch.End();
            }
            else if (state == -1)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(timerFont, "You failed to collect all the chickens before Cluck arrived!", new Vector2(windowWidth / 2, windowHeight / 2), Color.White);
                spriteBatch.End();
            }
        }

        private void drawGUI()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, "Chickens:" + remainingChickens, new Vector2(graphics.GraphicsDevice.Viewport.Width - (int)timerFont.MeasureString("Chickens: " + remainingChickens).X, 0), Color.White);
            spriteBatch.DrawString(timerFont, time, new Vector2(0, 0), Color.White);
            spriteBatch.End();
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

        private BoundingSphere calBoundingSphere(Model mod)
        {
            List<Vector3> points = new List<Vector3>();
            BoundingSphere sphere;

            Matrix[] boneTransforms = new Matrix[mod.Bones.Count];
            mod.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in mod.Meshes)
            {
                Console.WriteLine("mesh count " + mod.Meshes.Count);

                foreach (ModelMeshPart mmp in mesh.MeshParts)
                {
                    Console.WriteLine("meshpart count " + mesh.MeshParts.Count);
                    VertexPositionNormalTexture[] vertices =
                        new VertexPositionNormalTexture[mmp.VertexBuffer.VertexCount];

                    mmp.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                    foreach (VertexPositionNormalTexture vertex in vertices)
                    {
                        Vector3 point = Vector3.Transform(vertex.Position,
                            boneTransforms[mesh.ParentBone.Index]);

                        points.Add(point);
                    }
                }
            }
            Console.WriteLine("point count " + points.Count);
            sphere = BoundingSphere.CreateFromPoints(points);
            sphere = sphere.Transform(Matrix.CreateScale(0.9f));
            //sphere = sphere.Transform(Matrix.CreateTranslation(new Vector3(0,0,-800000)));
            return sphere;
        }

        private BoundingBox calBoundingBox(Model mod, Vector3 worldPos)
        {
            List<Vector3> points = new List<Vector3>();
            BoundingBox box;

            Matrix[] boneTransforms = new Matrix[mod.Bones.Count];
            mod.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in mod.Meshes)
            {
                Console.WriteLine("mesh count " + mod.Meshes.Count);

                foreach (ModelMeshPart mmp in mesh.MeshParts)
                {
                    Console.WriteLine("meshpart count " + mesh.MeshParts.Count);
                    VertexPositionNormalTexture[] vertices =
                        new VertexPositionNormalTexture[mmp.VertexBuffer.VertexCount];

                    mmp.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                    foreach (VertexPositionNormalTexture vertex in vertices)
                    {
                        Vector3 point = Vector3.Transform(vertex.Position,
                            boneTransforms[mesh.ParentBone.Index]);

                        Matrix mat = Matrix.CreateTranslation(worldPos);

                        point = Vector3.Transform(point, mat);

                        points.Add(point);
                    }
                }
            }
            Console.WriteLine("point count " + points.Count);
            box = BoundingBox.CreateFromPoints(points);
            //sphere = sphere.Transform(Matrix.CreateTranslation(new Vector3(0,0,-800000)));
            return box;
        }

        private void SetBoundingSphereSize(int size)
        {
            boundingSphereSize.M11 = size;
            boundingSphereSize.M12 = 0;
            boundingSphereSize.M13 = 0;
            boundingSphereSize.M14 = 0;

            boundingSphereSize.M21 = 0;
            boundingSphereSize.M22 = size;
            boundingSphereSize.M23 = 0;
            boundingSphereSize.M24 = 0;

            boundingSphereSize.M31 = 0;
            boundingSphereSize.M32 = 0;
            boundingSphereSize.M33 = size;
            boundingSphereSize.M34 = 0;

            boundingSphereSize.M41 = 0;
            boundingSphereSize.M42 = 0;
            boundingSphereSize.M43 = 0;
            boundingSphereSize.M44 = 1;

        }

        //protected BoundingSphere CalculateBoundingSphere(Model mod)
        //{
        //    BoundingSphere mergedSphere = new BoundingSphere();
        //    BoundingSphere[] boundingSpheres;
        //    int index = 0;
        //    int meshCount = mod.Meshes.Count;

        //    boundingSpheres = new BoundingSphere[meshCount];
        //    foreach (ModelMesh mesh in mod.Meshes)
        //    {
        //        boundingSpheres[index++] = mesh.BoundingSphere;
        //    }

        //    mergedSphere = boundingSpheres[0];
        //    if ((mod.Meshes.Count) > 1)
        //    {
        //        index = 1;
        //        do
        //        {
        //            mergedSphere = BoundingSphere.CreateMerged(mergedSphere,
        //                boundingSpheres[index]);
        //            index++;
        //        } while (index < mod.Meshes.Count);
        //    }
        //    mergedSphere.Center.Y = 0;
        //    return mergedSphere;
        //}

        private void RenderBox(BoundingBox box)
        {

            Vector3[] corners = box.GetCorners();

            VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            }

            /* Set your own effect parameters here */
            BasicEffect boxEffect = new BasicEffect(graphics.GraphicsDevice);

            boxEffect.World = Matrix.Identity;
            boxEffect.View = camera.ViewMatrix;
            boxEffect.Projection = camera.ProjectionMatrix;
            boxEffect.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (EffectPass pass in boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList, primitiveList, 0, 8,
                    bBoxIndices, 0, 12);
            }
        }
    }
}
