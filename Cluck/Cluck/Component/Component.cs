using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class Component
    {
        private int myFlag;

        public Component(int flag)
        {
            myFlag = flag;
        }

        public int GetFlag()
        {
            return myFlag;
        }
    }
}
