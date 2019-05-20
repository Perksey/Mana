using System;

namespace Mana.Utilities
{
    public static class Assert
    {
        public static void AreEqual(object a, object b)
        {
            if (!a.Equals(b))
                throw new Exception("Assertion failed.");
        }
        
        public static void That(bool a)
        {
            if (!a)
                throw new Exception("Assertion failed.");
        }
    }
}