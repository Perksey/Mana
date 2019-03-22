using System;

namespace Mana
{
    /// <summary>
    /// Represents a keyboard key.
    /// </summary>
    public enum Key
    {
        /// <summary>
        /// A key that is not known or supported.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The A key.
        /// </summary>
        A,

        /// <summary>
        /// The B key.
        /// </summary>
        B,

        /// <summary>
        /// The C key.
        /// </summary>
        C,

        /// <summary>
        /// The D key.
        /// </summary>
        D,

        /// <summary>
        /// The E key.
        /// </summary>
        E,

        /// <summary>
        /// The F key.
        /// </summary>
        F,

        /// <summary>
        /// The G key.
        /// </summary>
        G,

        /// <summary>
        /// The H key.
        /// </summary>
        H,

        /// <summary>
        /// The I key.
        /// </summary>
        I,

        /// <summary>
        /// The J key.
        /// </summary>
        J,

        /// <summary>
        /// The K key.
        /// </summary>
        K,

        /// <summary>
        /// The L key.
        /// </summary>
        L,

        /// <summary>
        /// The M key.
        /// </summary>
        M,

        /// <summary>
        /// The N key.
        /// </summary>
        N,

        /// <summary>
        /// The O key.
        /// </summary>
        O,

        /// <summary>
        /// The P key.
        /// </summary>
        P,

        /// <summary>
        /// The Q key.
        /// </summary>
        Q,

        /// <summary>
        /// The R key.
        /// </summary>
        R,

        /// <summary>
        /// The S key.
        /// </summary>
        S,

        /// <summary>
        /// The T key.
        /// </summary>
        T,

        /// <summary>
        /// The U key.
        /// </summary>
        U,

        /// <summary>
        /// The V key.
        /// </summary>
        V,

        /// <summary>
        /// The W key.
        /// </summary>
        W,

        /// <summary>
        /// The X key.
        /// </summary>
        X,

        /// <summary>
        /// The Y key.
        /// </summary>
        Y,

        /// <summary>
        /// The Z key.
        /// </summary>
        Z,

        /// <summary>
        /// The Escape key.
        /// </summary>
        Escape,

        /// <summary>
        /// The Backtick key.
        /// </summary>
        Backtick,

        /// <summary>
        /// The 0 or ) key.
        /// </summary>
        Number0,

        /// <summary>
        /// The 1 or ! key.
        /// </summary>
        Number1,

        /// <summary>
        /// The 2 or @ key.
        /// </summary>
        Number2,

        /// <summary>
        /// The 3 or # key.
        /// </summary>
        Number3,

        /// <summary>
        /// The 4 or $ key.
        /// </summary>
        Number4,

        /// <summary>
        /// The 5 or % key.
        /// </summary>
        Number5,

        /// <summary>
        /// The 6 or ^ key.
        /// </summary>
        Number6,

        /// <summary>
        /// The 7 or &amp; key.
        /// </summary>
        Number7,

        /// <summary>
        /// The 8 or * key.
        /// </summary>
        Number8,

        /// <summary>
        /// The 9 or ( key.
        /// </summary>
        Number9,

        /// <summary>
        /// The - or _ key.
        /// </summary>
        Hyphen,

        /// <summary>
        /// The = or + key.
        /// </summary>
        Equals,

        /// <summary>
        /// The Backspace key.
        /// </summary>
        Backspace,

        /// <summary>
        /// The Tab key.
        /// </summary>
        Tab,

        /// <summary>
        /// The [ or { key.
        /// </summary>
        LeftBracket,

        /// <summary>
        /// The ] or } key.
        /// </summary>
        RightBracket,

        /// <summary>
        /// The \ or | key.
        /// </summary>
        BackSlash,

        /// <summary>
        /// The Caps Lock key.
        /// </summary>
        CapsLock,

        /// <summary>
        /// The ; or : key.
        /// </summary>
        Semicolon,

        /// <summary>
        /// The &apos; or &quot; key.
        /// </summary>
        Quote,

        /// <summary>
        /// The Enter key.
        /// </summary>
        Enter,

        /// <summary>
        /// The Left Shift key.
        /// </summary>
        LeftShift,

        /// <summary>
        /// The , or &lt; key.
        /// </summary>
        Comma,

        /// <summary>
        /// The . or &gt; key.
        /// </summary>
        Period,

        /// <summary>
        /// The / or ? key.
        /// </summary>
        ForwardSlash,

        /// <summary>
        /// The Right Shift key.
        /// </summary>
        RightShift,

        /// <summary>
        /// The Left Control key.
        /// </summary>
        LeftControl,

        /// <summary>
        /// The Windows key.
        /// </summary>
        Windows,

        /// <summary>
        /// The Left Alt key.
        /// </summary>
        LeftAlt,

        /// <summary>
        /// The Space key.
        /// </summary>
        Space,

        /// <summary>
        /// The Right Alt key.
        /// </summary>
        RightAlt,

        /// <summary>
        /// The Applications key.
        /// </summary>
        Applications,

        /// <summary>
        /// The Right Control key.
        /// </summary>
        RightControl,

        /// <summary>
        /// The F1 key.
        /// </summary>
        F1,

        /// <summary>
        /// The F2 key.
        /// </summary>
        F2,

        /// <summary>
        /// The F3 key.
        /// </summary>
        F3,

        /// <summary>
        /// The F4 key.
        /// </summary>
        F4,

        /// <summary>
        /// The F5 key.
        /// </summary>
        F5,

        /// <summary>
        /// The F6 key.
        /// </summary>
        F6,

        /// <summary>
        /// The F7 key.
        /// </summary>
        F7,

        /// <summary>
        /// The F8 key.
        /// </summary>
        F8,

        /// <summary>
        /// The F9 key.
        /// </summary>
        F9,

        /// <summary>
        /// The F10 key.
        /// </summary>
        F10,

        /// <summary>
        /// The F11 key.
        /// </summary>
        F11,

        /// <summary>
        /// The F12 key.
        /// </summary>
        F12,

        /// <summary>
        /// The Scroll Lock key.
        /// </summary>
        ScrollLock,

        /// <summary>
        /// The Pause Break key.
        /// </summary>
        PauseBreak,

        /// <summary>
        /// The Insert key.
        /// </summary>
        Insert,

        /// <summary>
        /// The Home key.
        /// </summary>
        Home,

        /// <summary>
        /// The Page Up key.
        /// </summary>
        PageUp,

        /// <summary>
        /// The Delete key.
        /// </summary>
        Delete,

        /// <summary>
        /// The End key.
        /// </summary>
        End,

        /// <summary>
        /// The Page Down key.
        /// </summary>
        PageDown,

        /// <summary>
        /// The Up Arrow key.
        /// </summary>
        Up,

        /// <summary>
        /// The Left Arrow key.
        /// </summary>
        Left,

        /// <summary>
        /// The Down Arrow key.
        /// </summary>
        Down,

        /// <summary>
        /// The Right Arrow key.
        /// </summary>
        Right,

        /// <summary>
        /// The Keypad's Numeric Lock key. 
        /// </summary>
        NumLock,

        /// <summary>
        /// The Keypad's Divide (/) key.
        /// </summary>
        KeypadDivide,

        /// <summary>
        /// The Keypad's Multiply (*) key.
        /// </summary>
        KeypadMultiply,

        /// <summary>
        /// The Keypad's Subtract (-) key.
        /// </summary>
        KeypadSubtract,

        /// <summary>
        /// The Keypad's Add (+) key.
        /// </summary>
        KeypadAdd,

        /// <summary>
        /// The Keypad's Decimal (.) key.
        /// </summary>
        KeypadDecimal,

        /// <summary>
        /// The Keypad's 0 key.
        /// </summary>
        Keypad0,

        /// <summary>
        /// The Keypad's 1 key.
        /// </summary>
        Keypad1,

        /// <summary>
        /// The Keypad's 2 key.
        /// </summary>
        Keypad2,

        /// <summary>
        /// The Keypad's 3 key.
        /// </summary>
        Keypad3,

        /// <summary>
        /// The Keypad's 4 key.
        /// </summary>
        Keypad4,

        /// <summary>
        /// The Keypad's 5 key.
        /// </summary>
        Keypad5,

        /// <summary>
        /// The Keypad's 6 key.
        /// </summary>
        Keypad6,

        /// <summary>
        /// The Keypad's 7 key.
        /// </summary>
        Keypad7,

        /// <summary>
        /// The Keypad's 8 key.
        /// </summary>
        Keypad8,

        /// <summary>
        /// The Keypad's 9 key.
        /// </summary>
        Keypad9,
    }

    internal static class KeyConverter
    {
        public static Key FromDesktopScanCode(uint scanCode)
        {
            switch (scanCode)
            {
                case 1:
                    return Key.LeftShift;
                case 2:
                    return Key.RightShift;
                case 3:
                    return Key.LeftControl;
                case 4:
                    return Key.RightControl;
                case 5:
                    return Key.LeftAlt;
                case 6:
                    return Key.RightAlt;
                case 7:
                    return Key.Windows;
                case 10:
                    return Key.F1;
                case 11:
                    return Key.F2;
                case 12:
                    return Key.F3;
                case 13:
                    return Key.F4;
                case 14:
                    return Key.F5;
                case 15:
                    return Key.F6;
                case 16:
                    return Key.F7;
                case 17:
                    return Key.F8;
                case 18:
                    return Key.F9;
                case 19:
                    return Key.F10;
                case 20:
                    return Key.F11;
                case 21:
                    return Key.F12;
                case 45:
                    return Key.Up;
                case 46:
                    return Key.Down;
                case 47:
                    return Key.Left;
                case 48:
                    return Key.Right;
                case 49:
                case 82:
                    return Key.Enter;
                case 50:
                    return Key.Escape;
                case 51:
                    return Key.Space;
                case 52:
                    return Key.Tab;
                case 53:
                    return Key.Backspace;
                case 54:
                    return Key.Insert;
                case 55:
                    return Key.Delete;
                case 56:
                    return Key.PageUp;
                case 57:
                    return Key.PageDown;
                case 58:
                    return Key.Home;
                case 59:
                    return Key.End;
                case 60:
                    return Key.CapsLock;
                case 61:
                    return Key.ScrollLock;
                case 63:
                    return Key.PauseBreak;
                case 64:
                    return Key.NumLock;
                case 67:
                    return Key.Keypad0;
                case 68:
                    return Key.Keypad1;
                case 69:
                    return Key.Keypad2;
                case 70:
                    return Key.Keypad3;
                case 71:
                    return Key.Keypad4;
                case 72:
                    return Key.Keypad5;
                case 73:
                    return Key.Keypad6;
                case 74:
                    return Key.Keypad7;
                case 75:
                    return Key.Keypad8;
                case 76:
                    return Key.Keypad9;
                case 77:
                    return Key.KeypadDivide;
                case 78:
                    return Key.KeypadMultiply;
                case 79:
                    return Key.KeypadSubtract;
                case 80:
                    return Key.KeypadAdd;
                case 81:
                    return Key.KeypadDecimal;
                case 83:
                    return Key.A;
                case 84:
                    return Key.B;
                case 85:
                    return Key.C;
                case 86:
                    return Key.D;
                case 87:
                    return Key.E;
                case 88:
                    return Key.F;
                case 89:
                    return Key.G;
                case 90:
                    return Key.H;
                case 91:
                    return Key.I;
                case 92:
                    return Key.J;
                case 93:
                    return Key.K;
                case 94:
                    return Key.L;
                case 95:
                    return Key.M;
                case 96:
                    return Key.N;
                case 97:
                    return Key.O;
                case 98:
                    return Key.P;
                case 99:
                    return Key.Q;
                case 100:
                    return Key.R;
                case 101:
                    return Key.S;
                case 102:
                    return Key.T;
                case 103:
                    return Key.U;
                case 104:
                    return Key.V;
                case 105:
                    return Key.W;
                case 106:
                    return Key.X;
                case 107:
                    return Key.Y;
                case 108:
                    return Key.Z;
                case 109:
                    return Key.Number0;
                case 110:
                    return Key.Number1;
                case 111:
                    return Key.Number2;
                case 112:
                    return Key.Number3;
                case 113:
                    return Key.Number4;
                case 114:
                    return Key.Number5;
                case 115:
                    return Key.Number6;
                case 116:
                    return Key.Number7;
                case 117:
                    return Key.Number8;
                case 118:
                    return Key.Number9;
                case 119:
                    return Key.Backtick;
                case 120:
                    return Key.Hyphen;
                case 121:
                    return Key.Equals;
                case 122:
                    return Key.LeftBracket;
                case 123:
                    return Key.RightBracket;
                case 124:
                    return Key.Semicolon;
                case 125:
                    return Key.Quote;
                case 126:
                    return Key.Comma;
                case 127:
                    return Key.Period;
                case 128:
                    return Key.ForwardSlash;
                case 129:
                    return Key.BackSlash;
                default:
#if DEBUG
                    throw new NotImplementedException("Unsupported key pressed: " + scanCode + ". This should be added to the KeyConverter.");
#else
                    return Key.Unknown;
#endif
            }
        }
    }
}
