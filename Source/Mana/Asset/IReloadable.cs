namespace Mana.Asset
{
    public interface IReloadable
    {
        void Reload(AssetManager assetManager, object info);
    }
}