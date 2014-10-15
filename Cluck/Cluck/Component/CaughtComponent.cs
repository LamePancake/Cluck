using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    class CaughtComponent : Component
    {
        public CaughtComponent()
            : base((int)component_flags.caught)
        { }
    }
}
