using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class FenceComponent : Component
    {
        public FenceComponent()
            : base((int)component_flags.fence)
        {

        }
    }
}
