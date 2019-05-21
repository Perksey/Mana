using OpenTK.Graphics.OpenGL4;

namespace Mana.Utilities.Extensions
{
    public static class DebugTypeExtensions
    {
        public static string GetName(this DebugType type)
        {
            return type switch
            {
                DebugType.DontCare => "Don't Care",
                DebugType.DebugTypeError => "Error",
                DebugType.DebugTypeMarker => "Marker",
                DebugType.DebugTypeOther => "Other",
                DebugType.DebugTypePerformance => "Performance",
                DebugType.DebugTypePortability => "Portability",
                DebugType.DebugTypeDeprecatedBehavior => "Deprecated",
                DebugType.DebugTypePopGroup => "Pop Group",
                DebugType.DebugTypePushGroup => "Push Group",
                DebugType.DebugTypeUndefinedBehavior => "Undefined Behavior",
                _ => "???",
            };
        }
    }
}