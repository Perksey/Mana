using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public class GLExtensions
    {
        public readonly bool ARB_DirectStateAccess;
        public readonly bool ARB_TextureStorage;
        public readonly bool KHR_Debug;
        
        public GLExtensions()
        {
            int major = GLHelper.GetInt(GetPName.MajorVersion);
            int minor = GLHelper.GetInt(GetPName.MinorVersion);
            var extensions = new HashSet<string>();

            for (uint i = 0; i < GLHelper.GetInt(GetPName.NumExtensions); i++)
            {
                string extension = GLHelper.GetString(StringNameIndexed.Extensions, i);

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    extensions.Add(extension);
                }
            }

            bool HasExtension(string extensionName) => extensions.Contains(extensionName);

            ARB_DirectStateAccess      = HasExtension("GL_ARB_direct_state_access");
            ARB_TextureStorage         = HasExtension("GL_ARB_texture_storage");
            KHR_Debug                  = HasExtension("GL_KHR_debug");
        }
    }
}