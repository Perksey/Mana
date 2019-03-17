using System.IO;
using Newtonsoft.Json;

namespace Mana.Asset.Loaders
{
    public class JsonLoader<T> : IAssetLoader<T>
        where T : ManaAsset
    {
        public T Load(AssetManager manager, AssetSource assetSource)
        {
            using (StreamReader streamReader = new StreamReader(assetSource.Stream))
            {
                return JsonConvert.DeserializeObject<T>(streamReader.ReadToEnd());
            }
        }
    }
}