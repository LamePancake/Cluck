﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck
{
    class KinematicComponent : Component
    {
        public float maxAcceleration;
        public float maxSpeed;
        public float maxAngularAcceleration;
        public float maxRotation;

        public float rotation;
        public Vector3 velocity;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            maxAcceleration = 0;
        }

        public KinematicComponent(float maxAccel, float maximumSpeed, float maximumRotation, float maxAngularAccel)
            : base((int)component_flags.kinematic)
        {
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            maxRotation = maximumRotation;
            maxAngularAcceleration = maxAngularAccel;
            
            velocity = Vector3.One;
            rotation = 0;
        }
    }
}
