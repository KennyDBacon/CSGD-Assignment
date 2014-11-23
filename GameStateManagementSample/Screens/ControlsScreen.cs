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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class ControlsScreen : MenuScreen
    {
        #region Initialization

        Texture2D controlsImg;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public ControlsScreen()
            : base("")
        {

        }

        #endregion

        #region Handle Input
        public override void Activate(bool instancePreserved)
        {
            base.Activate(instancePreserved);

            ContentManager content = ScreenManager.Game.Content;
            controlsImg = content.Load<Texture2D>("xbox-control");
        }

        public override void HandleInput(GameTime gameTime, GameStateManagement.InputState input)
        {
            base.HandleInput(gameTime, input);

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            if (gamePadState.IsButtonDown(Buttons.B))
            {
                ExitScreen();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(controlsImg, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        #endregion
    }
}
