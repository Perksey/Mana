using System;
using System.Collections.Generic;

namespace Mana.Utilities.Algorithm
{
    /// <summary>
    /// Represents a <see cref="Dictionary"/> with common operations wrapped in lock statements.
    /// </summary>
    public class LockedDictionary<TKey, TValue>
    {
        public Dictionary<TKey, TValue> Dictionary { get; }
        
        private object _lock = new object();

        public LockedDictionary()
        {
            Dictionary = new Dictionary<TKey, TValue>();
        }
        
        public LockedDictionary(int capacity)
        {
            Dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (_lock)
            {
                return Dictionary.TryGetValue(key, out value);
            }
        }

        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                return Dictionary.Remove(key);
            }
        }

        public void ForEach(Action<KeyValuePair<TKey, TValue>> action)
        {
            lock (_lock)
            {
                foreach (var kvp in Dictionary)
                {
                    action.Invoke(kvp);
                }
            }
        }
    }
}