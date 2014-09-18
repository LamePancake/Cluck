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
    class BasicShape
    {

        public Vector3 size;
        public Vector3 position;
        public VertexPositionNormalTexture[] vertices;
        public int primitiveCount;

        public BasicShape()
        {
            this.size = new Vector3(0, 0, 0);
            this.position = new Vector3(0, 0, 0);
            this.vertices = null;
            BuildShape();
        }

        public BasicShape(Vector3 size, Vector3 position)
        {
            this.size = size;
            this.position = position;
            this.vertices = null;
            BuildShape();
        }

        public virtual void BuildShape()
        {
            primitiveCount = 0;
        }

        public void RenderShape(GraphicsDevice device, Effect shapeEffect)
        {
            if (vertices != null)
            {
                RasterizerState rs = new RasterizerState();
                rs.CullMode = CullMode.CullClockwiseFace;
                device.RasterizerState = rs;
                foreach (EffectPass pass in shapeEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    device.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, vertices, 0, primitiveCount);
                }
            }
        }

    }
}
