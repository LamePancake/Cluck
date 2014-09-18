using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace COMP7051Lab3
{
    class Cube : BasicShape
    {
        public override void BuildShape()
        {
            primitiveCount = 12;
            vertices = new VertexPositionNormalTexture[36];
            Vector2 Texcoords = new Vector2(0f, 0f);

            Vector3[] face = new Vector3[6];
            //TopLeft
            face[0] = new Vector3(-size.X, size.Y, 0.0f);
            //BottomLeft
            face[1] = new Vector3(-size.X, -size.Y, 0.0f);
            //TopRight
            face[2] = new Vector3(size.X, size.Y, 0.0f);
            //BottomLeft
            face[3] = new Vector3(-size.X, -size.Y, 0.0f);
            //BottomRight
            face[4] = new Vector3(size.X, -size.Y, 0.0f);
            //TopRight
            face[5] = new Vector3(size.X, size.Y, 0.0f);

            //front face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i] = new VertexPositionNormalTexture(face[i] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, Texcoords);
                vertices[i + 3] = new VertexPositionNormalTexture(face[i + 3] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, Texcoords);
            }

            //back face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 6] = new VertexPositionNormalTexture(face[2 - i] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, Texcoords);
                vertices[i + 6 + 3] = new VertexPositionNormalTexture(face[5 - i] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, Texcoords);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 12] = new VertexPositionNormalTexture(Vector3.Transform(face[i], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, Texcoords);
                vertices[i + 12 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[i + 3], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, Texcoords);
            }

            //Right face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 18] = new VertexPositionNormalTexture(Vector3.Transform(face[2 - i], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, Texcoords);
                vertices[i + 18 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5 - i], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, Texcoords);

            }

            //Top face

            Matrix RotX90 = Matrix.CreateRotationX(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 24] = new VertexPositionNormalTexture(Vector3.Transform(face[i], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, Texcoords);
                vertices[i + 24 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[i + 3], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, Texcoords);

            }

            //Bottom face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 30] = new VertexPositionNormalTexture(Vector3.Transform(face[2 - i], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, Texcoords);
                vertices[i + 30 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5 - i], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, Texcoords);
            }
        }

    }
}
