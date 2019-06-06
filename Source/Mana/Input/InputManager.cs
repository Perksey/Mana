using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Mana.Utilities;
using OpenTK;
using OpenTK.Input;

namespace Mana
{
    /// <summary>
    /// Represents a class that handles input management for a <see cref="ManaWindow"/> instance.
    /// </summary>
    public class InputManager
    {
        public const int SUPPORTED_GAMEPAD_COUNT = 4;

        public static ManaWindow HoveredWindow = null;

        private static Logger _log = Logger.Create();
        
        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;

        private MouseState _previousMouseState;
        private MouseState _currentMouseState;

        private GamePadState[] _previousGamePadStates;
        private GamePadState[] _currentGamePadStates;

        public event Action<char> KeyTyped; 

        public InputManager(ManaWindow window)
        {
            Window = window;
            Window.MouseEnter += WindowOnMouseEnter;
            Window.MouseLeave += WindowOnMouseLeave;
            Window.MouseMove += WindowOnMouseMove;
            Window.KeyPress += WindowOnKeyPress;
            
            _previousGamePadStates = new GamePadState[SUPPORTED_GAMEPAD_COUNT];
            _currentGamePadStates = new GamePadState[SUPPORTED_GAMEPAD_COUNT];

            KeyTyped += Input.OnKeyTyped;
        }

        private void WindowOnKeyPress(object sender, KeyPressEventArgs e)
        {
            KeyTyped?.Invoke(e.KeyChar);
        }

        public ManaWindow Window { get; }
        
        public Point MousePosition { get; private set; } = new Point(int.MinValue, int.MinValue);

        public bool MouseLeft => _currentMouseState.LeftButton == ButtonState.Pressed;
        public bool MouseMiddle => _currentMouseState.MiddleButton == ButtonState.Pressed;
        public bool MouseRight => _currentMouseState.RightButton == ButtonState.Pressed;

        public int MouseWheel => _currentMouseState.Wheel;
        public int MouseWheelDelta => _currentMouseState.Wheel - _previousMouseState.Wheel;
        
        public void Update()
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();

            for (int i = 0; i < SUPPORTED_GAMEPAD_COUNT; i++)
            {
                _previousGamePadStates[i] = _currentGamePadStates[i];
                _currentGamePadStates[i] = GamePad.GetState(i);

                if (_currentGamePadStates[i].IsConnected && !_previousGamePadStates[i].IsConnected)
                {
                    _log.Debug($"Gamepad Connected: [{i}]: {GamePad.GetName(i)}");
                }
                
                if (!_currentGamePadStates[i].IsConnected && _previousGamePadStates[i].IsConnected)
                {
                    _log.Debug($"Gamepad Disconnected: [{i}]: {GamePad.GetName(i)}");
                }
            }
        }
        
        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyDown(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently up.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently up.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyUp(Key key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Gets a value that indicates if any keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <returns>A value that indicates if any keyboard <see cref="Key"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAnyKeyDown()
        {
            return _currentKeyboardState.IsAnyKeyDown;
        }

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasKeyPressed(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was released this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was released this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasKeyReleased(Key key)
        {
            return _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
        }
        
        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently down.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseDown(MouseButton button)
        {
            return _currentMouseState.IsButtonDown(button);
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently up.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently up.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseUp(MouseButton button)
        {
            return _currentMouseState.IsButtonUp(button);
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasMousePressed(MouseButton button)
        {
            return _currentMouseState.IsButtonDown(button) && _previousMouseState.IsButtonUp(button);
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was released this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was released this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasMouseReleased(MouseButton button)
        {
            return _currentMouseState.IsButtonUp(button) && _previousMouseState.IsButtonDown(button);
        }

        /// <summary>
        /// Gets a value that indicates whether the given gamepad button is currently down.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <param name="index">The controller index (player).</param>
        /// <returns>A value that indicates whether the given gamepad button is currently down.</returns>
        public bool IsButtonDown(Buttons button, int index = 0)
        {
            // There should be a better way to do this, but there doesn't seem to be.
            // Maybe this will be improved in OpenTK 4.0?
            
            switch (button)
            {
                case Buttons.DPadUp:
                    return _currentGamePadStates[index].DPad.IsUp;
                case Buttons.DPadDown:
                    return _currentGamePadStates[index].DPad.IsDown;
                case Buttons.DPadLeft:
                    return _currentGamePadStates[index].DPad.IsLeft;
                case Buttons.DPadRight:
                    return _currentGamePadStates[index].DPad.IsRight;
                case Buttons.Start:
                    return _currentGamePadStates[index].Buttons.Start == ButtonState.Pressed;
                case Buttons.Back:
                    return _currentGamePadStates[index].Buttons.Back == ButtonState.Pressed;
                case Buttons.LeftStick:
                    return _currentGamePadStates[index].Buttons.LeftStick == ButtonState.Pressed;
                case Buttons.RightStick:
                    return _currentGamePadStates[index].Buttons.RightStick == ButtonState.Pressed;
                case Buttons.LeftShoulder:
                    return _currentGamePadStates[index].Buttons.LeftShoulder == ButtonState.Pressed;
                case Buttons.RightShoulder:
                    return _currentGamePadStates[index].Buttons.RightShoulder == ButtonState.Pressed;
                case Buttons.Home:
                    return _currentGamePadStates[index].Buttons.BigButton == ButtonState.Pressed;
                case Buttons.A:
                    return _currentGamePadStates[index].Buttons.A == ButtonState.Pressed;
                case Buttons.B:
                    return _currentGamePadStates[index].Buttons.B == ButtonState.Pressed;
                case Buttons.X:
                    return _currentGamePadStates[index].Buttons.X == ButtonState.Pressed;
                case Buttons.Y:
                    return _currentGamePadStates[index].Buttons.Y == ButtonState.Pressed;
                case Buttons.LeftThumbstickLeft:
                    return _currentGamePadStates[index].ThumbSticks.Left.X < 0.0f;
                case Buttons.LeftThumbstickDown:
                    return _currentGamePadStates[index].ThumbSticks.Left.Y < 0.0f;
                case Buttons.LeftThumbstickRight:
                    return _currentGamePadStates[index].ThumbSticks.Left.X > 0.0f;
                case Buttons.LeftThumbstickUp:
                    return _currentGamePadStates[index].ThumbSticks.Left.Y > 0.0f;
                case Buttons.RightThumbstickLeft:
                    return _currentGamePadStates[index].ThumbSticks.Right.X < 0.0f;
                case Buttons.RightThumbstickDown:
                    return _currentGamePadStates[index].ThumbSticks.Right.Y < 0.0f;
                case Buttons.RightThumbstickRight:
                    return _currentGamePadStates[index].ThumbSticks.Right.X > 0.0f;
                case Buttons.RightThumbstickUp:
                    return _currentGamePadStates[index].ThumbSticks.Right.Y > 0.0f;
                case Buttons.RightTrigger:
                    return _currentGamePadStates[index].Triggers.Right > 0.0f;
                case Buttons.LeftTrigger:
                    return _currentGamePadStates[index].Triggers.Left > 0.0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(button), button, null);
            }
        }
        
        private void WindowOnMouseLeave(object sender, EventArgs e)
        {
            if (HoveredWindow == Window)
                HoveredWindow = null;
        }

        private void WindowOnMouseEnter(object sender, EventArgs e)
        {
            HoveredWindow = Window;
        }
        
        private void WindowOnMouseMove(object sender, MouseMoveEventArgs e)
        {
            MousePosition = e.Position;
        }
    }
}