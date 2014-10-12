using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cluck
{
    //TODO: Change gameTime to float and remove call to PlayerComponent.Draw
    class RenderSystem : GameSystem
    {
        FirstPersonCamera camera;

        public RenderSystem(FirstPersonCamera cam)
            : base((int)component_flags.renderable)
        {
            camera = cam;
        }

        public void Update(List<GameEntity> world, GameTime gameTime)
        {
            foreach (GameEntity entity in world)
            {
                Renderable renderable;
                
                if (entity.HasComponent(myFlag) && !entity.HasComponent((int)component_flags.kinematic) && !entity.HasComponent((int)component_flags.arm))
                {
                    renderable = entity.GetComponent<Renderable>();
                    Render(renderable);
                }
                else if (entity.HasComponent((int)component_flags.position) && entity.HasComponent((int)component_flags.renderable))
                {
                    PositionComponent position = entity.GetComponent <PositionComponent>();

                    renderable = entity.GetComponent<Renderable>();
                    
                    Matrix final = Matrix.CreateRotationY(position.GetOrientation()) * Matrix.CreateTranslation(position.GetPosition());

                    renderable.SetMatrix(final);

                    Render(renderable);
                }
                else if (entity.HasComponent((int)component_flags.arm) && entity.HasComponent((int)component_flags.renderable))
                {
                    renderable = entity.GetComponent<Renderable>();
                    ArmComponent arms = entity.GetComponent<ArmComponent>();

                    if (arms.WhichArm())
                    {
                        renderable.SetMatrix(camera.GetRightArmWorldMatrix());
                    }
                    else
                    {
                        renderable.SetMatrix(camera.GetLeftArmWorldMatrix());
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
            foreach (ModelMesh mm in model.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    //mmp.VertexBuffer;
                }
                foreach (BasicEffect be in mm.Effects)
                {
                    if (texture != null)
                    {
                        be.TextureEnabled = true;
                        be.Texture = texture;
                    }

                    be.EnableDefaultLighting();
                    be.World = groundMatrix[mm.ParentBone.Index] * world;
                    be.View = camera.ViewMatrix;
                    be.Projection = camera.ProjectionMatrix;
                    //BoundingSphereRenderer.Render(mm.BoundingSphere, , be.View, be.Projection, Color.Red);
                }
                mm.Draw();
            }
        }
    }
}
