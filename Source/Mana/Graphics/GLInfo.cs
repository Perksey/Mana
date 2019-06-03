using System.Collections.Generic;
using System.Linq;
using Mana.Utilities;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public static class GLInfo
    {
        private static Logger _logger = Logger.Create();
        
        private static bool _initialized = false;

        /// <summary>
        /// Gets the major version number of the current OpenGL version.
        /// </summary>
        public static int Major { get; private set; }
        
        /// <summary>
        /// Gets the minor version number of the current OpenGL version.
        /// </summary>
        public static int Minor { get; private set; }
        
        /// <summary>
        /// Gets the maximum number of texture image units supported by the system.
        /// </summary>
        public static int MaxTextureImageUnits { get; private set; }
        
        /// <summary>
        /// Gets the maximum texture size supported by the system.
        /// </summary>
        public static int MaxTextureSize { get; private set; }
        
        /// <summary>
        /// Gets a value that indicates whether this system's graphics hardware
        /// supports Direct State Access (ARB_DirectStateAccess || Version >= 4.5).
        /// </summary>
        public static bool HasDirectStateAccess { get; private set; }
            
        /// <summary>
        /// Gets a value that indicates whether this system's graphics hardware
        /// supports Buffer Storage (ARB_BufferStorage || Version >= 4.4).
        /// </summary>
        public static bool HasBufferStorage { get; private set; }
        
        /// <summary>
        /// Gets a value that indicates whether this system's graphics hardware
        /// supports Debug (KHR_Debug || Version >= 4.3)
        /// </summary>
        public static bool HasDebug { get; private set; }
        
        /// <summary>
        /// Gets a value that indicates whether this system's graphics hardware
        /// supports Separate Shader Objects (ARB_separate_shader_objects || Version >= 4.1)
        /// </summary>
        public static bool HasSeparateShaderObjects { get; private set; }

        /// <summary>
        /// Populates the properties of the static <see cref="GLInfo"/> class with values from OpenGL.
        /// This will early-exit if the properties have already been loaded.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            bool core = true;

            HashSet<string> extensions;
            if (core)
            {
                extensions = Enumerable.Range(0, GL.GetInteger(GetPName.NumExtensions))
                                       .Select(i => GL.GetString(StringNameIndexed.Extensions, i))
                                       .Where(x => !string.IsNullOrWhiteSpace(x))
                                       .ToHashSet();
            }
            else
            {
                extensions = GL.GetString(StringName.Extensions)
                               .Split(' ')
                               .Where(x => !string.IsNullOrWhiteSpace(x))
                               .ToHashSet();
            }

            int Int(GetPName name) => GL.GetInteger(name);
            bool Ext(string name) => extensions.Contains(name);
            bool Ver(int major, int minor) => (Major * 10) + Minor >= (major * 10) + minor;
            
            Major                    = Int(GetPName.MajorVersion);
            Minor                    = Int(GetPName.MinorVersion);
            MaxTextureImageUnits     = Int(GetPName.MaxCombinedTextureImageUnits);
            MaxTextureSize           = Int(GetPName.MaxTextureSize);
            
            HasDirectStateAccess     = Ext("GL_ARB_direct_state_access")     || Ver(4, 5);
            HasBufferStorage         = Ext("GL_ARB_buffer_storage")          || Ver(4, 4);
            HasDebug                 = Ext("GL_KHR_debug")                   || Ver(4, 3);
            HasSeparateShaderObjects = Ext("GL_ARB_separate_shader_objects") || Ver(4, 1);
            
            // _logger.Debug($"Major:                    {Major}");
            // _logger.Debug($"Minor:                    {Minor}");
            // _logger.Debug($"MaxTextureImageUnits:     {MaxTextureImageUnits}");
            // _logger.Debug($"MaxTextureSize:           {MaxTextureSize}");
            // _logger.Debug($"HasDirectStateAccess:     {HasDirectStateAccess}");
            // _logger.Debug($"HasBufferStorage:         {HasBufferStorage}");
            // _logger.Debug($"HasDebug:                 {HasDebug}");
            // _logger.Debug($"HasSeparateShaderObjects: {HasSeparateShaderObjects}");

            _initialized = true;
        }
    }
}