using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#if ANDROID
using Microsoft.Xna.Framework.Input.Touch;
#endif

namespace Engine.Engines
{
    public class InputEngine : GameComponent
    {
        public static Color clearColor;
#if ANDROID
        static TouchCollection currentTouchState;
        public static GestureType currentGestureType = GestureType.None;
        public static Vector2 PreviousGesturePosition;
        public static Vector2 CurrentGesturePosition;
        static public TouchCollection CurrentTouchState
        {
            get
            {
                return currentTouchState;
            }

            set
            {
                currentTouchState = value;
            }
        }


#endif
#if WINDOWS


        private static GamePadState previousPadState;
        private static GamePadState currentPadState;

        private static KeyboardState previousKeyState;
        private static KeyboardState currentKeyState;

        private static Vector2 previousMousePos;
        private static Vector2 currentMousePos;

        private static MouseState previousMouseState;
        private static MouseState currentMouseState;
#endif
        public InputEngine(Game _game)
            : base(_game)
        {


#if ANDROID
            CurrentTouchState = TouchPanel.GetState();
#endif

#if WINDOWS
            currentPadState = GamePad.GetState(PlayerIndex.One);
            currentKeyState = Keyboard.GetState();
#endif
            _game.Components.Add(this);
        }

        
        public static void ClearState()
        {
#if WINDOWS


            previousMouseState = Mouse.GetState();
            currentMouseState = Mouse.GetState();
            previousKeyState = Keyboard.GetState();
            currentKeyState = Keyboard.GetState();
#endif

#if ANDROID
            CurrentGesturePosition = PreviousGesturePosition;
            currentGestureType = GestureType.None;
#endif
        }

        public override void Update(GameTime gametime)
        {
#if ANDROID
            CurrentTouchState = TouchPanel.GetState();
            if (CurrentTouchState.Count == 1)
            {
                PreviousGesturePosition = CurrentGesturePosition;
                CurrentGesturePosition = CurrentTouchState[0].Position;
            }

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gesture = TouchPanel.ReadGesture();
                switch (gesture.GestureType)
                {
                    case GestureType.None:
                        clearColor = Color.CornflowerBlue;
                        break;
                    case GestureType.DoubleTap:
                        clearColor = clearColor == Color.CornflowerBlue ? Color.Red : Color.CornflowerBlue;
                        break;
                    case GestureType.Tap:
                        clearColor = clearColor == Color.CornflowerBlue ? Color.Black : Color.CornflowerBlue;
                        break;
                    case GestureType.Flick:
                        clearColor = clearColor == Color.CornflowerBlue ? Color.Pink : Color.CornflowerBlue;
                        break;
                    case GestureType.HorizontalDrag:
                        clearColor = clearColor == Color.CornflowerBlue ? Color.Peru : Color.CornflowerBlue;
                        break;
                    case GestureType.VerticalDrag:
                        clearColor = clearColor == Color.CornflowerBlue ? Color.Wheat : Color.CornflowerBlue;
                        break;

                }
                currentGestureType = gesture.GestureType;
            }
#endif
#if WINDOWS
            previousPadState = currentPadState;
            previousKeyState = currentKeyState;

            currentPadState = GamePad.GetState(PlayerIndex.One);
            currentKeyState = Keyboard.GetState();


            previousMouseState = currentMouseState;
            currentMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            currentMouseState = Mouse.GetState();


            KeysPressedInLastFrame.Clear();
            CheckForTextInput();
#endif
            base.Update(gametime);
        }
#if WINDOWS
        public List<string> KeysPressedInLastFrame = new List<string>();

        private void CheckForTextInput()
        {
            foreach(var key in Enum.GetValues(typeof(Keys)) as Keys[])
            {
                if(IsKeyPressed(key))
                {
                    KeysPressedInLastFrame.Add(key.ToString());
                    break;
                }
            }
        }

        public static bool IsButtonPressed(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonUp(buttonToCheck) && previousPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsButtonHeld(Buttons buttonToCheck)
        {
            if (currentPadState.IsButtonDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsKeyHeld(Keys buttonToCheck)
        {
            if (currentKeyState.IsKeyDown(buttonToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool IsKeyPressed(Keys keyToCheck)
        {
            if (currentKeyState.IsKeyUp(keyToCheck) && previousKeyState.IsKeyDown(keyToCheck))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static GamePadState CurrentPadState
        {
            get { return currentPadState; }
            set { currentPadState = value; }
        }
        public static KeyboardState CurrentKeyState
        {
            get { return currentKeyState; }
        }

        public static MouseState CurrentMouseState
        {
            get { return currentMouseState; }
        }

        public static MouseState PreviousMouseState
        {
            get { return previousMouseState; }
        }



        public static bool IsMouseLeftClick()
        {
            if (currentMouseState.LeftButton == ButtonState.Released && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else 
                return false;
        }

        public static bool IsMouseRightClick()
        {
            if (currentMouseState.RightButton == ButtonState.Released && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseRightHeld()
        {
            if (currentMouseState.RightButton == ButtonState.Pressed && previousMouseState.RightButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static bool IsMouseLeftHeld()
        {
            if (currentMouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Pressed)
                return true;
            else
                return false;
        }

        public static Vector2 MousePosition
        {
            get { return currentMousePos; }
        }
#endif

    }
}
