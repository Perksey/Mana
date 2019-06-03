using System;
using System.Collections.Generic;
using System.Drawing;
using Mana.Utilities;
using OpenTK;
using OpenTK.Input;

namespace Mana.Input
{
    /// <summary>
    /// Represents a class that handles input management for a <see cref="ManaWindow"/> instance.
    /// </summary>
    public class InputManager
    {
        public static readonly Key[] AllKeys = EnumHelper.GetValues<Key>();

        private KeyboardState _currentKeyboardState;

        private int _mouseWheel;
        private int _currentMouseWheel;
        private int _previousMouseWheel;
        
        public int Index = 0;
        public static int currentIndex = 0;
        
        public static ManaWindow HoveredWindow = null;
        
        public InputManager(ManaWindow window)
        {
            Index = currentIndex;
            currentIndex++;
            
            Window = window;

            Window.MouseMove += WindowOnMouseMove;
            Window.MouseDown += WindowOnMouseDown;
            Window.MouseUp += WindowOnMouseUp;
            
            Window.MouseEnter += WindowOnMouseEnter;
            Window.MouseLeave += WindowOnMouseLeave;
            Window.MouseWheel += WindowOnMouseWheel;

            Window.KeyPress += WindowOnKeyPress;
        }
        
        public ManaWindow Window { get; }
        
        public Queue<char> CharacterBuffer { get; } = new Queue<char>(2048);

        public Point MousePosition { get; private set; }

        public bool MouseLeft { get; private set; }

        public bool MouseMiddle { get; private set; }

        public bool MouseRight { get; private set; }

        public int MouseWheel => _currentMouseWheel;

        public int MouseWheelDelta => _currentMouseWheel - _previousMouseWheel;
        
        public bool IsKeyDown(Key key)
        {
            return _currentKeyboardState.IsKeyDown(key);
        }
        
        public void Update()
        {
            _currentKeyboardState = Keyboard.GetState();

            _previousMouseWheel = _currentMouseWheel;
            _currentMouseWheel = _mouseWheel;
        }
        
        private void WindowOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _mouseWheel = e.Value;
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

        private void WindowOnKeyPress(object sender, KeyPressEventArgs e)
        {
            CharacterBuffer.Enqueue(e.KeyChar);
        }

        private void WindowOnMouseMove(object sender, MouseMoveEventArgs e)
        {
            MousePosition = new Point(e.X, e.Y);
        }

        private void WindowOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    MouseLeft = true;
                    break;
                case MouseButton.Middle:
                    MouseMiddle = true;
                    break;
                case MouseButton.Right:
                    MouseRight = true;
                    break;
                case MouseButton.Button1:
                    break;
                case MouseButton.Button2:
                    break;
                case MouseButton.Button3:
                    break;
                case MouseButton.Button4:
                    break;
                case MouseButton.Button5:
                    break;
                case MouseButton.Button6:
                    break;
                case MouseButton.Button7:
                    break;
                case MouseButton.Button8:
                    break;
                case MouseButton.Button9:
                    break;
                case MouseButton.LastButton:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WindowOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButton.Left:
                    MouseLeft = false;
                    break;
                case MouseButton.Middle:
                    MouseMiddle = false;
                    break;
                case MouseButton.Right:
                    MouseRight = false;
                    break;
                case MouseButton.Button1:
                    break;
                case MouseButton.Button2:
                    break;
                case MouseButton.Button3:
                    break;
                case MouseButton.Button4:
                    break;
                case MouseButton.Button5:
                    break;
                case MouseButton.Button6:
                    break;
                case MouseButton.Button7:
                    break;
                case MouseButton.Button8:
                    break;
                case MouseButton.Button9:
                    break;
                case MouseButton.LastButton:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}