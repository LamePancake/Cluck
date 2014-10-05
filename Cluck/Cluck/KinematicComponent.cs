using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    class KinematicComponent : Component
    {
        public float maxAcceleration;
        public Vector3 velocity;
        public float maxSpeed;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            maxAcceleration = 0;
        }

        public KinematicComponent(float maxAccel, float maximumSpeed)
            : base((int)component_flags.kinematic)
        {
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            velocity = Vector3.One;
        }
    }
}
