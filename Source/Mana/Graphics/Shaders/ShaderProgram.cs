using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Mana.Asset;
using OpenTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public class ShaderProgram : ManaAsset, IGraphicsResource
    {
        internal bool Disposed = false;
        internal bool Linked = false;
        internal Dictionary<uint, ShaderAttributeInfo> AttributesByLocation;
        internal string VertexShaderPath;
        internal string FragmentShaderPath;
        
        public ShaderProgram(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
            GraphicsDevice.Resources.Add(this);

            Handle = (GLHandle)GL.CreateProgram();
            GLHelper.CheckLastError();
        }

        public GLHandle Handle { get; private set; }

        public GraphicsDevice GraphicsDevice { get; }

        public int AttributeCount { get; internal set; }
        
        public Dictionary<string, ShaderAttributeInfo> Attributes { get; internal set; }
        
        public int UniformCount { get; internal set; }
        
        public Dictionary<string, ShaderUniformInfo> Uniforms { get; internal set; }
      
        
        #region SetUniform Methods

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, bool value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));
            
            GL.Uniform1(info.Location, value ? 1 : 0);
            GLHelper.CheckLastError();
#else
            GL.Uniform1(Uniforms[name].Location, value ? 1 : 0);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, int value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));
            
            GL.Uniform1(info.Location, value);
            GLHelper.CheckLastError();
#else
            GL.Uniform1(Uniforms[name].Location, value);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, float value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));
            
            GL.Uniform1(info.Location, value);
            GLHelper.CheckLastError();
#else
            GL.Uniform1(Uniforms[name].Location, value);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, double value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));
            
            GL.Uniform1(info.Location, value);
            GLHelper.CheckLastError();
#else
            GL.Uniform1(Uniforms[name].Location, value);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector2 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform2(info.Location, Unsafe.As<Vector2, OpenTK.Vector2>(ref value));
            GLHelper.CheckLastError();
#else
            GL.Uniform2(Uniforms[name].Location, Unsafe.As<Vector2, OpenTK.Vector2>(ref value));
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector2 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform2(info.Location, 1, ref value.X);
            GLHelper.CheckLastError();
#else
            GL.Uniform2(Uniforms[name].Location, 1, ref value.X);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector3 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform3(info.Location, Unsafe.As<Vector3, OpenTK.Vector3>(ref value));
            GLHelper.CheckLastError();
#else
            GL.Uniform3(Uniforms[name].Location, Unsafe.As<Vector3, OpenTK.Vector3>(ref value));
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector3 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform3(info.Location, 1, ref value.X);
            GLHelper.CheckLastError();
#else
            GL.Uniform3(Uniforms[name].Location, 1, ref value.X);
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform4(info.Location, Unsafe.As<Vector4, OpenTK.Vector4>(ref value));
            GLHelper.CheckLastError();
#else
            GL.Uniform4(Uniforms[name].Location, Unsafe.As<Vector4, OpenTK.Vector4>(ref value));
#endif
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.Uniform4(info.Location, 1, ref value.X);
            GLHelper.CheckLastError();
#else
            GL.Uniform4(Uniforms[name].Location, 1, ref value.X);
#endif
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Matrix4x4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
#if DEBUG
            if (!Uniforms.TryGetValue(name, out ShaderUniformInfo info))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found.", nameof(name));

            GL.UniformMatrix4(info.Location, 1, false, ref value.M11);
            GLHelper.CheckLastError();
#else
            GL.UniformMatrix4(Uniforms[name].Location, 1, false, ref value.M11);
#endif
        }
        
        #endregion
        
        #region TrySetUniform

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, bool value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform1(info.Location, value ? 1 : 0);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, int value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform1(info.Location, value);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, float value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform1(info.Location, value);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, double value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform1(info.Location, value);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector2 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform2(info.Location, Unsafe.As<Vector2, OpenTK.Vector2>(ref value));
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector2 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform2(info.Location, 1, ref value.X);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector3 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform3(info.Location, Unsafe.As<Vector3, OpenTK.Vector3>(ref value));
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector3 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform3(info.Location, 1, ref value.X);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform4(info.Location, Unsafe.As<Vector4, OpenTK.Vector4>(ref value));
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.Uniform4(info.Location, 1, ref value.X);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Matrix4x4 value)
        {
            GraphicsDevice.BindShaderProgram(this);
            
            bool success = Uniforms.TryGetValue(name, out ShaderUniformInfo info);

            if (success)
            {
                GL.UniformMatrix4(info.Location, 1, false, ref value.M11);
                GLHelper.CheckLastError();
            }
            
            return success;
        }
        
        #endregion
        
        
        public void Link(params Shader[] shaders)
        {
            ShaderHelper.AttachShaders(Handle, shaders);
            ShaderHelper.LinkShader(Handle);
            ShaderHelper.DetachShaders(Handle, shaders);

            Linked = true;
        }
        
        public override void Dispose()
        {
            Debug.Assert(!Disposed);
            
            GraphicsDevice.Resources.Remove(this);
            GraphicsDevice.UnbindShaderProgram(this);
            
            GL.DeleteProgram(Handle);
            GLHelper.CheckLastError();
           
            Disposed = true;
        }
    }
}