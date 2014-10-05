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
    public class PlayerComponent
    {
        private Model leftArm;
        private Model rightArm;
        private Texture2D armsDiffuse;
        private FirstPersonCamera camera;
        private Matrix leftArmWorldMatrix;
        private Matrix rightArmWorldMatrix;

        private Matrix[] leftArmMatrix;
        private Matrix[] rightArmMatrix;

        private const float ARM_SCALE = 0.03f;
        private const float ARM_X_OFFSET = 0.8f;//0.45f;
        private const float ARM_Y_OFFSET = -0.6f;//-13.5f;//-0.75;
        private const float ARM_Z_OFFSET = 1.55f;//9.0f;//1.65f;
        private const float LEFT_ARM_X_OFFSET = -0.8f;

        private const float MIN_ARM_X_OFFSET = 0.5f;
        private const float MIN_LEFT_ARM_X_OFFSET = -0.5f;

        float leftXOffset;
        float rightXOffset;

        bool clap = false;

        public PlayerComponent(FirstPersonCamera c, Model la, Model ra, Texture2D ad)
        {
            camera = c;
            leftArm = la;
            rightArm = ra;
            armsDiffuse = ad;
            leftArmWorldMatrix = Matrix.Identity;
            rightArmWorldMatrix = Matrix.Identity;
            leftXOffset = LEFT_ARM_X_OFFSET;
            rightXOffset = ARM_X_OFFSET;
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
            if(isCatch)
                clap = true;
        }

        public void Draw(GameTime gameTime)
        {
            if (clap)
            {
                if (leftXOffset <= MIN_LEFT_ARM_X_OFFSET && rightXOffset >= MIN_ARM_X_OFFSET)
                {
                    leftXOffset += 0.05f;
                    rightXOffset -= 0.05f;
                }
                else
                {
                    clap = false;
                }
            }
            else
            {
                if (leftXOffset > LEFT_ARM_X_OFFSET && rightXOffset < ARM_X_OFFSET)
                {
                    leftXOffset -= 0.05f;
                    rightXOffset += 0.05f;
                }
            }

            leftArmMatrix = new Matrix[leftArm.Bones.Count];
            leftArm.CopyAbsoluteBoneTransformsTo(leftArmMatrix);
            leftArmWorldMatrix = camera.ArmWorldMatrix(rightXOffset, ARM_Y_OFFSET, ARM_Z_OFFSET, ARM_SCALE);

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
            rightArmWorldMatrix = camera.ArmWorldMatrix(leftXOffset, ARM_Y_OFFSET, ARM_Z_OFFSET, ARM_SCALE);
            foreach (ModelMesh mm in rightArm.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    be.TextureEnabled = true;
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
