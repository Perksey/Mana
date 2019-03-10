namespace Mana.Graphics
{
    public static class GraphicsMetrics
    {
        internal static int _clearCount = 0;

        internal static void Reset()
        {
            _clearCount = 0;
        }
    }
}