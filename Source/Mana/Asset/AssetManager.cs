using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mana.Asset.Loaders;
using Mana.Graphics;
using Mana.Graphics.Textures;
using Mana.Utilities;
using Mana.Utilities.Algorithm;

namespace Mana.Asset
{
    /// <summary>
    /// Represents an AssetManager capable of loading and managing game assets.
    /// </summary>
    public class AssetManager : IDisposable
    {
        private static Dictionary<Type, IAssetLoader> _assetLoaders = new Dictionary<Type, IAssetLoader>
        {
            [typeof(Texture2D)] = new Texture2DLoader(),
        };
        
        private Action<KeyValuePair<string, IAsset>> _unloadCacheFunc;
        private LockedDictionary<string, IAsset> _assetCache = new LockedDictionary<string, IAsset>();

        public AssetManager(ResourceManager resourceManager)
        {
            ResourceManager = resourceManager;
            _unloadCacheFunc = UnloadCache;

            AsyncRenderContext = new RenderContext(ResourceManager, 
                                                   ResourceManager.MainContext.WindowInfo);
        }
        
        /// <summary>
        /// The <see cref="ResourceManager"/> for the AssetManager. 
        /// </summary>
        public ResourceManager ResourceManager { get; }

        /// <summary>
        /// The <see cref="RenderContext"/> that the AssetManager will use for asynchronous asset loading jobs. 
        /// </summary>
        public RenderContext AsyncRenderContext { get; }
        
        /// <inheritdoc/>
        public void Dispose()
        {
            _assetCache.ForEach(_unloadCacheFunc);
        }

        /// <summary>
        /// Loads an asset of the given type from the given path.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the asset to load.</typeparam>
        /// <param name="path">The path to the given asset.</param>
        /// <returns>The loaded asset.</returns>
        [DebuggerStepThrough]
        public T Load<T>(string path)
            where T : IAsset
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = path.Replace('\\', '/')
                       .Replace('/', Path.DirectorySeparatorChar);

            if (_assetCache.TryGetValue(path, out var cachedAsset))
            {
                if (!(cachedAsset is T castCached))
                    throw new ArgumentException($"Cached asset of type \"{cachedAsset.GetType().Name}\" cannot be" +
                                                $" loaded as type \"{typeof(T).FullName}\".");
                return castCached;
            }
            
            if (!_assetLoaders.TryGetValue(typeof(T), out IAssetLoader loader))
                throw new ArgumentException($"AssetManager does not contain a loader for type \"{typeof(T).FullName}\".");

            if (!(loader is IAssetLoader<T> typedLoader))
                throw new Exception($"Invalid registered loader for type \"{typeof(T).FullName}\".");

            // TODO: Allow streams from nested files (zip/packages)
            var fileStream = File.OpenRead(path);
            
            var asset = typedLoader.Load(this,
                                         AsyncRenderContext.IsCurrent
                                             ? AsyncRenderContext
                                             : ResourceManager.MainContext,
                                         fileStream,
                                         path);

            asset.SourcePath = path;
            asset.AssetManager = this;
            
            asset.OnAssetLoaded();

            return asset;
        }

        /// <summary>
        /// Unloads an asset.
        /// </summary>
        /// <param name="asset">The asset to unload.</param>
        public void Unload(IAsset asset)
        {
            if (!_assetCache.Remove(asset.SourcePath))
                throw new ArgumentException("Asset was not found in AssetManager. This will occur if the SourcePath" +
                                            " is changed manually.");

            // TODO: This
            // asset.IsUnloading = true;
            // OnAssetUnloaded();
        }

        private void UnloadCache(KeyValuePair<string, IAsset> kvp)
        {
            bool removed = _assetCache.Remove(kvp.Key);
            Assert.That(removed);
                    
            // TODO: OnAssetUnloaded(kvp.Value);
            kvp.Value.Dispose();
        }
    }
}