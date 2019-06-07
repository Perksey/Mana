using System;
using System.IO;
using Mana.Audio;
using Mana.Graphics;

namespace Mana.Asset.Loaders
{
    public class SoundLoader : IAssetLoader<Sound>
    {
        public Sound Load(AssetManager manager, 
                          RenderContext renderContext, 
                          Stream sourceStream, 
                          string sourcePath)
        {
            if (AudioBackend.Backend == null)
                throw new InvalidOperationException("Cannot load a Sound without an initialized Audio backend.");

            return AudioBackend.Backend.CreateAudioClipFromStream(sourceStream);
        }
    }
}