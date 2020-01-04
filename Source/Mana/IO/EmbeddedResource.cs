using System;
using System.IO;
using System.Reflection;

namespace Mana.IO
{
    public static class EmbeddedResource
    {
        public static byte[] LoadFromAssembly(Assembly assembly, string name)
        {
            using Stream stream = assembly.GetManifestResourceStream(name);

            if (stream == null)
                throw new InvalidOperationException("Embedded resourcen not found.");

            using var reader = new BinaryReader(stream);

            return reader.ReadBytes((int)stream.Length);
        }
    }
}
