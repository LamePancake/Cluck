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
        public Vector3 wanderTarget;
        public float wanderOffset = 900;
        public float wanderRadius = 50;
        public float wanderJitter = 3.5f;

        public SteeringComponent(PositionComponent targetPos) : base((int)component_flags.aiSteering)
        {
            target = targetPos;

            Random rando = new Random();

            double theta = rando.NextDouble() * (2*Math.PI);

            wanderTarget = new Vector3((float)(wanderRadius * Math.Cos(theta)), 0, (float)(wanderRadius * Math.Sin(theta)));
        }

        public Vector3 GetTarget()
        {
            return target.GetPosition();
        }
    }
}
