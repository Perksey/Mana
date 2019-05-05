using System.IO;
using Newtonsoft.Json;

namespace Mana.Asset.Loaders
{
    public class JsonLoader<T> : IAssetLoader<T>
        where T : ManaAsset
    {
        public T Load(AssetManager manager, Stream sourceStream, string sourcePath)
        {
            using (StreamReader streamReader = new StreamReader(sourceStream))
            {
                return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
            }
        }
    }
}