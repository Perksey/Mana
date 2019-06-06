using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using OpenTK.Input;

namespace Mana
{
    public static class Input
    {
        private static InputManager _current;

        public static Point MousePosition => _current.MousePosition;
        
        public static event Action<char> KeyTyped;
        
        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyDown(Key key) => _current.IsKeyDown(key);
        
        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently up.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently up.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsKeyUp(Key key) => _current.IsKeyUp(key);

        /// <summary>
        /// Gets a value that indicates if any keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <returns>A value that indicates if any keyboard <see cref="Key"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAnyKeyDown() => _current.IsAnyKeyDown();

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WasKeyPressed(Key key) => _current.WasKeyPressed(key);

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was released this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was released this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WasKeyReleased(Key key) => _current.WasKeyReleased(key);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently down.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently down.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMouseDown(MouseButton button) => _current.IsMouseDown(button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently up.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently up.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMouseUp(MouseButton button) => _current.IsMouseUp(button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WasMousePressed(MouseButton button) => _current.WasMousePressed(button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was released this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was released this frame.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WasMouseReleased(MouseButton button) => _current.WasMouseReleased(button);

        /// <summary>
        /// Gets a value that indicates whether the given gamepad button is currently down.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <param name="index">The controller index (player).</param>
        /// <returns>A value that indicates whether the given gamepad button is currently down.</returns>
        public static bool IsButtonDown(Buttons button, int index = 0) => _current.IsButtonDown(button, index);

        internal static void SetInputManager(InputManager inputManager)
        {
            _current = inputManager;
        }
        
        internal static void OnKeyTyped(char c)
        {
            KeyTyped?.Invoke(c);
        }
    }
}