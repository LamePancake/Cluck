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

        public SteeringOutput Seek(Vector3 target, KinematicComponent agent)
	    {
		    SteeringOutput steering = new SteeringOutput();

            Vector3 toTarget = target - agent.position;

		    steering.linear = toTarget;

		    steering.linear.Normalize();
            steering.linear = steering.linear * agent.maxAcceleration;

		    steering.angular = 0;

		    return steering;
	    }
    }
}
