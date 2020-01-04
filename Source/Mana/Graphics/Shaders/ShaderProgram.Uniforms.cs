using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics.Shaders
{
    public partial class ShaderProgram
    {
        #region SetUniform

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, bool value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value ? 1 : 0);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, bool value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value ? 1 : 0);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value ? 1 : 0);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, int value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, int value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, float value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, float value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, double value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, double value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector2 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, Vector2 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector2 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, ref Vector2 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector3 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, Vector3 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector3 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, ref Vector3 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Vector4 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, Vector4 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Vector4 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, ref Vector4 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Matrix3x2 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, Matrix3x2 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Matrix3x2 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, ref Matrix3x2 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, Matrix4x4 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, Matrix4x4 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(string name, ref Matrix4x4 value)
        {
            int location = GetUniformLocation(name);

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetUniform(int location, ref Matrix4x4 value)
        {
            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }
        }

        #endregion

        #region TrySetUniform

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, bool value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value ? 1 : 0);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value ? 1 : 0);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, bool value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value ? 1 : 0);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value ? 1 : 0);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, int value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, int value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, float value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, float value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, double value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, double value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform1(Handle, location, value);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform1(location, value);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector2 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, Vector2 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector2 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, ref Vector2 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform2(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform2(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector3 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, Vector3 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector3 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, ref Vector3 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform3(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform3(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Vector4 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, Vector4 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Vector4 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, ref Vector4 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniform4(Handle, location, 1, ref value.X);
            }
            else
            {
                Bind(ParentContext);
                GL.Uniform4(location, 1, ref value.X);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Matrix3x2 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, Matrix3x2 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Matrix3x2 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, ref Matrix3x2 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix3x2(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix3x2(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, Matrix4x4 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, Matrix4x4 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform with the given name to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(string name, ref Matrix4x4 value)
        {
            int location = GetUniformLocation(name);
            if (location == -1)
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="ShaderProgram"/>'s uniform at the given location to a specified value, returning a value
        /// indicating whether the operation succeeded.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <param name="value">The value to set the shader uniform to.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TrySetUniform(int location, ref Matrix4x4 value)
        {
            if (!IsValidUniformLocation(location))
                return false;

            if (GLInfo.HasSeparateShaderObjects)
            {
                GL.ProgramUniformMatrix4(Handle, location, 1, false, ref value.M11);
            }
            else
            {
                Bind(ParentContext);
                GL.UniformMatrix4(location, 1, false, ref value.M11);
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Gets the location of the <see cref="ShaderProgram"/>'s uniform with a given name.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <returns>The location of the shader uniform.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetUniformLocation(string name)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), $"String argument {name} may not be null or whitespace.");

            if (!UniformLocations.TryGetValue(name, out int location))
                throw new ArgumentException($"Shader uniform with name \"{name}\" was not found. Was it optimized out?");

            return location;
#else
            return UniformLocations[name];
#endif
        }

        /// <summary>
        /// Gets the location of the <see cref="ShaderProgram"/>'s uniform, returning a value indicating whether the
        /// operation succeeded.
        /// </summary>
        /// <param name="name">The name of the shader uniform.</param>
        /// <param name="location">The location of the shader uniform.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetUniformLocation(string name, out int location)
        {
            return UniformLocations.TryGetValue(name, out location);
        }

        /// <summary>
        /// Gets a value indicating whether the given integer represents a location of an active shader uniform within
        /// the <see cref="ShaderProgram"/>.
        /// </summary>
        /// <param name="location">The location of the shader uniform.</param>
        /// <returns>
        /// A value indicating whether the given integer represents a location of an active shader uniform
        /// within the <see cref="ShaderProgram"/>.
        /// </returns>
        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValidUniformLocation(int location)
        {
            return ValidUniformLocations.Contains(location);
        }
    }
}
