using System;
using System.Drawing;
using System.Threading;
using ImGuiNET;
using Mana.Utilities;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using Vector2 = System.Numerics.Vector2;

namespace Mana.IMGUI.Viewport
{
    public class ImGuiWindow : ManaWindow
    {
        private static object _createLock = new object();

        private bool _destroyed = false;
        private object _lock = new object();
        private bool _updateStarted = false;

        private ImGuiRenderer _renderer;
        
        public ImGuiWindow(ImGuiViewportManager viewportManager, ImGuiViewportPtr viewport)
        {
            RenderScreenOnResize = false;
            ViewportManager = viewportManager;
            Dispatcher = new ViewportThreadDispatcher();

            Width = (int)viewport.Size.X;
            Height = (int)viewport.Size.Y;

            Location = new Point((int)viewport.Pos.X, (int)viewport.Pos.Y); 
            WindowBorder = WindowBorder.Hidden;

            _renderer = new ImGuiRenderer(viewportManager.ImGuiSystem, this);
            
            lock (_lock)
            {
                RenderContext.Release();    
            }

            _setWindowAlphaFunc = () =>
            {
                lock (_lock)
                {
                    WindowHelper.SetAlpha(this, _pendingAlpha);
                    _pendingAlpha = 1f;
                }
            };
        }
        
        public ImGuiViewportManager ViewportManager { get; }

        internal ViewportThreadDispatcher Dispatcher { get; private set; }
        
        public ImGuiSystem ImGuiSystem => ViewportManager.ImGuiSystem;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            
            if (!_updateStarted)
            {
                _updateStarted = true;
                Dispatcher.SetManagedThreadID(Thread.CurrentThread.ManagedThreadId);
            }
            
            Dispatcher.InvokeActionsInQueue();
            
            if (_destroyed)
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
        }

        protected override void Dispose(bool manual)
        {
            _renderer.Dispose();
            
            base.Dispose(manual);
        }

        public static ImGuiWindow CreateWindow(ImGuiViewportManager viewportManager, 
                                               ImGuiViewportPtr viewport, 
                                               int id)
        {
            ManaTimer t = ManaTimer.StartNew();
            
            ImGuiWindow window = null;

            new Thread(() =>
            {
                Thread.CurrentThread.Name = $"ImGuiViewport[{id}]";

                ManaTimer t2 = ManaTimer.StartNew();
                
                lock (_createLock)
                {
                    window = new ImGuiWindow(viewportManager, viewport);                    
                }

                t2.Tally("Inner Window Ctor");
                
                while (!window._destroyed)
                {
                    Thread.Sleep(1);   
                    window.Dispatcher.InvokeActionsInQueue();
                }

                window.Dispose();

            }).Start();
            
            while (true)
            {
                lock (_createLock)
                {
                    if (window != null)
                        break;
                }

                Thread.Sleep(1);
            }

            
            t.Tally("Window created");
            return window;
        }

        private ImGuiViewportPtr _ptr;
        public void RunOnOwnThread(ImGuiViewportPtr ptr)
        {
            Dispatcher.Invoke(Run);
            //ispatcher.Invoke(RenderContext.Release);
        }

        private Vector2 _pendingLocation = new Vector2(float.MinValue, float.MinValue);
        public void SetWindowPos(Vector2 loc)
        {
            lock (_lock)
            {
                _pendingLocation = loc;                
            }
            
            Dispatcher.Invoke(SetWindowPosImpl);
        }

        private void SetWindowPosImpl()
        {
            lock (_lock)
            {
                Location = new Point((int)_pendingLocation.X, (int)_pendingLocation.Y);
            }
        }

        public Vector2 GetWindowPos()
        {
            lock (_lock)
            {
                return new Vector2(Location.X, Location.Y);
            }
        }

        private Size _pendingSize;
        public void SetWindowSize(Vector2 size)
        {
            lock (_lock)
            {
                _pendingSize = new Size((int)size.X, (int)size.Y);
            }
            
            Dispatcher.Invoke(SetWindowSizeImpl);
        }

        private void SetWindowSizeImpl()
        {
            lock (_lock)
            {
                Size = _pendingSize;
                _pendingSize = Size.Empty;
            }
        }

        private ImGuiViewportPtr _renderingPtr;
        public void Render(ImGuiViewportPtr ptr)
        {
            lock (_lock)
            {
                _renderingPtr = ptr;
                //Render();
            }
            
            Dispatcher.InvokeAndWait(Render);
        }

        public void Render()
        {
            lock (_lock)
            {
                RenderContext.MakeCurrent();
                RenderContext.Clear(Color.Black);
                
                GL.Viewport(0, 0, Width, Height);
                _renderer.RenderDrawData(RenderContext, _renderingPtr.DrawData, Width, Height);
            }
        }
        
        public void SwapBuffers_(ImGuiViewportPtr ptr)
        {
            lock (_lock)
            {
                Dispatcher.Invoke(SwapBuffers_);
            }
        }
        
        public void SwapBuffers_()
        {
            RenderContext.MakeCurrent();
            SwapBuffers();
        }

        public void Destroy()
        {
            lock (_lock)
            {
                Dispatcher.Invoke(() =>
                {
                    _destroyed = true;
                });
            }
        }

        private string _pendingTitle = "";
        public void SetWindowTitle(string ptrToStringAnsi)
        {
            lock (_lock)
            {
                _pendingTitle = ptrToStringAnsi;
            }

            Dispatcher.InvokeAndWait(SetWindowTitleImpl);
        }

        private void SetWindowTitleImpl()
        {
            lock (_lock)
            {
                Title = _pendingTitle;
                _pendingTitle = "";
            }
        }

        private Action _setWindowAlphaFunc;
        private float _pendingAlpha = 1f;
        public void SetWindowAlpha(float alpha)
        {
            lock (_lock)
            {
                _pendingAlpha = alpha;
            }
            
            Dispatcher.InvokeAndWait(_setWindowAlphaFunc);
        }
    }
}