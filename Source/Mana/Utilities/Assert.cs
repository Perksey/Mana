using System;

namespace Mana.Utilities
{
    public static class Assert
    {
        public static void That(bool value)
        {
            if (!value)
                throw new Exception();
        }
    }
}
