using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    class KinematicComponent : Component
    {
        public Vector3 position;
        public float maxAcceleration;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            position = new Vector3(0, 0, 0);
            maxAcceleration = 0;
        }

        public KinematicComponent(Vector3 pos, float maxAccel)
            : base((int)component_flags.kinematic)
        {
            position = pos;
            maxAcceleration = maxAccel;
        }
    }
}
