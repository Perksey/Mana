using System.IO;

namespace Mana
{
    public abstract class AssetSource
    {
        /// <summary>
        /// Gets a stream through which the Asset's data can be retrieved.
        /// </summary>
        public abstract Stream Stream { get; }
        
        /// <summary>
        /// Gets the source path that the Asset was loaded from.
        /// </summary>
        public abstract string Path { get; }
    }
}