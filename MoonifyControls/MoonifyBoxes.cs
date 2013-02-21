using System;
using XNAControls;

namespace MoonifyControls
{
    internal static class MoonifyBoxes
    {
        public static Box EmptyBox
        {
            get { return new Box(1, 3, 204, 3, 1, 0, 4, 94, 4, 2); }
        }
        public static Box EmptyBoxFrame
        {
            get { return new Box(1, 3, 204, 3, 1, 0, 4, 94, 4, 2); }
        }
        public static Box EmptyBoxFill
        {
            get { return new Box(0, 3, 204, 3, 0, 0, 4, 94, 4, 0); }
        }
    }
}
