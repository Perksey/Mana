using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Mana.Asset.Loaders;
using Mana.Graphics;

namespace Mana.Asset
{
    /// <summary>
    /// An object that will handle the loading and unloading of asset files.
    /// </summary>
    public class AssetManager : IGraphicsResource
    {
        private static Dictionary<Type, IAssetLoader> _assetLoaders = new Dictionary<Type, IAssetLoader>
        {
            [typeof(Texture2D)] = new Texture2DLoader(),
        };

        private Dictionary<string, ManaAsset> _assetCache = new Dictionary<string, ManaAsset>();
        
        public AssetManager(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);
        }

        public GraphicsDevice GraphicsDevice { get; }

        public void Dispose()
        {
            GraphicsDevice.Resources.Remove(this);

            foreach (var kvp in _assetCache)
            {
                bool removed = _assetCache.Remove(kvp.Key);
                Debug.Assert(removed);
                
                OnAssetUnloaded(kvp.Value);
                kvp.Value.Dispose();
            }
        }

        /// <summary>
        /// Loads an asset of the given type from the given path.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the asset to load.</typeparam>
        /// <param name="path">The path to the given asset.</param>
        /// <returns>The loaded asset.</returns>
        [DebuggerStepThrough]
        public T Load<T>(string path)
            where T : ManaAsset
        {
            if (_assetCache.TryGetValue(path, out ManaAsset cachedAsset))
            {
                // Cached asset found.
                
                if (!(cachedAsset is T typedCachedAsset))
                {
                    throw new InvalidOperationException($"Cached asset of type \"{cachedAsset.GetType().FullName}\" cannot be loaded as type \"{typeof(T).FullName}\".");
                }

                return typedCachedAsset;
            }
            
            // Cached asset was not found.
                
            if (!_assetLoaders.TryGetValue(typeof(T), out IAssetLoader loader))
            {
                // Loader for type T was not found.
                throw new InvalidOperationException($"AssetManager does not contain a loader for asset type: \"{typeof(T).FullName}\".");
            }

            if (!(loader is IAssetLoader<T> typedLoader))
            {
                // Loader implements IAssetLoader but not IAssetLoader<T>, or was registered with the wrong type.
                throw new InvalidOperationException($"AssetManager contains an invalid registered loader: \"{loader.GetType().FullName}\".");
            }

            // Load the asset.
            
            AssetSource source = CreateAssetSource(path);

            T asset = typedLoader.Load(this, source);

            if (!typeof(T).IsValueType)
            {
                if (asset is ManaAsset manaAsset)
                {
                    manaAsset.SourcePath = path;
                    _assetCache.Add(path, manaAsset);
                    OnAssetLoaded(manaAsset, source);
                }
                else
                {
                    throw new InvalidOperationException("Cannot load reference type that doesn't implement the IAsset interface.");
                }
            }
            else
            {
                // Asset is value type
            }
            
            return asset;
        }

        /// <summary>
        /// Loads a string from the given path.
        /// This method does not cache the loaded data.
        /// </summary>
        /// <param name="path">The path to load the text from.</param>
        /// <returns>The loaded string.</returns>
        public string LoadString(string path)
        {
            AssetSource source = CreateAssetSource(path);

            using (StreamReader streamReader = new StreamReader(source.Stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Unloads a given <see cref="ManaAsset"/> from the AssetManager, and disposes it.
        /// </summary>
        /// <param name="asset">The <see cref="ManaAsset"/> to unload.</param>
        [DebuggerStepThrough]
        public void Unload(ManaAsset asset)
        {
            if (!_assetCache.Remove(asset.SourcePath))
            {
                throw new ArgumentException("Asset was not found in AssetManager. Was the SourcePath modified?");
            }
            
            OnAssetUnloaded(asset);
            asset.Dispose();
        }

        protected virtual AssetSource CreateAssetSource(string path)
        {
            return new FileAssetSource(path);
        }

        private void OnAssetLoaded(ManaAsset asset, AssetSource source)
        {
        }

        private void OnAssetUnloaded(ManaAsset asset)
        {
        }
        
        /// <summary>
        /// Registers a given <see cref="IAssetLoader"/> to be used to load assets of a given type. 
        /// </summary>
        /// <param name="assetLoader">The <see cref="IAssetLoader"/> to use for loading the given asset type.</param>
        /// <typeparam name="T">The <see cref="ManaAsset"/> type that the loader will load.</typeparam>
        public static void RegisterAssetLoader<T>(IAssetLoader assetLoader)
            where T : ManaAsset
        {
            _assetLoaders.Add(typeof(T), assetLoader);
        }

        /// <summary>
        /// Creates and registers an <see cref="IAssetLoader"/> to load <see cref="ManaAsset"/> objects of the given type
        /// using JSON. 
        /// </summary>
        /// <typeparam name="T">The <see cref="ManaAsset"/> type that the loader will load.</typeparam>
        public static void RegisterJsonLoader<T>()
            where T : ManaAsset
        {
            _assetLoaders.Add(typeof(T), new JsonLoader<T>());
        }
    }
}