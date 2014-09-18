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

namespace COMP7051Lab3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Cube myCube;
        BasicEffect effect;
        float angle = 0;
        float scalefactor = 0;
        Vector3 lightangle = new Vector3();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.AmbientLightColor = new Vector3(0.0f, 1.0f, 0.0f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            effect.LightingEnabled = true;

            base.Initialize();

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f,
                                (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 1f, 10f);
            effect.Projection = projection;
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);
            effect.View = V;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myCube = new Cube();
            myCube.size = new Vector3(1, 1, 1);
            myCube.position = new Vector3(0, 0, 0);
            myCube.BuildShape();

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if ((GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) ||
                (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)))
                this.Exit();

            // TODO: Add your update logic here

            if ((GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed) ||
                (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Left)))
                angle = angle - 0.005f;
            if ((GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed) ||
                (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Right)))
                angle = angle + 0.005f;
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Up))
            {
                scalefactor += 0.005f;
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Down))
            {
                scalefactor -= 0.005f;
            }

            float scale = scalefactor + ((GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y + 1.0f) / 2.0f + .5f);

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad4))
            {
                lightangle = new Vector3(1, 0, 0);
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad6))
            {
                lightangle = new Vector3(-1, 0, 0);
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad8))
            {
                lightangle = new Vector3(0, -1, 0);
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad2))
            {
                lightangle = new Vector3(0, 1, 0);
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad5))
            {
                lightangle = new Vector3(0, 0, -1);
            }
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.NumPad0))
            {
                lightangle = new Vector3(0, 0, 1);
            }


            Vector3 lightDir = lightangle;//Vector3(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X, GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, 1.0f);

            if (angle > 2 * Math.PI) angle = 0;
            Matrix R = Matrix.CreateRotationY(angle) * Matrix.CreateRotationX(.4f);
            Matrix T = Matrix.CreateTranslation(0.0f, 0f, 5f);
            Matrix S = Matrix.CreateScale(scale);
            effect.World = S * R * T;

            effect.DirectionalLight0.Direction = Vector3.Normalize(lightDir);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            myCube.RenderShape(GraphicsDevice, effect);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
