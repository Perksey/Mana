using System;

namespace Mana.Utilities.Algorithm
{
    public class RefList<T>
        where T : struct
    {
        private T[] _array;
        private int _length = 0;
        private bool _clear = false;

        public RefList(int initialCapacity = 0, bool clear = false)
        {
            _array = new T[initialCapacity];
            _clear = clear;
        }
        
        public ref T this[int index]
        {
            get
            {
                if (index < 0 || index >= _length)
                    throw new ArgumentOutOfRangeException(nameof(index));
                
                return ref _array[index];
            }
        }

        public int Length => _length;

        public int Capacity => _array.Length;

        public void Add(T item)
        {
            EnsureCapacity();

            _array[_length] = item;
            _length++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            _length--;

            if (index < _length)
                Array.Copy(_array, index + 1, _array, index, _length - index);

            if (_clear) // Only necessary if T contains reference types. (?)
                _array[_length] = default;
        }

        public void RemoveRange(int index, int length)
        {
            if (index < 0 || index >= _array.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (length <= 0 || index + length > _array.Length)
                throw new ArgumentOutOfRangeException(nameof(length));

            _length -= length;

            if (index + length != _array.Length)
                Array.Copy(_array, index + length, _array, index, _array.Length - (index + length));
            
            if (_clear) // Only necessary if T contains reference types. (?)
                Array.Clear(_array, _length, _array.Length - _length);
        }

        public void Clear()
        {
            _length = 0;
        }
        
        private void EnsureCapacity()
        {
            if (_array.Length < _length + 1)
            {
                var newArray = new T[_array.Length > 0 ? _array.Length * 2 : 4];
                Array.Copy(_array, 0, newArray, 0, _array.Length);
                _array = newArray;
            }
        }
    }
}