using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cluck
{
    class ArmComponent : Component
    {
        //False = left arm
        //True = right arm
        bool whichArm;

        /// <summary>
        /// Used to conform to the new() constraint in GetComponent<T>(). Don't instantiate them this way!
        /// </summary>
        public ArmComponent()
            : base((int)component_flags.arm)
        { }

        public ArmComponent(bool arm)
            : base((int)component_flags.arm)
        {
            whichArm = arm;
        }

        public bool WhichArm() { return whichArm; }
    }
}
