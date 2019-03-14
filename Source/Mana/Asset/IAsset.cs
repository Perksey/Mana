using System;

namespace Mana.Asset
{
    public interface IAsset : IDisposable
    {
        string SourcePath { get; set; }
    }
}