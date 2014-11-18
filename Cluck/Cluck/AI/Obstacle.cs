using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class Obstacle
    {
        public Obstacle(Vector3 pos, int rad)
        {
            position = pos;
            radius = rad;
            tagged = false;
        }

        public Vector3 position;
        public int radius;
        public bool tagged;
    }
}
