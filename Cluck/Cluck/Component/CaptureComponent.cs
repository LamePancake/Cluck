﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class CaptureComponent : Component
    {
        public CaptureComponent()
            : base((int)component_flags.capture)
        { }
    }
}
