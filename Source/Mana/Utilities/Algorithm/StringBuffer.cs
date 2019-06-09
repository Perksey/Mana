using System;
using System.Runtime.CompilerServices;

namespace Mana.Utilities.Algorithm
{
    /// <summary>
    /// Represents a contiguous array of characters.
    /// </summary>
    public class StringBuffer
    {
        private char[] _array;
        private int _length;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StringBuffer"/> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity for the object's internal character array.</param>
        public StringBuffer(int initialCapacity = 0)
        {
            if (initialCapacity < 0)
                throw new ArgumentException("Capacity may not be negative.", nameof(initialCapacity));
            
            _array = new char[initialCapacity];
            _length = 0;
        }
        
        public char this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }

        /// <summary>
        /// Gets the amount of characters currently stored in the <see cref="StringBuffer"/>.
        /// </summary>
        public int Length => _length;
        
        /// <summary>
        /// Gets the length of the <see cref="StringBuffer"/>'s backing array.
        /// </summary>
        public int Capacity => _array.Length;

        /// <summary>
        /// Appends a <see cref="string"/> to the end of the string buffer.
        /// </summary>
        /// <param name="str">The string to append.</param>
        public unsafe void Append(string str)
        {
            EnsureCapacity(_length + str.Length);

            fixed (char* sourcePtr = str)
            fixed (char* destPtr = &_array[_length])
            {
                Unsafe.CopyBlock(destPtr, sourcePtr, (uint)(str.Length * sizeof(char)));
            }
            
            _length += str.Length;
        }

        /// <summary>
        /// Appends a Span&lt;char&gt; to the <see cref="StringBuffer"/>.
        /// </summary>
        /// <param name="span">The span to append.</param>
        public unsafe void Append(Span<char> span)
        {
            EnsureCapacity(_length + span.Length);
            
            fixed (char* sourcePtr = &span.GetPinnableReference())
            fixed (char* destPtr = &_array[_length])
            {
                Unsafe.CopyBlock(destPtr, sourcePtr, (uint)(span.Length * sizeof(char)));
            }
            
            _length += span.Length;
        }
        
        /// <summary>
        /// Appends a <see cref="string"/> to the <see cref="StringBuffer"/>, starting from the given offset. This
        /// will overwrite existing characters.
        /// </summary>
        /// <param name="offset">The offset from the start of the buffer.</param>
        /// <param name="str">The string to append.</param>
        public unsafe void AppendAt(int offset, string str)
        {
            if (offset < 0 || offset > _length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            
            EnsureCapacity(offset + str.Length);

            fixed (char* sourcePtr = str)
            fixed (char* destPtr = &_array[offset])
            {
                Unsafe.CopyBlock(destPtr, sourcePtr, (uint)(str.Length * sizeof(char)));
            }

            _length = Math.Max(offset + str.Length, _length);
        }
        
        /// <summary>
        /// Appends a Span&lt;char&gt; to the <see cref="StringBuffer"/>, starting from the given offset. This
        /// will overwrite existing characters.
        /// </summary>
        /// <param name="offset">The offset from the start of the buffer.</param>
        /// <param name="span">The span to append.</param>
        public unsafe void AppendAt(int offset, Span<char> span)
        {
            if (offset < 0 || offset > _length)
                throw new ArgumentOutOfRangeException(nameof(offset));
            
            EnsureCapacity(offset + span.Length);

            fixed (char* sourcePtr = &span.GetPinnableReference())
            fixed (char* destPtr = &_array[offset])
            {
                Unsafe.CopyBlock(destPtr, sourcePtr, (uint)(span.Length * sizeof(char)));
            }

            _length = Math.Max(offset + span.Length, _length);
        }

        /// <summary>
        /// Sets the <see cref="StringBuffer"/>'s length to zero.
        /// </summary>
        public void Clear()
        {
            _length = 0;
        }
        
        /// <summary>
        /// Gets a Span&lt;char&gt; over the characters of the <see cref="StringBuffer"/>.
        /// </summary>
        /// <returns></returns>
        public Span<char> GetSpan()
        {
            return new Span<char>(_array, 0, _length);
        }

        /// <summary>
        /// Gets a ReadOnlySpanSpan&lt;char&gt; over the characters of the <see cref="StringBuffer"/>.
        /// </summary>
        /// <returns></returns>
        public ReadOnlySpan<char> GetReadOnlySpan()
        {
            return new ReadOnlySpan<char>(_array, 0, _length);
        }

        /// <summary>
        /// Creates and returns a new character array containing the characters within the <see cref="StringBuffer"/>.
        /// </summary>
        public char[] ToArray()
        {
            var newArray = new char[_length];
            Array.Copy(_array, newArray, _length);
            return newArray;
        }
        
        private void EnsureCapacity(int capacity)
        {
            if (_array.Length < capacity)
            {
                int newCapacity = _array.Length > 0 ? _array.Length * 2 : 4;
                
                if (newCapacity < capacity)
                    newCapacity = capacity;
                
                var newArray = new char[newCapacity];
                Array.Copy(_array, 0, newArray, 0, _array.Length);
                _array = newArray;
            }
        }
    }
}