using System;
using System.Diagnostics;

namespace Mana.Utilities
{
    /// <summary>
    /// A utility class of debug-only assertions that simply throw exceptions.
    /// </summary>
    public static class Assert
    {
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void That(bool value)
        {
            if (!value)
                throw new Exception("Assertion failed.");
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void NotNull<T>(T instance)
            where T : class
        {
            That(instance != null);
        }
        
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void IsNull<T>(T instance)
            where T : class
        {
            That(instance == null);
        }
    }
}