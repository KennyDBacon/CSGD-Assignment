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

            /*
            bullet.Text = "Bullet: " + gameSettings.Bullet.ToString();

            if (gameSettings.Snake == true)
            {
                snake.Text = "Snake: Released";
            }
            else
            {
                snake.Text = "Snake: In cage";
            }

            if (gameSettings.Blade == true)
            {
                blade.Text = "Circular Blade: Activated";
            }
            else
            {
                blade.Text = "Circular Blade: Deactivated";
            }

            gameSettings.Save();
            */

            defaultSetting.Text = "Restore Default";
        }

        #endregion

        #region Handle Input

        void BulletSelected(object sender, PlayerIndexEventArgs e)
        {
            BulletAmount += 1;

            if (BulletAmount == 11)
                BulletAmount = 5;

            /*
            gameSettings.Bullet += 1;
            if (gameSettings.Bullet >= 11)
                gameSettings.Bullet = 5;
            */
            SetMenuEntryText();
        }

        void SnakeSelected(object sender, PlayerIndexEventArgs e)
        {
            SnakeEnabled = !SnakeEnabled;

            //gameSettings.Snake = !gameSettings.Snake;

            SetMenuEntryText();
        }

        void BladeSelected(object sender, PlayerIndexEventArgs e)
        {
            BladeEnabled = !BladeEnabled;

            //gameSettings.Blade = !gameSettings.Blade;

            SetMenuEntryText();
        }

        void DefaultSelected(object sender, PlayerIndexEventArgs e)
        {
            BulletAmount = 5;
            SnakeEnabled = false;
            BladeEnabled = false;

            /*
            gameSettings.Bullet = 5;
            gameSettings.Snake = false;
            gameSettings.Blade = false;
            */

            SetMenuEntryText();
        }
        #endregion
    }
}
