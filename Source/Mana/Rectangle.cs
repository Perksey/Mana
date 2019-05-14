namespace Mana
{
    public struct Rectangle
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        
        public bool Equals(Rectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            return obj is Rectangle other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int Left => X;

        public int Right => X + Width;
        public int Top => Y;

        public int Bottom => Top + Height;

        public static bool operator ==(Rectangle left, Rectangle right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Rectangle left, Rectangle right)
        {
            return !(left.Equals(right));
        }
    }
}