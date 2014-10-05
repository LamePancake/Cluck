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

        public void Update(List<GameEntity> world)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    AIThinking thinking = (AIThinking)entity.GetComponent((int)component_flags.aiThinking);
                    thinking.Update();
                }

                if (entity.HasComponent((int)component_flags.kinematic))
                {
                    KinematicComponent kinematics = (KinematicComponent)entity.GetComponent((int)component_flags.kinematic);

                    Vector3 target = new Vector3(0,0,50);

                    SteeringOutput output = steeringBehaviours.Seek(target, kinematics);


                }
            }
        }
    }
}
