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
using GameStateManagement;
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
        MenuEntry defaultSetting;

        #endregion

        #region Initialization
        //GameSettings gameSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DifficultyMenuScreen()
            : base("Difficulty Settings")
        {
            //gameSettings = new GameSettings();

            bullet = new MenuEntry(string.Empty);
            snake = new MenuEntry(string.Empty);
            blade = new MenuEntry(string.Empty);
            defaultSetting = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            bullet.Selected += BulletSelected;
            snake.Selected += SnakeSelected;
            blade.Selected += BladeSelected;
            defaultSetting.Selected += DefaultSelected;
            back.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(bullet);
            MenuEntries.Add(snake);
            MenuEntries.Add(blade);
            MenuEntries.Add(defaultSetting);
            MenuEntries.Add(back);
        }

        void SetMenuEntryText()
        {
            bullet.Text = "Bullet: " + BulletAmount.ToString();

            if (SnakeEnabled == true)
            {
                snake.Text = "Snake: Released";
            }
            else
            {
                snake.Text = "Snake: In cage";
            }

            if (BladeEnabled == true)
            {
                blade.Text = "Circular Blade: Activated";
            }
            else
            {
                blade.Text = "Circular Blade: Deactivated";
            }

            defaultSetting.Text = "Restore Default";

            //gameSettings.Save();
        }

        #endregion

        #region Handle Input

        void BulletSelected(object sender, PlayerIndexEventArgs e)
        {
            BulletAmount += 1;

            if (BulletAmount == 11)
                BulletAmount = 5;
            SetMenuEntryText();
        }

        void SnakeSelected(object sender, PlayerIndexEventArgs e)
        {
            SnakeEnabled = !SnakeEnabled;

            SetMenuEntryText();
        }

        void BladeSelected(object sender, PlayerIndexEventArgs e)
        {
            BladeEnabled = !BladeEnabled;

            SetMenuEntryText();
        }

        void DefaultSelected(object sender, PlayerIndexEventArgs e)
        {
            BulletAmount = 5;
            SnakeEnabled = false;
            BladeEnabled = false;

            SetMenuEntryText();
        }
        #endregion
    }
}
