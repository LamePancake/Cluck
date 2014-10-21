using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class AIThinking : Component
    {
        private State currentState;
        private GameEntity myOwner;

        /// <summary>
        /// Used to conform to the new() constraint in GetComponent<T>(). Don't instantiate them this way!
        /// </summary>
        public AIThinking() : base((int)component_flags.aiThinking) { }

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

        public void Update(GameTime time)
        {
            currentState.Execute(this, myOwner, time);
        }
    }
}
