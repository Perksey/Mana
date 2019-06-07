using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using ImGuiNET;

namespace Mana.IMGUI.Extensions
{
    public static unsafe class ImDrawListExtensions
    {
        public static void AddText(this ImDrawListPtr draw, Vector2 pos, uint col, Span<char> text)
        {
            char* textPtr = (char*)Unsafe.AsPointer(ref text.GetPinnableReference());
            int byteCount = Encoding.UTF8.GetByteCount(textPtr, text.Length);
            byte* nativeTextBegin = stackalloc byte[byteCount + 1];
            int nativeTextBeginOffset = Encoding.UTF8.GetBytes(textPtr, text.Length, nativeTextBegin, byteCount);
            nativeTextBegin[nativeTextBeginOffset] = 0;
            ImGuiNative.ImDrawList_AddText(draw.NativePtr, pos, col, nativeTextBegin, null);
        }
    }
}