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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Model leftArm;
        private BasicEffect effect;
        private Texture2D armsDiffuse;
        private List<GameEntity> world;
        private AISystem aiSystem;

        public Cluck()
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

            base.Initialize();

            world = new List<GameEntity>();
            aiSystem = new AISystem(this);

            TestEntity testEntity = new TestEntity();

            world.Add(testEntity);

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

            // TODO: use this.Content to load your game content here
            armsDiffuse = Content.Load<Texture2D>(@"Textures\arms_diffuse");

            leftArm = Content.Load<Model>(@"Models\arm_left");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            aiSystem.UpdateWorld(world);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            //leftArm.Draw(GraphicsDevice, effect, "diffuseMapTexture", armsDiffuse);

            foreach (ModelMesh mm in leftArm.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    be.EnableDefaultLighting();
                }
                mm.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
