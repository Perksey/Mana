using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class GLExtensions
    {
        public readonly bool ARB_DirectStateAccess;
        public readonly bool ARB_BufferStorage;
        public readonly bool ARB_TextureStorage;
        public readonly bool KHR_Debug;
        public readonly int Major;
        public readonly int Minor;
        
        public GLExtensions()
        {
            var extensions = new HashSet<string>();
            
            Major = GLHelper.GetInteger(GetPName.MajorVersion);
            Minor = GLHelper.GetInteger(GetPName.MinorVersion);

            for (uint i = 0; i < GLHelper.GetInteger(GetPName.NumExtensions); i++)
            {
                string extension = GLHelper.GetString(StringNameIndexed.Extensions, i);

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    extensions.Add(extension);
                }
            }

            bool HasExtension(string extensionName) => extensions.Contains(extensionName);

            ARB_DirectStateAccess      = HasExtension("GL_ARB_direct_state_access");
            ARB_BufferStorage          = HasExtension("GL_ARB_buffer_storage");
            ARB_TextureStorage         = HasExtension("GL_ARB_texture_storage");
            KHR_Debug                  = HasExtension("GL_KHR_debug");
        }
    }
}