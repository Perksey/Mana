using System.Collections.Generic;

namespace Mana.Graphics
{
    public class GraphicsResourceContainer
    {
        private readonly List<IGraphicsResource> _resources = new List<IGraphicsResource>();
        
        public int Count => _resources.Count;

        public void Add<TResource>(TResource resource)
            where TResource : class, IGraphicsResource
        {
            _resources.Add(resource);
        }

        public void Remove<TResource>(TResource resource)
            where TResource : class, IGraphicsResource
        {
            _resources.Remove(resource);
        }
    }
}