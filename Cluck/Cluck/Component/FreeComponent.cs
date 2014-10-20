using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    class FreeComponent : Component
    {
        public FreeComponent()
            : base((int)component_flags.free)
        { }
    }
}
