using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class FreeComponent : Component
    {
        public FreeComponent()
            : base((int)component_flags.free)
        { }
    }
}
