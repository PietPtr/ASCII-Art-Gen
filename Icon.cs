using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASCII_art
{
    class Icon : IComparable
    {
        public Vector2 position;
        public int blackPixels;
        public char character;

        public int CompareTo(object obj) //not mine (hele method)
        {
            if (obj == null) return 1;

            Icon otherIcon = obj as Icon;
            if (otherIcon != null)
                return this.blackPixels.CompareTo(otherIcon.blackPixels);
            else
                throw new ArgumentException("Object is not a icon");
        }

        public Icon(Vector2 position, int blackPixels, char character)
        {
            this.position = position;
            this.blackPixels = blackPixels;
            this.character = character;
        }
    }
}
