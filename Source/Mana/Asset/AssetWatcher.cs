using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Mana.Utilities;

namespace Mana.Asset
{
    public abstract class AssetWatcher : IDisposable
    {
        private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private List<string> _fileNames = new List<string>();
        private DateTime _timeOfLastReload;
        
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

        protected abstract void Reload();

        protected void WatchPaths(params string[] paths)
        {
            foreach (string path in paths)
            {
                WatchPath(path);
            }
        }

        protected void WatchPath(string path)
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
            
            Console.WriteLine($"Watching Path: {path} [{fileName}]");
        }

        private void ReloadImpl()
        {
            Reload();
        }

        private void RequestReload()
        {
            if (DateTime.Now - _timeOfLastReload > TimeSpan.FromMilliseconds(250))
            {
                Dispatcher.OnEarlyUpdate(ReloadImpl);
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