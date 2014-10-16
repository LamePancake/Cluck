using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    interface State
    {
        void Enter(AIThinking component, GameEntity entity);
        void Execute(AIThinking component, GameEntity entity, GameTime deltaTime);
        void Exit(AIThinking component, GameEntity entity);
    }
}
