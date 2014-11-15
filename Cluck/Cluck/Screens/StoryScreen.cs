#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
#endregion

namespace Cluck.Screens
{
    class StoryScreen : MenuScreen
    {
        public StoryScreen()
            : base("Story")
        {
            // Create our menu entries.
            MenuEntry skipMenuEntry = new MenuEntry("Skip Story");

            MenuEntries.Add(skipMenuEntry);
        }
    }
}
