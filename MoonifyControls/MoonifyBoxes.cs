﻿using System;
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
        public static Box ImageBox
        {
            get { return new Box(1, 5, 240, 5, 1, 0, 5, 240, 5, 2); }
        }

        public static Box SwitchBoxSmall
        {
            get { return new Box(1, 11, 12, 11, 0, 0, 7, 2, 7, 2); }
        }
        public static Box SwitchBoxBig
        {
            get { return new Box(0, 7, 52, 7, 0, 0, 8, 10, 6, 1); }
        }
    }
}
