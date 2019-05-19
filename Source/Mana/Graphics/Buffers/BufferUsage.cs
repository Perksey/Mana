using OpenTK.Graphics.OpenGL4;

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
        public static BufferStorageFlags GetBufferStorageFlags(this BufferUsage bufferUsage)
        {
            switch (bufferUsage)
            {
                case BufferUsage.StaticCopy:
                case BufferUsage.StaticRead:
                case BufferUsage.StaticDraw:
                    return BufferStorageFlags.None;
                default:
                    return BufferStorageFlags.DynamicStorageBit;
            }
        }
    }
}
