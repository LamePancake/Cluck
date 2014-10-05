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

        public AISystem() : base((int)component_flags.aiThinking)
        {
            steeringBehaviours = new SteeringBehaviours();
        }

        public void Update(List<GameEntity> world, float deltaTime)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    AIThinking thinking = entity.GetComponent<AIThinking>();
                    thinking.Update();
                }

                if (entity.HasComponent((int)component_flags.kinematic) && entity.HasComponent((int)component_flags.aiSteering))
                {
                    Console.WriteLine("Booo");
                    KinematicComponent kinematics = entity.GetComponent <KinematicComponent>();

                    SteeringComponent steering = entity.GetComponent<SteeringComponent>();

                    SteeringOutput output = steeringBehaviours.Seek(steering.GetTarget(), kinematics);

                    kinematics.velocity += (output.linear * deltaTime);

                    Vector3 vel = kinematics.velocity * deltaTime;

                    if (vel.Length() > kinematics.maxSpeed)
                    {
                        vel.Normalize();
                        vel *= kinematics.maxSpeed;
                    }

                    kinematics.velocity = vel;

                    kinematics.position += kinematics.velocity;


                }
            }
        }
    }
}
