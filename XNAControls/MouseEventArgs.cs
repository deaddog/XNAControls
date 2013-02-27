using System;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    public class MouseEventArgs : EventArgs
    {
        private int x, y;
        private MouseButtons buttons;
        private int scroll;

        public MouseEventArgs(int x, int y, MouseButtons buttons, int scroll)
        {
            this.x = x;
            this.y = y;
            this.buttons = buttons;
            this.scroll = scroll;
        }

        public MouseButtons Buttons
        {
            get { return buttons; }
        }

        /// <summary>
        /// Get the mouse scroll wheel value difference since the wheel was last used.
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
