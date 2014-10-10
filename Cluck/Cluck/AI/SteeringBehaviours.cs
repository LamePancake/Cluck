using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SteeringOutput
    {
        public Vector3 linear;
        public float angular;
    }

    class SteeringBehaviours
    {

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

        public SteeringOutput Align(float targetOrientation, PositionComponent agentPos, KinematicComponent agentKinematic)
	    {
		    float targetRadius = 10;
		    float slowRadius = 30;

		    float timeToTarget = 0.1f;

		    SteeringOutput steering = new SteeringOutput();
            float rotation = targetOrientation - agentPos.GetOrientation();

            //Map to range -pi, pi
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
		    else if (rotationSize > slowRadius)
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

		    var angularAcceleration = Math.Abs(steering.angular);

            if (angularAcceleration > agentKinematic.maxAngularAcceleration)
		    {
			    steering.angular /= angularAcceleration;
                steering.angular *= agentKinematic.maxAngularAcceleration;
		    }

		    steering.linear = new Vector3(0,0,0);

		    return steering;
	    }
    }
}
