#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using GameStateManagement;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace Cluck
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Initialization

        bool transitioned = false;
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base("", MenuSong.titleTheme)
        {
            Overlay = MenuOverLay.mainTitle;
            // Create our menu entries.
            MenuEntry playTutorialMenuEntry = new MenuEntry("\n\n\n\n\n\nTutorial");
            playTutorialMenuEntry.Position += new Vector2(500, 50000);
            MenuEntry playGameMenuEntry = new MenuEntry("\n\n\n\n\n\n\nCampaign");
            MenuEntry playArcadeMenuEntry = new MenuEntry("\n\n\n\n\n\n\n\nArcade");
            MenuEntry optionsMenuEntry = new MenuEntry("\n\n\n\n\n\n\n\n\nOptions");
            MenuEntry exitMenuEntry = new MenuEntry("\n\n\n\n\n\n\n\n\nExit");

            // Hook up menu event handlers.
            playTutorialMenuEntry.Selected += PlayTutorialMenuEntrySelected;
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            playArcadeMenuEntry.Selected += PlayArcadeMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playTutorialMenuEntry);
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(playArcadeMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Tutorial menu entry is selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayTutorialMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new TutorialScreen());
            StopMusic();
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(Cluck.currentLevel));
            StopMusic();
        }

        void PlayArcadeMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new ArcadeScreen());
            StopMusic();
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
