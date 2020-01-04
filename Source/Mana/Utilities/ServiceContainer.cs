using System;
using System.Collections.Generic;

namespace Mana.Utilities
{
    public class ServiceContainer
    {
        private Dictionary<Type, object> _services = new Dictionary<Type, object>(8);

        public void AddService<T>(T service)
            where T : class
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _services.Add(typeof(T), service);
        }

        public void RemoveService<T>()
            where T : class
        {
            _services.Remove(typeof(T));
        }

        public T GetService<T>()
            where T : class
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            return null;
        }
    }
}
