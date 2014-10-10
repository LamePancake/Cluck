using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SensoryMemoryComponent : Component
    {
        private double FOV = Math.PI; // 180 degrees
        private double rangeOfSight = 1000;

        public SensoryMemoryComponent()
            : base((int)component_flags.sensory)
        {

        }

        public bool WithinView(Vector3 entityPos, Vector3 entityHeading, Vector3 otherEntityPos)
        {
            Vector3 toTarget = otherEntityPos - entityPos;

            Console.WriteLine("Dist " + toTarget.Length());

            if (toTarget.Length() < rangeOfSight)
            {
                toTarget.Normalize();

                return (Vector3.Dot(entityHeading, (toTarget)) >= Math.Cos(FOV / 2.0));
            }

            return false;
        }
    }
}
