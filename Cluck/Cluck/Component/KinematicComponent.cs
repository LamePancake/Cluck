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
        public Vector3 velocity;
        public float rotation;
        public float maxSpeed;
        public float maxRotation;
        public float maxAngularAcceleration;

        public KinematicComponent() : base((int)component_flags.kinematic)
        {
            maxAcceleration = 0;
        }

        public KinematicComponent(float maxAccel, float maximumSpeed, float maximumRotation, float maximumAngualrAccel)
            : base((int)component_flags.kinematic)
        {
            maxAcceleration = maxAccel;
            maxSpeed = maximumSpeed;
            maxAngularAcceleration = maximumAngualrAccel;
            maxRotation = maximumRotation;
            velocity = Vector3.One;
        }
    }
}
