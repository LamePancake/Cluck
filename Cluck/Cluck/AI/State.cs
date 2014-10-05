using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    interface State
    {
        void Enter(AIThinking component, GameEntity entity);
        void Execute(AIThinking component, GameEntity entity);
        void Exit(AIThinking component, GameEntity entity);
    }
}
