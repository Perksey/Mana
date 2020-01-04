using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using osuTK;
using osuTK.Input;

namespace Mana
{
    /// <summary>
    /// Represents a class that handles input management for a <see cref="ManaWindow"/> instance.
    /// </summary>
    public class ManaWindowInputProvider : IInputProvider
    {
        public const int SUPPORTED_GAMEPAD_COUNT = 4;

        private KeyboardState _previousKeyboardState;
        private KeyboardState _currentKeyboardState;

        private MouseState _previousMouseState;
        private MouseState _currentMouseState;

        private GamePadState[] _previousGamePadStates;
        private GamePadState[] _currentGamePadStates;

        public event Action<char> KeyTyped;
        public event Action<int> GamePadConnected;
        public event Action<int> GamePadDisconnected;

        public ManaWindowInputProvider(ManaWindow window)
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
                    GamePadConnected?.Invoke(i);
                }

                if (!_currentGamePadStates[i].IsConnected && _previousGamePadStates[i].IsConnected)
                {
                    GamePadDisconnected?.Invoke(i);
                }
            }
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyDown(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyUp(Key key)
        {
            return _currentKeyboardState.IsKeyUp(key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAnyKeyDown()
        {
            return _currentKeyboardState.IsAnyKeyDown;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasKeyPressed(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key) && _previousKeyboardState.IsKeyUp(key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasKeyReleased(Key key)
        {
            return _previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyUp(key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseDown(MouseButton button)
        {
            return _currentMouseState.IsButtonDown(button);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseUp(MouseButton button)
        {
            return _currentMouseState.IsButtonUp(button);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasMousePressed(MouseButton button)
        {
            return _currentMouseState.IsButtonDown(button) && _previousMouseState.IsButtonUp(button);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WasMouseReleased(MouseButton button)
        {
            return _currentMouseState.IsButtonUp(button) && _previousMouseState.IsButtonDown(button);
        }

        /// <inheritdoc/>
        public bool IsButtonDown(Buttons button, int index = 0)
        {
            return button switch
            {
                Buttons.DPadUp => _currentGamePadStates[index].DPad.IsUp,
                Buttons.DPadDown => _currentGamePadStates[index].DPad.IsDown,
                Buttons.DPadLeft => _currentGamePadStates[index].DPad.IsLeft,
                Buttons.DPadRight => _currentGamePadStates[index].DPad.IsRight,
                Buttons.Start => _currentGamePadStates[index].Buttons.Start == ButtonState.Pressed,
                Buttons.Back => _currentGamePadStates[index].Buttons.Back == ButtonState.Pressed,
                Buttons.LeftStick => _currentGamePadStates[index].Buttons.LeftStick == ButtonState.Pressed,
                Buttons.RightStick => _currentGamePadStates[index].Buttons.RightStick == ButtonState.Pressed,
                Buttons.LeftShoulder => _currentGamePadStates[index].Buttons.LeftShoulder == ButtonState.Pressed,
                Buttons.RightShoulder => _currentGamePadStates[index].Buttons.RightShoulder == ButtonState.Pressed,
                Buttons.Home => _currentGamePadStates[index].Buttons.BigButton == ButtonState.Pressed,
                Buttons.A => _currentGamePadStates[index].Buttons.A == ButtonState.Pressed,
                Buttons.B => _currentGamePadStates[index].Buttons.B == ButtonState.Pressed,
                Buttons.X => _currentGamePadStates[index].Buttons.X == ButtonState.Pressed,
                Buttons.Y => _currentGamePadStates[index].Buttons.Y == ButtonState.Pressed,
                Buttons.LeftThumbstickLeft => _currentGamePadStates[index].ThumbSticks.Left.X < 0.0f,
                Buttons.LeftThumbstickDown => _currentGamePadStates[index].ThumbSticks.Left.Y < 0.0f,
                Buttons.LeftThumbstickRight => _currentGamePadStates[index].ThumbSticks.Left.X > 0.0f,
                Buttons.LeftThumbstickUp => _currentGamePadStates[index].ThumbSticks.Left.Y > 0.0f,
                Buttons.RightThumbstickLeft => _currentGamePadStates[index].ThumbSticks.Right.X < 0.0f,
                Buttons.RightThumbstickDown => _currentGamePadStates[index].ThumbSticks.Right.Y < 0.0f,
                Buttons.RightThumbstickRight =>_currentGamePadStates[index].ThumbSticks.Right.X > 0.0f,
                Buttons.RightThumbstickUp => _currentGamePadStates[index].ThumbSticks.Right.Y > 0.0f,
                Buttons.RightTrigger => _currentGamePadStates[index].Triggers.Right > 0.0f,
                Buttons.LeftTrigger => _currentGamePadStates[index].Triggers.Left > 0.0f,
                _ => throw new ArgumentOutOfRangeException(nameof(button), button, null),
            };
        }

        private void WindowOnMouseLeave(object sender, EventArgs e)
        {
        }

        private void WindowOnMouseEnter(object sender, EventArgs e)
        {
        }

        private void WindowOnMouseMove(object sender, MouseMoveEventArgs e)
        {
            MousePosition = e.Position;
        }
    }
}
