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

        RectangleF playerRect;
        Vector2 playerPosition;

        RectangleF[] snakeRect;
        Vector2[] snakeMovementArray;
        float enemySpeed;
        float playerSpeed;

        // Test level
        bool snakeSpawn;
        bool snakeExplode;
        float centerX, centerY, radius, speed, angle, xCoord, yCoord, speedScale;
        RectangleF[] circlePosition;

        float timer;
        int bulletLevelQuarter;

        Texture2D enemy;
        Texture2D playerTex;

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

                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // Reset pause mode when new game starts
                ScreenManager.StartGame = false;
                ScreenManager.PauseGame = true;

                // TESTING (cubes are 48 x 48)
                enemy = content.Load<Texture2D>("blackCube");
                playerTex = content.Load<Texture2D>("whiteCube");

                /*
                // spawn points
                spOne = new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Y + 30); // upper left
                spTwo = new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Height - 30 - enemy.Height); // lower left
                spThree = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 30 - enemy.Width, ScreenManager.GraphicsDevice.Viewport.Y + 30); // upper right
                spFour = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 30 - enemy.Width, ScreenManager.GraphicsDevice.Viewport.Height - 30 - enemy.Height); // lower right

                rectOne = new RectangleF(spOne.X, spOne.Y, enemy.Width, enemy.Height);
                rectTwo = new RectangleF(spTwo.X, spTwo.Y, enemy.Width, enemy.Height);
                rectThree = new RectangleF(spThree.X, spThree.Y, enemy.Width, enemy.Height);
                rectFour = new RectangleF(spFour.X, spFour.Y, enemy.Width, enemy.Height);

                enemyRect[0] = rectOne;
                enemyRect[1] = rectTwo;
                enemyRect[2] = rectThree;
                enemyRect[3] = rectFour;
                */
                
                playerPosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - playerTex.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2 - playerTex.Height / 2);
                playerRect = new RectangleF(playerPosition.X, playerPosition.Y, playerTex.Width, playerTex.Height); // player spawn

                // How many enemies will there be
                snakeRect = new RectangleF[10];

                for (int i = 0; i < snakeRect.Length; i++)
                {
                    snakeRect[i] = new RectangleF(-100, -100, enemy.Width, enemy.Height);
                }

                // Setup spawnpoint at the beginning of the game
                Random rand = new Random();
                int randomSpawn;
                float numX, numY;

                /*
                for (int i = 0; i < enemyRect.Length; i++)
                {
                    randomSpawn = rand.Next(0, 2);

                    if (randomSpawn == 0)
                    {
                        do
                        {
                            numX = rand.Next(-enemy.Width - 49, ScreenManager.GraphicsDevice.Viewport.Width + 50);
                        } while (numX > -enemy.Width && numX < ScreenManager.GraphicsDevice.Viewport.Width);

                        numY = rand.Next(-enemy.Height - 49, ScreenManager.GraphicsDevice.Viewport.Height + 50);
                    }
                    else
                    {
                        numX = rand.Next(-enemy.Width - 49, ScreenManager.GraphicsDevice.Viewport.Width + 50);

                        do
                        {
                            numY = rand.Next(-enemy.Height - 49, ScreenManager.GraphicsDevice.Viewport.Height + 50);
                        } while (numY > -enemy.Height && numY < ScreenManager.GraphicsDevice.Viewport.Height);
                    }

                    enemyRect[i] = new RectangleF(numX, numY, enemy.Width, enemy.Height);
                }
                 */

                snakeMovementArray = new Vector2[snakeRect.Length];

                playerSpeed = 10f;
                enemySpeed = 15f;

                timer = 0;

                bulletLevelQuarter = 0;

                snakeSpawn = false;
                snakeExplode = false;

                // Setup circular blade position
                centerX = ScreenManager.GraphicsDevice.Viewport.Width / 2 - enemy.Width / 2;
                centerY = ScreenManager.GraphicsDevice.Viewport.Height / 2 - enemy.Height / 2;
                radius = 100;
                speed = 60;
                speedScale = (float)((0.001 * 2 * Math.PI) / speed);
                circlePosition = new RectangleF[4];
                for (int i = 0; i < circlePosition.Length; i++)
                {
                    circlePosition[i] = new RectangleF(-100, -100, enemy.Width, enemy.Height);
                }

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

                    // Snake movement - will not work with bullet yet
                    SnakeMovement();

                    // Bullet entry - will not work with snake movement
                    //BulletLevel();

                    // circular blade movement
                    angle = (float)gameTime.TotalGameTime.TotalSeconds;
                    CircularBlade(angle);
                }
            }
        }

        public void CircularBlade(float angle)
        {
            for (int i = 0; i < circlePosition.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        xCoord = (float)(centerX + Math.Sin(angle) * radius);
                        yCoord = (float)(centerY + Math.Cos(angle) * radius);
                        break;
                    case 1:
                        xCoord = (float)(centerX + Math.Sin(angle - 1.5f) * radius);
                        yCoord = (float)(centerY + Math.Cos(angle - 1.5f) * radius);
                        break;
                    case 2:
                        xCoord = (float)(centerX + Math.Sin(angle - 3.0f) * radius);
                        yCoord = (float)(centerY + Math.Cos(angle - 3.0f) * radius);
                        break;
                    case 3:
                        xCoord = (float)(centerX + Math.Sin(angle - 4.5f) * radius);
                        yCoord = (float)(centerY + Math.Cos(angle - 4.5f) * radius);
                        break;
                }

                circlePosition[i].X = xCoord;
                circlePosition[i].Y = yCoord;
            }

            centerX += 1;
        }

        public void SnakeMovement()
        {
                // Move like snake when entering
            if (snakeExplode == false)
            {
                #region Snake Beginning
                if (snakeSpawn == false)
                {
                    // setup the snake
                    for (int i = 0; i < snakeRect.Length; i++)
                    {
                        if (i == 0)
                        {
                            snakeRect[i].X = 0;
                            snakeRect[i].Y = ScreenManager.GraphicsDevice.Viewport.Height / 2 - enemy.Height / 2;
                        }
                        else
                        {
                            snakeRect[i].X = snakeRect[i - 1].X - 100;
                            snakeRect[i].Y = snakeRect[i - 1].Y - 100;
                        }
                    }

                    snakeSpawn = true;
                }


                for (int i = snakeRect.Length - 1; i > 0; i--)
                {
                    snakeRect[i].X = snakeRect[i - 1].X;
                    snakeRect[i].Y = snakeRect[i - 1].Y;

                    /*
                    if (enemyRect[enemyRect.Length - 1].X >= ScreenManager.GraphicsDevice.Viewport.Width)
                    {
                        snakeSpawn = false;
                    }*/
                }

                snakeRect[0].X += 0.5f * enemySpeed;
                snakeRect[0].Y += (float)Math.Cos(snakeRect[0].X / 40) * 20; // divide by x to make the Y-motion smoother, multiply cos will make Y position higher

                // Explode when snake reaches edge of screen
                if (snakeRect[0].Right >= ScreenManager.GraphicsDevice.Viewport.Width)
                {
                    snakeExplode = true;

                    Random rand = new Random();
                    for (int i = 0; i < snakeRect.Length; i++)
                    {
                        do{
                            snakeMovementArray[i].X = (float)rand.NextDouble();
                        } while(snakeMovementArray[i].X < 0.4f);

                        snakeMovementArray[i].X *= -1;
                        snakeMovementArray[i].X *= enemySpeed;

                        float randomY;
                        do
                        {
                            randomY = rand.Next(-1, 2);
                        } while (randomY == 0);

                        snakeMovementArray[i].Y = (float)rand.NextDouble() * randomY * enemySpeed;
                    }
                }
                #endregion
            }
                // Explode when reached one edge
            else
            {
                #region Snake Explosion
                for (int i = 0; i < snakeRect.Length; i++)
                {
                    snakeRect[i].X += snakeMovementArray[i].X;
                    snakeRect[i].Y += snakeMovementArray[i].Y;

                    if (snakeRect[i].Top <= ScreenManager.GraphicsDevice.Viewport.Y)
                    {
                        snakeMovementArray[i].Y = Math.Abs(snakeMovementArray[i].Y);
                    }
                    else if (snakeRect[i].Bottom >= ScreenManager.GraphicsDevice.Viewport.Height - enemy.Height)
                    {
                        snakeMovementArray[i].Y = Math.Abs(snakeMovementArray[i].Y) * -1;
                    }

                    if (snakeRect[i].Right <= ScreenManager.GraphicsDevice.Viewport.X)
                    {
                        snakeMovementArray[i].X = 0;
                        snakeMovementArray[i].Y = 0;
                    }
                }

                Vector2 stoppedCube = new Vector2(0,0);
                int allCubeStopped = 0;
                for (int i = 0; i < snakeRect.Length; i++)
                {
                    if (snakeMovementArray[i].Equals(stoppedCube))
                    {
                        allCubeStopped++;
                    }
                }

                if (allCubeStopped == snakeRect.Length)
                {
                    snakeExplode = false;
                    snakeSpawn = false;
                }
                #endregion
            }
        }

        #region Bullet Level
        public void BulletLevel()
        {
            if (bulletLevelQuarter < enemyRect.Length)
            {
                if (timer >= bulletLevelQuarter * 3) // Spawn new enemy every 3 seconds
                {
                    enemyMovementArray[bulletLevelQuarter] = new Vector2((playerRect.X - enemyRect[bulletLevelQuarter].X), (playerRect.Y - enemyRect[bulletLevelQuarter].Y));
                    enemyMovementArray[bulletLevelQuarter].Normalize();
                    enemyMovementArray[bulletLevelQuarter] *= enemySpeed;
                    bulletLevelQuarter++;
                }
            }

            // Moving the enemies
            for (int index = 0; index < bulletLevelQuarter; index++)
            {
                enemyRect[index].X += enemyMovementArray[index].X;
                enemyRect[index].Y += enemyMovementArray[index].Y;

                if (enemyRect[index].Left > ScreenManager.GraphicsDevice.Viewport.Width + 50 ||
                    enemyRect[index].Right < ScreenManager.GraphicsDevice.Viewport.X - 50 ||
                    enemyRect[index].Top > ScreenManager.GraphicsDevice.Viewport.Height + 50 ||
                    enemyRect[index].Bottom < ScreenManager.GraphicsDevice.Viewport.Y - 50)
                {
                    // Moving right
                    BulletSpawnPoint(index);
                }
            }
        }

        public void BulletSpawnPoint(int index)
        {
            Random rand = new Random();
            int randomSpawn, numX, numY;

            randomSpawn = rand.Next(0, 2);

            if (randomSpawn == 0)
            {
                do
                {
                    numX = rand.Next(-enemy.Width - 49, ScreenManager.GraphicsDevice.Viewport.Width + 50);
                } while (numX > -enemy.Width && numX < ScreenManager.GraphicsDevice.Viewport.Width);

                numY = rand.Next(-enemy.Height - 49, ScreenManager.GraphicsDevice.Viewport.Height + 50);
            }
            else
            {
                numX = rand.Next(-enemy.Width - 49, ScreenManager.GraphicsDevice.Viewport.Width + 50);

                do
                {
                    numY = rand.Next(-enemy.Height - 49, ScreenManager.GraphicsDevice.Viewport.Height + 50);
                } while (numY > -enemy.Height && numY < ScreenManager.GraphicsDevice.Viewport.Height);
            }

            enemyRect[index].X = numX;
            enemyRect[index].Y = numY;

            enemyMovementArray[index] = new Vector2((playerRect.X - enemyRect[index].X), (playerRect.Y - enemyRect[index].Y));
            enemyMovementArray[index].Normalize();
            enemyMovementArray[index] *= enemySpeed;
        }
        #endregion

        // not used for now
        public Vector2 EnemyMovement(int index)
        {
            Vector2 movement = new Vector2(0, 0);

            movement.X = playerPosition.X - enemyRect[index].X;
            movement.Y = playerPosition.Y - enemyRect[index].Y;

            movement.Normalize();
            movement *= 10f;

            return movement;
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
                Vector2 boundary = playerRect.Position + movement * playerSpeed;

                if (boundary.X >= 0 && boundary.X <= ScreenManager.GraphicsDevice.Viewport.Width - playerTex.Width)
                {
                    if (boundary.Y >= 0 && boundary.Y <= ScreenManager.GraphicsDevice.Viewport.Height - playerTex.Height)
                    {
                        playerRect.X += movement.X * playerSpeed;
                        playerRect.Y += movement.Y * playerSpeed;
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
            for (int i = 0; i < snakeRect.Length; i++)
            {
                Vector2 enemyStartSpawn = snakeRect[i].Position;
                spriteBatch.Draw(enemy, enemyStartSpawn, Color.Red);
            }

            spriteBatch.Draw(playerTex, playerRect.Position, Color.White);

            /*
            for (int i = 0; i < circlePosition.Length; i++)
            {
                spriteBatch.Draw(enemy, circlePosition[i].Position, Color.Black);
            }
             */

            spriteBatch.Draw(enemy, circlePosition[0].Position, Color.Black);
            spriteBatch.Draw(enemy, circlePosition[1].Position, Color.Yellow);
            spriteBatch.Draw(enemy, circlePosition[2].Position, Color.Red);
            spriteBatch.Draw(enemy, circlePosition[3].Position, Color.Green);

            spriteBatch.DrawString(gameFont, timer.ToString(), new Vector2(100, 100), Color.White);
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
