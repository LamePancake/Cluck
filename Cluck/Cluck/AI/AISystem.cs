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
        List<Obstacle> obstacles;
        public AISystem() : base((int)component_flags.aiThinking) { }
        public AISystem(List<GameEntity> world, List<Obstacle> obs) : base((int)component_flags.aiThinking)
        {
            steeringBehaviours = new SteeringBehaviours();

            walls = new List<GameEntity>();
            obstacles = obs;

            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent((int)component_flags.fence) && entity.HasComponent((int)component_flags.renderable) && entity.HasComponent((int)component_flags.position))
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

                    // If the chicken is in the air, we don't want to apply any AI behaviours
                    if (!kinematics.IsGrounded)
                        continue;

                    // this is a hack!! Here be dragons.
                    //if (sensory.WithinView(position.GetPosition(), kinematics.velocity, playerPos))
                    //{
                    //    sensory.PlayerSpotted(true);
                    //}
                    //else
                    //{
                    //    sensory.PlayerSpotted(false);
                    //}

                    SteeringOutput output = steering.Calculate(obstacles, walls, position, kinematics, deltaTime.ElapsedGameTime.Milliseconds);
                    //SteeringOutput output = steeringBehaviours.Seek(position, kinematics);

                    // update velocity and rotation
                    kinematics.Velocity += (output.linear * deltaTime.ElapsedGameTime.Milliseconds);
                    kinematics.Rotation += (output.angular * deltaTime.ElapsedGameTime.Milliseconds);
                    
                    // clamp rotation
                    float rot = kinematics.Rotation;

                    float targetRotation = Math.Abs(rot);

                    if (targetRotation > kinematics.MaxRotation)
                    {
                        rot /= targetRotation;
                        rot *= kinematics.MaxRotation;

                        kinematics.Rotation = rot;
                    }

                    // clamp velocity
                    Vector3 vel = kinematics.Velocity;
                    
                    if (vel.Length() > kinematics.MaxSpeed)
                    {
                        vel.Normalize();
                        vel *= kinematics.MaxSpeed;

                        kinematics.Velocity = vel;
                    }

                    if (vel.LengthSquared() > 0.0001)
                    {
                        Vector3 temp = kinematics.Velocity;
                        //temp = Matrix.CreateRotationY(position.GetOrientation()).Forward;
                        temp.Normalize();
                        kinematics.Heading = temp;
                        kinematics.Side = Util.PerpInZPlane(kinematics.Heading);
                    }

                    //if (steering.flying || position.GetPosition().Y > 0) // apply gravity sort of
                    //{
                    //    kinematics.velocity.Y -= (float)(deltaTime.ElapsedGameTime.TotalSeconds)/4;
                    //}

                    Vector3 newPos = position.GetPosition() + kinematics.Velocity;
                    
                    // Update position and orientation
                    position.SetPosition(newPos);
                    //position.SetOrientation(position.GetOrientation() + kinematics.rotation);

                    SteeringOutput facingDirection = steeringBehaviours.Face(position.GetPosition() + kinematics.Velocity, position);
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
