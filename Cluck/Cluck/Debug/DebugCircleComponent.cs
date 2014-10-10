using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;

namespace Cluck.Debug
{
    class DebugCircleComponent : Component
    {
        public const int NumGroups = 5;

        BoundingSphere primarySphere;
        BoundingSphere[] secondarySpheres = new BoundingSphere[NumGroups];

        public DebugCircleComponent()
            : base((int)component_flags.debugCircle)
        {
            primarySphere = new BoundingSphere();
            primarySphere.Center = new Vector3(0, 0, 0);
            primarySphere.Radius = 5;
        }

        public void SetCenter(Vector3 center)
        {
            primarySphere.Center = center;
        }

        public void SetRadius(int rad)
        {
            primarySphere.Radius = rad;
        }

        public BoundingSphere GetBounding()
        {
            return primarySphere;
        }
    }
}
