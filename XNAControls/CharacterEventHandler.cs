using System;

namespace XNAControls
{
    /// <summary>
    /// Represents the method that will handle the <see cref="KeyboardInput.CharacterEntered"/> event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="CharacterEventArgs"/> that contains the event data.</param>
    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);
}
