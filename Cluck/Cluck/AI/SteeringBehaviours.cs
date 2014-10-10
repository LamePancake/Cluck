using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using Cluck.Debug;

namespace Cluck.AI
{
    class SteeringOutput
    {
        public Vector3 linear;
        public float angular;
    }

    class SteeringBehaviours
    {
        private Random randomGen;

        public SteeringBehaviours()
        {
            randomGen = new Random();
        }

        private double RandomClamped()
        {
            return (randomGen.NextDouble() - randomGen.NextDouble());
        }

        public SteeringOutput Seek(Vector3 target, Vector3 agentPos, KinematicComponent agent)
	    {
		    SteeringOutput steering = new SteeringOutput();

            Vector3 toTarget = target - agentPos;

		    steering.linear = toTarget;

		    steering.linear.Normalize();
            steering.linear = steering.linear * agent.maxAcceleration;

		    steering.angular = 0;

		    return steering;
	    }

        public SteeringOutput Wander(PositionComponent agentPos, KinematicComponent agentkinematic, SteeringComponent agentSteering, double timeElapsed)
        {
            SteeringOutput steering = new SteeringOutput();

            double jitterThisFrame = agentSteering.wanderJitter * timeElapsed;

            agentSteering.wanderTarget += new Vector3((float)(RandomClamped() * jitterThisFrame), 0, (float)(RandomClamped() * jitterThisFrame));

	        agentSteering.wanderTarget.Normalize();

	        agentSteering.wanderTarget *= agentSteering.wanderRadius;

            Vector3 targetLocal = agentSteering.wanderTarget + new Vector3(agentSteering.wanderOffset, 0, 0);

            Vector3 targetWorld = Util.PointToWorldSpace(targetLocal, agentkinematic.heading, agentkinematic.side, agentPos.GetPosition());

            steering = Seek(targetWorld, agentPos.GetPosition(), agentkinematic);

            return steering;
        }
        
        public SteeringOutput Align(float targetOrientation, PositionComponent agentPos, KinematicComponent agentKinematic)
	    {
            float targetRadius = 0.15f;
            float slowRadius = (float)Math.PI/2;

		    float timeToTarget = 1f;

		    SteeringOutput steering = new SteeringOutput();
            float rotation = targetOrientation - agentPos.GetOrientation();

            //Map to range -pi, pi
            float num2pi = (float)Math.Floor(rotation / (2 * Math.PI) + 0.5);
            rotation = (float)(rotation - num2pi * (2 * Math.PI));

            if (rotation > Math.PI)
            {
                rotation -= (float)(2*Math.PI);
            }
            else if (rotation < -Math.PI)
            {
                rotation += (float)(2 * Math.PI);
            }

		    var rotationSize = Math.Abs(rotation);

		    float targetRotation;

		    if (rotationSize < targetRadius)
		    {
			    return steering;
		    }
		    
            if (rotationSize > slowRadius)
		    {
                targetRotation = agentKinematic.maxRotation;
		    }
		    else
		    {
                targetRotation = (agentKinematic.maxRotation * rotationSize) / slowRadius;
		    }

		    targetRotation *= (rotation / rotationSize);

            steering.angular = targetRotation - agentKinematic.rotation;

		    steering.angular /= timeToTarget;

		    float angularAcceleration = Math.Abs(steering.angular);

            if (angularAcceleration > agentKinematic.maxAngularAcceleration)
		    {
			    steering.angular /= angularAcceleration;
                steering.angular *= agentKinematic.maxAngularAcceleration;
		    }

		    steering.linear = new Vector3(0,0,0);

		    return steering;
	    }

        public SteeringOutput Face(Vector3 target, PositionComponent agentPos)
        {
            float facingOffset = (float)Math.PI/2;

            Vector3 direction = target - agentPos.GetPosition();
            
            var steering = new SteeringOutput();

            if (direction.Length() == 0)
            {
                return steering;
            }

            float targetOrientation = (float)Math.Atan2(-direction.Z, direction.X) + facingOffset;

            Console.WriteLine(targetOrientation);

            steering.angular = targetOrientation;

            steering.linear = new Vector3(0,0,0);

            return steering;
        }
    }
}
