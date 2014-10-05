using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class KinematicComponent : Component
    {
        public Vector3 position;
        public float maxAcceleration;
        public Vector3 velocity;
        public float maxSpeed;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            position = new Vector3(0, 0, 0);
            maxAcceleration = 0;
        }

        public KinematicComponent(Vector3 pos, float maxAccel, float maximumSpeed)
            : base((int)component_flags.kinematic)
        {
            position = pos;
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            velocity = Vector3.One;
        }
    }
}
