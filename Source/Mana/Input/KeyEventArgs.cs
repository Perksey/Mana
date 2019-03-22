using System;

namespace Mana
{
    public struct KeyEventArgs
    {
        public KeyEventArgs(Key key)
        {
            Key = key;
        }

        public Key Key { get; }
    }
}
