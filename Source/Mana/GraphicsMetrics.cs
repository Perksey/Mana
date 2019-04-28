namespace Mana
{
    public static class Metrics
    {
        internal static long _clearCount = 0;
        internal static long _drawCalls = 0;
        internal static long _primitiveCount = 0;
        internal static int _framesPerSecond = 0;
        internal static float _totalMegabytes = 0;

        public static long ClearCount => _clearCount;
        public static long DrawCalls => _drawCalls;
        public static long PrimitiveCount => _primitiveCount;
        public static int FramesPerSecond => _framesPerSecond;
        public static float MillisecondsPerFrame => (1f / FramesPerSecond) * 1000f;
        public static float TotalMegabytes => _totalMegabytes;
        
        internal static void Reset()
        {
            _clearCount = 0;
            _drawCalls = 0;
            _primitiveCount = 0;
            
            // FramesPerSecond and TotalMegabytes are set to absolute values,
            // so they don't have to be reset every frame.
        }
    }
}