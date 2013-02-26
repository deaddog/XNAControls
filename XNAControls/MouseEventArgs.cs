using System;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    public class MouseEventArgs : EventArgs
    {
        private MouseState state;

        public MouseEventArgs(MouseState state)
        {
            this.state = state;
        }

        /// <summary>
        /// Gets the state of the left mouse button.
        /// </summary>
        public ButtonState LeftButton
        {
            get { return state.LeftButton; }
        }

        /// <summary>
        /// Gets the state of the middle mouse button.
        /// </summary>
        public ButtonState MiddleButton
        {
            get { return state.MiddleButton; }
        }

        /// <summary>
        /// Gets the state of the right mouse button.
        /// </summary>
        public ButtonState RightButton
        {
            get { return state.RightButton; }
        }

        /// <summary>
        /// Get the cumulative mouse scroll wheel value since the game was started.
        /// </summary>
        public int ScrollWheelValue
        {
            get { return state.ScrollWheelValue; }
        }

        /// <summary>
        /// Gets the horizontal postition of the mouse cursor.
        /// </summary>
        public int X
        {
            get { return state.X; }
        }

        /// <summary>
        /// Gets the vertical postition of the mouse cursor.
        /// </summary>
        public int Y
        {
            get { return state.Y; }
        }
    }
}
