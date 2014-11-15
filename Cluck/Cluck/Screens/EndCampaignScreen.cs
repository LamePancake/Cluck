#region File Description
//-----------------------------------------------------------------------------
// EndCampaignScreen.cs
//
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
    class EndCampaignScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public EndCampaignScreen(int score, int highscore)
            : base("Cluck has been satisfied! Your village is safe for the rest of the year!\n Your Score: " + score + "\n High-Score: " + highscore)
        {
            // Create our menu entries.
            MenuEntry mainMenuGameMenuEntry = new MenuEntry("Go To Back Main Menu");

            // Hook up menu event handlers.
            mainMenuGameMenuEntry.Selected += MainMenuGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(mainMenuGameMenuEntry);
        }


        #endregion

        #region Handle Input
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
