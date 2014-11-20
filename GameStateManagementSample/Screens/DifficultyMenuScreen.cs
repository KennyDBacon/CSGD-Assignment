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

        void SetMenuEntryText()
        {
            // default settings if player set everything to false
            if (ScreenManager.BulletEnabled == false && ScreenManager.SnakeEnabled == false && ScreenManager.BladeEnabled == false)
            {
                ScreenManager.BulletEnabled = true;
            }

            bullet.Text = "Bullet: " + ScreenManager.BulletEnabled;
            snake.Text = "Snake: " + ScreenManager.SnakeEnabled;
            blade.Text = "Circular Blade: " + ScreenManager.BladeEnabled;
        }

        #endregion

        #region Handle Input

        void BulletSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.BulletEnabled = !ScreenManager.BulletEnabled;

            SetMenuEntryText();
        }

        void SnakeSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.SnakeEnabled = !ScreenManager.SnakeEnabled;

            SetMenuEntryText();
        }

        void BladeSelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.BladeEnabled = !ScreenManager.BladeEnabled;

            SetMenuEntryText();
        }
        #endregion
    }
}
