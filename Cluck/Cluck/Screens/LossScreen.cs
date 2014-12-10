#region File Description
//-----------------------------------------------------------------------------
// LossScreen.cs
//
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using GameStateManagement;
#endregion

namespace Cluck
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class LossScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public LossScreen()
            : base("You Lost!\n Cluck showed up and there weren't enough chickens to "
             + "appease him!\nYour entire village was turned into goats!", MenuSong.nothing)
        {
            // Create our menu entries.
            Background = BackgroundTexture.lostCampaign;
            MenuEntry playAgainGameMenuEntry = new MenuEntry("Play Again");
            MenuEntry mainMenuGameMenuEntry = new MenuEntry("Go To Back Main Menu");

            // Hook up menu event handlers.
            playAgainGameMenuEntry.Selected += PlayAgainGameMenuEntrySelected;
            mainMenuGameMenuEntry.Selected += MainMenuGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(playAgainGameMenuEntry);
            MenuEntries.Add(mainMenuGameMenuEntry);
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the player selects Play Again after losing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayAgainGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            Cluck.currentLevel = 1;
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(Cluck.currentLevel));
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
            Cluck.currentLevel = 1;
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }


        #endregion
    }
}
