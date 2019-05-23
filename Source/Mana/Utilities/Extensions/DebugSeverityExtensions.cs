using OpenTK.Graphics.OpenGL4;

namespace Mana.Utilities.Extensions
{
    public static class DebugSeverityExtensions
    {
        public static string GetName(this DebugSeverity severity)
        {
            return severity switch 
            {
                DebugSeverity.DontCare => "Don't Care",
                DebugSeverity.DebugSeverityHigh => "High",
                DebugSeverity.DebugSeverityMedium => "Medium",
                DebugSeverity.DebugSeverityLow => "Low",
                DebugSeverity.DebugSeverityNotification => "Notif",
                _ => "???",
            };
        }
    }
}