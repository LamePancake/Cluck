using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cluck
{
    class Util
    {
        static Random randomGen = new Random();

        public Util()
        {
        }

        public static double RandomClamped()
        {
            return (randomGen.NextDouble() - randomGen.NextDouble());
        }

        public static double Random()
        {
            return randomGen.NextDouble();
        }

        /*
        Desc:
        Trasforms a specified point to world space.
        Parameters:
        point - the point to be transformed.
        heading - forward velocity component.
        side - side velocity component.
        pos - x and y to translate by.
        */
        public static Vector3 PointToWorldSpace(Vector3 point,Vector3 heading, Vector3 side, Vector3 pos)
        {
	        //make a copy of the point
	        Vector3 TransPoint = point;

	        //create a transformation matrix
	        Matrix matTransform = Matrix.Identity;

	        //rotate
            matTransform.M11 = heading.X;
            matTransform.M13 = heading.Z;
            matTransform.M31 = side.X;
            matTransform.M33 = side.Z;

            //Console.WriteLine("Pos: " + Matrix.CreateTranslation(pos));
	        //and translate
	        matTransform *= Matrix.CreateTranslation(pos);

            //Console.WriteLine("Trans " + matTransform);

	        //now transform the vertex
            TransPoint = Vector3.Transform(TransPoint, matTransform);

            //Console.WriteLine(TransPoint);

	        return TransPoint;
        }

        public static Vector3 Vec3RotateAroundOrigin(Vector3 vec, float ang)
        {
            //create a transformation matrix
	        Matrix matTransform = Matrix.Identity;
  
            //rotate
            matTransform *= Matrix.CreateRotationY(ang);
	
            //now transform the object's vertices
            vec = Vector3.Transform(vec, matTransform);

            return vec;
        }

        public static Vector3 PerpInZPlane(Vector3 vec)
        {
            return new Vector3(-vec.Z, vec.Y, vec.X);
        }
    }
}
