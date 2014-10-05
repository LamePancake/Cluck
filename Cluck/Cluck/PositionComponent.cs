using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Cluck
{
    class PositionComponent : Component
    {
        private Vector3 pos;
        private Vector3 orient;

        public PositionComponent(Vector3 position, Vector3 orientation) : base((int)component_flags.position)
        {
            pos = position;
            orient = orientation;
        }

        public Vector3 getPosition()
        {
            return pos;
        }

        public Vector3 getOrientation()
        {
            return orient;
        }

        public void setPosition(Vector3 newPos)
        {
            if (newPos == null)
                throw new ArgumentNullException("PlayerComponent.setPosition: newPos cannot be null");
            pos = newPos;
        }

        //public void setOrientation(Vector3 3)

        
    }
}
