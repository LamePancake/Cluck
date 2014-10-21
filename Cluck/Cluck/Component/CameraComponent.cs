using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class CameraComponent : Component
    {
        FirstPersonCamera camera;

        /// <summary>
        /// Used to conform to the new() constraint in GetComponent<T>(). Don't instantiate them this way!
        /// </summary>
        public CameraComponent() : base((int)component_flags.camera) { }

        public CameraComponent(FirstPersonCamera cam)
            :base((int)component_flags.camera)
        {
            camera = cam;
        }
    }
}
