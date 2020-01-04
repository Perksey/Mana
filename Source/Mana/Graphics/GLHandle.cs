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
        /// Represents a GLHandle containing a zero value. This can be used as "null" because OpenGL will
        /// never generate a zero handle unless an error occurs.
        /// </summary>
        public static readonly GLHandle Zero = new GLHandle(0);

        private readonly uint _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="GLHandle"/> struct.
        /// </summary>
        /// <param name="id">The OpenGL handle name.</param>
        /// <exception cref="InvalidOperationException">Thrown if the given <see cref="uint"/> is too large.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GLHandle(uint id)
        {
            _id = id;

            // TODO: After OpenTK 4.0 is released, remove all traces of "int" from GLHandle.
            // In OpenTK 3.0 there is an issue where OpenGL functions that should use "uint" are using "int"
            if (id > int.MaxValue)
            {
                throw new InvalidOperationException();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator int(GLHandle handle)
        {
            return (int)handle._id;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator GLHandle(uint name)
        {
            return new GLHandle(name);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator GLHandle(int name)
        {
            if (name < 0)
            {
                throw new InvalidOperationException("Cannot create a GLHandle with a negative name.");
            }

            return new GLHandle((uint)name);
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
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is GLHandle other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id.ToString();
        }
    }
}
