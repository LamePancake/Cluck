using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SteeringComponent : Component
    {
        private PositionComponent target;
        public Vector2 wanderTarget;
        public float wanderOffset = 600;
        public float wanderRadius = 400;
        public float wanderJitter = 20;

        public SteeringComponent(PositionComponent targetPos) : base((int)component_flags.aiSteering)
        {
            target = targetPos;

            Random rando = new Random();

            double theta = rando.NextDouble() * (2*Math.PI);

            wanderTarget = new Vector2((float)(wanderRadius * Math.Cos(theta)),(float)(wanderRadius * Math.Sin(theta)));
        }

        public Vector3 GetTarget()
        {
            return target.GetPosition();
        }
    }
}
