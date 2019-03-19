namespace Mana.Graphics
{
    public static class GraphicsMetrics
    {
        internal static int _clearCount = 0;
        internal static int _drawCalls = 0;
        internal static int _primitiveCount = 0;

        internal static void Reset()
        {
            _clearCount = 0;
            _drawCalls = 0;
            _primitiveCount = 0;
        }
    }
}