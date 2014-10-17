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

        public SteeringBehaviours()
        {
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

            agentSteering.wanderTarget += new Vector3((float)(Util.RandomClamped() * jitterThisFrame), 0, (float)(Util.RandomClamped() * jitterThisFrame));

	        agentSteering.wanderTarget.Normalize();

	        agentSteering.wanderTarget *= agentSteering.wanderRadius;

            Vector3 targetLocal = agentSteering.wanderTarget + new Vector3(agentSteering.wanderOffset, 0, 0);

            Vector3 targetWorld = Util.PointToWorldSpace(targetLocal, agentkinematic.heading, agentkinematic.side, agentPos.GetPosition());

            steering = Seek(targetWorld, agentPos.GetPosition(), agentkinematic);

            return steering;
        }

        private void CreateFeelers(PositionComponent agentPos, KinematicComponent agentkinematic, SteeringComponent agentSteering)
        {
            agentSteering.feelers.Clear();

            //feeler pointing straight in front
            agentSteering.feelers.Add(agentPos.GetPosition() + (agentSteering.feelerLength * agentkinematic.heading));

            //feeler to left
            Vector3 temp = agentkinematic.heading;
            temp = Util.Vec3RotateAroundOrigin(temp, (float)((Math.PI / 2) * 3.5f));
            agentSteering.feelers.Add(agentPos.GetPosition() + (agentSteering.feelerLength / 2.0f * temp));

            //feeler to right
            temp = agentkinematic.heading;
            temp = Util.Vec3RotateAroundOrigin(temp, (float)(Math.PI / 2) * 0.5f);
            agentSteering.feelers.Add(agentPos.GetPosition() + (agentSteering.feelerLength / 2.0f * temp));
        }

        public SteeringOutput WallAvoidance(List<GameEntity> walls, PositionComponent agentPos, KinematicComponent agentkinematic, SteeringComponent agentSteering)
        {
            SteeringOutput steering = new SteeringOutput();

          CreateFeelers(agentPos, agentkinematic, agentSteering);
          
          float distToThisIP    = 0.0f;
          float distToClosestIP = float.MaxValue;

          int ClosestWall = -1;

          Vector3 ClosestPoint;
          int IntersectedFace = -1;

          for (int wisker = 0; wisker < agentSteering.feelers.Count; ++wisker)
          {
              //agentSteering.feelers[wisker] = agentSteering.feelers[wisker] / agentSteering.feelers[wisker].Length();
              Ray wiskerRay = new Ray(agentPos.GetPosition(), agentSteering.feelers[wisker]);
              //
              //wiskerRay.Direction = agentSteering.feelers[wisker];
              //Console.WriteLine("Dir: " + wiskerRay.Direction);
              //wiskerRay.Position = agentPos.GetPosition();
              //Console.WriteLine("Pos: " + wiskerRay.Position);

            for (int wall = 0; wall < walls.Count; ++wall)
            {
                if (walls[wall].HasComponent((int)component_flags.renderable) && walls[wall].HasComponent((int)component_flags.fence))
                {
                    BoundingBox box = walls[wall].GetComponent<Renderable>().GetBoundingBox();
                    int face = -1;
                    Util.IntersectRayVsBox(box, wiskerRay,out distToThisIP,out face);

                    if (distToThisIP < distToClosestIP)
                    {
                        Console.WriteLine("Interrrrsectiionnn1 " + agentPos.GetPosition());
                        distToClosestIP = distToThisIP;

                        ClosestWall = wall;

                        IntersectedFace = face;

                        Console.WriteLine("Interrrrsectiionnn Face: " + IntersectedFace);

                        //ClosestPoint = point;
                    }
                }
            }

  
            if (ClosestWall >= 0)
            {
                //Vector3 OverShoot = m_Feelers[wisker] - ClosestPoint;
	  
                //SteeringForce = walls[ClosestWall].Normal() * OverShoot.Length();

            }

          }

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

        public SteeringOutput Flee(PositionComponent agentPos, KinematicComponent agentKinematic, Vector3 scaryPos)
        {
            SteeringOutput steering = new SteeringOutput();
	        const double panicDistanceSq = 1000.0;
            Vector3 agentPosition = agentPos.GetPosition();

            scaryPos.Y = agentPosition.Y; // don't have chickens go through the ground.

            Vector3 awayFromScary = agentPosition - scaryPos;

            steering.linear = awayFromScary;

            if (awayFromScary.Length() > panicDistanceSq)
	        {
                steering.linear = new Vector3(0, 0, 0);
                
                return steering;
	        }

            steering.linear.Normalize();

            steering.linear = steering.linear * agentKinematic.maxAcceleration;

            steering.angular = 0;

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

            steering.angular = targetOrientation;

            steering.linear = new Vector3(0,0,0);

            return steering;
        }
    }
}
