using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Mana.Graphics;
using Mana.Logging;
using Mana.Utilities;
using OpenTK.Input;

namespace Mana
{
    public static class Input
    {
        internal static IGameWindow Window;
        
        private static Logger _log = Logger.Create();

        private static Key[] _keyValues;
        private static bool[] _keysDown;
        private static bool[] _lastKeysDown;

        private static MouseState _mouseState;
        private static MouseButton[] _mouseButtonValues;
        private static bool[] _mouseButtonsDown;
        private static bool[] _lastMouseButtonsDown;
        private static int _lastMouseWheel;
        

        public static event Action<char> KeyTyped;
        public static event Action<Key> KeyPressed;
        public static event Action<Key> KeyReleased;

        public static event Action<MouseButton> MouseButtonPressed;
        public static event Action<MouseButton> MouseButtonReleased;

        public static Vector2 MousePosition { get; private set; } = Vector2.Zero;
        public static int MouseWheel { get; private set; } = 0;

        public static int MouseWheelDelta { get; private set; } = 0;
        
        #region Keyboard

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="Key"/> is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> value.</param>
        /// <returns>A value that indicates whether the specified <see cref="Key"/> is currently down.</returns>
        public static bool IsKeyDown(Key key)
        {
            return _keysDown[(int)key];
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="Key"/> is currently up.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> value.</param>
        /// <returns>A value that indicates whether the specified <see cref="Key"/> is currently up.</returns>
        public static bool IsKeyUp(Key key)
        {
            return !_keysDown[(int)key];
        }

        /// <summary>
        /// Gets a value that indicates whether any <see cref="Key"/> is currently down.
        /// </summary>
        /// <returns>A value that indicates whether any <see cref="Key"/> is currently down.</returns>
        public static bool IsAnyKeyDown()
        {
            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (_keysDown[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> value.</param>
        /// <returns>A value that indicates if the given <see cref="Key"/> was pressed this frame.</returns>
        public static bool WasKeyPressed(Key key)
        {
            return _keysDown[(int)key] && !_lastKeysDown[(int)key];
        }

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="Key"/> was released this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> value.</param>
        /// <returns>A value that indicates if the given <see cref="Key"/> was released this frame.</returns>
        public static bool WasKeyReleased(Key key)
        {
            return !_keysDown[(int)key] && _lastKeysDown[(int)key];
        }

        /// <summary>
        /// Gets a value that indicates whether any <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <returns>A value that indicates whether any <see cref="Key"/> was pressed this frame.</returns>
        public static bool WasAnyKeyPressed()
        {
            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (_keysDown[i] && !_lastKeysDown[i])
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets a value that indicates whether any <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> that was pressed this frame.</param>
        /// <returns>A value that indicates whether any <see cref="Key"/> was pressed this frame.</returns>
        public static bool WasAnyKeyPressed(out Key key)
        {
            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (_keysDown[i] && !_lastKeysDown[i])
                {
                    key = (Key)i;
                    return true;
                }
            }

            key = Key.Unknown;
            return false;
        }
        
        /// <summary>
        /// Gets a value that indicates whether any <see cref="Key"/> was released this frame.
        /// </summary>
        /// <returns>A value that indicates whether any <see cref="Key"/> was released this frame.</returns>
        public static bool WasAnyKeyReleased()
        {
            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (!_keysDown[i] && _lastKeysDown[i])
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// Gets a value that indicates whether any <see cref="Key"/> was released this frame.
        /// </summary>
        /// /// <param name="key">The <see cref="Key"/> that was released this frame.</param>
        /// <returns>A value that indicates whether any <see cref="Key"/> was released this frame.</returns>
        public static bool WasAnyKeyReleased(out Key key)
        {
            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (!_keysDown[i] && _lastKeysDown[i])
                {
                    key = (Key)i;
                    return true;
                }
            }

            key = Key.Unknown; 
            return false;
        }

        /// <summary>
        /// Retrieves an IEnumerable of all currently pressed keyboard keys.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Key> GetPressedKeys()
        {
            var keys = new List<Key>(8);

            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (_keysDown[i])
                {
                    keys.Add((Key)i);
                }
            }

            return keys;
        }
        
        /// <summary>
        /// Retrieves a list of all currently pressed keyboard keys. This overload accepts an existing list,
        /// clears it, and adds the 
        /// </summary>
        /// <param name="keyList"></param>
        public static void GetPressedKeys(List<Key> keyList)
        {
            keyList.Clear();

            for (int i = 0; i < _keyValues.Length; i++)
            {
                if (_keysDown[i])
                {
                    keyList.Add((Key)i);
                }
            }
        }
        
        #endregion
        
        #region Mouse
        
        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently down.
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButton"/> value.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently down.</returns>
        public static bool IsMouseDown(MouseButton mouseButton)
        {
            return _mouseButtonsDown[(int)mouseButton];
        }
        
        #endregion
        
        internal static void Initialize(IGameWindow window)
        {
            Window = window;
            
            // Keyboard
            _keyValues = EnumHelper.GetValues<Key>();
            _keysDown = new bool[_keyValues.Length];
            _lastKeysDown = new bool[_keyValues.Length];
            
            // Mouse
            _mouseButtonValues = EnumHelper.GetValues<MouseButton>();
            _mouseButtonsDown = new bool[_mouseButtonValues.Length];
            _lastMouseButtonsDown = new bool[_mouseButtonValues.Length];
        }

        internal static void Update()
        {
            // for (int i = 0; i < _keysDown.Length; i++)
            // {
            //     _lastKeysDown[i] = _keysDown[i];
            // }

            Buffer.BlockCopy(_keysDown, 0, _lastKeysDown, 0, _keysDown.Length);
            Buffer.BlockCopy(_mouseButtonsDown, 0, _lastMouseButtonsDown, 0, _mouseButtonsDown.Length);

            MouseWheelDelta = MouseWheel - _lastMouseWheel;
            _lastMouseWheel = MouseWheel;
        }

        public static void OnKeyTyped(char character)
        {
            KeyTyped?.Invoke(character);
        }

        public static void OnKeyPressed(Key key)
        {
            bool state = _keysDown[(int)key];
            
            _keysDown[(int)key] = true;

            if (!state)
            {
                KeyPressed?.Invoke(key);
            }
        }

        public static void OnKeyReleased(Key key)
        {
            bool state = _keysDown[(int)key];
            
            _keysDown[(int)key] = false;

            if (state)
            {
                KeyReleased?.Invoke(key);    
            }
        }

        public static void OnMouseButtonPressed(MouseButton mouseButton)
        {
            bool state = _mouseButtonsDown[(int)mouseButton];

            _mouseButtonsDown[(int)mouseButton] = true;

            if (!state)
            {
                MouseButtonPressed?.Invoke(mouseButton);
            }
        }

        public static void OnMouseButtonReleased(MouseButton mouseButton)
        {
            bool state = _mouseButtonsDown[(int)mouseButton];

            _mouseButtonsDown[(int)mouseButton] = false;

            if (state)
            {
                MouseButtonReleased?.Invoke(mouseButton);
            }
        }

        public static void OnMouseMoved(Point position)
        {
            MousePosition = new Vector2(position.X, position.Y);
        }

        public static void OnMouseScroll(int delta, int value)
        {
            MouseWheel = value;
        }
    }
}