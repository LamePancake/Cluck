using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck.AI
{
    class AISystem : GameSystem
    {

        SteeringBehaviours steeringBehaviours;
        List<GameEntity> walls;
        public AISystem() : base((int)component_flags.aiThinking) { }
        public AISystem(List<GameEntity> world) : base((int)component_flags.aiThinking)
        {
            steeringBehaviours = new SteeringBehaviours();

            walls = new List<GameEntity>();

            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent((int)component_flags.renderable) && entity.HasComponent((int)component_flags.position))
                {
                    walls.Add(entity);
                }
            }
        }

        public void Update(List<GameEntity> world, GameTime deltaTime, Vector3 playerPos)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    AIThinking thinking = entity.GetComponent<AIThinking>(component_flags.aiThinking);
                    thinking.Update(deltaTime);
                }

                if (entity.HasComponent((int)component_flags.kinematic) 
                    && entity.HasComponent((int)component_flags.aiSteering)
                    && entity.HasComponent((int)component_flags.position)
                    && entity.HasComponent((int)component_flags.sensory))
                {
                    KinematicComponent kinematics = entity.GetComponent <KinematicComponent>(component_flags.kinematic);

                    SteeringComponent steering = entity.GetComponent<SteeringComponent>(component_flags.aiSteering);

                    PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);

                    SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>(component_flags.sensory);

                    // this is a hack!! Here be dragons.
                    //if (sensory.WithinView(position.GetPosition(), kinematics.velocity, playerPos))
                    //{
                    //    sensory.PlayerSpotted(true);
                    //}
                    //else
                    //{
                    //    sensory.PlayerSpotted(false);
                    //}

                    SteeringOutput output = steering.Calculate(walls, position, kinematics, deltaTime.ElapsedGameTime.Milliseconds);
                    //SteeringOutput output = steeringBehaviours.Seek(position, kinematics);

                    // update velocity and rotation
                    kinematics.velocity += (output.linear * deltaTime.ElapsedGameTime.Milliseconds);
                    kinematics.rotation += (output.angular * deltaTime.ElapsedGameTime.Milliseconds);
                    
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

                    Vector3 newPos = position.GetPosition() + kinematics.velocity;

                    //Prevent falling through map
                    //if (newPos.Y < 0)
                    //{
                    //    newPos.Y = 0;
                    //}

                    // Update position and orientation
                    position.SetPosition(newPos);
                    //position.SetOrientation(position.GetOrientation() + kinematics.rotation);

                    SteeringOutput facingDirection = steeringBehaviours.Face(position.GetPosition() + kinematics.velocity, position);
                    position.SetOrientation(facingDirection.angular);

                }

                if (entity.HasComponent((int)component_flags.sensory) && entity.HasComponent((int)component_flags.position) && entity.HasComponent((int)component_flags.kinematic))
                {
                    SensoryMemoryComponent sensory = entity.GetComponent<SensoryMemoryComponent>(component_flags.sensory);

                    PositionComponent position = entity.GetComponent<PositionComponent>(component_flags.position);

                    KinematicComponent kinematics = entity.GetComponent <KinematicComponent>(component_flags.kinematic);

                    sensory.UpdateSenses(world, deltaTime);

                    //Console.WriteLine("in sight: " + sensory.WithinView(position.GetPosition(), kinematics.heading, playerPos));
                }
            }
        }
    }
}
