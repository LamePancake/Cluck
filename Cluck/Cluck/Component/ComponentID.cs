using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public enum component_flags
    {
        none = 0x00000,
        aiThinking = 0x00002,
        renderable = 0x00004,
        kinematic = 0x00008,
        aiSteering = 0x00010,
        position = 0x00020,
        collidable = 0x00040,
        sensory = 0x00100,
        arm = 0x00200,
        caught = 0x00080,
        camera = 0x00400,
        capture = 0x00800,
        fence = 0x01000,
        audioEmitter = 0x02000,
        free = 0x04000,
        audioListener = 0x08000,
        /*
         = 0x10000,*/
    };
}
