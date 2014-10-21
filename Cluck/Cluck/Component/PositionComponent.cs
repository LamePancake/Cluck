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
        private float orient;

        /// <summary>
        /// Used to conform to the new() constraint in GetComponent<T>(). Don't instantiate them this way!
        /// </summary>
        public PositionComponent() : base((int)component_flags.position) { }
        public PositionComponent(Vector3 position, float orientation) : base((int)component_flags.position)
        {
            pos = position;
            orient = orientation;
        }

        public Vector3 GetPosition()
        {
            return pos;
        }

        public float GetOrientation()
        {
            return orient;
        }

        public void SetPosition(Vector3 newPos)
        {
            if (newPos == null)
                throw new ArgumentNullException("PlayerComponent.setPosition: newPos cannot be null");
            pos = newPos;
        }

        public void SetOrientation(float orientation)
        {
            orient = orientation;
        }
    }
}
