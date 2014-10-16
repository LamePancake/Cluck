using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class Renderable : Component
    {
        private Model entityModel;
        private Matrix worldMatrix;
        private Texture2D entityTexture;
        private BoundingSphere entitySphere;
        private BoundingBox entityBox;

        public Renderable(Model model, Texture2D texture, BoundingSphere sphere)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entitySphere = sphere;
        }

        public Renderable(Model model, Texture2D texture, BoundingBox box)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entityBox = box;
        }

        public Model GetModel()
        {
            return entityModel;
        }

        public void SetModel(Model model)
        {
            entityModel = model;
        }

        public Matrix GetMatrix()
        {
            return worldMatrix;
        }

        public void SetMatrix(Matrix mat)
        {
            worldMatrix = mat;
        }

        public Texture2D GetTexture()
        {
            return entityTexture;
        }

        public void SetTexture(Texture2D texture)
        {
            entityTexture = texture;
        }

        public BoundingSphere GetBoundingSphere()
        {
            return entitySphere;
        }

        public void SetBoundingSphere(BoundingSphere boundsphere)
        {
            entitySphere = boundsphere;
        }

        public BoundingBox GetBoundingBox()
        {
            return entityBox;
        }

        public void SetBoundingSphere(BoundingBox boundbox)
        {
            entityBox = boundbox;
        }
    }
}
