using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public static class ShaderHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CompileShader(GLHandle shaderHandle)
        {
            GL.CompileShader(shaderHandle);
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out int status);

            if (status != 1)
            {
                string shaderInfoLog = GL.GetShaderInfoLog(shaderHandle);
                
                throw new ShaderCompileException(shaderInfoLog);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LinkShader(GLHandle programHandle)
        {
            GL.LinkProgram(programHandle);

            GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out int status);

            if (status != 1)
            {
                string programInfoLog = GL.GetProgramInfoLog(programHandle);
                throw new ShaderProgramLinkException(programInfoLog);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AttachShaders(GLHandle programHandle, params Shader[] shaders)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                Debug.Assert(!shaders[i].Disposed);
                
                GL.AttachShader(programHandle, shaders[i].Handle);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DetachShaders(GLHandle programHandle, params Shader[] shaders)
        {
            for (int i = 0; i < shaders.Length; i++)
            {
                Debug.Assert(!shaders[i].Disposed);

                GL.DetachShader(programHandle, shaders[i].Handle);
            }
        }

        public static void GetAttributeInfo(GLHandle programHandle,
                                            out int attributeCount,
                                            out Dictionary<string, ShaderAttributeInfo> attributes,
                                            out Dictionary<uint, ShaderAttributeInfo> attributesByLocation)
        {
            GL.GetProgram(programHandle, GetProgramParameterName.ActiveAttributes, out attributeCount);
            
            attributes = new Dictionary<string, ShaderAttributeInfo>(attributeCount);
            attributesByLocation = new Dictionary<uint, ShaderAttributeInfo>(attributeCount);

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(programHandle,
                                   i,
                                   256,
                                   out _,
                                   out int size,
                                   out ActiveAttribType type,
                                   out string name);

                int location = GL.GetAttribLocation(programHandle, name);
                
                ShaderAttributeInfo info = new ShaderAttributeInfo(name, location, size, type);
                attributes.Add(name, info);
                attributesByLocation.Add((uint)location, info);
            }
        }

        public static void GetUniformInfo(GLHandle programHandle,
                                          out int uniformCount,
                                          out Dictionary<string, ShaderUniformInfo> uniforms)
        {
            GL.GetProgram(programHandle, GetProgramParameterName.ActiveUniforms, out uniformCount);
            
            uniforms = new Dictionary<string, ShaderUniformInfo>(uniformCount);

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(programHandle,
                                    i,
                                    256,
                                    out _,
                                    out int size,
                                    out ActiveUniformType type,
                                    out string name);

                int location = GL.GetUniformLocation(programHandle, name);
                
                ShaderUniformInfo info = new ShaderUniformInfo(name, location, size, type);
                uniforms.Add(name, info);
            }
        }

        public static void BuildShaderInfo(ShaderProgram shaderProgram)
        {
            GetAttributeInfo(shaderProgram.Handle,
                                          out int attributeCount,
                                          out var attributes,
                                          out var attributesByLocation);
            shaderProgram.AttributeCount = attributeCount;
            shaderProgram.Attributes = attributes;
            shaderProgram.AttributesByLocation = attributesByLocation;

            GetUniformInfo(shaderProgram.Handle,
                                        out int uniformCount,
                                        out var uniforms);
            shaderProgram.UniformCount = uniformCount;
            shaderProgram.Uniforms = uniforms;
        }
    }
}