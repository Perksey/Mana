namespace Mana.Graphics.Buffers
{
    public enum BufferUsage
    {
        StreamDraw = 35040,
        StreamRead = 35041,
        StreamCopy = 35042,
        StaticDraw = 35044,
        StaticRead = 35045,
        StaticCopy = 35046,
        DynamicDraw = 35048,
        DynamicRead = 35049,
        DynamicCopy = 35050,
    }

    public static class BufferUsageExtensions
    {
        public static bool IsDynamic(this BufferUsage bufferUsage)
        {
            return bufferUsage == BufferUsage.DynamicDraw ||
                   bufferUsage == BufferUsage.DynamicRead ||
                   bufferUsage == BufferUsage.DynamicCopy;
        }
    }
}
