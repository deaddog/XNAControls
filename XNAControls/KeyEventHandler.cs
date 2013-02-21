using System;

namespace XNAControls
{
    /// <summary>
    /// Represents the method that will handle the <see cref="KeyboardInput.KeyDown"/> and the <see cref="KeyboardInput.KeyUp"/> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="KeyEventArgs"/> that contains the event data.</param>
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);
}
