using System;

namespace XNAControls
{
    /// <summary>
    /// Represents the method that will handle the <see cref="xfloat.Elapsed"/> and the <see cref="xfloat.Tick"/> event of an <see cref="xfloat"/> object.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="FloatMoveEventArgs"/> that contains the event data.</param>
    public delegate void FloatMoveEventHandler(object sender, FloatMoveEventArgs e);
}
