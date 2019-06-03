using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ImGuiNET;
using Mana.Utilities;
using Mana.Utilities.Extensions;
using OpenTK;
using Vector2 = System.Numerics.Vector2;

namespace Mana.IMGUI.Viewport
{
    public unsafe class ImGuiViewportManager
    {
        private static Logger _log = new Logger("Viewport");
        
        private object _bindLock = new object();
        private object _threadLock = new object();
        
        private PlatformFunctionManager _platformFunctionManager;
        
        private Dictionary<IntPtr, ManaWindow> _boundWindows = new Dictionary<IntPtr, ManaWindow>();
        private int _windowID = 1;
        
        private ImGuiPlatformMonitor[] _monitorData;
        private GCHandle _monitorDataPin;

        private ImGuiPlatformMonitorPtr[] _monitorPtrData;
        private GCHandle _monitorPtrDataPin;

        private ImVector<ImGuiPlatformMonitor> _monitorsPtrVector;
        private GCHandle _monitorsPtrVectorPin;

        private GlobalInputHelper _globalInputHelper;
        
        public ImGuiViewportManager(ImGuiSystem imguiSystem)
        {
            ImGuiSystem = imguiSystem;
            
            _platformFunctionManager = new PlatformFunctionManager(this);
            
            ImGuiViewportPtr viewport = ImGui.GetMainViewport();
            var handle = BindWindow(ManaWindow.MainWindow);
            viewport.PlatformHandle = handle;

            SetUpMonitorData();
            
            _globalInputHelper= new GlobalInputHelper();
        }

        public ImGuiSystem ImGuiSystem { get; }
        
        public IntPtr BindWindow(ManaWindow window)
        {
            lock (_bindLock)
            {
                IntPtr id = new IntPtr(_windowID++);
                //var id = window.WindowInfo.Handle;
                _boundWindows.Add(id, window);
                return id;                
            }
        }

        public void UnbindWindow(IntPtr id)
        {
            lock (_bindLock)
            {
                _boundWindows.Remove(id);                
            }
        }

        public ManaWindow GetWindow(IntPtr handle)
        {
            return _boundWindows[handle];
        }
        
        #region Platform Window Functions
        
        public void CreateWindow(ImGuiViewportPtr ptr)
        {
            var window = ImGuiWindow.CreateWindow(this, ptr, _windowID + 1);
            ptr.PlatformHandle = BindWindow(window);
        }

        public void DestroyWindow(ImGuiViewportPtr ptr)
        {
            if (!_boundWindows.TryGetValue(ptr.PlatformHandle, out var window)) return;
            if (!(window is ImGuiWindow imguiWindow)) return;

            imguiWindow.Destroy();

            UnbindWindow(ptr.PlatformHandle);
        }
        
        public void ShowWindow(ImGuiViewportPtr ptr)
        {
            if (!_boundWindows.TryGetValue(ptr.PlatformHandle, out var window)) return;
            if (!(window is ImGuiWindow imguiWindow)) return;
            
            imguiWindow.RunOnOwnThread(ptr);
        }

        public void SetWindowPos(ImGuiViewportPtr ptr, Vector2 pos)
        {
            var offset = ManaWindow.MainWindow.Location; 
            
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    iw.SetWindowPos(new Vector2((pos.X + 8) + offset.X, (pos.Y + 31) + offset.Y));
                    //iw.SetWindowPos(new Vector2((pos.X + 0) + offset.X, (pos.Y + 0) + offset.Y));
                    //iw.SetWindowPos(new Vector2(pos.X , pos.Y));
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public Vector2 GetWindowPos(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    return iw.GetWindowPos();
                }

                //return window.Location.ToVector2();

                // MAIN WINDOW
                //return window.Location.ToVector2();

                return Vector2.Zero;
            }
                
            return Vector2.Zero;
        }
        
        public void SetWindowSize(ImGuiViewportPtr ptr, Vector2 size)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    iw.SetWindowSize(size);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        
        public Vector2 GetWindowSize(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                return new Vector2(window.Width, window.Height);
            }
            
            return Vector2.Zero;
        }

        public void SetWindowFocus(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                // TODO: With OpenTK 4.0?
            }
        }
        
        public bool GetWindowFocus(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                return window.Focused;
            }
            
            _log.Error("Attempting to check Focused of unbound window.");
            return false;
        }

        public bool GetWindowMinimized(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                return window.WindowState == WindowState.Minimized;
            }

            _log.Error("Attempting to check WindowState of unbound window.");
            return false;
        }

        public void SetWindowAlpha(ImGuiViewportPtr ptr, float alpha)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    iw.SetWindowAlpha(alpha);
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public void SetWindowTitle(ImGuiViewportPtr ptr, char* c)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    iw.SetWindowTitle(Marshal.PtrToStringAnsi(new IntPtr(c)));
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public void RenderWindow(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow imguiWindow)
                {
                    imguiWindow.Render(ptr);
                }
            }
            else
            {
                _log.Error("Attempting to render unbound window.");
            }
        }

        public void SwapBuffers(ImGuiViewportPtr ptr)
        {
            if (_boundWindows.TryGetValue(ptr.PlatformHandle, out var window))
            {
                if (window is ImGuiWindow iw)
                {
                    iw.SwapBuffers_(ptr);
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        
        public void UpdateWindow(ImGuiViewportPtr ptr)
        {
        }
        
        #endregion
        
        private void SetUpMonitorData()
        {
            ImGuiPlatformIOPtr platformIO = ImGui.GetPlatformIO();
            
            var platformMonitors = new List<ImGuiPlatformMonitor>();
            
            {
                int i = 0;
                while (true)
                {
                    ImGuiPlatformMonitor m = new ImGuiPlatformMonitor();
                    DisplayDevice display = DisplayDevice.GetDisplay((DisplayIndex)i);
                    if (display == null)
                        break;
                    m.MainPos.X = display.Bounds.X;
                    m.MainPos.Y = display.Bounds.Y;
                    m.MainSize.X = display.Width;
                    m.MainSize.Y = display.Height;
                    m.DpiScale = 1.0f;
                    m.WorkPos = m.MainPos;
                    m.WorkSize = m.MainSize;
                    i++;
                    
                    if (m.MainSize.X < 0.0f || m.MainSize.Y < 0.0f)
                        throw new InvalidOperationException();

                    platformMonitors.Add(m);
                }
            }

            _monitorData = platformMonitors.ToArray();
            _monitorDataPin = GCHandle.Alloc(_monitorData, GCHandleType.Pinned);
            
            var platformMonitorsPtrs = new ImGuiPlatformMonitorPtr[_monitorData.Length];

            for (int i = 0; i < _monitorData.Length; i++)
            {
                fixed (ImGuiPlatformMonitor* ptr = &_monitorData[i])
                {
                    platformMonitorsPtrs[i] = new ImGuiPlatformMonitorPtr(ptr);
                }
            }

            _monitorPtrData = platformMonitorsPtrs;
            _monitorPtrDataPin = GCHandle.Alloc(_monitorPtrData, GCHandleType.Pinned);
            _monitorsPtrVector = new ImVector<ImGuiPlatformMonitor>(_monitorPtrData.Length, 
                                                                       _monitorPtrData.Length, 
                                                                       _monitorDataPin.AddrOfPinnedObject());

            unsafe
            {
                ImGuiPlatformIO* ptr = platformIO;
                ImVector v = Unsafe.As<ImVector<ImGuiPlatformMonitor>, ImVector>(ref _monitorsPtrVector);
                ptr->Monitors = v;
            }
        }

        public IEnumerable<ManaWindow> GetViewportWindows()
        {
            foreach (var window in _boundWindows.Values)
                yield return window;
        }

        public Vector2 GlobalMouseLocation => GlobalInputHelper.MouseLocation.ToVector2();
        public bool GlobalMouseLeft => _globalInputHelper.MouseLeft;
    }
}