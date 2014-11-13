#region File Description
//-----------------------------------------------------------------------------
// InstructionScreen.cs
//
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Cluck
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class InstructionScreen : MenuScreen
    {
        #region Fields
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public InstructionScreen(int stage)
            : base("")
        {
            MenuEntry nextInstructionMenuEntry;

            if (stage == 0)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    ChangeTitle("Movement\n\nUse Left Analog stick to move. Use the Right Analog stick to look around.\n");
                }
                else
                {
                    ChangeTitle("Movement Part I\n\nUse W A S D to move. Use the Mouse to look around.\n");
                }
            }
            else if (stage == 1)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    ChangeTitle("\n\n\n\nMovement Part II\n\nPress down on the Left Analog stick to sprint.\n Press LB button to crouch.\nSprinting decreases your stamina bar on the left.\n"
                        + "If the stamina bar is empty you have to wait \nuntil it recharges before you can sprint again.\n");
                }
                else
                {
                    ChangeTitle("\n\n\n\nMovement Part II\n\nUse Left Shift to sprint. Press C to crouch.\nSprinting decreases your stamina bar on the left.\n"
                        + "If the stamina bar is empty you have to \nwait until it recharges before you can sprint again.\n");
                }
            }
            else if (stage == 2)
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    ChangeTitle("\n\n\n\n\n\n\n\nObjective\n\nObserve the time on the top left corner. \nYou must catch all the chickens before time runs out, \notherwise Cluck will smite us.\n"
                        + "\nCrouch or slide to catch the chickens, they're short little devils. \nWhen you are in range of the chicken, they will light up yellow.\n"
                        + "\nPress RB while in range to catch the chicken.\nPlace them in the chicken pen for each chicken to count.\n");
                }
                else
                {
                    ChangeTitle("\n\n\n\n\n\n\n\nObjective\n\nObserve the time on the top left corner. \nYou must catch all the chickens before time runs out, \notherwise Cluck will smite your village of mud eaters.\n"
                        + "\nCrouch or slide to catch the chickens, they're short little devils. \nWhen you are in range of the chicken, they will light up yellow.\n"
                        + "\nPress Left Mouse Button while in range to catch the chicken.\nPlace them in the chicken pen for each chicken to count.\n");
                }
            }
            else if (stage == 3)
            {
                ChangeTitle("\n\n\nCapture\n\nAs you carry the chicken back, notice button prompts may show up. \nThe chicken would escape during this time if you don't\n"
                    + " press the indicated button enough times.\n");
            }
            else if (stage == 4)
            {
                ChangeTitle("\n\n\n\nTips\n\nYou can also slide to lower your elevation for chicken capturing. \nThis can be done by crouching right after sprinting.\n\n"
                    + "Sliding however will consume a portion of your stamina bar.\n");
            }
            else if (stage == 5)
            {
                ChangeTitle("\n\n\n\n\nYup, that's all the tips this sage glue sniffer can tell you. \nCatch the remaining chickens to finish the tutorial!\nGo out and save your village of mud eaters from an angry Cluck!\n");
            }
            // Create our menu entries.

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                nextInstructionMenuEntry  = new MenuEntry("\n\n\n\n\n\n\n\n\n\n\nPress Back to try it");
            }
            else
            {
                nextInstructionMenuEntry = new MenuEntry("\n\n\n\n\n\n\n\n\n\n\nPress Escape to try it");
            }
            // Add entries to the menu.
            MenuEntries.Add(nextInstructionMenuEntry);
        }


        #endregion
    }
}
