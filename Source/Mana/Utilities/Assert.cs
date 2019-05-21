using System;
using System.Diagnostics;

namespace Mana.Utilities
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void AreEqual(object a, object b)
        {
            if (!a.Equals(b))
                throw new Exception("Assertion failed.");
        }
        
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public static void That(bool a)
        {
            if (!a)
                throw new Exception("Assertion failed.");
        }
    }
}