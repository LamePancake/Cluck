using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck.AI
{
    class AISystem : GameSystem
    {

        public AISystem(Game game) : base(game, (int)component_flags.aiThinking)
        {

        }

        public void Update(List<GameEntity> world)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    Console.WriteLine("YUP");
                }
            }
        }

        public void UpdateWorld(List<GameEntity> world)
        {
            foreach (GameEntity entity in world)
            {
                if (entity.HasComponent(myFlag))
                {
                    Console.WriteLine("YUP");
                }
            }
        }
    }
}
