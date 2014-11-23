#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameStateManagement;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class DefeatScreen : MenuScreen
    {
        #region Initialization

        ContentManager content;
        SpriteFont font;

        //GameSettings gameSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DefeatScreen()
            : base("You failed...")
        {
            //gameSettings = new GameSettings();

            // Create our menu entries.
            MenuEntry restartGameMenuEntry = new MenuEntry("Again!");
            MenuEntry quitGameMenuEntry = new MenuEntry("I'll be back");

            // Hook up menu event handlers.
            restartGameMenuEntry.Selected += StartGameSelected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(restartGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        /// 

        void StartGameSelected(object sender, PlayerIndexEventArgs e)
        {
            //ScreenManager.ResetGame = false;
            ScreenManager.StartGame = false;
            ScreenManager.PauseGame = true;
            ExitScreen();
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

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
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());
        }

        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            content = new ContentManager(ScreenManager.Game.Services, "Content");
            font = content.Load<SpriteFont>("menufont");
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            spriteBatch.Begin();

            Vector2 centerScreen = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            string congrat = "Too bad!";
            string msg1 = "You've survived against";
            string msg2 = BulletAmount + " Bullets";

            string msg3 = "";
            if (SnakeEnabled == true)
            {
                msg3 = "One crazy exploding snake";
            }
            else
            {
                msg3 = "No snake";
            }

            string msg4 = "";
            if (BladeEnabled == true)
            {
                msg4 = "One killer boxy blade";
            }
            else
            {
                msg4 = "No circular blade";
            }

            spriteBatch.DrawString(font, congrat, new Vector2(centerScreen.X - font.MeasureString(congrat).X / 2, centerScreen.Y), Color.White);
            spriteBatch.DrawString(font, msg1, new Vector2(centerScreen.X - font.MeasureString(msg1).X / 2, centerScreen.Y + font.MeasureString(msg1).Y), Color.White);
            spriteBatch.DrawString(font, msg2, new Vector2(centerScreen.X - font.MeasureString(msg2).X / 2, centerScreen.Y + font.MeasureString(msg1).Y + font.MeasureString(msg2).Y), Color.White);
            spriteBatch.DrawString(font, msg3, new Vector2(centerScreen.X - font.MeasureString(msg3).X / 2, centerScreen.Y + font.MeasureString(msg1).Y + font.MeasureString(msg2).Y + font.MeasureString(msg3).Y), Color.White);
            spriteBatch.DrawString(font, msg4, new Vector2(centerScreen.X - font.MeasureString(msg4).X / 2, centerScreen.Y + font.MeasureString(msg1).Y + font.MeasureString(msg2).Y + font.MeasureString(msg3).Y + font.MeasureString(msg4).Y), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
