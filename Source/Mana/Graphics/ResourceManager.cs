using System.Threading;
using OpenTK.Graphics;

namespace Mana.Graphics
{
    /// <summary>
    /// Represents a ResourceManager capable of managing graphics resources.
    /// </summary>
    public class ResourceManager
    {
        private static ResourceManager _default;
        
        public IGraphicsContext ShareContext { get; private set; }
        
        public RenderContext MainContext { get; private set; }
        
        public ResourceManager(IGraphicsContext shareContext)
        {
            ShareContext = shareContext;
        }
        
        public static ResourceManager GetDefault(ManaWindow window)
        {
            return _default ??= new ResourceManager(window.Context);
        }
        
        internal void OnResourceCreated<T>(T resource)
            where T : GraphicsResource
        {
        }

        internal void OnResourceDisposed<T>(T resource)
            where T : GraphicsResource
        {
        }

        internal void SetMainContext(RenderContext mainContext)
        {
            MainContext = mainContext;
        }
    }
}