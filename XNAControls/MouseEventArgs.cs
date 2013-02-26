using System;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    public class MouseEventArgs : EventArgs
    {
        private int x, y;
        private ButtonState left, middle, right;
        private int scroll;

        public MouseEventArgs(int x, int y, bool leftDown, bool middleDown, bool rightDown, int scroll)
        {
            this.x = x;
            this.y = y;
            this.left = leftDown ? ButtonState.Pressed : ButtonState.Released;
            this.middle = middleDown ? ButtonState.Pressed : ButtonState.Released;
            this.right = rightDown ? ButtonState.Pressed : ButtonState.Released;
            this.scroll = scroll;
        }

        /// <summary>
        /// Gets the state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton
        {
            get { return left; }
        }

        /// <summary>
        /// Gets the state of the middle mouse button.
        /// </summary>
        public ButtonState MiddleButton
        {
            get { return middle; }
        }

        /// <summary>
        /// Gets the state of the right mouse button.
        /// </summary>
        public ButtonState RightButton
        {
            get { return right; }
        }

        /// <summary>
        /// Get the cumulative mouse scroll wheel value since the game was started.
        /// </summary>
        public int ScrollWheelValue
        {
            get { return scroll; }
        }

        /// <summary>
        /// Gets the horizontal postition of the mouse cursor.
        /// </summary>
        public int X
        {
            get { return x; }
        }

        /// <summary>
        /// Gets the vertical postition of the mouse cursor.
        /// </summary>
        public int Y
        {
            get { return y; }
        }
    }
}
