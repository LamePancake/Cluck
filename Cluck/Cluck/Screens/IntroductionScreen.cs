#region File Description
//-----------------------------------------------------------------------------
// IntroductionScreen.cs
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
    class IntroductionScreen : MenuScreen
    {
        #region Fields
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public IntroductionScreen()
            : base("\n\n\n\n\n\n\n\nOnce upon a time, there was a village of mud eaters.\n\nEvery year the deity Cluck comes for the tribute of chickens\nduring the 10 days of Cluckmas."
            + "\n\nIf not sated, he may unleash his wrath upon the village."
            + "\n\nThis year a meddling mud eater keeps releasing the chickens.\n\nIt is up to you to save Cluckmas!")
        {
            // Create our menu entries.
            MenuEntry startGameMenuEntry = new MenuEntry("\n\n\n\n\n\n\n\n\n\nBegin your journey...");

            // Hook up menu event handlers.
            startGameMenuEntry.Selected += StartGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(startGameMenuEntry);
            SetEscCommand(false);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void StartGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to save them? I mean, I wouldn't.";

            MessageBoxScreen confirmStartMessageBox = new MessageBoxScreen(message);

            confirmStartMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
            //confirmStartMessageBox.Cancelled += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmStartMessageBox, ControllingPlayer);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            Cluck.currentLevel = 1;
            ScreenManager.AddScreen(new BackgroundScreen(), null);
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
        }

        #endregion
    }
}
