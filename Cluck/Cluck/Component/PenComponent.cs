using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class PenComponent : Component
    {
        public PenComponent()
            : base((int)component_flags.pen)
        { }
    }
}
