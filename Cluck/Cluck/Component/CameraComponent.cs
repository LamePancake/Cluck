using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class CameraComponent : Component
    {
        FirstPersonCamera camera;

        public CameraComponent(FirstPersonCamera cam)
            :base((int)component_flags.camera)
        {
            camera = cam;
        }
    }
}
