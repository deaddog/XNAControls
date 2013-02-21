using System;

namespace XNAControls
{
    /// <summary>
    /// Provides data for the <see cref="KeyboardInput.CharacterEntered"/> event.
    /// </summary>
    public class CharacterEventArgs : EventArgs
    {
        private readonly char character;
        private readonly int lParam;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterEventArgs"/> class.
        /// </summary>
        /// <param name="character">The character that was entered.</param>
        /// <param name="lParam">Additional key-press information.</param>
        public CharacterEventArgs(char character, int lParam)
        {
            this.character = character;
            this.lParam = lParam;
        }

        /// <summary>
        /// Gets the character that was pressed.
        /// </summary>
        public char Character
        {
            get { return character; }
        }
        internal int Param
        {
            get { return lParam; }
        }
        internal int RepeatCount
        {
            get { return lParam & 0xffff; }
        }
        internal bool ExtendedKey
        {
            get { return (lParam & (1 << 24)) > 0; }
        }
        /// <summary>
        /// Gets a boolean value indicating if the Alt-key was down as this character was entered.
        /// </summary>
        public bool AltPressed
        {
            get { return (lParam & (1 << 29)) > 0; }
        }
        internal bool PreviousState
        {
            get { return (lParam & (1 << 30)) > 0; }
        }
        internal bool TransitionState
        {
            get { return (lParam & (1 << 31)) > 0; }
        }
    }
}
