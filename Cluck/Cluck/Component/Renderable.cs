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

        public Renderable(Model model, Texture2D texture)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
            entityTexture = texture;
            entityModel.Meshes[0].BoundingSphere.Equals(entityModel.Meshes[0].BoundingSphere.Radius * 20);
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
    }
}
