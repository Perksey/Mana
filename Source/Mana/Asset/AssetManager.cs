using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Mana.Asset.Loaders;
using Mana.Asset.Reloading;
using Mana.Graphics;
using Mana.Graphics.Shaders;
using Mana.Graphics.Textures;
using Mana.Utilities.Algorithm;

namespace Mana.Asset
{
    /// <summary>
    /// Represents an AssetManager capable of loading and managing game assets.
    /// </summary>
    public class AssetManager : IDisposable
    {
        internal static readonly Texture2DLoader Texture2DLoader = new Texture2DLoader();
        internal static readonly ShaderProgramLoader ShaderProgramLoader = new ShaderProgramLoader();

        private static Dictionary<Type, IAssetLoader> _assetLoaders = new Dictionary<Type, IAssetLoader>
        {
            [typeof(Texture2D)] = Texture2DLoader,
            [typeof(ShaderProgram)] = ShaderProgramLoader,
        };

        internal readonly object AssetLock = new object();
        internal readonly AssetReloader AssetReloader;

        private LockedDictionary<string, IAsset> _assetCache = new LockedDictionary<string, IAsset>();
        private List<IAsset> _loadedAssets = new List<IAsset>();

        public AssetManager(RenderContext renderContext)
        {
            RenderContext = renderContext;
            RenderContext.Validate(true);

            AsyncRenderContext = RenderContext.CreateOffscreen();
            AssetReloader = new AssetReloader(this);
        }

        public bool Disposed { get; private set; }

        public string RootPath { get; set; } = "";

        /// <summary>
        /// The main <see cref="RenderContext"/> used by the AssetManager.
        /// </summary>
        public RenderContext RenderContext { get; set;  }

        /// <summary>
        /// The <see cref="RenderContext"/> the AssetManager will use for asynchronous asset loading jobs.
        /// </summary>
        public RenderContext AsyncRenderContext { get; }

        public void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Dispose(true);
            GC.SuppressFinalize(this);

            Disposed = true;
        }

        /// <summary>
        /// Loads an asset of the given type from the given path.
        /// </summary>
        /// <typeparam name="TAsset">The <see cref="Type"/> of the asset to load.</typeparam>
        /// <param name="path">The path to the given asset.</param>
        /// <param name="liveReload">Whether the asset will be reloaded upon being changed.</param>
        /// <returns>The loaded asset.</returns>
        [DebuggerStepThrough]
        public TAsset Load<TAsset>(string path, bool liveReload = false)
            where TAsset : IAsset
        {
            path = Path.GetFullPath(Path.Combine(RootPath, path));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            path = path.Replace('\\', '/')
                       .Replace('/', Path.DirectorySeparatorChar);

            if (_assetCache.TryGetValue(path, out var cachedAsset))
            {
                if (!(cachedAsset is TAsset castCached))
                    throw new ArgumentException($"Cached asset of type \"{cachedAsset.GetType().Name}\" cannot be" +
                                                $" loaded as type \"{typeof(TAsset).FullName}\".");
                return castCached;
            }

            if (!_assetLoaders.TryGetValue(typeof(TAsset), out IAssetLoader loader))
                throw new ArgumentException($"AssetManager does not contain a loader for type \"{typeof(TAsset).FullName}\".");

            if (!(loader is IAssetLoader<TAsset> typedLoader))
                throw new Exception($"Invalid registered loader for type \"{typeof(TAsset).FullName}\".");

            var stream = GetStreamFromPath(path);

            TAsset asset = typedLoader.Load(this,
                                            AsyncRenderContext.IsCurrent
                                                ? AsyncRenderContext
                                                : RenderContext,
                                            stream,
                                            path);

            asset.SourcePath = path;
            asset.AssetManager = this;

            _loadedAssets.Add(asset);
            _assetCache.Add(path, asset);

            OnAssetLoaded(asset, liveReload);

            return asset;
        }

        public Stream GetStreamFromPath(string path)
        {
            // TODO: Allow streams from nested files (zip/packages)
            return File.OpenRead(path);
        }

        public void Unload(IAsset asset)
        {
            if (!_assetCache.Remove(asset.SourcePath))
                throw new ArgumentException("Asset was not found in AssetManager. This will occur if the asset's " +
                                            "SourcePath value is changed manually.");

            if (asset is IReloadableAsset reloadableAsset)
                AssetReloader.RemoveReloadable(reloadableAsset);

            OnAssetUnloading(asset);

            _loadedAssets.Remove(asset);
            asset.Dispose();
        }

        public void Update()
        {
            AssetReloader.Update();
        }

        protected virtual void Dispose(bool disposing)
        {
            foreach (var loadedAsset in _loadedAssets.ToArray())
            {
                Unload(loadedAsset);
            }

            _assetCache.Clear();

            AsyncRenderContext.Dispose();
        }

        private void OnAssetLoaded(IAsset asset, bool liveReload)
        {
            asset.OnAssetLoaded();

            if (liveReload)
            {
                if (!(asset is IReloadableAsset reloadableAsset))
                    throw new InvalidOperationException($"Asset of type {asset.GetType().Name} is not reloadable.");

                AssetReloader.AddReloadable(reloadableAsset);
            }
        }

        private void OnAssetUnloading(IAsset asset)
        {
            if (asset is IReloadableAsset reloadable)
            {
                AssetReloader.RemoveReloadable(reloadable);
            }
        }
    }
}
