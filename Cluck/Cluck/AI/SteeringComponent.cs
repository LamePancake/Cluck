using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.AI
{
    class SteeringComponent : Component
    {
        private PositionComponent target;

        public SteeringComponent(PositionComponent targetPos) : base((int)component_flags.aiSteering)
        {
            target = targetPos;
        }

        public Vector3 GetTarget()
        {
            return target.GetPosition();
        }
    }
}
