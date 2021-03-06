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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class PlayerComponent : Component
    {
        private Model leftArm;
        private Model rightArm;
        private Texture2D armsDiffuse;
        private FirstPersonCamera camera;
        private Matrix leftArmWorldMatrix;
        private Matrix rightArmWorldMatrix;

        private Matrix[] leftArmMatrix;
        private Matrix[] rightArmMatrix;

        public PlayerComponent(FirstPersonCamera c, Model la, Model ra, Texture2D ad)
            : base((int)component_flags.player)
        {
            camera = c;
            leftArm = la;
            rightArm = ra;
            armsDiffuse = ad;
            leftArmWorldMatrix = Matrix.Identity;
            rightArmWorldMatrix = Matrix.Identity;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize()
        {
;
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, bool isCatch)
        {
        }

        public void Draw(GameTime gameTime)
        {
            leftArmMatrix = new Matrix[leftArm.Bones.Count];
            leftArm.CopyAbsoluteBoneTransformsTo(leftArmMatrix);
            leftArmWorldMatrix = camera.GetRightArmWorldMatrix();

            foreach (ModelMesh mm in leftArm.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    be.TextureEnabled = true;
                    be.EnableDefaultLighting();
                    be.World = leftArmMatrix[mm.ParentBone.Index] * leftArmWorldMatrix;
                    be.View = camera.ViewMatrix;
                    be.Projection = camera.ProjectionMatrix;
                    be.Texture = armsDiffuse;
                }
                mm.Draw();
            }

            rightArmMatrix = new Matrix[rightArm.Bones.Count];
            rightArm.CopyAbsoluteBoneTransformsTo(rightArmMatrix);
            rightArmWorldMatrix = camera.GetLeftArmWorldMatrix();
            foreach (ModelMesh mm in rightArm.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    be.TextureEnabled = true ;
                    be.EnableDefaultLighting();
                    be.World = rightArmMatrix[mm.ParentBone.Index] * rightArmWorldMatrix;
                    be.View = camera.ViewMatrix;
                    be.Projection = camera.ProjectionMatrix;
                    be.Texture = armsDiffuse;
                }
                mm.Draw();
            }
        }
    }
}
