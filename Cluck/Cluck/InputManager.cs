using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Cluck
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputManager
    {
        private MouseState currentMouseState;
        private MouseState previousMouseState;
        private KeyboardState currentKeyboardState;
        private KeyboardState previousKeyboardState;
        private GamePadState previousGamePadState;
        private GamePadState currentGamePadState;
        private float sensitivity = 10.0f;

        public InputManager()
        {
#if WINDOWS
            currentMouseState = Mouse.GetState();
#endif
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public Input Update(Rectangle client)
        {
            Input i = new Input();
#if WINDOWS
            ProcessMouse(ref i, client);
#endif
            //if (currentGamePadState.IsConnected)
            //{
                ProcessController(ref i);
            //}

            ProcessKeyboard(ref i);

            return i;
        }

        private void ProcessController(ref Input i)
        {
            previousGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            float x = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X;
            float y = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y;

            i.SetClap(GamePad.GetState(PlayerIndex.One).Triggers.Right);

            if (x > 0)
            {
                i.SetRight(x);
            }
            else if (x < 0)
            {
                i.SetLeft(x);
            }

            if (y > 0)
            {
                i.SetForward(y);
            }
            else if (y < 0)
            {
                i.SetBackward(y);
            }

            if (previousGamePadState.IsButtonUp(Buttons.A) && currentGamePadState.IsButtonDown(Buttons.A))
            {
                i.SetJumping(true);
            }

            if (previousGamePadState.Buttons.RightShoulder == ButtonState.Released && currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed)
            {
                i.SetClapping(true);
            }

            if (currentGamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                i.SetCrouching(true);
            }

            if (currentGamePadState.IsButtonDown(Buttons.LeftStick))
            {
                if (i.GetForward() > 0 || i.GetBackward() > 0 || i.GetLeft() < 0 || i.GetRight() > 0)
                {
                    i.SetSprinting(true);
                }
                else
                {
                    i.SetSprinting(false);
                }
            }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X != 0)
            {
                i.SetViewX(-GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X * sensitivity);
            }

            if (GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y != 0)
            {
                i.SetViewY(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y * sensitivity);
            }

            if (currentGamePadState.IsButtonDown(Buttons.LeftStick) && currentGamePadState.IsButtonDown(Buttons.LeftShoulder) && previousGamePadState.IsButtonUp(Buttons.LeftShoulder))
            {
                if (i.GetForward() > 0)
                {
                    i.SetSliding(true);
                    i.SetSprinting(false);
                    i.SetCrouching(false);
                }
                else
                {
                    i.SetSliding(false);
                    i.SetSprinting(false);
                    i.SetCrouching(false);
                }
            }

            if(previousGamePadState.IsButtonUp(Buttons.X) && currentGamePadState.IsButtonDown(Buttons.X))
            {
                i.SetXButton(true);
            }

            if (previousGamePadState.IsButtonUp(Buttons.Y) && currentGamePadState.IsButtonDown(Buttons.Y))
            {
                i.SetYButton(true);
            }

            if (previousGamePadState.IsButtonUp(Buttons.B) && currentGamePadState.IsButtonDown(Buttons.B))
            {
                i.SetBButton(true);
            }

            if (previousGamePadState.IsButtonUp(Buttons.A) && currentGamePadState.IsButtonDown(Buttons.A))
            {
                i.SetAButton(true);
            }
        }

        private void ProcessKeyboard(ref Input i)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.W))
            {
                i.SetForward(1.0f);
            }

            if (currentKeyboardState.IsKeyDown(Keys.S))
            {
                i.SetBackward(-1.0f);
            }

            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                i.SetLeft(-1.0f);
            }

            if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                i.SetRight(1.0f);
            }

            if (previousKeyboardState.IsKeyUp(Keys.Space) && currentKeyboardState.IsKeyDown(Keys.Space))
            {
                i.SetJumping(true);
            }

            if (currentKeyboardState.IsKeyDown(Keys.C))
            {
                i.SetCrouching(true);
            }

            if (currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (i.GetForward() > 0 || i.GetBackward() < 0 || i.GetLeft() < 0 || i.GetRight() > 0)
                {
                    i.SetSprinting(true);
                }
                else
                {
                    i.SetSprinting(false);
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.LeftShift) && currentKeyboardState.IsKeyDown(Keys.C) && previousKeyboardState.IsKeyUp(Keys.C))
            {
                if (i.GetForward() > 0)
                {
                    i.SetSliding(true);
                    i.SetSprinting(false);
                    i.SetCrouching(false);
                }
                else
                {
                    i.SetSliding(false);
                    i.SetSprinting(false);
                    i.SetCrouching(false);
                }
            }

            if (previousKeyboardState.IsKeyUp(Keys.Q) && currentKeyboardState.IsKeyDown(Keys.Q))
            {
                i.SetXButton(true);
            }

            if (previousKeyboardState.IsKeyUp(Keys.E) && currentKeyboardState.IsKeyDown(Keys.E))
            {
                i.SetYButton(true);
            }

            if (previousKeyboardState.IsKeyUp(Keys.R) && currentKeyboardState.IsKeyDown(Keys.R))
            {
                i.SetBButton(true);
            }

            if (previousKeyboardState.IsKeyUp(Keys.F) && currentKeyboardState.IsKeyDown(Keys.F))
            {
                i.SetAButton(true);
            }
        }

        private void ProcessMouse(ref Input i, Rectangle client)
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();

            int centerX = client.Width / 2;
            int centerY = client.Height / 2;
            int deltaX = centerX - currentMouseState.X;
            int deltaY = centerY - currentMouseState.Y;

            Mouse.SetPosition(centerX, centerY);

            i.AddViewX((float)deltaX);
            i.AddViewY((float)deltaY);

            if (previousMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
            {
                i.SetClapping(true);
            }
        }
    }
}
