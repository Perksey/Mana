using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mana.Logging;
using Mana.Utilities;

namespace Mana.Asset
{
    public class AssetWatcher : IDisposable
    {
        private static Logger _log = Logger.Create();
        
        private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private List<string> _fileNames = new List<string>();
        private DateTime _timeOfLastReload;
        private AssetManager _assetManager;
        private ReloadableAsset _reloadable;

        public AssetWatcher(AssetManager assetManager, ReloadableAsset reloadable)
        {
            _assetManager = assetManager;
            _reloadable = reloadable;
            
            AddWatchPath(reloadable.SourcePath);
        }
        
        public void Dispose()
        {
            foreach (FileSystemWatcher item in _watchers)
            {
                item.Changed -= OnChanged;
                item.Renamed -= OnRenamed;
                item.Created -= OnCreated;
                
                item.Dispose();
            }

            _watchers.Clear();
        }

        protected void Reload()
        {
            if (_reloadable.Reload(_assetManager))
            {
                _log.LogMessage($"Reloaded {_reloadable.GetType().Name}: {_reloadable.SourcePath}", LogLevel.Debug, ConsoleColor.Green);
            }
        }

        public void AddWatchPaths(params string[] paths)
        {
            foreach (string path in paths)
            {
                AddWatchPath(path);
            }
        }

        public void AddWatchPath(string path)
        {
            Debug.Assert(File.Exists(path));
            
            string fileName = Path.GetFileName(path);
            
            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                Filter = fileName,
            };

            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.Created += OnCreated;
            
            watcher.EnableRaisingEvents = true;

            _watchers.Add(watcher);
            _fileNames.Add(fileName);
        }

        private void RequestReload()
        {
            if (DateTime.Now - _timeOfLastReload > TimeSpan.FromMilliseconds(250))
            {
                Dispatcher.OnEarlyUpdate(() => Reload());
                _timeOfLastReload = DateTime.Now;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            lock (this)
            {
                Thread.Sleep(200);
                RequestReload();
            }
        }

        private void OnRenamed(object sender, FileSystemEventArgs e)
        {
            if (_fileNames.Contains(e.Name))
            {
                RequestReload();
            }
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            if (_fileNames.Contains(e.Name))
            {
                RequestReload();
            }
        }
    }
}