﻿#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
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
using System.IO;
using GameStateManagement;
using SkinnedModel;
#endregion

namespace Cluck
{

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class ArcadeScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        public static int NUM_CHICKEN_SOUNDS = 8;
        public SoundEffect[] CHICKEN_SOUNDS = new SoundEffect[NUM_CHICKEN_SOUNDS];

        public const Int32 INIT_WORLD_SIZE = 1024;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Initialize an array of indices for the box. 12 lines require 24 indices
        short[] bBoxIndices = {
                0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
            };

        private int minutesAllotted = 5;

        private Model ground;
        private Model forest;
        private Model fence;
        private Model leftArm;
        private Model rightArm;
        private Model chicken;
        private Model penBase;
        private Model chickenPen;
        private Model cluck;
        private Song testSong;
        private Song fastSong;
        private Song currentSong;
        private Matrix boundingSphereSize;
        private int boundingSize;
        private SpriteFont timerFont;
        private static TimeSpan timer;
        private Boolean timeStart;
        private string time;
        private Texture2D armsDiffuse;
        private Texture2D chickenDiffuse;
        private Texture2D grassDiffuse;
        private Texture2D woodDiffuse;
        private Texture2D treeDiffuse;
        private Texture2D healthBar;
        private KeyboardState oldKeyState;
        private KeyboardState curKeyState;
        private static int winState;
        private float boundingArmScale;
        private float boundingPenScale;
        private float boundingChickenScale;

        private const float CAMERA_FOVX = 85f;
        private const float CAMERA_ZNEAR = 0.01f;
        private const float CAMERA_ZFAR = 2048.0f * 2.0f;
        private const float CAMERA_PLAYER_EYE_HEIGHT = 100;
        private const float CAMERA_ACCELERATION_X = 900.0f;
        private const float CAMERA_ACCELERATION_Y = 900.0f;
        private const float CAMERA_ACCELERATION_Z = 900.0f;
        private const float CAMERA_VELOCITY_X = 300.0f;
        private const float CAMERA_VELOCITY_Y = 300.0f;
        private const float CAMERA_VELOCITY_Z = 300.0f;
        private const float CAMERA_RUNNING_MULTIPLIER = 2.0f;
        private const float CAMERA_SLIDING_MULTIPLIER = 2.5f;
        private const float CAMERA_CROUCHING_MULTIPLIER = 0.85f;
        private const float CAMERA_RUNNING_JUMP_MULTIPLIER = 1.5f;
        private const int FENCE_LINKS_WIDTH = 1;
        private const int FENCE_LINKS_HEIGHT = 1;
        private const int FENCE_WIDTH = 211 * 11;
        private const int PEN_WIDTH = 0;//118;
        private const int PEN_LINKS_WIDTH = 1;
        private const int PEN_LINKS_HEIGHT = 1;
        private const int PEN_BASE_WIDTH = 110;
        private const int PEN_YCOORD = 500;
        private const int PEN_XCOORD = 500;
        private const int PEN_SPAWNBUFFER = 100;

        private FirstPersonCamera camera;

        private int windowWidth;
        private int windowHeight;

        private List<GameEntity> world;
        private AISystem aiSystem;
        private RenderSystem renderSystem;
        private PhysicsSystem physicsSystem;
        private AudioSystem audioSystem;

        Model SkySphere;
        Effect SkySphereEffect;
        Effect ToonEffect;
        Effect ToonEffectNoAnimation;

        public int total_num_of_chickens;
        public int caughtChickens;
        public static bool chickenCaught = false;

        // The current high score, if there is one, is stored in "highscore".
        // The FileStream will read this and set the current high score.
        private FileStream highScoreFile;
        private Int64 curHighScore;

        // Shown upon achieving a new high score (in case that wasn't obvious).
        private string highScoreMsg = "New high score!";
        private bool showHighScoreMsg = false;

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        private float intensityBlue;

        private const int MAX_BUTTON_SCALE = 6;
        private Texture2D[] qteButtons;
        private Texture2D[] qteKeys;
        private int buttonSize = 75;
        private int buttonScale = MAX_BUTTON_SCALE;
        private Rectangle buttonPos;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public ArcadeScreen()
        {
            Cluck.currentLevel = 10;
            camera = Cluck.camera;
            camera.Reset();
            graphics = Cluck.graphics;

            total_num_of_chickens = 15;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            //set boundingsphere scale
            boundingSize = 50;
            SetBoundingSphereSize(boundingSize);
            // Create the world
            world = new List<GameEntity>(INIT_WORLD_SIZE);
            winState = 0;
            boundingArmScale = 1.2f;
            boundingPenScale = 0.6f;
            boundingChickenScale = 1.0f;

            windowWidth = graphics.GraphicsDevice.DisplayMode.Width / 2;
            windowHeight = graphics.GraphicsDevice.DisplayMode.Height / 2;

            timer = new TimeSpan(0, minutesAllotted, 0);

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
            camera.VelocityRunning = new Vector3(
               camera.VelocityWalking.X * CAMERA_RUNNING_MULTIPLIER,
               camera.VelocityWalking.Y * CAMERA_RUNNING_JUMP_MULTIPLIER,
               camera.VelocityWalking.Z * CAMERA_RUNNING_MULTIPLIER);
            camera.VelocitySliding = new Vector3(
               camera.VelocityWalking.X * CAMERA_SLIDING_MULTIPLIER,
               camera.VelocityWalking.Y,
               camera.VelocityWalking.Z * CAMERA_SLIDING_MULTIPLIER);
            camera.VelocityCrouching = new Vector3(
               camera.VelocityWalking.X * CAMERA_CROUCHING_MULTIPLIER,
               camera.VelocityWalking.Y * CAMERA_CROUCHING_MULTIPLIER,
               camera.VelocityWalking.Z * CAMERA_CROUCHING_MULTIPLIER);
            camera.Perspective(
                CAMERA_FOVX,
                (float)windowWidth / (float)windowHeight,
                CAMERA_ZNEAR, CAMERA_ZFAR);

            caughtChickens = 0;

            renderSystem = new RenderSystem(camera, graphics.GraphicsDevice);
            physicsSystem = new PhysicsSystem(camera);

            qteButtons = new Texture2D[4];
            qteKeys = new Texture2D[4];
            buttonPos = new Rectangle((int)(graphics.GraphicsDevice.Viewport.Width * 0.5) - 37, (int)(graphics.GraphicsDevice.Viewport.Height * 0.2 - 37), buttonSize, buttonSize);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                // Create a new SpriteBatch, which can be used to draw textures.
                spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

                timerFont = content.Load<SpriteFont>("MessageFont");

                // TODO: use this.Content to load your game content here
                ToonEffect = content.Load<Effect>(@"Effects\Toon");
                ToonEffectNoAnimation = content.Load<Effect>(@"Effects\ToonNoAnimation");
                armsDiffuse = content.Load<Texture2D>(@"Textures\arms_diffuse");
                chickenDiffuse = content.Load<Texture2D>(@"Textures\chicken_diffuse");
                grassDiffuse = content.Load<Texture2D>(@"Textures\grassplaceholder");
                woodDiffuse = content.Load<Texture2D>(@"Textures\wood_diffuse");
                treeDiffuse = content.Load<Texture2D>(@"Textures\tree_diffuse");
                healthBar = content.Load<Texture2D>(@"Textures\HealthBar");

                leftArm = content.Load<Model>(@"Models\arm_left");
                rightArm = content.Load<Model>(@"Models\arm_right");

                fence = content.Load<Model>(@"Models\fence_side");
                ground = content.Load<Model>(@"Models\ground_v2");

                chicken = content.Load<Model>(@"Models\chicken_animv2");
                cluck = content.Load<Model>(@"Models\cluck");
                forest = content.Load<Model>(@"Models\tree_side");

                penBase = content.Load<Model>(@"Models\pen_base_large");
                chickenPen = content.Load<Model>(@"Models\chicken_pen_side_large");

                testSong = content.Load<Song>(@"Audio\Yoshi_looped");
                fastSong = content.Load<Song>(@"Audio\Yoshi_looped_fast");
                CHICKEN_SOUNDS[0] = content.Load<SoundEffect>(@"Audio\Cluck1");
                CHICKEN_SOUNDS[1] = content.Load<SoundEffect>(@"Audio\Cluck2");
                CHICKEN_SOUNDS[2] = content.Load<SoundEffect>(@"Audio\Cluck3");
                CHICKEN_SOUNDS[3] = content.Load<SoundEffect>(@"Audio\Cluck4");
                CHICKEN_SOUNDS[4] = content.Load<SoundEffect>(@"Audio\Cluck5");
                CHICKEN_SOUNDS[5] = content.Load<SoundEffect>(@"Audio\Cluck6");
                CHICKEN_SOUNDS[6] = content.Load<SoundEffect>(@"Audio\Cluck7");
                CHICKEN_SOUNDS[7] = content.Load<SoundEffect>(@"Audio\Cluck8");
                SoundEffect.DistanceScale = 1000f;
                SoundEffect.DopplerScale = 0.1f;

                audioSystem = new AudioSystem(CHICKEN_SOUNDS);

                time = timer.ToString();

                GameEntity playerEntitiy = new GameEntity();
                playerEntitiy.AddComponent(new CameraComponent(camera));
                playerEntitiy.AddComponent(new PositionComponent(camera.Position, camera.Orientation.W));
                playerEntitiy.AddComponent(new AudioListenerComponent());
                world.Add(playerEntitiy);

                GameEntity groundEntity = new GameEntity();

                GameEntity leftArmEntity = new GameEntity();
                GameEntity rightArmEntity = new GameEntity();

                GameEntity penBaseEntity = new GameEntity();
                GameEntity chickenPenEntity = new GameEntity();

                BuildBounds(fence, woodDiffuse);

                int i = 0;

                for (i = 0; i < total_num_of_chickens; ++i)
                {
                    // new chicken entity
                    GameEntity chickenEntity = new GameEntity();

                    // create chicken components
                    KinematicComponent chickinematics = new KinematicComponent(0.08f, 3f, (float)Math.PI / 4, 0.1f);

                    Vector3 chickenPosition = GetRandomChickenSpawn();

                    SkinningData skinningData = chicken.Tag as SkinningData;

                    if (skinningData == null)
                        throw new InvalidOperationException
                            ("This model does not contain a SkinningData tag.");

                    AnimationClip clip = skinningData.AnimationClips["Take 001"];

                    PositionComponent chickenPos = new PositionComponent(chickenPosition, (float)(Util.RandomClamped() * Math.PI));
                    SteeringComponent chickenSteering = new SteeringComponent(chickenPos);
                    chickenSteering.SetScaryEntity(playerEntitiy);
                    SensoryMemoryComponent chickenSensory = new SensoryMemoryComponent(chickenPos, chickinematics);
                    AIThinking chickenThink = new AIThinking(chickenEntity, Meander.Instance);
                    Renderable chickenRenderable = new Renderable(chicken, chickenDiffuse, calBoundingSphere(chicken, boundingChickenScale), new AnimationPlayer(skinningData), ToonEffect);

                    chickenRenderable.GetAnimationPlayer().StartClip(clip);

                    // add chicken components to chicken
                    chickenEntity.AddComponent(chickenRenderable);
                    chickenEntity.AddComponent(chickinematics);
                    chickenEntity.AddComponent(chickenSteering);
                    chickenEntity.AddComponent(chickenPos);
                    chickenEntity.AddComponent(chickenSensory);
                    chickenEntity.AddComponent(chickenThink);
                    chickenEntity.AddComponent(new CollidableComponent());
                    chickenEntity.AddComponent(new FreeComponent());
                    chickenEntity.AddComponent(new AudioEmitterComponent(CHICKEN_SOUNDS[0].CreateInstance()));

                    world.Add(chickenEntity);
                }

                leftArmEntity.AddComponent(new CollidableComponent());
                leftArmEntity.AddComponent(new Renderable(leftArm, armsDiffuse, calBoundingSphere(leftArm, boundingArmScale), ToonEffectNoAnimation));
                leftArmEntity.AddComponent(new ArmComponent(false));

                rightArmEntity.AddComponent(new CollidableComponent());
                rightArmEntity.AddComponent(new Renderable(rightArm, armsDiffuse, calBoundingSphere(rightArm, boundingArmScale), ToonEffectNoAnimation));
                rightArmEntity.AddComponent(new ArmComponent(true));

                groundEntity.AddComponent(new Renderable(ground, grassDiffuse, ground.Meshes[0].BoundingSphere, ToonEffectNoAnimation));

                penBaseEntity.AddComponent(new Renderable(penBase, null, calBoundingSphere(penBase, boundingPenScale), ToonEffectNoAnimation));
                penBaseEntity.AddComponent(new CaptureComponent());
                penBaseEntity.AddComponent(new CollidableComponent());
                penBaseEntity.AddComponent(new PositionComponent(new Vector3(500, 0, 500), 0.0f));

                BuildPen(chickenPen, woodDiffuse);

                BuildForest(forest, treeDiffuse);

                world.Add(groundEntity);

                world.Add(leftArmEntity);
                world.Add(rightArmEntity);
                world.Add(penBaseEntity);
                world.Add(chickenPenEntity);

                // now create the AI system.
                aiSystem = new AISystem(world);

                intensityBlue = 1.0f;
                SkySphereEffect = content.Load<Effect>("SkySphere");
                TextureCube SkyboxTexture = content.Load<TextureCube>(@"Textures\sky");
                TextureCube SkyboxTextureRed = content.Load<TextureCube>(@"Textures\skyDark");
                SkySphere = content.Load<Model>(@"Models\SphereHighPoly");

                // Set the parameters of the effect
                SkySphereEffect.Parameters["ViewMatrix"].SetValue(camera.ViewMatrix);
                SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(camera.ProjectionMatrix);
                SkySphereEffect.Parameters["SkyboxTexture"].SetValue(SkyboxTexture);
                SkySphereEffect.Parameters["SkyboxTextureRed"].SetValue(SkyboxTextureRed);
                SkySphereEffect.Parameters["IntensityBlue"].SetValue(intensityBlue);

                // Set the Skysphere Effect to each part of the Skysphere model
                foreach (ModelMesh mesh in SkySphere.Meshes)
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = SkySphereEffect;
                    }
                }

                // Plays  Yoshi's island
                currentSong = testSong;
                MediaPlayer.Play(testSong);
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.45f;

                qteButtons[(int)Cluck.buttons.xq] = content.Load<Texture2D>(@"Textures\x");
                qteButtons[(int)Cluck.buttons.ye] = content.Load<Texture2D>(@"Textures\y");
                qteButtons[(int)Cluck.buttons.br] = content.Load<Texture2D>(@"Textures\b");
                qteButtons[(int)Cluck.buttons.af] = content.Load<Texture2D>(@"Textures\a");

                qteKeys[(int)Cluck.buttons.xq] = content.Load<Texture2D>(@"Textures\q");
                qteKeys[(int)Cluck.buttons.ye] = content.Load<Texture2D>(@"Textures\e");
                qteKeys[(int)Cluck.buttons.br] = content.Load<Texture2D>(@"Textures\r");
                qteKeys[(int)Cluck.buttons.af] = content.Load<Texture2D>(@"Textures\f");

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw

        private Vector3 GetRandomChickenSpawn()
        {
            Vector3 randomPos;

            Boolean inPen = true;

            do
            {
                randomPos = new Vector3((float)(Util.RandomClamped() * INIT_WORLD_SIZE), 0, (float)(Util.RandomClamped() * INIT_WORLD_SIZE));

                if ((randomPos.X > (PEN_BASE_WIDTH + PEN_SPAWNBUFFER + PEN_XCOORD)
                    || (randomPos.X < PEN_XCOORD - PEN_SPAWNBUFFER)) &&
                    (randomPos.Y > (PEN_BASE_WIDTH + PEN_SPAWNBUFFER + PEN_YCOORD)
                    || (randomPos.Y < PEN_YCOORD - PEN_SPAWNBUFFER)))
                {
                    inPen = false;
                }
            }
            while (inPen);

            return randomPos;
        }

        /// <summary>
        /// Gets the current high score.
        /// </summary>
        private void LoadHighScore()
        {
            highScoreFile = new FileStream("highscore", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            FileInfo info = new FileInfo("highscore");

            if (info.Length == 0)
            {
                curHighScore = 0x01111111;
            }
            else
            {

                byte[] rawHighScore = new byte[8];
                highScoreFile.Read(rawHighScore, 0, 8);
                curHighScore = BitConverter.ToInt32(rawHighScore, 0);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void SpawnNewChicken()
        {
            // new chicken entity
            GameEntity chickenEntity = new GameEntity();

            Random rand = new Random();
            Vector3 chickenPosition = new Vector3(500 + rand.Next(-100, 100), 0, 500 + rand.Next(-100, 100));

            SkinningData skinningData = chicken.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            PositionComponent chickenPos = new PositionComponent(chickenPosition, (float)(Util.RandomClamped() * Math.PI));

            Renderable chickenRenderable = new Renderable(chicken, chickenDiffuse, calBoundingSphere(chicken, boundingChickenScale), new AnimationPlayer(skinningData), ToonEffect);

            chickenRenderable.GetAnimationPlayer().StartClip(clip);

            // add chicken components to chicken
            chickenEntity.AddComponent(chickenRenderable);
            chickenEntity.AddComponent(chickenPos);
            chickenEntity.AddComponent(new CollidableComponent());
            chickenEntity.AddComponent(new AudioEmitterComponent(CHICKEN_SOUNDS[0].CreateInstance()));

            world.Add(chickenEntity);
        }

        private void RespawnChicken()
        {
            for (int i = 0; i < world.Count; i++)
            {
                if (world[i].HasComponent((int)component_flags.aiSteering) && !world[i].HasComponent((int)component_flags.free))
                {
                    world[i].GetComponent<PositionComponent>(component_flags.position).SetPosition(GetRandomChickenSpawn());
                    world[i].AddComponent(new FreeComponent());
                    break;
                }
            }
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                if (MediaPlayer.State == MediaState.Paused)
                {
                    MediaPlayer.Resume();
                }

                if (timer.Seconds <= 30 && timer.Minutes < 1 && timer.Hours < 1)
                {
                    intensityBlue -= 0.003f;
                    intensityBlue = MathHelper.Clamp(intensityBlue, 0, 1);
                    if (!MediaPlayer.Equals(currentSong, fastSong))
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(fastSong);
                        currentSong = fastSong;
                    }

                }
                else
                {
                    if (!MediaPlayer.Equals(currentSong, testSong))
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(testSong);
                        currentSong = testSong;
                    }
                }


                timeStart = true;
                curKeyState = Keyboard.GetState();

                if (timer > TimeSpan.Zero && timeStart)
                {
                    timer -= gameTime.ElapsedGameTime;
                }

                time = String.Format("{0,2:D2}", timer.Hours) + ":" + String.Format("{0,2:D2}", timer.Minutes) + ":" + String.Format("{0,2:D2}", timer.Seconds);

                KeepCameraInBounds();

                if (camera.chickenCaught && GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    Random r = new Random();
                    GamePad.SetVibration(PlayerIndex.One, (float)r.NextDouble() / 2, (float)r.NextDouble() / 2);
                }
                else if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                }

                if (camera.chickenCaught)
                {
                    buttonSize += buttonScale;
                    int x = (int)(graphics.GraphicsDevice.Viewport.Width * 0.5) - (buttonSize / 2);
                    int y = (int)(graphics.GraphicsDevice.Viewport.Height * 0.2) - (buttonSize / 2);

                    buttonPos = new Rectangle(x, y, buttonSize, buttonSize);

                    if (buttonSize >= 120)
                    {
                        buttonScale = -MAX_BUTTON_SCALE;
                    }
                    else if (buttonSize <= 75)
                    {
                        buttonScale = MAX_BUTTON_SCALE;
                    }
                }
                else
                {
                    buttonSize = 75;
                    buttonScale = MAX_BUTTON_SCALE;
                }

                aiSystem.Update(world, gameTime, camera.Position);
                physicsSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
                audioSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
                oldKeyState = curKeyState;

                if (chickenCaught)
                {
                    RespawnChicken();
                    SpawnNewChicken();
                    caughtChickens++;
                    chickenCaught = false;
                }
                
                if (timer <= TimeSpan.Zero)
                {
                    winState = 1;
                }

            }

            //CheckWinState(winState);
        }

        /// <summary>
        /// Updates the current high score.
        /// </summary>
        /// <param name="gameTime">The total time until the chickens were all caught.</param>
        private void UpdateHighScore(GameTime gameTime)
        {
            // Write the new high score if we beat it
            Int64 elapsedMillis = (Int64)gameTime.TotalGameTime.TotalMilliseconds;
            if (elapsedMillis < curHighScore)
            {
                showHighScoreMsg = true;
                byte[] bytes = BitConverter.GetBytes(elapsedMillis);
                highScoreFile.Seek(0, SeekOrigin.Begin);
                highScoreFile.Write(bytes, 0, 8);
            }
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = false;
#if xbox
            gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];
#endif

            PlayerIndex player;

            if (winState == 1)
            {
                MediaPlayer.Stop();
                ScreenManager.AddScreen(new ArcadeWinScreen(caughtChickens), ControllingPlayer);
            }

            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                MediaPlayer.Pause();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            SkySphereEffect.Parameters["ViewMatrix"].SetValue(camera.ViewMatrix);
            SkySphereEffect.Parameters["ProjectionMatrix"].SetValue(camera.ProjectionMatrix);
            SkySphereEffect.Parameters["IntensityBlue"].SetValue(intensityBlue);

            // Draw the sphere model that the effect projects onto
            foreach (ModelMesh mesh in SkySphere.Meshes)
            {
                mesh.Draw();
            }

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            // TODO: Add your drawing code here

            renderSystem.Update(world, gameTime);
            drawGUI();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }

            base.Draw(gameTime);
        }

        private void drawGUI()
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(timerFont, "Chickens:" + caughtChickens, new Vector2(graphics.GraphicsDevice.Viewport.Width - (int)timerFont.MeasureString("Chickens: " + caughtChickens).X, 0), Color.White);
            spriteBatch.DrawString(timerFont, time, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(healthBar, new Rectangle((int)(graphics.GraphicsDevice.Viewport.Width * 0.01) + 44 / 2, graphics.GraphicsDevice.Viewport.Height / 2 - healthBar.Height / 2, 44, healthBar.Height), new Rectangle(45, 0, healthBar.Width - 44, healthBar.Height), Color.Gray);
            spriteBatch.Draw(healthBar, new Rectangle((int)(graphics.GraphicsDevice.Viewport.Width * 0.01) + 44 / 2, (int)((graphics.GraphicsDevice.Viewport.Height / 2 - healthBar.Height / 2) + (healthBar.Height * (1 - camera.GetStaminaRatio()))), 44, (int)(healthBar.Height * camera.GetStaminaRatio())), new Rectangle(45, 0, healthBar.Width - 44, healthBar.Height), Color.Yellow);
            spriteBatch.Draw(healthBar, new Rectangle((int)(graphics.GraphicsDevice.Viewport.Width * 0.01) + 44 / 2, graphics.GraphicsDevice.Viewport.Height / 2 - healthBar.Height / 2, 44, healthBar.Height), new Rectangle(0, 0, 44, healthBar.Height), Color.White);

            if (camera.chickenCaught)
            {
                int button = camera.GetQTE().getCurrentButton();
                bool controller = GamePad.GetState(PlayerIndex.One).IsConnected;
                switch (button)
                {
                    case (int)Cluck.buttons.xq:
                        spriteBatch.Draw(controller ? qteButtons[button] : qteKeys[button], buttonPos, Color.White);
                        break;
                    case (int)Cluck.buttons.ye:
                        spriteBatch.Draw(controller ? qteButtons[button] : qteKeys[button], buttonPos, Color.White);
                        break;
                    case (int)Cluck.buttons.br:
                        spriteBatch.Draw(controller ? qteButtons[button] : qteKeys[button], buttonPos, Color.White);
                        break;
                    case (int)Cluck.buttons.af:
                        spriteBatch.Draw(controller ? qteButtons[button] : qteKeys[button], buttonPos, Color.White);
                        break;
                }
            }

            spriteBatch.End();
        }

        private void KeepCameraInBounds()
        {
            Vector3 newPos = camera.Position;

            if (camera.Position.X < -1070.0f)
                newPos.X = -1070.0f;

            if (camera.Position.X > 1070.0f)
                newPos.X = 1070.0f;

            if (camera.Position.Y > 1070.0f)
                newPos.Y = 1070.0f;

            if (camera.Position.Y < -1.0f)
                newPos.Y = -1.0f;

            if (camera.Position.Z < -1070.0f)
                newPos.Z = -1070.0f;

            if (camera.Position.Z > 1070.0f)
                newPos.Z = 1070.0f;

            camera.Position = newPos;
        }

        private BoundingSphere calBoundingSphere(Model mod, float boundingScale)
        {
            List<Vector3> points = new List<Vector3>();
            BoundingSphere sphere;

            Matrix[] boneTransforms = new Matrix[mod.Bones.Count];
            mod.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in mod.Meshes)
            {
                //Console.WriteLine("mesh count " + mod.Meshes.Count);

                foreach (ModelMeshPart mmp in mesh.MeshParts)
                {
                    //Console.WriteLine("meshpart count " + mesh.MeshParts.Count);
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
            //Console.WriteLine("point count " + points.Count);
            sphere = BoundingSphere.CreateFromPoints(points);
            sphere = sphere.Transform(Matrix.CreateScale(boundingScale));
            //sphere = sphere.Transform(Matrix.CreateTranslation(new Vector3(0,0,-800000)));
            return sphere;
        }

        private BoundingBox calBoundingBox(Model mod, Vector3 worldPos, float orientation)
        {
            List<Vector3> points = new List<Vector3>();
            BoundingBox box;

            Matrix[] boneTransforms = new Matrix[mod.Bones.Count];
            mod.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mesh in mod.Meshes)
            {
                //Console.WriteLine("mesh count " + mod.Meshes.Count);

                foreach (ModelMeshPart mmp in mesh.MeshParts)
                {
                    //Console.WriteLine("meshpart count " + mesh.MeshParts.Count);
                    VertexPositionNormalTexture[] vertices =
                        new VertexPositionNormalTexture[mmp.VertexBuffer.VertexCount];

                    mmp.VertexBuffer.GetData<VertexPositionNormalTexture>(vertices);

                    foreach (VertexPositionNormalTexture vertex in vertices)
                    {
                        Vector3 point = Vector3.Transform(vertex.Position,
                            boneTransforms[mesh.ParentBone.Index]);

                        Matrix mat = Matrix.CreateRotationY(orientation) * Matrix.CreateTranslation(worldPos);
                        //mat *= Matrix.CreateRotationY(orientation);

                        point = Vector3.Transform(point, mat);

                        points.Add(point);
                    }
                }
            }
            //Console.WriteLine("point count " + points.Count);
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

        private void BuildBounds(Model fence, Texture2D texture)
        {
            for (int x = 0; x < FENCE_LINKS_WIDTH; x++)
            {
                GameEntity fenceEntityTop = new GameEntity();
                GameEntity fenceEntityBottom = new GameEntity();
                PositionComponent topPos;
                PositionComponent bottomPos;

                fenceEntityTop.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2) + (FENCE_WIDTH / 2) + (FENCE_WIDTH * x),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2)), 0f));
                topPos = fenceEntityTop.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityTop.AddComponent(new Renderable(fence, texture,
                                                           calBoundingBox(fence, topPos.GetPosition(), topPos.GetOrientation()), ToonEffectNoAnimation));
                fenceEntityTop.AddComponent(new FenceComponent());

                fenceEntityBottom.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2) + (FENCE_WIDTH / 2) + (FENCE_WIDTH * x),
                    0,
                    (FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2)), (float)Math.PI));
                bottomPos = fenceEntityBottom.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityBottom.AddComponent(new Renderable(fence, texture, calBoundingBox(fence, bottomPos.GetPosition(), bottomPos.GetOrientation()), ToonEffectNoAnimation));
                fenceEntityBottom.AddComponent(new FenceComponent());

                world.Add(fenceEntityTop);
                world.Add(fenceEntityBottom);
            }

            for (int y = 0; y < FENCE_LINKS_HEIGHT; y++)
            {
                GameEntity fenceEntityLeft = new GameEntity();
                GameEntity fenceEntityRight = new GameEntity();

                PositionComponent leftPos;
                PositionComponent rightPos;

                fenceEntityLeft.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2) + (FENCE_WIDTH * y) + (FENCE_WIDTH / 2)), (float)Math.PI / 2));
                leftPos = fenceEntityLeft.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityLeft.AddComponent(new Renderable(fence, texture, calBoundingBox(fence, leftPos.GetPosition(), leftPos.GetOrientation()), ToonEffectNoAnimation));
                fenceEntityLeft.AddComponent(new FenceComponent());

                fenceEntityRight.AddComponent(new PositionComponent(new Vector3((FENCE_LINKS_WIDTH * FENCE_WIDTH / 2),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2) + (FENCE_WIDTH * y) + (FENCE_WIDTH / 2)), (float)Math.PI / 2 + (float)Math.PI));
                rightPos = fenceEntityRight.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityRight.AddComponent(new Renderable(fence, texture, calBoundingBox(fence, rightPos.GetPosition(), rightPos.GetOrientation()), ToonEffectNoAnimation));
                fenceEntityRight.AddComponent(new FenceComponent());

                world.Add(fenceEntityLeft);
                world.Add(fenceEntityRight);
            }
        }

        private void BuildPen(Model pen, Texture2D texture)
        {
            for (int x = 0; x < PEN_LINKS_WIDTH; x++)
            {
                GameEntity penEntityTop = new GameEntity();
                GameEntity penEntityBottom = new GameEntity();

                PositionComponent topPos;
                PositionComponent bottomPos;

                penEntityTop.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_WIDTH * PEN_WIDTH / 2) + (PEN_WIDTH * x) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_WIDTH * PEN_WIDTH / 2) + PEN_YCOORD), 0f));
                topPos = penEntityTop.GetComponent<PositionComponent>(component_flags.position);
                penEntityTop.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, topPos.GetPosition(), topPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityTop.AddComponent(new FenceComponent());

                penEntityBottom.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_WIDTH * PEN_WIDTH / 2) + (PEN_WIDTH * x) + PEN_XCOORD,
                    0,
                    (PEN_LINKS_WIDTH * PEN_WIDTH / 2) + PEN_YCOORD), (float)Math.PI));
                bottomPos = penEntityBottom.GetComponent<PositionComponent>(component_flags.position);
                penEntityBottom.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, bottomPos.GetPosition(), bottomPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityBottom.AddComponent(new FenceComponent());

                world.Add(penEntityTop);
                world.Add(penEntityBottom);
            }

            for (int y = 0; y < PEN_LINKS_HEIGHT; y++)
            {
                GameEntity penEntityLeft = new GameEntity();
                GameEntity penEntityRight = new GameEntity();

                PositionComponent leftPos;
                PositionComponent rightPos;

                penEntityLeft.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_HEIGHT * PEN_WIDTH / 2) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_HEIGHT * PEN_WIDTH / 2) + (PEN_WIDTH * y) + PEN_YCOORD), (float)Math.PI / 2));
                leftPos = penEntityLeft.GetComponent<PositionComponent>(component_flags.position);
                penEntityLeft.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, leftPos.GetPosition(), leftPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityLeft.AddComponent(new FenceComponent());

                penEntityRight.AddComponent(new PositionComponent(new Vector3((PEN_LINKS_HEIGHT * PEN_WIDTH / 2) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_HEIGHT * PEN_WIDTH / 2) + (PEN_WIDTH * y) + PEN_YCOORD), (float)Math.PI / 2 + (float)Math.PI));
                rightPos = penEntityRight.GetComponent<PositionComponent>(component_flags.position);
                penEntityRight.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, rightPos.GetPosition(), rightPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityRight.AddComponent(new FenceComponent());

                world.Add(penEntityLeft);
                world.Add(penEntityRight);
            }
        }

        private void BuildPenBase(Model pen, Texture2D texture)
        {
            for (int x = 0; x < PEN_LINKS_WIDTH; x++)
            {
                GameEntity penEntityTop = new GameEntity();
                GameEntity penEntityBottom = new GameEntity();

                PositionComponent topPos;
                PositionComponent bottomPos;

                penEntityTop.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_WIDTH * PEN_BASE_WIDTH / 2) + (PEN_BASE_WIDTH * x) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_WIDTH * PEN_BASE_WIDTH / 2) + PEN_YCOORD), 0f));
                topPos = penEntityTop.GetComponent<PositionComponent>(component_flags.position);
                penEntityTop.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, topPos.GetPosition(), topPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityTop.AddComponent(new CaptureComponent());
                penEntityTop.AddComponent(new CollidableComponent());

                penEntityBottom.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_WIDTH * PEN_BASE_WIDTH / 2) + (PEN_BASE_WIDTH * x) + PEN_XCOORD,
                    0,
                    (PEN_LINKS_WIDTH * PEN_BASE_WIDTH / 2) + PEN_YCOORD), (float)Math.PI));
                bottomPos = penEntityBottom.GetComponent<PositionComponent>(component_flags.position);
                penEntityBottom.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, bottomPos.GetPosition(), bottomPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityBottom.AddComponent(new CaptureComponent());
                penEntityBottom.AddComponent(new CollidableComponent());

                world.Add(penEntityTop);
                world.Add(penEntityBottom);
            }

            for (int y = 0; y < PEN_LINKS_HEIGHT; y++)
            {
                GameEntity penEntityLeft = new GameEntity();
                GameEntity penEntityRight = new GameEntity();

                PositionComponent leftPos;
                PositionComponent rightPos;

                penEntityLeft.AddComponent(new PositionComponent(new Vector3((-PEN_LINKS_HEIGHT * PEN_BASE_WIDTH / 2) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_HEIGHT * PEN_BASE_WIDTH / 2) + (PEN_BASE_WIDTH * y) + PEN_YCOORD), (float)Math.PI / 2));
                leftPos = penEntityLeft.GetComponent<PositionComponent>(component_flags.position);
                penEntityLeft.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, leftPos.GetPosition(), leftPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityLeft.AddComponent(new CaptureComponent());
                penEntityLeft.AddComponent(new CollidableComponent());

                penEntityRight.AddComponent(new PositionComponent(new Vector3((PEN_LINKS_HEIGHT * PEN_BASE_WIDTH / 2) + PEN_XCOORD,
                    0,
                    (-PEN_LINKS_HEIGHT * PEN_BASE_WIDTH / 2) + (PEN_BASE_WIDTH * y) + PEN_YCOORD), (float)Math.PI / 2 + (float)Math.PI));
                rightPos = penEntityRight.GetComponent<PositionComponent>(component_flags.position);
                penEntityRight.AddComponent(new Renderable(pen, texture, calBoundingBox(pen, rightPos.GetPosition(), rightPos.GetOrientation()), ToonEffectNoAnimation));
                penEntityRight.AddComponent(new CaptureComponent());
                penEntityRight.AddComponent(new CollidableComponent());

                world.Add(penEntityLeft);
                world.Add(penEntityRight);
            }
        }

        private void BuildForest(Model trees, Texture2D texture)
        {
            for (int x = 0; x < FENCE_LINKS_WIDTH; x++)
            {
                GameEntity fenceEntityTop = new GameEntity();
                GameEntity fenceEntityBottom = new GameEntity();

                PositionComponent topPos;
                PositionComponent bottomPos;

                fenceEntityTop.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2) + (FENCE_WIDTH / 2) + (FENCE_WIDTH * x),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2)), 0f));
                topPos = fenceEntityTop.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityTop.AddComponent(new Renderable(trees, texture, calBoundingBox(trees, topPos.GetPosition(), topPos.GetOrientation()), ToonEffectNoAnimation));

                fenceEntityBottom.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2) + (FENCE_WIDTH / 2) + (FENCE_WIDTH * x),
                    0,
                    (FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2)), (float)Math.PI));
                bottomPos = fenceEntityBottom.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityBottom.AddComponent(new Renderable(trees, texture, calBoundingBox(trees, bottomPos.GetPosition(), bottomPos.GetOrientation()), ToonEffectNoAnimation));

                world.Add(fenceEntityTop);
                world.Add(fenceEntityBottom);
            }

            for (int y = 0; y < FENCE_LINKS_HEIGHT; y++)
            {
                GameEntity fenceEntityLeft = new GameEntity();
                GameEntity fenceEntityRight = new GameEntity();

                PositionComponent leftPos;
                PositionComponent rightPos;

                fenceEntityLeft.AddComponent(new PositionComponent(new Vector3((-FENCE_LINKS_WIDTH * FENCE_WIDTH / 2),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2) + (FENCE_WIDTH * y) + (FENCE_WIDTH / 2)), (float)Math.PI / 2));
                leftPos = fenceEntityLeft.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityLeft.AddComponent(new Renderable(trees, texture, calBoundingBox(trees, leftPos.GetPosition(), leftPos.GetOrientation()), ToonEffectNoAnimation));

                fenceEntityRight.AddComponent(new PositionComponent(new Vector3((FENCE_LINKS_WIDTH * FENCE_WIDTH / 2),
                    0,
                    (-FENCE_LINKS_HEIGHT * FENCE_WIDTH / 2) + (FENCE_WIDTH * y) + (FENCE_WIDTH / 2)), (float)Math.PI / 2 + (float)Math.PI));
                rightPos = fenceEntityRight.GetComponent<PositionComponent>(component_flags.position);
                fenceEntityRight.AddComponent(new Renderable(trees, texture, calBoundingBox(trees, rightPos.GetPosition(), rightPos.GetOrientation()), ToonEffectNoAnimation));

                world.Add(fenceEntityLeft);
                world.Add(fenceEntityRight);
            }
        }

        #endregion
    }
}