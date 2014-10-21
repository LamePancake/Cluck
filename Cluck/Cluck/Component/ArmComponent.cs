using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    public class ArmComponent : Component
    {
        //False = left arm
        //True = right arm
        bool whichArm;

        public ArmComponent(bool arm)
            : base((int)component_flags.arm)
        {
            whichArm = arm;
        }

        public bool WhichArm() { return whichArm; }
    }
}
