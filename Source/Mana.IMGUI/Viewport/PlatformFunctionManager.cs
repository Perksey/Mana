using System.Numerics;
using System.Runtime.InteropServices;
using ImGuiNET;

namespace Mana.IMGUI.Viewport
{
    public unsafe class PlatformFunctionManager
    {
        public delegate void CreateWindowFunc(ImGuiViewportPtr ptr);
        public delegate void DestroyWindowFunc(ImGuiViewportPtr ptr);
        public delegate void ShowWindowFunc(ImGuiViewportPtr ptr);
        public delegate void SetWindowPosFunc(ImGuiViewportPtr ptr, Vector2 pos);
        public delegate Vector2 GetWindowPosFunc(ImGuiViewportPtr ptr);
        public delegate void SetWindowSizeFunc(ImGuiViewportPtr ptr, Vector2 size);
        public delegate Vector2 GetWindowSizeFunc(ImGuiViewportPtr ptr);
        public delegate void SetWindowFocusFunc(ImGuiViewportPtr ptr);
        public delegate bool GetWindowFocusFunc(ImGuiViewportPtr ptr);
        public delegate bool GetWindowMinimizedFunc(ImGuiViewportPtr ptr);
        public delegate void SetWindowAlphaFunc(ImGuiViewportPtr ptr, float alpha);
        public delegate void SetWindowTitleFunc(ImGuiViewportPtr ptr, char* c);
        public delegate void RenderWindowFunc(ImGuiViewportPtr ptr);
        public delegate void UpdateWindowFunc(ImGuiViewportPtr ptr);
        public delegate void SwapBuffersFunc(ImGuiViewportPtr ptr);
        
        private CreateWindowFunc _createWindowDelegate;
        private DestroyWindowFunc _destroyWindowDelegate;
        private ShowWindowFunc _showWindowDelegate;
        private SetWindowPosFunc _setWindowPosDelegate;
        private GetWindowPosFunc _getWindowPosDelegate;
        private SetWindowSizeFunc _setWindowSizeDelegate;
        private GetWindowSizeFunc _getWindowSizeDelegate;
        private SetWindowFocusFunc _setWindowFocusDelegate;
        private GetWindowFocusFunc _getWindowFocusDelegate;
        private GetWindowMinimizedFunc _getWindowMinimizedDelegate;
        private SetWindowAlphaFunc _setWindowAlphaDelegate;
        private SetWindowTitleFunc _setWindowTitleDelegate;
        private RenderWindowFunc _renderWindowDelegate;
        // private UpdateWindowFunc _updateWindowDelegate;
        private SwapBuffersFunc _swapBuffersDelegate;

        public PlatformFunctionManager(ImGuiViewportManager manager)
        {
            var platformIO = ImGui.GetPlatformIO();
            
            _createWindowDelegate       = manager.CreateWindow;
            _destroyWindowDelegate      = manager.DestroyWindow;
            _showWindowDelegate         = manager.ShowWindow;
            _setWindowPosDelegate       = manager.SetWindowPos;
            _getWindowPosDelegate       = manager.GetWindowPos;
            _setWindowSizeDelegate      = manager.SetWindowSize;
            _getWindowSizeDelegate      = manager.GetWindowSize;
            _setWindowFocusDelegate     = manager.SetWindowFocus;
            _getWindowFocusDelegate     = manager.GetWindowFocus;
            _getWindowMinimizedDelegate = manager.GetWindowMinimized;
            _setWindowAlphaDelegate     = manager.SetWindowAlpha;
            _setWindowTitleDelegate     = manager.SetWindowTitle;
            _renderWindowDelegate       = manager.RenderWindow; 
            // _updateWindowDelegate       = manager.UpdateWindow;
            _swapBuffersDelegate        = manager.SwapBuffers;
           
            platformIO.Platform_CreateWindow       = Marshal.GetFunctionPointerForDelegate(_createWindowDelegate);
            platformIO.Platform_DestroyWindow      = Marshal.GetFunctionPointerForDelegate(_destroyWindowDelegate);
            platformIO.Platform_ShowWindow         = Marshal.GetFunctionPointerForDelegate(_showWindowDelegate);
            platformIO.Platform_SetWindowPos       = Marshal.GetFunctionPointerForDelegate(_setWindowPosDelegate);
            platformIO.Platform_GetWindowPos       = Marshal.GetFunctionPointerForDelegate(_getWindowPosDelegate);
            platformIO.Platform_SetWindowSize      = Marshal.GetFunctionPointerForDelegate(_setWindowSizeDelegate);
            platformIO.Platform_GetWindowSize      = Marshal.GetFunctionPointerForDelegate(_getWindowSizeDelegate);
            platformIO.Platform_SetWindowFocus     = Marshal.GetFunctionPointerForDelegate(_setWindowFocusDelegate);
            platformIO.Platform_GetWindowFocus     = Marshal.GetFunctionPointerForDelegate(_getWindowFocusDelegate);
            platformIO.Platform_GetWindowMinimized = Marshal.GetFunctionPointerForDelegate(_getWindowMinimizedDelegate);
            platformIO.Platform_SetWindowAlpha     = Marshal.GetFunctionPointerForDelegate(_setWindowAlphaDelegate);
            platformIO.Platform_SetWindowTitle     = Marshal.GetFunctionPointerForDelegate(_setWindowTitleDelegate);
            platformIO.Platform_RenderWindow       = Marshal.GetFunctionPointerForDelegate(_renderWindowDelegate);
            // platformIO.Platform_UpdateWindow       = Marshal.GetFunctionPointerForDelegate(_updateWindowDelegate);
            platformIO.Platform_SwapBuffers        = Marshal.GetFunctionPointerForDelegate(_swapBuffersDelegate);
        }
    }
}