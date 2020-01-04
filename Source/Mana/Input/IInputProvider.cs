using System;
using System.Drawing;
using osuTK.Input;

namespace Mana
{
    public interface IInputProvider
    {
        event Action<char> KeyTyped;

        /// <summary>
        /// Gets a value that indicates the current mouse position relative to the window's viewport area.
        /// </summary>
        Point MousePosition { get; }

        /// <summary>
        /// Gets a value that indicates whether the left mouse button is currently down.
        /// </summary>
        bool MouseLeft { get; }

        /// <summary>
        /// Gets a value that indicates whether the middle mouse button is currently down.
        /// </summary>
        bool MouseMiddle { get; }

        /// <summary>
        /// Gets a value that indicates whether the right mouse button is currently down.
        /// </summary>
        bool MouseRight { get; }

        /// <summary>
        /// Gets a value that indicates the current mouse wheel value.
        /// </summary>
        int MouseWheel { get; }

        /// <summary>
        /// Gets a value that indicates the change in mouse wheel value since the last frame.
        /// </summary>
        int MouseWheelDelta { get; }

        /// <summary>
        /// Updates the input provider's internal state (used by polling methods.)
        /// </summary>
        void Update();

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently down.</returns>
        bool IsKeyDown(Key key);

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> is currently up.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> is currently up.</returns>
        bool IsKeyUp(Key key);

        /// <summary>
        /// Gets a value that indicates if any keyboard <see cref="Key"/> is currently down.
        /// </summary>
        /// <returns>A value that indicates if any keyboard <see cref="Key"/> is currently down.</returns>
        bool IsAnyKeyDown();

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was pressed this frame.</returns>
        bool WasKeyPressed(Key key);

        /// <summary>
        /// Gets a value that indicates whether the given keyboard <see cref="Key"/> was released this frame.
        /// </summary>
        /// <param name="key">The <see cref="Key"/> to check.</param>
        /// <returns>A value that indicates whether the given keyboard <see cref="Key"/> was released this frame.</returns>
        bool WasKeyReleased(Key key);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently down.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently down.</returns>
        bool IsMouseDown(MouseButton button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> is currently up.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> is currently up.</returns>
        bool IsMouseUp(MouseButton button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was pressed this frame.</returns>
        bool WasMousePressed(MouseButton button);

        /// <summary>
        /// Gets a value that indicates whether the given <see cref="MouseButton"/> was released this frame.
        /// </summary>
        /// <param name="button">The <see cref="MouseButton"/> to check.</param>
        /// <returns>A value that indicates whether the given <see cref="MouseButton"/> was released this frame.</returns>
        bool WasMouseReleased(MouseButton button);

        /// <summary>
        /// Gets a value that indicates whether the given gamepad button is currently down.
        /// </summary>
        /// <param name="button">The gamepad button to check.</param>
        /// <param name="index">The controller index (player).</param>
        /// <returns>A value that indicates whether the given gamepad button is currently down.</returns>
        bool IsButtonDown(Buttons button, int index = 0);
    }
}
