using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Cluck.Debug;

namespace Cluck.AI
{
    class AISystem : GameSystem
    {
        SteeringBehaviours steeringBehaviours;

        public AISystem() : base((int)component_flags.aiThinking)
        {
            steeringBehaviours = new SteeringBehaviours();
        }

        public void Update(List<GameEntity> world, float deltaTime, Vector3 playerPos)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    AIThinking thinking = entity.GetComponent<AIThinking>();
                    thinking.Update();
                }

                if (entity.HasComponent((int)component_flags.kinematic) 
                    && entity.HasComponent((int)component_flags.aiSteering)
                    && entity.HasComponent((int)component_flags.position))
                {
                    KinematicComponent kinematics = entity.GetComponent <KinematicComponent>();

                    SteeringComponent steering = entity.GetComponent<SteeringComponent>();

                    PositionComponent position = entity.GetComponent<PositionComponent>();
                    
                    SteeringOutput output = steeringBehaviours.Wander(position, kinematics, steering, deltaTime);

                    // update velocity and rotation
                    kinematics.velocity += (output.linear * deltaTime);
                    kinematics.rotation += (output.angular * deltaTime);
                    
                    // clamp rotation
                    float rot = kinematics.rotation;

                    float targetRotation = Math.Abs(rot);

                    if (targetRotation > kinematics.maxRotation)
                    {
                        rot /= targetRotation;
                        rot *= kinematics.maxRotation;

                        kinematics.rotation = rot;
                    }

                    // clamp velocity
                    Vector3 vel = kinematics.velocity;
                    
                    if (vel.Length() > kinematics.maxSpeed)
                    {
                        vel.Normalize();
                        vel *= kinematics.maxSpeed;

                        kinematics.velocity = vel;
                    }

                    if (vel.LengthSquared() > 0.0001)
                    {
                        Vector3 temp = kinematics.velocity;
                        temp.Normalize();
                        kinematics.heading = temp;
                        kinematics.side = Util.PerpInZPlane(kinematics.heading);
                    }
                    
                    // Update position and orientation
                    position.SetPosition(position.GetPosition() + kinematics.velocity);
                    //position.SetOrientation(position.GetOrientation() + kinematics.rotation);

                    SteeringOutput facingDirection = steeringBehaviours.Face(position.GetPosition() + kinematics.velocity, position);
                    position.SetOrientation(facingDirection.angular);

                }

                if (entity.HasComponent((int)component_flags.sensory) && entity.HasComponent((int)component_flags.position) && entity.HasComponent((int)component_flags.kinematic))
                {
                    SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>();

                    PositionComponent position = entity.GetComponent<PositionComponent>();

                    KinematicComponent kinematics = entity.GetComponent <KinematicComponent>();

                    Console.WriteLine("in sight: " + sensory.WithinView(position.GetPosition(), kinematics.heading, playerPos));
                }
            }
        }
    }
}
