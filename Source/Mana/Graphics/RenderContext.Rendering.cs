using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using Mana.Graphics.Buffers;
using Mana.Graphics.Shaders;
using Mana.Graphics.Vertex;
using Mana.Graphics.Vertex.Types;
using osuTK.Graphics.OpenGL4;

namespace Mana.Graphics
{
    public partial class RenderContext
    {
        public void Clear(Color color)
        {
            ClearColor = color;
            //GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
    }
}
