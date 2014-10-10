using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    enum component_flags
    {
        none = 0x00000,
        aiThinking = 0x00002,
        renderable = 0x00004,
        kinematic = 0x00008,
        aiSteering = 0x00010,
        position = 0x00020,
        collidable = 0x00040,
        player = 0x00080,
        /*
         = 0x00100,
         = 0x00200,
         = 0x00400,
         = 0x00800,
         = 0x01000,
         = 0x02000,
         = 0x04000,
         = 0x08000,
         = 0x10000,*/
    };
}
