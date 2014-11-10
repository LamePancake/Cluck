﻿#region File Description
//-----------------------------------------------------------------------------
// LossScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace Cluck
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class WinScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public WinScreen()
            : base("You managed to prevent Cluck's wrath!")
        {
            // Create our menu entries.
            MenuEntry continueGameMenuEntry = new MenuEntry("Next Level");
            MenuEntry mainMenuGameMenuEntry = new MenuEntry("Go To Back Main Menu");

            // Hook up menu event handlers.
            continueGameMenuEntry.Selected += ContinueGameMenuEntrySelected;
            mainMenuGameMenuEntry.Selected += MainMenuGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(continueGameMenuEntry);
            MenuEntries.Add(mainMenuGameMenuEntry);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the player selects Play Again after losing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContinueGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            CurrentLevel += 1;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(CurrentLevel));
        }

        /// <summary>
        /// Event handler for when the player selects Go Back To Main Menu after losing.
        /// </summary>
        void MainMenuGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want go back to main menu?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            CurrentLevel = 1;
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }


        #endregion
    }
}