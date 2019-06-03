using System.Drawing;

namespace Mana.IMGUI.Viewport
{
    public class GlobalInputHelper
    {
        private MouseHook _mouseHook;
        
        private static object _mouseLock = new object();
        private static Point _mouseLocation;
        public static Point MouseLocation 
        { 
            get
            {
                lock (_mouseLock)
                {
                    return _mouseLocation;
                }
            }

            set
            {
                lock (_mouseLock)
                {
                    _mouseLocation = value;
                }
            }
        }
        
        private object _mouseLeftLock = new object();
        private bool _mouseLeft;
        public bool MouseLeft
        { 
            get
            {
                lock (_mouseLeftLock)
                {
                    return _mouseLeft;
                }
            }

            set
            {
                lock (_mouseLeftLock)
                {
                    _mouseLeft = value;
                }
            }
        }
        
        public GlobalInputHelper()
        {
            _mouseHook = new MouseHook();
            
            _mouseHook.MouseMove += MouseHookCallback;
            _mouseHook.LeftButtonDown += MouseHookOnLeftButtonDown;
            _mouseHook.LeftButtonUp += MouseHookOnLeftButtonUp;

            _mouseHook.Install();
        }

        private void MouseHookOnLeftButtonUp(MouseHook.MSLLHOOKSTRUCT mousestruct)
        {
            MouseLeft = false;
        }

        private void MouseHookOnLeftButtonDown(MouseHook.MSLLHOOKSTRUCT mousestruct)
        {
            MouseLeft = true;
        }

        public Point GetMouseLocation()
        {
            return MouseLocation;
        }
        
        private void MouseHookCallback(MouseHook.MSLLHOOKSTRUCT mouseStruct)
        {
            MouseLocation = new Point(mouseStruct.pt.x, mouseStruct.pt.y);
        }
    }
}