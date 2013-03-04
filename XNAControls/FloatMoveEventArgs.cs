using System;

namespace XNAControls
{
    /// <summary>
    /// Provides data for the <see cref="xfloat.Elapsed"/> and the <see cref="xfloat.Tick"/> event of an <see cref="xfloat"/> object.
    /// </summary>
    public class FloatMoveEventArgs : EventArgs
    {
        private xfloat value;
        private DateTime signal;

        internal FloatMoveEventArgs(DateTime signal, xfloat value)
            : base()
        {
            this.signal = signal;
            this.value = value;
        }

        /// <summary>
        /// Gets the time the <see cref="xfloat.Elapsed"/> or the <see cref="xfloat.Tick"/> event was raised.
        /// </summary>
        public DateTime SignalTime
        {
            get { return signal; }
        }

        /// <summary>
        /// Gets the <see cref="xfloat"/> associated with the event.
        /// </summary>
        public xfloat Value
        {
            get { return value; }
        }
    }
}
