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

        public Renderable(Model model)
            : base((int)component_flags.renderable)
        {
            entityModel = model;
            worldMatrix = Matrix.Identity;
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
    }
}
