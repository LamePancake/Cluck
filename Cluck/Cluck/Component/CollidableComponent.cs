﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class CollidableComponent : Component
    {
        public CollidableComponent()
            : base((int)component_flags.collidable)
        { }
    }
}
