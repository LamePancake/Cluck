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
        public float maxSpeed;
        public float maxAngularAcceleration;
        public float maxRotation;

        public float rotation;
        public Vector3 velocity;

        public Vector3 heading;
        public Vector3 side;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            position = new Vector3(0, 0, 0);
            maxAcceleration = 0;
        }

        public KinematicComponent(Vector3 pos, float maxAccel, float maximumSpeed, float maximumRotation, float maxAngularAccel) 
            : base((int)component_flags.kinematic)
        {
            position = pos;
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            maxRotation = maximumRotation;
            maxAngularAcceleration = maxAngularAccel;
            
            velocity = Vector3.Zero;
            heading = new Vector3(1,0,0);
            side = Util.PerpInZPlane(heading);
            rotation = 0;
        }
    }
}
