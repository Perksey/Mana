using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mana.Graphics.Shaders;
using Mana.Utilities.Extensions;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Vertex
{
    public class VertexTypeInfo
    {
        private static Dictionary<Type, VertexTypeInfo> _vertexTypeInfoCache;
        private static bool _initialized;

        internal readonly int VertexStride;
        internal readonly VertexAttributeInfo[] Attributes;

        public VertexTypeInfo(Type type)
        {
            var fields = type.GetFields();
            Attributes = new VertexAttributeInfo[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                Attributes[i] = VertexHelper.GetVertexAttributeInfo(fields[i].FieldType);
                VertexStride += Attributes[i].Size * Attributes[i].ComponentCount;
            }
        }

        internal static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _vertexTypeInfoCache = AppDomain.CurrentDomain
                                            .GetAssemblies()
                                            .SelectMany(a => a.GetTypes())
                                            .Where(t => t.IsValueType && t.HasParent<IVertexType>())
                                            .ToDictionary(t => t, t => new VertexTypeInfo(t));
            _initialized = true;
        }

        public static VertexTypeInfo Get<T>()
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("VertexTypeInfo.Initialize() must be called before Get().");
            }

            if (_vertexTypeInfoCache.TryGetValue(typeof(T), out VertexTypeInfo vertexTypeInfo))
            {
                return vertexTypeInfo;
            }

            throw new ArgumentOutOfRangeException(typeof(T).FullName, "Type not found in VertexTypeInfo cache. Was the type generated at runtime?");
        }

        public void Apply(ShaderProgram program)
        {
            int location = 0;
            for (uint i = 0; i < Attributes.Length; i++)
            {
                EnableDisableAttributes(program, i);

                VertexAttributeInfo attribute = Attributes[i];

                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       VertexStride,
                                       new IntPtr(location));

                location += attribute.Size * attribute.ComponentCount;
            }
        }

        internal void Apply(ShaderProgram program, IntPtr offset)
        {
            int location = 0;
            for (uint i = 0; i < Attributes.Length; i++)
            {
                EnableDisableAttributes(program, i);

                VertexAttributeInfo attribute = Attributes[i];

                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       VertexStride,
                                       offset + location);

                location += attribute.Size * attribute.ComponentCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableDisableAttributes(ShaderProgram program, uint i)
        {
            if (program.AttributesByLocation.TryGetValue(i, out _))
            {
                GL.EnableVertexAttribArray(i);
            }
            else
            {
                GL.DisableVertexAttribArray(i);
            }
        }
    }
}
