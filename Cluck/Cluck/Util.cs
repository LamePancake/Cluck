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

        public static bool IntersectRayVsBox(BoundingBox a_kBox,
                          Ray a_kRay,
                          out float a_fDist,
                          out int a_nFace)
        {
            a_nFace = -1;
            a_fDist = float.MaxValue;

            // Preform the collision query  
            float? fParam = a_kRay.Intersects(a_kBox);

            // No collision, return false.  
            if (fParam.HasValue == false)
                return false;

            // Asign the distance along the ray our intersection point is  
            a_fDist = fParam.Value;

            // Compute the intersection point  
            Vector3 vIntersection = a_kRay.Position + a_kRay.Direction * a_fDist;

            // Determine the side of the box the ray hit, this is slower than  
            // more obvious methods but it's extremely tolerant of numerical  
            // drift (aka rounding errors)  
            Vector3 vDistMin = vIntersection - a_kBox.Min;
            Vector3 vDistMax = vIntersection - a_kBox.Max;

            vDistMin.X = (float)Math.Abs(vDistMin.X);
            vDistMin.Y = (float)Math.Abs(vDistMin.Y);
            vDistMin.Z = (float)Math.Abs(vDistMin.Z);

            vDistMax.X = (float)Math.Abs(vDistMax.X);
            vDistMax.Y = (float)Math.Abs(vDistMax.Y);
            vDistMax.Z = (float)Math.Abs(vDistMax.Z);

            // Start off assuming that our intersection point is on the  
            // negative x face of the bounding box.  
            a_nFace = 0;
            float fMinDist = vDistMin.X;

            // +X  
            if (vDistMax.X < fMinDist)
            {
                a_nFace = 1;
                fMinDist = vDistMax.X;
            }

            // -Y  
            if (vDistMin.Y < fMinDist)
            {
                a_nFace = 2;
                fMinDist = vDistMin.Y;
            }

            // +Y  
            if (vDistMax.Y < fMinDist)
            {
                a_nFace = 3;
                fMinDist = vDistMax.Y;
            }

            // -Z  
            if (vDistMin.Z < fMinDist)
            {
                a_nFace = 4;
                fMinDist = vDistMin.Z;
            }

            // +Z  
            if (vDistMax.Z < fMinDist)
            {
                a_nFace = 5;
                fMinDist = vDistMin.Z;
            }

            return true;
        }


    }
}
