using System;
using System.Runtime.CompilerServices;

namespace Mana.Graphics
{
    /// <summary>
    /// Represents a handle to an OpenGL object.
    /// </summary>
    public struct GLHandle : IEquatable<GLHandle>
    {
        /// <summary>
        /// Represents a GLHandle containing a zero value. This can be used as null because OpenGL will
        /// never create a valid object with a zero handle.
        /// </summary>
        public static readonly GLHandle Zero = new GLHandle(0);
        
        private readonly int _id;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GLHandle(int id)
        {
            _id = id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(GLHandle handle)
        {
            return handle._id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator GLHandle(int name)
        {
            return new GLHandle(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(GLHandle left, GLHandle right)
        {
            return left._id == right._id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(GLHandle left, GLHandle right)
        {
            return left._id != right._id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(GLHandle other)
        {
            return _id == other._id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is GLHandle other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return _id;
        }
    }
}