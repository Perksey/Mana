using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Mana.Asset.Reloading
{
    internal class AssetWatcher : IAssetWatcher
    {
        //private static Logger _log = Logger.Create();

        private AssetReloader _assetReloader;
        private IReloadableAsset _asset;
        private List<FileSystemWatcher> _watchers;
        private Stopwatch _stopWatch = new Stopwatch();

        public AssetWatcher(AssetManager assetManager, IReloadableAsset reloadableAsset)
        {
            _assetReloader = assetManager.AssetReloader;
            _asset = reloadableAsset;

            _watchers = new List<FileSystemWatcher>();

            foreach (var pathToWatch in reloadableAsset.GetLiveReloadAssetPaths())
            {
                _watchers.Add(WatchFilePath(pathToWatch));
            }
        }

        private FileSystemWatcher WatchFilePath(string path)
        {
            var fullPath = Path.GetFullPath(path);

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(fullPath),
                Filter = Path.GetFileName(fullPath),
            };

            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;

            return watcher;
        }
        public void Dispose()
        {
            foreach (FileSystemWatcher watcher in _watchers)
            {
                watcher.Changed -= OnChanged;
                watcher.Dispose();
            }

            _watchers.Clear();
            _watchers = null;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                if (_stopWatch.IsRunning && _stopWatch.ElapsedMilliseconds < 15)
                {
                    return;
                }

                Thread.Sleep(100);
                _assetReloader.Dispatcher.Invoke(() => { _asset.Reload(_assetReloader.AssetManager); });

                _stopWatch.Restart();
            }
        }
    }
}
