﻿#region File Description
//-----------------------------------------------------------------------------
// TutorialScreen.cs
//
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
    class TutorialScreen : GameScreen
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

        private int minutesAllotted;
        private int secondsAllotted;
        private int deathSecondsAlotted;
        private Boolean cluckExist;
        private int instructionStage;
        private int instructionIntermissionTime;
        private int instructionCount;

        private Model ground;
        private Model forest;
        private Model fence;
        private Model leftArm;
        private Model rightArm;
        private Model chicken;
        private Model penBase;
        private Model chickenPen;
        private Model cluck;
        private Cue testSong;
        private Cue currentSong;
        private AudioCategory musicCategory;
        private AudioEngine engine;
        private SoundBank soundBank;
        private WaveBank waveBank;
        private Matrix boundingSphereSize;
        private int boundingSize;
        private SpriteFont timerFont;
        private static TimeSpan timer;
        private static TimeSpan deathTimer;
        private static TimeSpan tutTimer;
        private Boolean timeStart;
        private Boolean tutorialOn;
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
        private const float CAMERA_SLIDING_MULTIPLIER = 3.0f;
        private const float CAMERA_CROUCHING_MULTIPLIER = 1.0f;
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
        private const float PEN_TRANS = 118.8f;
        private const float PEN_BOUNDARY_BUFFER = 55.0f;
        private const float PEN_LEFT_BOUND = (-PEN_LINKS_WIDTH * PEN_WIDTH / 2) + PEN_XCOORD - PEN_TRANS;
        private const float PEN_RIGHT_BOUND = (PEN_LINKS_WIDTH * PEN_WIDTH / 2) + PEN_XCOORD + PEN_TRANS;
        private const float PEN_TOP_BOUND = (-PEN_LINKS_WIDTH * PEN_WIDTH / 2) + PEN_YCOORD - PEN_TRANS;
        private const float PEN_BOTTOM_BOUND = (PEN_LINKS_HEIGHT * PEN_WIDTH / 2) + PEN_YCOORD + PEN_TRANS;

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
        public static int remainingChickens;

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
        InputAction instructionAction;

        private float intensityBlue;

        private const int MAX_BUTTON_SCALE = 6;
        private Texture2D[] qteButtons;
        private Texture2D[] qteKeys;
        private int buttonSize = 75;
        private int buttonScale = MAX_BUTTON_SCALE;
        private Rectangle buttonPos;

        private Texture2D clock;
        private Texture2D chickenPic;

        public static bool scored = false;
        private float scoreScale = 0.1f;
        private float scoreScaleAmount = 0.08f;

        private float amountOfTimeToAdd = 10;

        private AnimatedTexture sprintTexture;
        private AnimatedTexture slideTexture;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public TutorialScreen()
        {
            camera = Cluck.camera;
            graphics = Cluck.graphics;
            instructionStage = 0;

            secondsAllotted = 0;
            minutesAllotted = 10;
            deathSecondsAlotted = 18;
            instructionIntermissionTime = 30;
            instructionCount = 6;

            total_num_of_chickens = 3;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //camera = new FirstPersonCamera(this);
            //Components.Add(camera);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);

            instructionAction = new InputAction(
                new Buttons[] { Buttons.A },
                new Keys[] { Keys.Enter },
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

            timer = new TimeSpan(0, minutesAllotted, secondsAllotted);
            deathTimer = new TimeSpan(0, 0, deathSecondsAlotted);
            tutTimer = new TimeSpan(0, 0, 0);
            timeStart = false;
            cluckExist = false;
            tutorialOn = false;

            camera.PositionUpdate = KeepCameraInBounds;
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

            remainingChickens = total_num_of_chickens;

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

                camera.Reset();
                ComponentLists.ClearLists();

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

                chickenPic = content.Load<Texture2D>(@"Textures\chicken");
                clock = content.Load<Texture2D>(@"Textures\clock");

                leftArm = content.Load<Model>(@"Models\arm_left_extended");
                rightArm = content.Load<Model>(@"Models\arm_right_extended");

                fence = content.Load<Model>(@"Models\fence_side");
                ground = content.Load<Model>(@"Models\ground_v2");

                chicken = content.Load<Model>(@"Models\chicken_animv2");
                cluck = content.Load<Model>(@"Models\cluck");
                forest = content.Load<Model>(@"Models\tree_side");

                penBase = content.Load<Model>(@"Models\pen_base_large");
                chickenPen = content.Load<Model>(@"Models\chicken_pen_side_large");

                CHICKEN_SOUNDS[0] = content.Load<SoundEffect>(@"Audio\Cluck1");
                CHICKEN_SOUNDS[1] = content.Load<SoundEffect>(@"Audio\Cluck2");
                CHICKEN_SOUNDS[2] = content.Load<SoundEffect>(@"Audio\Cluck3");
                CHICKEN_SOUNDS[3] = content.Load<SoundEffect>(@"Audio\Cluck4");
                CHICKEN_SOUNDS[4] = content.Load<SoundEffect>(@"Audio\Cluck5");
                CHICKEN_SOUNDS[5] = content.Load<SoundEffect>(@"Audio\Cluck6");
                CHICKEN_SOUNDS[6] = content.Load<SoundEffect>(@"Audio\Cluck7");
                CHICKEN_SOUNDS[7] = content.Load<SoundEffect>(@"Audio\Cluck8");
                SoundEffect.DistanceScale = 100f;

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
                    KinematicComponent chickinematics = new KinematicComponent(0.08f, 3f, (float)Math.PI / 4, 0.1f, 2.6f);

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

                List<Obstacle> trees = new List<Obstacle>();

                // now create the AI system.
                aiSystem = new AISystem(world, trees);

                intensityBlue = 1.0f;
                SkySphereEffect = content.Load<Effect>("SkySphere");
                TextureCube SkyboxTexture = content.Load<TextureCube>(@"Textures\sky");
                TextureCube SkyboxTextureRed = content.Load<TextureCube>(@"Textures\skyRed");
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

                qteButtons[(int)Cluck.buttons.xq] = content.Load<Texture2D>(@"Textures\x");
                qteButtons[(int)Cluck.buttons.ye] = content.Load<Texture2D>(@"Textures\y");
                qteButtons[(int)Cluck.buttons.br] = content.Load<Texture2D>(@"Textures\b");
                qteButtons[(int)Cluck.buttons.af] = content.Load<Texture2D>(@"Textures\a");

                qteKeys[(int)Cluck.buttons.xq] = content.Load<Texture2D>(@"Textures\q");
                qteKeys[(int)Cluck.buttons.ye] = content.Load<Texture2D>(@"Textures\e");
                qteKeys[(int)Cluck.buttons.br] = content.Load<Texture2D>(@"Textures\r");
                qteKeys[(int)Cluck.buttons.af] = content.Load<Texture2D>(@"Textures\f");

                sprintTexture = new AnimatedTexture(Vector2.Zero, 0, 1.0f, 0.5f, false);
                sprintTexture.Load(content, @"Textures\sprint_motion", 2, 6);
                sprintTexture.Pause();

                slideTexture = new AnimatedTexture(Vector2.Zero, 0, 1.0f, 0.5f, true);
                slideTexture.Load(content, @"Textures\mud_streaks", 8, 8);
                slideTexture.Pause();

                camera.Walk = content.Load<SoundEffect>(@"Audio\walk").CreateInstance();
                camera.Slide = content.Load<SoundEffect>(@"Audio\slide").CreateInstance();

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();

                // Initialize audio objects.
                engine = new AudioEngine("Content\\Audio\\Yoshi_dynamic.xgs");
                soundBank = new SoundBank(engine, "Content\\Audio\\CluckSound.xsb");
                waveBank = new WaveBank(engine, "Content\\Audio\\CluckWave.xwb");

                // Get the category.
                musicCategory = engine.GetCategory("Music");

                // Get the different songs for playback.
                testSong = soundBank.GetCue("Yoshi_looped_xact");
                currentSong = testSong;
                currentSong.Play();
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
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected void UnloadContent()
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
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (tutTimer <= TimeSpan.Zero)
            {
                tutorialOn = true;
                tutTimer = new TimeSpan(0, 0, instructionIntermissionTime);
            }
            else
            {
                tutorialOn = false;
                tutTimer -= gameTime.ElapsedGameTime;
            }
            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                timeStart = true;

                UpdateAudio(gameTime);

                // Allows the game to exit
                //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                //    this.Exit();

                //if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                //    this.Exit();

                //winState = 0;
                curKeyState = Keyboard.GetState();

                if (timer > TimeSpan.Zero && timeStart)
                {
                    timer -= gameTime.ElapsedGameTime;
                }

                time = String.Format("{0,2:D2}", timer.Hours) + ":" + String.Format("{0,2:D2}", timer.Minutes) + ":" + String.Format("{0,2:D2}", timer.Seconds);
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

                if (camera.IsSprinting() && !camera.IsSliding() && !camera.isCrouching())
                {
                    sprintTexture.Play();
                }
                else
                {
                    sprintTexture.Pause();
                }

                sprintTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

                if (camera.IsSliding())
                {
                    slideTexture.Play();
                }
                else
                {
                    slideTexture.Reset();
                    slideTexture.Pause();
                }

                slideTexture.UpdateFrame((float)gameTime.ElapsedGameTime.TotalSeconds);

                aiSystem.Update(world, gameTime, camera.Position);
                physicsSystem.Update(world, gameTime);
                audioSystem.Update(world, gameTime.ElapsedGameTime.Milliseconds);
                oldKeyState = curKeyState;
                if (timer <= TimeSpan.Zero)
                {
                    PlayDeathScene(gameTime);
                }

                if (timer > TimeSpan.Zero && remainingChickens <= 0)
                {
                    winState = 1;
                }
                else if (deathTimer <= TimeSpan.Zero && remainingChickens > 0)
                {
                    winState = -1;
                }

            }

            //if (winState != 0)
            //{
            //    ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            //}
            // && (currentSong.IsStopped || currentSong.IsStopping)

            //CheckWinState(winState);
        }

        /// <summary>
        /// Update the audio as necessary.
        /// </summary>
        private void UpdateAudio(GameTime gameTime)
        {
            if (IsActive)
            {
                // Adjust the song's pitch (and thus playback speed)
                if (currentSong.IsStopped && timer.TotalSeconds > 0 && winState == 0)
                {
                    testSong = soundBank.GetCue("Yoshi_looped_xact");
                    currentSong = testSong;
                    currentSong.Play();
                }
                if (currentSong.IsPaused)
                {
                    currentSong.Resume();
                }
                engine.Update();
            }
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
#if XBOX
            gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];
#endif

            PlayerIndex player;



            if (tutorialOn)
            {
                if (instructionStage < instructionCount)
                {
                    currentSong.Pause();
                    ScreenManager.AddScreen(new InstructionScreen(instructionStage), ControllingPlayer);
                    instructionStage++;
                }
            }

            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                currentSong.Pause();
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
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


            if (winState != 0)
            {
                currentSong.Stop(AudioStopOptions.AsAuthored);
                ScreenManager.AddScreen(new TutorialEndScreen(), ControllingPlayer);
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
            //drawWinState(winState);
            //RenderBox(testBox);

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
            spriteBatch.Draw(chickenPic, new Rectangle(graphics.GraphicsDevice.Viewport.Width - (int)timerFont.MeasureString(" x " + remainingChickens).X - (int)(graphics.GraphicsDevice.Viewport.Width * 0.01f + 50), (int)(graphics.GraphicsDevice.Viewport.Height * 0.01f), 50, 50), Color.White);

            Vector2 tempVector = new Vector2(graphics.GraphicsDevice.Viewport.Width - (int)timerFont.MeasureString(" x " + remainingChickens).X - (int)(graphics.GraphicsDevice.Viewport.Width * 0.01f), (int)(graphics.GraphicsDevice.Viewport.Height * 0.01f + 10));
            for (int layer = 0; layer < 4; layer++)
            {
                tempVector.X += layer;
                tempVector.Y += layer;
                spriteBatch.DrawString(timerFont, " x " + remainingChickens, tempVector, Color.Black);
            }

            spriteBatch.DrawString(timerFont, " x " + remainingChickens, tempVector, Color.White);

            spriteBatch.Draw(clock, new Rectangle((int)(graphics.GraphicsDevice.Viewport.Width * 0.01f), (int)(graphics.GraphicsDevice.Viewport.Height * 0.01f), 50, 50), Color.White);

            tempVector =  new Vector2((int)(graphics.GraphicsDevice.Viewport.Width * 0.01f) + 50, (int)(graphics.GraphicsDevice.Viewport.Height * 0.01f + 10));
            for (int layer = 0; layer < 4; layer++)
            {
                tempVector.X += layer;
                tempVector.Y += layer;
                spriteBatch.DrawString(timerFont, time, tempVector, Color.Black);
            }

            spriteBatch.DrawString(timerFont, time, tempVector, Color.White);

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

            if (scored)
            {
                tempVector = new Vector2((int)(graphics.GraphicsDevice.Viewport.Width * 0.01f) + 100, (int)(graphics.GraphicsDevice.Viewport.Height * 0.01f + 55));
                for (int layer = 0; layer < 4; layer++)
                {
                    tempVector.X += layer;
                    tempVector.Y += layer;
                    spriteBatch.DrawString(timerFont, "+" + amountOfTimeToAdd, tempVector, Color.Black, 0, new Vector2(0, 0), scoreScale, new SpriteEffects(), 0);
                }
                spriteBatch.DrawString(timerFont, "+" + amountOfTimeToAdd, tempVector, Color.GreenYellow, 0, new Vector2(0, 0), scoreScale, new SpriteEffects(), 0);

                scoreScale += scoreScaleAmount;
                if (scoreScale >= 2.2)
                {
                    scoreScaleAmount = -0.08f;
                }
                else if (scoreScale <= 0)
                {
                    scored = false;
                    scoreScale = 0.1f;
                    scoreScaleAmount = 0.08f;
                }
            }

            if (!sprintTexture.IsPaused)
            {
                sprintTexture.DrawFrame(spriteBatch, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            }

            if (!slideTexture.IsPaused)
            {
                slideTexture.DrawFrame(spriteBatch, new Rectangle(0, 0, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height));
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Keeps the camera out of the pen and within the bounds of the level.
        /// </summary>
        /// <param name="prevPos">The camera's previous position.</param>
        /// <param name="desiredPos">The camera's desired position.</param>
        /// <param name="forwards">The forward direction of the camera.</param>
        /// <returns></returns>
        private void KeepCameraInBounds(ref Vector3 prev, ref Vector3 desiredPos, ref Vector3 forwards, out Vector3 finalPos)
        {
            Vector3 newPos = desiredPos;
            const float penRight = PEN_RIGHT_BOUND + PEN_BOUNDARY_BUFFER;
            const float penLeft = PEN_LEFT_BOUND - PEN_BOUNDARY_BUFFER;
            const float penTop = PEN_TOP_BOUND - PEN_BOUNDARY_BUFFER;
            const float penBottom = PEN_BOTTOM_BOUND + PEN_BOUNDARY_BUFFER;

            #region Stay in Bounds
            // East and West fences
            if (desiredPos.X < -1070.0f)
                newPos.X = -1070.0f;
            else if (desiredPos.X > 1070.0f)
                newPos.X = 1070.0f;

            // Crouching height
            if (desiredPos.Y < -1.0f)
                newPos.Y = -1.0f;

            // North and South fences
            if (desiredPos.Z < -1070.0f)
                newPos.Z = -1070.0f;
            else if (desiredPos.Z > 1070.0f)
                newPos.Z = 1070.0f;

            #endregion
            #region Stay out of Pen
            // Get them out of the pen if they're in there
            if (desiredPos.X < penRight && desiredPos.X > penLeft
               && desiredPos.Z < penBottom && desiredPos.Z > penTop)
            {
                bool wasInZ = prev.Z < penBottom && prev.Z > penTop;
                bool wasInX = prev.X < penRight && prev.X > penLeft;

                // If they're moving positively along Z, then they came from the top part of the pen
                if (!wasInZ)
                {
                    if (forwards.Z > 0)
                        newPos.Z = penTop;
                    // Otherwise they're coming from the top part of the pen
                    else
                        newPos.Z = penBottom;
                }
                else if (!wasInX)
                {
                    if (forwards.X > 0)
                        newPos.X = penLeft;
                    else
                        newPos.X = penRight;
                }
            }
            #endregion
            finalPos = newPos;
        }

        private void PlayDeathScene(GameTime gameTime)
        {
            if (!cluckExist)
            {
                GameEntity cluckEntity = new GameEntity();

                Vector3 cluckPosition = new Vector3(0, 10000, 0);
                PositionComponent cluckPos = new PositionComponent(cluckPosition, (float)(Util.RandomClamped() * Math.PI));
                //Renderable cluckRenderable = new Renderable(cluck, chickenDiffuse, calBoundingSphere(cluck, boundingChickenScale), new AnimationPlayer(cluckSkinningData), ToonEffect);
                Renderable cluckRenderable = new Renderable(cluck, chickenDiffuse, calBoundingSphere(cluck, boundingChickenScale), ToonEffectNoAnimation);

                cluckEntity.AddComponent(cluckPos);
                cluckEntity.AddComponent(cluckRenderable);
                world.Add(cluckEntity);
                cluckExist = true;
            }

            if (deathTimer > TimeSpan.Zero)
            {
                deathTimer -= gameTime.ElapsedGameTime;
                intensityBlue -= 0.025f;
                intensityBlue = MathHelper.Clamp(intensityBlue, 0.0f, 1.0f);
            }

            Vector3 currentPosition = world.Last<GameEntity>().GetComponent<PositionComponent>(component_flags.position).GetPosition();
            if (currentPosition.Y > 0)
            {
                world.Last<GameEntity>().GetComponent<PositionComponent>(component_flags.position).SetPosition(currentPosition + new Vector3(0, -50, 0));
            }
            else
            {
                if (world.Last<GameEntity>().GetComponent<Renderable>(component_flags.renderable).GetAnimationPlayer() == null)
                {
                    SkinningData cluckSkinningData = cluck.Tag as SkinningData;

                    if (cluckSkinningData == null)
                        throw new InvalidOperationException
                            ("This model does not contain a SkinningData tag.");
                    AnimationClip cluckClip = cluckSkinningData.AnimationClips["Take 001"];
                    world.Last<GameEntity>().GetComponent<Renderable>(component_flags.renderable).SetAnimationPlayer(new AnimationPlayer(cluckSkinningData));
                    world.Last<GameEntity>().GetComponent<Renderable>(component_flags.renderable).SetEffect(ToonEffect);
                    world.Last<GameEntity>().GetComponent<Renderable>(component_flags.renderable).GetAnimationPlayer().StartClip(cluckClip);
                }
            }
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
                graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList, primitiveList, 0, 8,
                    bBoxIndices, 0, 12);
            }
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
