using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mana.Graphics.Shaders;
using Mana.Logging;
using Mana.Utilities.Reflection;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Vertex
{
    public class VertexTypeInfo
    {
        private static Logger _log = Logger.Create();
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

            var asdfasd = 1;
            var sdsdfsd = 1.0f;
            var asdfsdf = true;
        }

        internal static void Initialize()
        {
            if (_initialized)
                throw new InvalidOperationException("VertexTypeInfo should not be initialized more than once.");

            _vertexTypeInfoCache = AppDomain.CurrentDomain
                                           .GetAssemblies()
                                           .SelectMany(a => a.GetTypes())
                                           .Where(t => t.HasAttribute<VertexTypeAttribute>(false))
                                           .ToDictionary(t => t, t => new VertexTypeInfo(t));
            _initialized = true;
        }

        public static VertexTypeInfo Get<T>()
        {
            Debug.Assert(_initialized);

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

                Dictionary<string[], Dictionary<int, List<string>>> foo = new Dictionary<string[], Dictionary<int, List<string>>>();
                
                GLHelper.CheckLastError();
                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       VertexStride,
                                       new IntPtr(location));
                GLHelper.CheckLastError();

                location += attribute.Size * attribute.ComponentCount;
            }
        }

        internal void Apply(ShaderProgram program, IntPtr offset)
        {
            int location = 0;
            for (uint i = 0; i < Attributes.Length; i++)
            {
                EnableDisableAttributes(program, i);

                //program.AttributesByLocation.TryGetValue(i, out ShaderAttributeInfo info);

                VertexAttributeInfo attribute = Attributes[i];
                
                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       VertexStride,
                                       offset + location);
                GLHelper.CheckLastError();

                location += attribute.Size * attribute.ComponentCount;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableDisableAttributes(ShaderProgram program, uint i)
        {
            if (program.AttributesByLocation.TryGetValue(i, out _))
            {
                GL.EnableVertexAttribArray(i);
                GLHelper.CheckLastError();
            }
            else
            {
                GL.DisableVertexAttribArray(i);
                GLHelper.CheckLastError();
            }
        }
    }
}
