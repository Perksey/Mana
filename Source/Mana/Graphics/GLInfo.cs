using System;
using System.Collections.Generic;
using System.Linq;
using Mana.Utilities;
using osuTK.Graphics.OpenGL;

namespace Mana.Graphics
{
    /// <summary>
    /// A static class of useful OpenGL information.
    /// </summary>
    public static class GLInfo
    {
        private static Logger _log = Logger.Create();
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
        /// Gets a <see cref="Version"/> object representing the current OpenGL Version.
        /// </summary>
        public static Version Version { get; private set; }

        /// <summary>
        /// Gets the OpenGL string: 'ShadingLanguageVersion'
        /// </summary>
        public static string ShadingLanguageVersion { get; private set; }

        /// <summary>
        /// Gets the OpenGL string: 'Vendor'
        /// </summary>
        public static string Vendor { get; private set; }

        /// <summary>
        /// Gets the OpenGL string: 'Renderer'
        /// </summary>
        public static string Renderer { get; private set; }

        /// <summary>
        /// Gets a HashSet of available OpenGL extension names. For best performance, the presence of an extension
        /// should be cached rather than continuously queried.
        /// </summary>
        public static HashSet<string> Extensions { get; private set; }

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
        /// Populates the properties of <see cref="GLInfo"/> with values from OpenGL.
        /// This will early-exit if the properties have already been loaded.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            // Extensions = new HashSet<string>(GL.GetString(StringName.Extensions)
            //                                    .Split(' ')
            //                                    .Where(x => !string.IsNullOrWhiteSpace(x)));

            int numExtensions = GL.GetInteger(GetPName.NumExtensions);

            Extensions = new HashSet<string>(Enumerable.Range(0, numExtensions)
                                                       .Select(n => GL.GetString(StringNameIndexed.Extensions, n)));

            bool Ext(string name) => Extensions.Contains(name);

            Major                    = Int(GetPName.MajorVersion);
            Minor                    = Int(GetPName.MinorVersion);
            MaxTextureImageUnits     = Int(GetPName.MaxCombinedTextureImageUnits);
            MaxTextureSize           = Int(GetPName.MaxTextureSize);

            ShadingLanguageVersion   = Str(StringName.ShadingLanguageVersion);
            Vendor                   = Str(StringName.Vendor);
            Renderer                 = Str(StringName.Renderer);

            Version = new Version(Major, Minor);


            HasDirectStateAccess     = Ext("GL_ARB_direct_state_access")     || Ver(4, 5);
            HasBufferStorage         = Ext("GL_ARB_buffer_storage")          || Ver(4, 4);
            HasDebug                 = Ext("GL_KHR_debug")                   || Ver(4, 3);
            HasSeparateShaderObjects = Ext("GL_ARB_separate_shader_objects") || Ver(4, 1);

            _initialized = true;

            _log.Info($"OpenGL Version: {Major}.{Minor}");
            _log.Info($"numExtensions: {numExtensions}");
            _log.Info($"Extensions.Count: {Extensions.Count}");
            _log.Info($"HasDirectStateAccess: {HasDirectStateAccess}");
            _log.Info($"HasBufferStorage: {HasBufferStorage}");
            _log.Info($"HasDebug: {HasDebug}");
            _log.Info($"HasSeparateShaderObjects: {HasSeparateShaderObjects}");
        }

        private static int Int(GetPName name) => GL.GetInteger(name);

        private static string Str(StringName name) => GL.GetString(name);

        private static bool Ver(int major, int minor) => (Major * 10) + Minor >= (major * 10) + minor;
    }
}
