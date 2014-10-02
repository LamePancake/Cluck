using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class GameSystem : GameComponent
    {
        protected int myFlag;

        public GameSystem(Game game, int flag) : base(game)
        {
            myFlag = flag;
        }
    }
}
