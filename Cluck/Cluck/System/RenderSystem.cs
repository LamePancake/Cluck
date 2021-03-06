﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace Cluck
{
    //TODO: Change gameTime to float and remove call to PlayerComponent.Draw
    class RenderSystem : GameSystem
    {
        FirstPersonCamera camera;
        GraphicsDevice graphicsDevice;
        private float prevTime;


        public RenderSystem(FirstPersonCamera cam, GraphicsDevice gdev)
            : base((int)component_flags.renderable)
        {
            camera = cam;
            graphicsDevice = gdev;
            prevTime = 0.0f;
        }

        public void Update(List<GameEntity> world, GameTime gameTime)
        {
            foreach (GameEntity entity in world)
            {
                Renderable renderable;

                if (entity.HasComponent((int)component_flags.capture))
                {
                    continue;
                }
                else if(entity.HasComponent((int)component_flags.caught))
                {
                    PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);
                    position.SetPosition(camera.GetChickenPosition());
                    renderable = entity.GetComponent<Renderable>(component_flags.renderable);
                    Matrix final = Matrix.CreateRotationX(MathHelper.ToRadians(camera.PitchDegrees)) * Matrix.CreateRotationY(MathHelper.ToRadians(camera.HeadingDegrees)) * Matrix.CreateTranslation(position.GetPosition());

                    renderable.SetMatrix(final);

                    renderable.SetMatrix(final);

                    renderable.SetBorderSize(0.2f);
                    renderable.SetLineColor(new Vector4(0, 0, 0, 1));
                    renderable.SetAmbientColor(new Vector4(1, 1, 1, 1));
                    renderable.SetAmbientIntensity(0.1f);

                    if (renderable.GetAnimationPlayer() != null)
                    {
                        renderable.GetAnimationPlayer().Update(gameTime.ElapsedGameTime, true, final);
                    }

                    Render(renderable);
                }
                else if (entity.HasComponent((int)component_flags.position) && entity.HasComponent((int)component_flags.renderable))
                {
                    PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);
                    renderable = entity.GetComponent<Renderable>(component_flags.renderable);
                    Matrix final = Matrix.CreateRotationY(position.GetOrientation()) * Matrix.CreateTranslation(position.GetPosition());

                    renderable.SetMatrix(final);

                    if (renderable.GetAnimationPlayer() != null)
                    {
                        renderable.GetAnimationPlayer().Update(gameTime.ElapsedGameTime, true, final);
                    }

                    Render(renderable);
                }
                else if (entity.HasComponent((int)component_flags.renderable) && !entity.HasComponent((int)component_flags.arm))
                {
                    renderable = entity.GetComponent<Renderable>(component_flags.renderable);

                    Render(renderable);
                }
                else if (entity.HasComponent((int)component_flags.arm) && entity.HasComponent((int)component_flags.renderable))
                {
                    renderable = entity.GetComponent<Renderable>(component_flags.renderable);
                    ArmComponent arms = entity.GetComponent<ArmComponent>(component_flags.arm);

                    renderable.SetBorderSize(0.0125f);
                    if (arms.WhichArm())
                    {
                        renderable.SetMatrix(camera.GetRightArmWorldMatrix((float)gameTime.ElapsedGameTime.TotalSeconds));
                    }
                    else
                    {
                        renderable.SetMatrix(camera.GetLeftArmWorldMatrix((float)gameTime.ElapsedGameTime.TotalSeconds));
                    }

                    Render(renderable);
                }
            }
        }



        private void Render(Renderable rend)
        {
            Model model = rend.GetModel();
            Matrix world = rend.GetMatrix();
            Texture2D texture = rend.GetTexture();

            Matrix[] groundMatrix = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(groundMatrix);

            Matrix[] bones = null;
            if(rend.GetAnimationPlayer() != null)
                 bones = rend.GetAnimationPlayer().GetSkinTransforms();

            foreach (ModelMesh mm in model.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                    mmp.Effect = rend.GetEffect();
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(groundMatrix[mm.ParentBone.Index]));
                    mmp.Effect.Parameters["World"].SetValue(groundMatrix[mm.ParentBone.Index] * world);
                    mmp.Effect.Parameters["WorldInverseTranspose"].SetValue(worldInverseTransposeMatrix);
                    mmp.Effect.Parameters["View"].SetValue(camera.ViewMatrix);
                    mmp.Effect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    mmp.Effect.Parameters["Texture"].SetValue(rend.GetTexture());
                    mmp.Effect.Parameters["LineThickness"].SetValue(rend.GetBorderSize());
                    mmp.Effect.Parameters["LineColor"].SetValue(rend.GetLineColor());
                    mmp.Effect.Parameters["AmbientColor"].SetValue(rend.GetAmbientColor());
                    mmp.Effect.Parameters["AmbientIntensity"].SetValue(rend.GetAmbientIntensity());
                    
                    if (bones != null)
                    {
                        mmp.Effect.Parameters["Bones"].SetValue(bones);
                    }
                }
                //if (bones == null)
                //{
                //    foreach (BasicEffect be in mm.Effects)
                //    {
                //        if (texture != null)
                //        {
                //            be.TextureEnabled = true;
                //            be.Texture = texture;
                //        }

                //        be.EnableDefaultLighting();
                //        be.World = groundMatrix[mm.ParentBone.Index] * world;
                //        be.View = camera.ViewMatrix;
                //        be.Projection = camera.ProjectionMatrix;
                //        //BoundingSphereRenderer.Render(rend.GetBoundingSphere(), graphicsDevice, be.View, be.Projection, be.World, Color.Red, Color.Green, Color.Blue);
                //    }
                //}
                //else
                //{
                //    foreach (SkinnedEffect effect in mm.Effects)
                //    {
                //        effect.SetBoneTransforms(bones);

                //        effect.View = camera.ViewMatrix; ;
                //        effect.Projection = camera.ProjectionMatrix;

                //        effect.EnableDefaultLighting();

                //        effect.SpecularColor = new Vector3(0.25f);
                //        effect.SpecularPower = 16;
                //    }
                //}
                //foreach (BasicEffect be in mm.Effects)
                //{
                //    if (texture != null)
                //    {
                //        be.TextureEnabled = true;
                //        be.Texture = texture;
                //    }

                //    be.EnableDefaultLighting();
                //    be.World = groundMatrix[mm.ParentBone.Index] * world;
                //    be.View = camera.ViewMatrix;
                //    be.Projection = camera.ProjectionMatrix;
                //    //BoundingSphereRenderer.Render(rend.GetBoundingSphere(), graphicsDevice, be.View, be.Projection, be.World, Color.Red, Color.Green, Color.Blue);
                //}
                mm.Draw();
            }
        }
    }
}
