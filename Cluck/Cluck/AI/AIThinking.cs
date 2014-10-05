using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class AIThinking : Component
    {
        private State currentState;
        private GameEntity myOwner;

        public AIThinking(GameEntity owner, State initialState)
            : base((int)component_flags.aiThinking)
        {
            currentState = initialState;
            myOwner = owner;
        }

        public void ChangeStates(State newState)
        {
            currentState.Exit(this, myOwner);

            currentState = newState;

            currentState.Enter(this, myOwner);
        }

        public void Update()
        {
            currentState.Execute(this, myOwner);
        }
    }
}
