using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mana.Utilities.Reflection;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Vertex
{
    internal class VertexTypeInfo
    {
        private static Dictionary<Type, VertexTypeInfo> vertexTypeInfoCache;
        private static bool initialized;

        internal readonly int vertexStride;
        internal readonly VertexAttributeInfo[] attributes;

        public VertexTypeInfo(Type type)
        {
            FieldInfo[] fields = type.GetFields();
            attributes = new VertexAttributeInfo[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                attributes[i] = VertexHelper.GetVertexAttributeInfo(fields[i].FieldType);
                vertexStride += attributes[i].Size * attributes[i].ComponentCount;
            }
        }

        internal static void Initialize()
        {
            if (initialized)
                return;

            vertexTypeInfoCache = AppDomain.CurrentDomain
                                           .GetAssemblies()
                                           .SelectMany(a => a.GetTypes())
                                           .Where(t => t.HasAttribute<VertexTypeAttribute>(false))
                                           .ToDictionary(t => t, t => new VertexTypeInfo(t));
            initialized = true;
        }

        internal static VertexTypeInfo Get<T>()
        {
            Debug.Assert(initialized);

            if (vertexTypeInfoCache.TryGetValue(typeof(T), out VertexTypeInfo vertexTypeInfo))
            {
                return vertexTypeInfo;
            }

            throw new ArgumentOutOfRangeException(typeof(T).FullName, "Type not found in VertexTypeInfo cache. Was the type generated at runtime?");
        }

        /*
        internal void Apply(ShaderProgram program)
        {
            int location = 0;
            for (uint i = 0; i < attributes.Length; i++)
            {
                VertexAttributeInfo attribute = attributes[i];

                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       vertexStride,
                                       location);
                GLHelper.CheckLastError();

                EnableDisableAttributes(program, i);

                location += attribute.Size * attribute.ComponentCount;
            }
        }

        internal void Apply(ShaderProgram program, IntPtr offset)
        {
            int location = 0;
            for (uint i = 0; i < attributes.Length; i++)
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

                program.AttributesByLocation.TryGetValue(i, out ShaderAttributeInfo val);

                VertexAttributeInfo attribute = attributes[i];

                GL.VertexAttribPointer(i,
                                       attribute.ComponentCount,
                                       attribute.Type,
                                       attribute.Normalize,
                                       vertexStride,
                                       offset + location);
                GLHelper.CheckLastError();

                //EnableDisableAttributes(program, i);

                location += attribute.Size * attribute.ComponentCount;
            }
        }
        */

        #region Private Helpers

        /*
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
        */

        #endregion
    }
}
