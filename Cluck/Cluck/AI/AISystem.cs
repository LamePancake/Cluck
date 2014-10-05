using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck.AI
{
    class AISystem : GameSystem
    {

        public AISystem() : base((int)component_flags.aiThinking)
        {

        }

        public void Update(List<GameEntity> world)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    AIThinking thinking = (AIThinking)entity.GetComponent(myFlag);
                    thinking.Update();
                }
            }
        }
    }
}
