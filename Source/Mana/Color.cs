using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;
using Mana.Utilities;
using OpenTK.Graphics;

namespace Mana
{
    /// <summary>
    /// Represents a 32-bit color structure with four 8-bit components (R, G, B, A).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Color : IEquatable<Color>
    {
        /// <summary>
        /// The red component of the color, represented within the range [0, 255].
        /// </summary>
        public byte R;

        /// <summary>
        /// The green component of the color, represented within the range [0, 255].
        /// </summary>
        public byte G;

        /// <summary>
        /// The blue component of the color, represented within the range [0, 255].
        /// </summary>
        public byte B;

        /// <summary>
        /// The alpha component of the color, represented within the range [0, 255].
        /// </summary>
        public byte A;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from the given components.
        /// </summary>
        /// <param name="r">The 8-bit red component of the color.</param>
        /// <param name="g"> The 8-bit green component of the color.</param>
        /// <param name="b">The 8-bit blue component of the color.</param>
        /// <param name="a">The 8-bit alpha component of the color.</param>
        public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
            : this()
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from the given components.
        /// </summary>
        /// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        /// <param name="a">The alpha component of the color.</param>
        public Color(float r, float g, float b, float a)
            : this(ClampToByte(r * byte.MaxValue),
                   ClampToByte(g * byte.MaxValue),
                   ClampToByte(b * byte.MaxValue),
                   ClampToByte(a * byte.MaxValue))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct from the given components.
        /// </summary>
        /// <param name="r">The red component of the color.</param>
        /// <param name="g">The green component of the color.</param>
        /// <param name="b">The blue component of the color.</param>
        public Color(float r, float g, float b)
            : this(ClampToByte(r * byte.MaxValue),
                   ClampToByte(g * byte.MaxValue),
                   ClampToByte(b * byte.MaxValue),
                   byte.MaxValue)
        {
        }

        #region Named Colors

        /// <summary>
        /// Gets the named <see cref="Color"/> "Transparent" with the RGBA components: (255, 255, 255, 0).
        /// </summary>
        public static Color Transparent => new Color(255, 255, 255, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "TransparentBlack" with the RGBA components: (0, 0, 0, 0).
        /// </summary>
        public static Color TransparentBlack => new Color(0, 0, 0, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "AliceBlue" with the RGB components: (240, 248, 255).
        /// </summary>
        public static Color AliceBlue => new Color(240, 248, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "AntiqueWhite" with the RGB components: (250, 235, 215).
        /// </summary>
        public static Color AntiqueWhite => new Color(250, 235, 215);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Aqua" with the RGB components: (0, 255, 255).
        /// </summary>
        public static Color Aqua => new Color(0, 255, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Aquamarine" with the RGB components: (127, 255, 212).
        /// </summary>
        public static Color Aquamarine => new Color(127, 255, 212);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Azure" with the RGB components: (240, 255, 255).
        /// </summary>
        public static Color Azure => new Color(240, 255, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Beige" with the RGB components: (245, 245, 220).
        /// </summary>
        public static Color Beige => new Color(245, 245, 220);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Bisque" with the RGB components: (255, 228, 196).
        /// </summary>
        public static Color Bisque => new Color(255, 228, 196);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Black" with the RGB components: (0, 0, 0).
        /// </summary>
        public static Color Black => new Color(0, 0, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "BlanchedAlmond" with the RGB components: (255, 235, 205).
        /// </summary>
        public static Color BlanchedAlmond => new Color(255, 235, 205);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Blue" with the RGB components: (0, 0, 255).
        /// </summary>
        public static Color Blue => new Color(0, 0, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "BlueViolet" with the RGB components: (138, 43, 226).
        /// </summary>
        public static Color BlueViolet => new Color(138, 43, 226);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Brown" with the RGB components: (165, 42, 42).
        /// </summary>
        public static Color Brown => new Color(165, 42, 42);

        /// <summary>
        /// Gets the named <see cref="Color"/> "BurlyWood" with the RGB components: (222, 184, 135).
        /// </summary>
        public static Color BurlyWood => new Color(222, 184, 135);

        /// <summary>
        /// Gets the named <see cref="Color"/> "CadetBlue" with the RGB components: (95, 158, 160).
        /// </summary>
        public static Color CadetBlue => new Color(95, 158, 160);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Chartreuse" with the RGB components: (127, 255, 0).
        /// </summary>
        public static Color Chartreuse => new Color(127, 255, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Chocolate" with the RGB components: (210, 105, 30).
        /// </summary>
        public static Color Chocolate => new Color(210, 105, 30);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Coral" with the RGB components: (255, 127, 80).
        /// </summary>
        public static Color Coral => new Color(255, 127, 80);

        /// <summary>
        /// Gets the named <see cref="Color"/> "CornflowerBlue" with the RGB components: (100, 149, 237).
        /// </summary>
        public static Color CornflowerBlue => new Color(100, 149, 237);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Cornsilk" with the RGB components: (255, 248, 220).
        /// </summary>
        public static Color Cornsilk => new Color(255, 248, 220);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Crimson" with the RGB components: (220, 20, 60).
        /// </summary>
        public static Color Crimson => new Color(220, 20, 60);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Cyan" with the RGB components: (0, 255, 255).
        /// </summary>
        public static Color Cyan => new Color(0, 255, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkBlue" with the RGB components: (0, 0, 139).
        /// </summary>
        public static Color DarkBlue => new Color(0, 0, 139);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkCyan" with the RGB components: (0, 139, 139).
        /// </summary>
        public static Color DarkCyan => new Color(0, 139, 139);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkGoldenrod" with the RGB components: (184, 134, 11).
        /// </summary>
        public static Color DarkGoldenrod => new Color(184, 134, 11);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkGray" with the RGB components: (169, 169, 169).
        /// </summary>
        public static Color DarkGray => new Color(169, 169, 169);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkGreen" with the RGB components: (0, 100, 0).
        /// </summary>
        public static Color DarkGreen => new Color(0, 100, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkKhaki" with the RGB components: (189, 183, 107).
        /// </summary>
        public static Color DarkKhaki => new Color(189, 183, 107);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkMagenta" with the RGB components: (139, 0, 139).
        /// </summary>
        public static Color DarkMagenta => new Color(139, 0, 139);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkOliveGreen" with the RGB components: (85, 107, 47).
        /// </summary>
        public static Color DarkOliveGreen => new Color(85, 107, 47);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkOrange" with the RGB components: (255, 140, 0).
        /// </summary>
        public static Color DarkOrange => new Color(255, 140, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkOrchid" with the RGB components: (153, 50, 204).
        /// </summary>
        public static Color DarkOrchid => new Color(153, 50, 204);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkRed" with the RGB components: (139, 0, 0).
        /// </summary>
        public static Color DarkRed => new Color(139, 0, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkSalmon" with the RGB components: (233, 150, 122).
        /// </summary>
        public static Color DarkSalmon => new Color(233, 150, 122);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkSeaGreen" with the RGB components: (143, 188, 139).
        /// </summary>
        public static Color DarkSeaGreen => new Color(143, 188, 139);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkSlateBlue" with the RGB components: (72, 61, 139).
        /// </summary>
        public static Color DarkSlateBlue => new Color(72, 61, 139);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkSlateGray" with the RGB components: (47, 79, 79).
        /// </summary>
        public static Color DarkSlateGray => new Color(47, 79, 79);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkTurquoise" with the RGB components: (0, 206, 209).
        /// </summary>
        public static Color DarkTurquoise => new Color(0, 206, 209);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DarkViolet" with the RGB components: (148, 0, 211).
        /// </summary>
        public static Color DarkViolet => new Color(148, 0, 211);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DeepPink" with the RGB components: (255, 20, 147).
        /// </summary>
        public static Color DeepPink => new Color(255, 20, 147);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DeepSkyBlue" with the RGB components: (0, 191, 255).
        /// </summary>
        public static Color DeepSkyBlue => new Color(0, 191, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DimGray" with the RGB components: (105, 105, 105).
        /// </summary>
        public static Color DimGray => new Color(105, 105, 105);

        /// <summary>
        /// Gets the named <see cref="Color"/> "DodgerBlue" with the RGB components: (30, 144, 255).
        /// </summary>
        public static Color DodgerBlue => new Color(30, 144, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Firebrick" with the RGB components: (178, 34, 34).
        /// </summary>
        public static Color Firebrick => new Color(178, 34, 34);

        /// <summary>
        /// Gets the named <see cref="Color"/> "FloralWhite" with the RGB components: (255, 250, 240).
        /// </summary>
        public static Color FloralWhite => new Color(255, 250, 240);

        /// <summary>
        /// Gets the named <see cref="Color"/> "ForestGreen" with the RGB components: (34, 139, 34).
        /// </summary>
        public static Color ForestGreen => new Color(34, 139, 34);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Fuchsia" with the RGB components: (255, 0, 255).
        /// </summary>
        public static Color Fuchsia => new Color(255, 0, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Gainsboro" with the RGB components: (220, 220, 220).
        /// </summary>
        public static Color Gainsboro => new Color(220, 220, 220);

        /// <summary>
        /// Gets the named <see cref="Color"/> "GhostWhite" with the RGB components: (248, 248, 255).
        /// </summary>
        public static Color GhostWhite => new Color(248, 248, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Gold" with the RGB components: (255, 215, 0).
        /// </summary>
        public static Color Gold => new Color(255, 215, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Goldenrod" with the RGB components: (218, 165, 32).
        /// </summary>
        public static Color Goldenrod => new Color(218, 165, 32);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Gray" with the RGB components: (128, 128, 128).
        /// </summary>
        public static Color Gray => new Color(128, 128, 128);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Green" with the RGB components: (0, 128, 0).
        /// </summary>
        public static Color Green => new Color(0, 128, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "GreenYellow" with the RGB components: (173, 255, 47).
        /// </summary>
        public static Color GreenYellow => new Color(173, 255, 47);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Honeydew" with the RGB components: (240, 255, 240).
        /// </summary>
        public static Color Honeydew => new Color(240, 255, 240);

        /// <summary>
        /// Gets the named <see cref="Color"/> "HotPink" with the RGB components: (255, 105, 180).
        /// </summary>
        public static Color HotPink => new Color(255, 105, 180);

        /// <summary>
        /// Gets the named <see cref="Color"/> "IndianRed" with the RGB components: (205, 92, 92).
        /// </summary>
        public static Color IndianRed => new Color(205, 92, 92);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Indigo" with the RGB components: (75, 0, 130).
        /// </summary>
        public static Color Indigo => new Color(75, 0, 130);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Ivory" with the RGB components: (255, 255, 240).
        /// </summary>
        public static Color Ivory => new Color(255, 255, 240);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Khaki" with the RGB components: (240, 230, 140).
        /// </summary>
        public static Color Khaki => new Color(240, 230, 140);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Lavender" with the RGB components: (230, 230, 250).
        /// </summary>
        public static Color Lavender => new Color(230, 230, 250);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LavenderBlush" with the RGB components: (255, 240, 245).
        /// </summary>
        public static Color LavenderBlush => new Color(255, 240, 245);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LawnGreen" with the RGB components: (124, 252, 0).
        /// </summary>
        public static Color LawnGreen => new Color(124, 252, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LemonChiffon" with the RGB components: (255, 250, 205).
        /// </summary>
        public static Color LemonChiffon => new Color(255, 250, 205);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightBlue" with the RGB components: (173, 216, 230).
        /// </summary>
        public static Color LightBlue => new Color(173, 216, 230);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightCoral" with the RGB components: (240, 128, 128).
        /// </summary>
        public static Color LightCoral => new Color(240, 128, 128);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightCyan" with the RGB components: (224, 255, 255).
        /// </summary>
        public static Color LightCyan => new Color(224, 255, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightGoldenrodYellow" with the RGB components: (250, 250, 210).
        /// </summary>
        public static Color LightGoldenrodYellow => new Color(250, 250, 210);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightGreen" with the RGB components: (144, 238, 144).
        /// </summary>
        public static Color LightGreen => new Color(144, 238, 144);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightGray" with the RGB components: (211, 211, 211).
        /// </summary>
        public static Color LightGray => new Color(211, 211, 211);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightPink" with the RGB components: (255, 182, 193).
        /// </summary>
        public static Color LightPink => new Color(255, 182, 193);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightSalmon" with the RGB components: (255, 160, 122).
        /// </summary>
        public static Color LightSalmon => new Color(255, 160, 122);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightSeaGreen" with the RGB components: (32, 178, 170).
        /// </summary>
        public static Color LightSeaGreen => new Color(32, 178, 170);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightSkyBlue" with the RGB components: (135, 206, 250).
        /// </summary>
        public static Color LightSkyBlue => new Color(135, 206, 250);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightSlateGray" with the RGB components: (119, 136, 153).
        /// </summary>
        public static Color LightSlateGray => new Color(119, 136, 153);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightSteelBlue" with the RGB components: (176, 196, 222).
        /// </summary>
        public static Color LightSteelBlue => new Color(176, 196, 222);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LightYellow" with the RGB components: (255, 255, 224).
        /// </summary>
        public static Color LightYellow => new Color(255, 255, 224);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Lime" with the RGB components: (0, 255, 0).
        /// </summary>
        public static Color Lime => new Color(0, 255, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "LimeGreen" with the RGB components: (50, 205, 50).
        /// </summary>
        public static Color LimeGreen => new Color(50, 205, 50);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Linen" with the RGB components: (250, 240, 230).
        /// </summary>
        public static Color Linen => new Color(250, 240, 230);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Magenta" with the RGB components: (255, 0, 255).
        /// </summary>
        public static Color Magenta => new Color(255, 0, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Maroon" with the RGB components: (128, 0, 0).
        /// </summary>
        public static Color Maroon => new Color(128, 0, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumAquamarine" with the RGB components: (102, 205, 170).
        /// </summary>
        public static Color MediumAquamarine => new Color(102, 205, 170);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumBlue" with the RGB components: (0, 0, 205).
        /// </summary>
        public static Color MediumBlue => new Color(0, 0, 205);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumOrchid" with the RGB components: (186, 85, 211).
        /// </summary>
        public static Color MediumOrchid => new Color(186, 85, 211);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumPurple" with the RGB components: (147, 112, 219).
        /// </summary>
        public static Color MediumPurple => new Color(147, 112, 219);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumSeaGreen" with the RGB components: (60, 179, 113).
        /// </summary>
        public static Color MediumSeaGreen => new Color(60, 179, 113);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumSlateBlue" with the RGB components: (123, 104, 238).
        /// </summary>
        public static Color MediumSlateBlue => new Color(123, 104, 238);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumSpringGreen" with the RGB components: (0, 250, 154).
        /// </summary>
        public static Color MediumSpringGreen => new Color(0, 250, 154);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumTurquoise" with the RGB components: (72, 209, 204).
        /// </summary>
        public static Color MediumTurquoise => new Color(72, 209, 204);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MediumVioletRed" with the RGB components: (199, 21, 133).
        /// </summary>
        public static Color MediumVioletRed => new Color(199, 21, 133);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MidnightBlue" with the RGB components: (25, 25, 112).
        /// </summary>
        public static Color MidnightBlue => new Color(25, 25, 112);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MintCream" with the RGB components: (245, 255, 250).
        /// </summary>
        public static Color MintCream => new Color(245, 255, 250);

        /// <summary>
        /// Gets the named <see cref="Color"/> "MistyRose" with the RGB components: (255, 228, 225).
        /// </summary>
        public static Color MistyRose => new Color(255, 228, 225);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Moccasin" with the RGB components: (255, 228, 181).
        /// </summary>
        public static Color Moccasin => new Color(255, 228, 181);

        /// <summary>
        /// Gets the named <see cref="Color"/> "NavajoWhite" with the RGB components: (255, 222, 173).
        /// </summary>
        public static Color NavajoWhite => new Color(255, 222, 173);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Navy" with the RGB components: (0, 0, 128).
        /// </summary>
        public static Color Navy => new Color(0, 0, 128);

        /// <summary>
        /// Gets the named <see cref="Color"/> "OldLace" with the RGB components: (253, 245, 230).
        /// </summary>
        public static Color OldLace => new Color(253, 245, 230);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Olive" with the RGB components: (128, 128, 0).
        /// </summary>
        public static Color Olive => new Color(128, 128, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "OliveDrab" with the RGB components: (107, 142, 35).
        /// </summary>
        public static Color OliveDrab => new Color(107, 142, 35);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Orange" with the RGB components: (255, 165, 0).
        /// </summary>
        public static Color Orange => new Color(255, 165, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "OrangeRed" with the RGB components: (255, 69, 0).
        /// </summary>
        public static Color OrangeRed => new Color(255, 69, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Orchid" with the RGB components: (218, 112, 214).
        /// </summary>
        public static Color Orchid => new Color(218, 112, 214);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PaleGoldenrod" with the RGB components: (238, 232, 170).
        /// </summary>
        public static Color PaleGoldenrod => new Color(238, 232, 170);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PaleGreen" with the RGB components: (152, 251, 152).
        /// </summary>
        public static Color PaleGreen => new Color(152, 251, 152);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PaleTurquoise" with the RGB components: (175, 238, 238).
        /// </summary>
        public static Color PaleTurquoise => new Color(175, 238, 238);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PaleVioletRed" with the RGB components: (219, 112, 147).
        /// </summary>
        public static Color PaleVioletRed => new Color(219, 112, 147);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PapayaWhip" with the RGB components: (255, 239, 213).
        /// </summary>
        public static Color PapayaWhip => new Color(255, 239, 213);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PeachPuff" with the RGB components: (255, 218, 185).
        /// </summary>
        public static Color PeachPuff => new Color(255, 218, 185);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Peru" with the RGB components: (205, 133, 63).
        /// </summary>
        public static Color Peru => new Color(205, 133, 63);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Pink" with the RGB components: (255, 192, 203).
        /// </summary>
        public static Color Pink => new Color(255, 192, 203);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Plum" with the RGB components: (221, 160, 221).
        /// </summary>
        public static Color Plum => new Color(221, 160, 221);

        /// <summary>
        /// Gets the named <see cref="Color"/> "PowderBlue" with the RGB components: (176, 224, 230).
        /// </summary>
        public static Color PowderBlue => new Color(176, 224, 230);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Purple" with the RGB components: (128, 0, 128).
        /// </summary>
        public static Color Purple => new Color(128, 0, 128);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Red" with the RGB components: (255, 0, 0).
        /// </summary>
        public static Color Red => new Color(255, 0, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "RosyBrown" with the RGB components: (188, 143, 143).
        /// </summary>
        public static Color RosyBrown => new Color(188, 143, 143);

        /// <summary>
        /// Gets the named <see cref="Color"/> "RoyalBlue" with the RGB components: (65, 105, 225).
        /// </summary>
        public static Color RoyalBlue => new Color(65, 105, 225);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SaddleBrown" with the RGB components: (139, 69, 19).
        /// </summary>
        public static Color SaddleBrown => new Color(139, 69, 19);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Salmon" with the RGB components: (250, 128, 114).
        /// </summary>
        public static Color Salmon => new Color(250, 128, 114);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SandyBrown" with the RGB components: (244, 164, 96).
        /// </summary>
        public static Color SandyBrown => new Color(244, 164, 96);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SeaGreen" with the RGB components: (46, 139, 87).
        /// </summary>
        public static Color SeaGreen => new Color(46, 139, 87);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SeaShell" with the RGB components: (255, 245, 238).
        /// </summary>
        public static Color SeaShell => new Color(255, 245, 238);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Sienna" with the RGB components: (160, 82, 45).
        /// </summary>
        public static Color Sienna => new Color(160, 82, 45);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Silver" with the RGB components: (192, 192, 192).
        /// </summary>
        public static Color Silver => new Color(192, 192, 192);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SkyBlue" with the RGB components: (135, 206, 235).
        /// </summary>
        public static Color SkyBlue => new Color(135, 206, 235);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SlateBlue" with the RGB components: (106, 90, 205).
        /// </summary>
        public static Color SlateBlue => new Color(106, 90, 205);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SlateGray" with the RGB components: (112, 128, 144).
        /// </summary>
        public static Color SlateGray => new Color(112, 128, 144);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Snow" with the RGB components: (255, 250, 250).
        /// </summary>
        public static Color Snow => new Color(255, 250, 250);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SpringGreen" with the RGB components: (0, 255, 127).
        /// </summary>
        public static Color SpringGreen => new Color(0, 255, 127);

        /// <summary>
        /// Gets the named <see cref="Color"/> "SteelBlue" with the RGB components: (70, 130, 180).
        /// </summary>
        public static Color SteelBlue => new Color(70, 130, 180);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Tan" with the RGB components: (210, 180, 140).
        /// </summary>
        public static Color Tan => new Color(210, 180, 140);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Teal" with the RGB components: (0, 128, 128).
        /// </summary>
        public static Color Teal => new Color(0, 128, 128);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Thistle" with the RGB components: (216, 191, 216).
        /// </summary>
        public static Color Thistle => new Color(216, 191, 216);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Tomato" with the RGB components: (255, 99, 71).
        /// </summary>
        public static Color Tomato => new Color(255, 99, 71);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Turquoise" with the RGB components: (64, 224, 208).
        /// </summary>
        public static Color Turquoise => new Color(64, 224, 208);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Violet" with the RGB components: (238, 130, 238).
        /// </summary>
        public static Color Violet => new Color(238, 130, 238);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Wheat" with the RGB components: (245, 222, 179).
        /// </summary>
        public static Color Wheat => new Color(245, 222, 179);

        /// <summary>
        /// Gets the named <see cref="Color"/> "White" with the RGB components: (255, 255, 255).
        /// </summary>
        public static Color White => new Color(255, 255, 255);

        /// <summary>
        /// Gets the named <see cref="Color"/> "WhiteSmoke" with the RGB components: (245, 245, 245).
        /// </summary>
        public static Color WhiteSmoke => new Color(245, 245, 245);

        /// <summary>
        /// Gets the named <see cref="Color"/> "Yellow" with the RGB components: (255, 255, 0).
        /// </summary>
        public static Color Yellow => new Color(255, 255, 0);

        /// <summary>
        /// Gets the named <see cref="Color"/> "YellowGreen" with the RGB components: (154, 205, 50).
        /// </summary>
        public static Color YellowGreen => new Color(154, 205, 50);

        #endregion

        public static bool operator ==(Color left, Color right)
        {
            return left.R == right.R &&
                   left.G == right.G &&
                   left.B == right.B &&
                   left.A == right.A;
        }

        public static bool operator !=(Color left, Color right)
        {
            return left.R != right.R ||
                   left.G != right.G ||
                   left.B != right.B ||
                   left.A != right.A;
        }

        public static Color operator *(Color value, float scale)
        {
            return new Color(ClampToByte(value.R * scale),
                             ClampToByte(value.G * scale),
                             ClampToByte(value.B * scale),
                             ClampToByte(value.A * scale));
        }

        /// <summary>
        /// Linearly interpolates from one Color to another by a given amount.
        /// </summary>
        /// <param name="from">The source Color.</param>
        /// <param name="to">The destination Color.</param>
        /// <param name="amount">The amount to interpolate by.</param>
        /// <returns>The result of the linear interpolation.</returns>
        public static Color Lerp(Color from, Color to, float amount)
        {
            return new Color(MathHelper.Lerp(from.R / 255f, to.R / 255f, amount),
                             MathHelper.Lerp(from.G / 255f, to.G / 255f, amount),
                             MathHelper.Lerp(from.B / 255f, to.B / 255f, amount),
                             MathHelper.Lerp(from.A / 255f, to.A / 255f, amount));
        }

        /// <summary>
        /// Multiplies the given <see cref="Color"/> by a given value.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to multiply.</param>
        /// <param name="scale">The value to multiply each component by.</param>
        /// <returns>The result of the multiplication.</returns>
        public static Color Multiply(Color value, float scale)
        {
            return new Color(ClampToByte(value.R * scale),
                             ClampToByte(value.G * scale),
                             ClampToByte(value.B * scale),
                             ClampToByte(value.A * scale));
        }

        /// <summary>
        /// Creates a <see cref="Color"/> object from a hex string. Possible formats: #RGB, #RGBA, #RRGGBB, #RRGGBBAA.
        /// </summary>
        /// <param name="hexCode">The hex string to interpret as a Color.</param>
        /// <returns><see cref="Color"/> interpretation of the given hex string.</returns>
        public static Color Parse(string hexCode)
        {
            string trimmed = hexCode.TrimStart('#');

            switch (trimmed.Length)
            {
                case 3:
                    {
                        byte r = byte.Parse(trimmed.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte g = byte.Parse(trimmed.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte b = byte.Parse(trimmed.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        return new Color((byte)(r * 17), (byte)(g * 17), (byte)(b * 17));
                    }

                case 4:
                    {
                        byte r = byte.Parse(trimmed.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte g = byte.Parse(trimmed.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte b = byte.Parse(trimmed.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte a = byte.Parse(trimmed.Substring(3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        return new Color((byte)(r * 17), (byte)(g * 17), (byte)(b * 17), (byte)(a * 17));
                    }

                case 6:
                    {
                        byte r = byte.Parse(trimmed.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte g = byte.Parse(trimmed.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte b = byte.Parse(trimmed.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        return new Color(r, g, b);
                    }

                case 8:
                    {
                        byte r = byte.Parse(trimmed.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte g = byte.Parse(trimmed.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte b = byte.Parse(trimmed.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                        byte a = byte.Parse(trimmed.Substring(6, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);

                        return new Color(r, g, b, a);
                    }

                default:
                    throw new FormatException("Input string was not in a correct format.");
            }
        }

        /// <summary>
        /// Creates a <see cref="Color"/> object from a hex string. A return value indicates whether the operation succeeded.
        /// Possible formats: #RGB, #RGBA, #RRGGBB, #RRGGBBAA.
        /// </summary>
        /// <param name="hexCode">The hex string to interpret as a Color.</param>
        /// <param name="color"><see cref="Color"/> interpretation of the given hex string, or all zeroes if the operation failed.</param>
        /// <returns><see cref="bool"/> value indicating whether the parsing operation succeeded.</returns>
        public static bool TryParse(string hexCode, out Color color)
        {
            string trimmed = hexCode.TrimStart('#');
            
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (trimmed.Length)
            {
                case 3:
                    {
                        if (byte.TryParse(trimmed.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r) &&
                            byte.TryParse(trimmed.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g) &&
                            byte.TryParse(trimmed.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
                        {
                            color = new Color((byte)(r * 17), (byte)(g * 17), (byte)(b * 17));
                            return true;
                        }

                        break;
                    }

                case 4:
                    {
                        if (byte.TryParse(trimmed.Substring(0, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r) &&
                            byte.TryParse(trimmed.Substring(1, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g) &&
                            byte.TryParse(trimmed.Substring(2, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b) &&
                            byte.TryParse(trimmed.Substring(3, 1), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte a))
                        {
                            color = new Color((byte)(r * 17), (byte)(g * 17), (byte)(b * 17), (byte)(a * 17));
                            return true;
                        }

                        break;
                    }

                case 6:
                    {
                        if (byte.TryParse(trimmed.Substring(0, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte r) &&
                            byte.TryParse(trimmed.Substring(2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte g) &&
                            byte.TryParse(trimmed.Substring(4, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte b))
                        {
                            color = new Color(r, g, b);
                            return true;
                        }

                        break;
                    }

                case 8:
                    {
                        if (byte.TryParse(trimmed.Substring(0, 2), NumberStyles.HexNumber, null, out byte r) &&
                            byte.TryParse(trimmed.Substring(2, 2), NumberStyles.HexNumber, null, out byte g) &&
                            byte.TryParse(trimmed.Substring(4, 2), NumberStyles.HexNumber, null, out byte b) &&
                            byte.TryParse(trimmed.Substring(6, 2), NumberStyles.HexNumber, null, out byte a))
                        {
                            color = new Color(r, g, b, a);
                            return true;
                        }

                        break;
                    }
            }

            color = default(Color);
            return false;
        }

        /// <summary>
        /// Creates a <see cref="Color"/> object from a <see cref="Vector3"/> object, with an alpha value of 1.
        /// </summary>
        /// <param name="value">The <see cref="Vector3"/> value.</param>
        /// <returns>The created <see cref="Color"/>.</returns>
        public static Color FromVector3(Vector3 value)
        {
            return new Color((byte)(value.X * 255f),
                             (byte)(value.Y * 255f),
                             (byte)(value.Z * 255f),
                             byte.MaxValue);
        }

        /// <summary>
        /// Creates a <see cref="Color"/> object from a <see cref="Vector4"/> object.
        /// </summary>
        /// <param name="value">The <see cref="Vector4"/> value.</param>
        /// <returns>The created <see cref="Color"/>.</returns>
        public static Color FromVector4(Vector4 value)
        {
            return new Color((byte)(value.X * 255f),
                             (byte)(value.Y * 255f),
                             (byte)(value.Z * 255f),
                             (byte)(value.W * 255f));
        }

        /// <summary>
        /// Creates a <see cref="Vector3"/> object from the <see cref="Color"/>.
        /// </summary>
        /// <returns>The created <see cref="Vector3"/>.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(R / 255f, G / 255f, B / 255f);
        }

        /// <summary>
        /// Creates a <see cref="Vector4"/> object from the <see cref="Color"/>.
        /// </summary>
        /// <returns>The created <see cref="Vector4"/>.</returns>
        public Vector4 ToVector4()
        {
            return new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified object.
        /// </summary>
        /// <param name="value">The object to compare to this instance.</param>
        /// <returns>true if the <paramref name="value">value</paramref> parameter equals the value of this instance; otherwise, false.</returns>
        public override bool Equals(object value)
        {
            if (ReferenceEquals(null, value)) return false;
            return value is Color other && Equals(other);
        }

        /// <summary>
        /// Returns a value indicating whether the value of this instance is equal to the value of the specified <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The <see cref="Color"/> to compare to this instance.</param>
        /// <returns>true if the <paramref name="value">value</paramref> parameter equals the value of this instance; otherwise, false.</returns>
        public bool Equals(Color value)
        {
            return R == value.R && G == value.G && B == value.B && A == value.A;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                int hashCode = R.GetHashCode();
                hashCode = (hashCode * 397) ^ G.GetHashCode();
                hashCode = (hashCode * 397) ^ B.GetHashCode();
                hashCode = (hashCode * 397) ^ A.GetHashCode();
                // ReSharper restore NonReadonlyMemberInGetHashCode
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Color"/> in the format: {(R, G, B, A) = ([red], [green], [blue], [alpha])}.
        /// </summary>
        /// <returns><see cref="string"/> representation of this <see cref="Color"/>.</returns>
        public override string ToString()
        {
            return $"{{(R, G, B, A) = ({R}, {G}, {B}, {A})}}";
        }

        /// <summary>
        /// Deconstructs the <see cref="Color"/> into its components.
        /// </summary>
        /// <param name="r">The red component of the <see cref="Color"/>.</param>
        /// <param name="g">The green component of the <see cref="Color"/>.</param>
        /// <param name="b">The blue component of the <see cref="Color"/>.</param>
        /// <param name="a">The alpha component of the <see cref="Color"/>.</param>
        public void Deconstruct(out byte r, out byte g, out byte b, out byte a)
        {
            r = R;
            g = G;
            b = B;
            a = A;
        }

        /// <summary>
        /// Deconstructs the <see cref="Color"/> into its components.
        /// </summary>
        /// <param name="r">The red component of the <see cref="Color"/>.</param>
        /// <param name="g">The green component of the <see cref="Color"/>.</param>
        /// <param name="b">The blue component of the <see cref="Color"/>.</param>
        public void Deconstruct(out byte r, out byte g, out byte b)
        {
            r = R;
            g = G;
            b = B;
        }

        public unsafe uint ToUint()
        {
            uint a = 0;
            var pointer = (byte*)&a;
            pointer[0] = R;
            pointer[1] = G;
            pointer[2] = B;
            pointer[3] = A;
            return a;
        }

        private static byte ClampToByte(float value)
        {
            return (byte)MathHelper.Clamp(value, byte.MinValue, byte.MaxValue);
        }

        public static implicit operator Color4(Color color)
        {
            return new Color4(color.R, color.G, color.B, color.A);
        }
    }
}