#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
#endregion

namespace GameStateManagementSample
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        SpriteFont gameFont;

        Random random = new Random();

        float pauseAlpha;

        InputAction pauseAction;

        Vector2 spOne, spTwo, spThree, spFour;
        Rectangle rectOne, rectTwo, rectThree, rectFour, playerRect;

        Vector2 playerPosition;

        Rectangle[] enemyRect;
        Vector2[] enemyMovementArray;
        int enemyMoveToken;

        float timer;
        int bulletLevelQuarter;

        Texture2D enemy;
        Texture2D player;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            pauseAction = new InputAction(
                new Buttons[] { Buttons.Start, Buttons.Back },
                new Keys[] { Keys.Escape },
                true);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                if (content == null)
                    content = new ContentManager(ScreenManager.Game.Services, "Content");

                gameFont = content.Load<SpriteFont>("gamefont");

                // Reset pause mode when new game starts
                ScreenManager.StartGame = false;
                ScreenManager.PauseGame = true;

                // TESTING (cubes are 48 x 48)
                enemy = content.Load<Texture2D>("blackCube");
                player = content.Load<Texture2D>("whiteCube");

                // spawn points
                spOne = new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Y + 30); // upper left
                spTwo = new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Height - 30 - enemy.Height); // lower left
                spThree = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 30 - enemy.Width, ScreenManager.GraphicsDevice.Viewport.Y + 30); // upper right
                spFour = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 30 - enemy.Width, ScreenManager.GraphicsDevice.Viewport.Height - 30 - enemy.Height); // lower right

                rectOne = new Rectangle(Convert.ToInt32(spOne.X), Convert.ToInt32(spOne.Y), enemy.Width, enemy.Height);
                rectTwo = new Rectangle(Convert.ToInt32(spTwo.X), Convert.ToInt32(spTwo.Y), enemy.Width, enemy.Height);
                rectThree = new Rectangle(Convert.ToInt32(spThree.X), Convert.ToInt32(spThree.Y), enemy.Width, enemy.Height);
                rectFour = new Rectangle(Convert.ToInt32(spFour.X), Convert.ToInt32(spFour.Y), enemy.Width, enemy.Height);

                playerPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width/2 - player.Width/2, ScreenManager.GraphicsDevice.Viewport.Height / 2 - player.Height/2);
                playerRect = new Rectangle(ScreenManager.GraphicsDevice.Viewport.Width / 2 - player.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2 - player.Height / 2, player.Width, player.Height); // player spawn
                
                enemyRect = new Rectangle[4];
                enemyRect[0] = rectOne;
                enemyRect[1] = rectTwo;
                enemyRect[2] = rectThree;
                enemyRect[3] = rectFour;

                enemyMovementArray = new Vector2[10 * enemyRect.Length];

                for (int i = 0; i < enemyRect.Length; i++)
                {
                    for (int a = 0; a < enemyMovementArray.Length / enemyRect.Length; a++)
                    {
                        if (a == 0 + (i * 10))
                        {
                            enemyMovementArray[a + (i * 10)].X = (playerRect.X - enemyRect[i].X) / 25;
                            enemyMovementArray[a + (i * 10)].Y = (playerRect.Y - enemyRect[i].Y) / 25;
                        }
                        else
                        {
                            enemyMovementArray[a + (i * 10)].X = (playerRect.X - (enemyRect[i].X + enemyMovementArray[a + (i * 10) - 1].X)) / 25;
                            enemyMovementArray[a + (i * 10)].Y = (playerRect.Y - (enemyRect[i].Y + enemyMovementArray[a + (i * 10) - 1].Y)) / 25;
                        }
                    }
                }

                enemyMoveToken = 0;

                timer = 0;

                bulletLevelQuarter = 0;

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }

#if WINDOWS_PHONE
            if (Microsoft.Phone.Shell.PhoneApplicationService.Current.State.ContainsKey("PlayerPosition"))
            {
                playerPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"];
                enemyPosition = (Vector2)Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"];
            }
#endif
        }


        public override void Deactivate()
        {
#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["PlayerPosition"] = playerPosition;
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State["EnemyPosition"] = enemyPosition;
#endif
            // reset timer
            timer = 0;

            base.Deactivate();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload()
        {
            content.Unload();

#if WINDOWS_PHONE
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("PlayerPosition");
            Microsoft.Phone.Shell.PhoneApplicationService.Current.State.Remove("EnemyPosition");
#endif
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);


            if (ScreenManager.StartGame == false && ScreenManager.PauseGame == true)
            {
                ScreenManager.PauseGame = false;
                ScreenManager.AddScreen(new ModeStartPauseScreen(), PlayerIndex.One);
            }
            else
            {
                if (IsActive)
                {
                    timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    TestChaseLevel();
                    // Bullet level
                    //BulletLevel();
                }
            }
        }

        public void TestChaseLevel()
        {
            for (int i = 0; i < enemyRect.Length; i++)
            {
                enemyRect[i].X += (int)enemyMovementArray[enemyMoveToken + (i * 10)].X;
                enemyRect[i].Y += (int)enemyMovementArray[enemyMoveToken + (i * 10)].Y;
            }

            for (int i = 0; i < enemyRect.Length; i++)
            {
                if (enemyMoveToken - 1 < 0)
                {
                    enemyMovementArray[enemyMoveToken + (i * 10)].X = (playerRect.X - (enemyRect[i].X + enemyMovementArray[((enemyMovementArray.Length / enemyRect.Length) - 1) + (i * 10)].X)) / 25;
                    enemyMovementArray[enemyMoveToken + (i * 10)].Y = (playerRect.Y - (enemyRect[i].Y + enemyMovementArray[((enemyMovementArray.Length / enemyRect.Length) - 1) + (i * 10)].Y)) / 25;
                }
                else
                {
                    enemyMovementArray[enemyMoveToken + (i * 10)].X = (playerRect.X - (enemyRect[i].X + enemyMovementArray[enemyMoveToken - 1 + (i * 10)].X)) / 25;
                    enemyMovementArray[enemyMoveToken + (i * 10)].Y = (playerRect.Y - (enemyRect[i].Y + enemyMovementArray[enemyMoveToken - 1 + (i * 10)].Y)) / 25;
                }
            }

            enemyMoveToken++;
            if (enemyMoveToken > 9)
            {
                enemyMoveToken = 0;
            }
        }

        public void BulletLevel()
        {
            if (bulletLevelQuarter < 4)
            {
                if (timer >= bulletLevelQuarter * 3) // Spawn new enemy every 3 seconds
                {
                    enemyMovementArray[bulletLevelQuarter] = new Vector2((playerPosition.X - enemyRect[bulletLevelQuarter].X) / 25, (playerPosition.Y - enemyRect[bulletLevelQuarter].Y) / 25);
                    bulletLevelQuarter++;
                }
            }

            // Moving the enemies
            for (int index = 0; index < bulletLevelQuarter; index++)
            {
                enemyRect[index].X += (int)enemyMovementArray[index].X + 1;
                enemyRect[index].Y += (int)enemyMovementArray[index].Y + 1;

                if (enemyRect[index].Left >= ScreenManager.GraphicsDevice.Viewport.Width ||
                    enemyRect[index].Right <= ScreenManager.GraphicsDevice.Viewport.X ||
                    enemyRect[index].Top >= ScreenManager.GraphicsDevice.Viewport.Height ||
                    enemyRect[index].Bottom <= ScreenManager.GraphicsDevice.Viewport.Y)
                {
                    // Moving right
                    if(enemyMovementArray[index].X >= 1)
                    BulletSpawnPoint(index);
                }

                // Player lose
                if (playerRect.Intersects(enemyRect[index]))
                {

                }
            }
        }

        public void BulletSpawnPoint(int index)
        {
            Random rand = new Random();
            int numX = rand.Next(0, 2);
            int numY = rand.Next(0, 2);

            switch (numX)
            {
                case 0: enemyRect[index].X = rand.Next(-60, -10);
                    break;
                case 1: enemyRect[index].X = rand.Next(1300, 1350);
                    break;
            }

            switch (numY)
            {
                case 0: enemyRect[index].Y = rand.Next(-60, -10);
                    break;
                case 1: enemyRect[index].Y = rand.Next(1300, 1350);
                    break;
            }

            enemyMovementArray[index] = new Vector2((playerPosition.X - enemyRect[index].X) / 20, (playerPosition.Y - enemyRect[index].Y) / 20);
        }

        public Vector2 EnemyMovement(int index)
        {
            Vector2 movement = new Vector2(0, 0);

            movement.X = playerPosition.X - enemyRect[index].X;
            movement.Y = playerPosition.Y - enemyRect[index].Y;

            movement.Normalize();
            movement *= 10f;

            return movement;
        }

        public Vector2 GetVector2(Rectangle tempRect)
        {
            Vector2 vectOfRect = new Vector2(tempRect.X, tempRect.Y);

            return vectOfRect;
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
#if WINDOWS_PHONE
                ScreenManager.AddScreen(new PhonePauseScreen(), ControllingPlayer);
#else
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
#endif
            }
            else
            {
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                if (keyboardState.IsKeyDown(Keys.Left))
                    movement.X--;

                if (keyboardState.IsKeyDown(Keys.Right))
                    movement.X++;

                if (keyboardState.IsKeyDown(Keys.Up))
                    movement.Y--;

                if (keyboardState.IsKeyDown(Keys.Down))
                    movement.Y++;

                Vector2 thumbstick = gamePadState.ThumbSticks.Left;

                movement.X += thumbstick.X;
                movement.Y -= thumbstick.Y;

                if (input.TouchState.Count > 0)
                {
                    Vector2 touchPosition = input.TouchState[0].Position;
                    Vector2 direction = touchPosition - playerPosition;
                    direction.Normalize();
                    movement += direction;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                // Prevent player from moving off screen
                Vector2 boundary = playerPosition + movement * 8f;

                if (boundary.X >= 0 && boundary.X <= ScreenManager.GraphicsDevice.Viewport.Width)
                {
                    if (boundary.Y >= 0 && boundary.Y <= ScreenManager.GraphicsDevice.Viewport.Height)
                    {
                        playerPosition += movement * 8f;
                    }
                }
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.CornflowerBlue, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            //spriteBatch.DrawString(gameFont, "// TODO", playerPosition, Color.Green);
            spriteBatch.Draw(enemy, enemyRect[0], Color.Red);
            spriteBatch.Draw(enemy, enemyRect[1], Color.Blue);
            spriteBatch.Draw(enemy, enemyRect[2], Color.Green);
            spriteBatch.Draw(enemy, enemyRect[3], Color.Purple);

            spriteBatch.Draw(player, playerPosition, Color.White);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }


        #endregion
    }
}
