using System;
using System.Collections.Generic;
using Mana.Graphics;
using Mana.Logging;
using Mana.Utilities;

namespace Mana
{
    public static class Input
    {
        internal static IGameWindow Window;
        
        private static Logger _log = Logger.Create();

        private static Key[] _keyValues;
        private static bool[] _keysDown;
        private static bool[] _lastKeysDown;

        public static event Action<char> KeyTyped;
        public static event Action<Key> KeyPressed;
        public static event Action<Key> KeyReleased;

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
        
        internal static void Initialize(IGameWindow window)
        {
            Window = window;
            
            // Keyboard
            _keyValues = EnumHelper.GetValues<Key>();
            _keysDown = new bool[_keyValues.Length];
            _lastKeysDown = new bool[_keyValues.Length];
        }

        internal static void Update()
        {
            // for (int i = 0; i < _keysDown.Length; i++)
            // {
            //     _lastKeysDown[i] = _keysDown[i];
            // }

            Buffer.BlockCopy(_keysDown, 0, _lastKeysDown, 0, _keysDown.Length);
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
    }
}