using OpenTK.Graphics.OpenGL4;

namespace Mana.Utilities.Extensions
{
    public static class DebugSourceExtensions
    {
        public static string GetName(this DebugSource source)
        {
            return source switch
            {
                DebugSource.DontCare => "Don't Care",
                DebugSource.DebugSourceApi => "Api",
                DebugSource.DebugSourceApplication => "Application",
                DebugSource.DebugSourceOther => "Other",
                DebugSource.DebugSourceShaderCompiler => "Shader Compiler",
                DebugSource.DebugSourceThirdParty => "Third Party",
                DebugSource.DebugSourceWindowSystem => "Window System",
                _ => "???",
            };
        }
    }
}