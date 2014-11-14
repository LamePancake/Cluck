using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class KinematicComponent : Component
    {
        public float maxAcceleration;
        public float maxRunSpeed;
        public float maxWalkSpeed;
        public float maxFlySpeed;
        public float maxSpeed;
        public float maxAngularAcceleration;
        public float maxRotation;

        public float rotation;
        public Vector3 velocity;

        public Vector3 heading;
        public Vector3 side;

        public KinematicComponent(float maxAccel, float maximumSpeed, float maximumRotation, float maxAngularAccel) 
            : base((int)component_flags.kinematic)
        {
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            maxRotation = maximumRotation;
            maxAngularAcceleration = maxAngularAccel;

            maxWalkSpeed = maximumSpeed;
            maxRunSpeed = maximumSpeed * 2.5f;
            maxFlySpeed = maxRunSpeed;

            velocity = Vector3.Zero;
            heading = new Vector3(1,0,0);
            side = Util.PerpInZPlane(heading);
            rotation = 0;
        }
    }
}
