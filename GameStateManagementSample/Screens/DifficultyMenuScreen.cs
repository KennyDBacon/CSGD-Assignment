#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class DifficultyMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry bullet;
        MenuEntry snake;
        MenuEntry blade;

        string enabled = "Enabled";
        string disabled = "Disabled";

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public DifficultyMenuScreen()
            : base("Difficulty Settings")
        {
            bullet = new MenuEntry(string.Empty);
            snake = new MenuEntry(string.Empty);
            blade = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            bullet.Selected += BulletSelected;
            snake.Selected += SnakeSelected;
            blade.Selected += BladeSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(bullet);
            MenuEntries.Add(snake);
            MenuEntries.Add(blade);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            bullet.Text = "Bullet: ";
            snake.Text = "Snake: ";
            blade.Text = "Circular Blade: ";
        }


        #endregion

        #region Handle Input

        void BulletSelected(object sender, PlayerIndexEventArgs e)
        {
            currentUngulate++;

            if (currentUngulate > Ungulate.Llama)
                currentUngulate = 0;

            SetMenuEntryText();
        }

        void SnakeSelected(object sender, PlayerIndexEventArgs e)
        {
            currentLanguage = (currentLanguage + 1) % languages.Length;

            SetMenuEntryText();
        }

        void BladeSelected(object sender, PlayerIndexEventArgs e)
        {
            frobnicate = !frobnicate;

            SetMenuEntryText();
        }
        #endregion
    }
}
