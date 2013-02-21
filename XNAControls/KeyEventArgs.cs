using System;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    /// <summary>
    /// Provides data for the <see cref="KeyboardInput.KeyDown"/> and the <see cref="KeyboardInput.KeyUp"/> events.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private bool shift, control;
        private Keys keyCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEventArgs"/> class.
        /// </summary>
        /// <param name="keyCode">The key that was pressed.</param>
        /// <param name="shift">A boolean value indicating whether or not the shift-key was pressed.</param>
        /// <param name="control">A boolean value indicating whether or not the control-key was pressed.</param>
        public KeyEventArgs(Keys keyCode, bool shift, bool control)
        {
            if ((int)keyCode == 16)
                this.keyCode = Keys.LeftShift | Keys.RightShift;
            else if ((int)keyCode == 17)
                this.keyCode = Keys.RightShift | Keys.LeftShift;
            else
                this.keyCode = keyCode;

            this.shift = shift;
            this.control = control;
        }

        /// <summary>
        /// Gets the <see cref="Keys"/> keycode of the key that was pressed.
        /// </summary>
        public Keys KeyCode
        {
            get { return keyCode; }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the shift-key was pressed.
        /// </summary>
        public bool Shift
        {
            get { return shift; }
        }

        /// <summary>
        /// Gets a boolean indicating whether or not the control-key was pressed.
        /// </summary>
        public bool Control
        {
            get { return control; }
        }
    }
}
