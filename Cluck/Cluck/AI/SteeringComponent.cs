using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SteeringComponent : Component
    {

        SteeringBehaviours steeringBehaviours;
        private PositionComponent target;
        public Vector3 wanderTarget;
        public float wanderOffset = 900;
        public float wanderRadius = 50;
        public float wanderJitter = 3.5f;
        private bool wanderOn;
        private bool fleeOn;

        public SteeringComponent(PositionComponent targetPos) : base((int)component_flags.aiSteering)
        {
            wanderOn = true;
            fleeOn = false;

            target = targetPos;

            Random rando = new Random();

            double theta = rando.NextDouble() * (2*Math.PI);

            wanderTarget = new Vector3((float)(wanderRadius * Math.Cos(theta)), 0, (float)(wanderRadius * Math.Sin(theta)));

            steeringBehaviours = new SteeringBehaviours();
        }

        public SteeringOutput Calculate(PositionComponent position, KinematicComponent kinematics, float deltaTime, Vector3 playerPos)
        {
            SteeringOutput steeringOut = new SteeringOutput();

            if (wanderOn)
            {
                steeringOut = steeringBehaviours.Wander(position, kinematics, this, deltaTime);
            }

            if (fleeOn)
            {
                steeringOut = steeringBehaviours.Flee(position, kinematics, playerPos);
            }

            return steeringOut;
        }

        public Vector3 GetTarget()
        {
            return target.GetPosition();
        }

        public void SetWander(bool state)
        {
            wanderOn = state;
        }

        public void SetFlee(bool state)
        {
            fleeOn = state;
        }
    }
}
