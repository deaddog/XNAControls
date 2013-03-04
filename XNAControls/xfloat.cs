using System;
using DeadDog.GUI;
using Microsoft.Xna.Framework;

namespace XNAControls
{
    /// <summary>
    /// Represents a "moveable" floating point value, through a simple interface.
    /// </summary>
    public class xfloat
    {
        private DateTime started;
        private bool running = false;
        private bool updatedonlast = false;
        private bool currentElapsed = false;

        private float value;
        private IMoveMethods method;
        private MovementInfo info;

        /// <summary>
        /// Initializes a new instance of the <see cref="xfloat"/> class with a specific starting point and method for further movement.
        /// </summary>
        /// <param name="value">The initial value of this <see cref="xfloat"/> instance.</param>
        /// <param name="method">The method used for calculating the movement of this <see cref="xfloat"/>.
        /// This value is cloned by the constructor. Thus multiple <see cref="xfloat"/> can use the "same" method.</param>
        public xfloat(float value, IMoveMethods method)
        {
            this.value = value;
            this.info = new MovementInfo(value, value, 0);
            this.method = (IMoveMethods)method.Clone();
            this.method.Info = this.info;
            this.started = DateTime.Now;
        }

        /// <summary>
        /// Updates the <see cref="CurrentValue"/> property of this <see cref="xfloat"/>.
        /// </summary>
        public void Update()
        {
            updatedonlast = running;
            if (!running)
                return;

            float oldval = this.value;
            float time = (float)((DateTime.Now - started).TotalSeconds);
            if (time >= method.Time)
            {
                time = method.Time;
                this.value = info.PointEnd;
                running = false;
            }
            else
                this.value = method.Position(time);

            if (this.value == float.NaN || this.value == float.PositiveInfinity || this.value == float.NegativeInfinity)
                this.value = oldval;

            ValueUpdated();

            FloatMoveEventArgs args = new FloatMoveEventArgs(DateTime.Now, this);
            OnTick(args);
            if (!running)
                OnElapsed(args);
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CurrentValue"/> was changed on the last Update call.
        /// </summary>
        public bool Updated
        {
            get { return updatedonlast; }
        }

        /// <summary>
        /// Implicitly converts an <see cref="xfloat"/> object to a float by returning it's CurrentValue property.
        /// </summary>
        /// <param name="item">The <see cref="xfloat"/> to be converted.</param>
        /// <returns>The CurrentValue property of the <see cref="xfloat"/> object.</returns>
        public static implicit operator float(xfloat item)
        {
            return item.value;
        }

        /// <summary>
        /// Gets or sets the target value of this <see cref="xfloat"/>.
        /// </summary>
        public float TargetValue
        {
            get { return info.PointEnd; }
            set
            {
                if (info.PointEnd == value)
                    return;

                DateTime now = DateTime.Now;

                float time = (float)((now - started).TotalSeconds);
                if (time >= method.Time)
                    time = method.Time;

                MovementInfo m;
                if (running)
                    m = new MovementInfo(method.Position(time), value, method.Speed(time));
                else
                    m = new MovementInfo(this.value, value, 0);
                method.Info = m;
                this.info = m;
                running = true;

                started = now;
            }
        }
        /// <summary>
        /// Gets or sets the current value of this <see cref="xfloat"/>.
        /// After setting, CurrentValue will always equal TargetValue.
        /// </summary>
        public float CurrentValue
        {
            get { return this.value; }
            set
            {
                if (!running && this.value == value)
                    return;

                running = false;
                this.value = value;
                this.info = new MovementInfo(value, value, 0);

                ValueUpdated();

                if (currentElapsed)
                {
                    FloatMoveEventArgs args = new FloatMoveEventArgs(DateTime.Now, this);
                    OnElapsed(args);
                }
            }
        }
        /// <summary>
        /// Returns the amount of the time required by the current transition.
        /// </summary>
        public float CurrentTime
        {
            get
            {
                if (method == null)
                    return 0;
                else
                    return method.Time;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Elapsed"/> event is raised when setting the CurrentValue property.
        /// Defaults to false.
        /// </summary>
        public bool CurrentRaisesElapsed
        {
            get { return currentElapsed; }
            set { currentElapsed = value; }
        }

        /// <summary>
        /// Sets the <see cref="IMoveMethods"/> method to use for movement in this <see cref="xfloat"/>.
        /// Any current move will automatically be stopped when calling this method.
        /// </summary>
        /// <param name="method">The <see cref="IMoveMethods"/> method to use for movement.</param>
        public void SetMethod(IMoveMethods method)
        {
            CurrentValue = TargetValue;
            this.method = method;
        }

        /// <summary>
        /// Occurs when TargetValue == CurrentValue.
        /// </summary>
        public event FloatMoveEventHandler Elapsed;
        /// <summary>
        /// Raises the <see cref="Elapsed"/> event.
        /// </summary>
        /// <param name="e">A <see cref="FloatMoveEventArgs"/> that contains the event data.</param>
        protected virtual void OnElapsed(FloatMoveEventArgs e)
        {
            if (Elapsed != null)
                Elapsed(this, e);
        }
        /// <summary>
        /// Occurs when the position of this <see cref="xfloat"/> is updated.
        /// Typically this happens once every cycle.
        /// </summary>
        public event FloatMoveEventHandler Tick;
        /// <summary>
        /// Raises the <see cref="Tick"/> event.
        /// </summary>
        /// <param name="e">A <see cref="FloatMoveEventArgs"/> that contains the event data.</param>
        protected virtual void OnTick(FloatMoveEventArgs e)
        {
            if (Tick != null)
                Tick(this, e);
        }

        /// <summary>
        /// Called everytime the value of this <see cref="xfloat"/> is updated, allowing to perform updates on deriving classes.
        /// </summary>
        protected virtual void ValueUpdated()
        {
        }
    }
}
