using System.IO;

namespace Mana
{
    public class FileAssetSource : AssetSource
    {
        public override Stream Stream { get; }
        
        public override string Path { get; }

        public FileAssetSource(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Asset file not found.", filePath);
            }

            Stream = File.OpenRead(filePath);
            Path = filePath;
        }
    }
}