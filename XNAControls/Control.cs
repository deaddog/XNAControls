using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public abstract class Control
    {
        private bool focused = false;
        private bool enabled;

        private ControlContainerBase parent;

        private Vector2 location;
        private Vector2 size;

        public Control(float initialwidth, float initialheight)
        {
            this.enabled = true;
            this.InnerBoundsChange(0, 0, initialwidth, initialheight);
        }

        public bool Focused
        {
            get { return focused; }
        }
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        public ControlContainerBase Parent
        {
            get { return parent; }
            internal set { parent = value; }
        }

        protected Vector2 OffsetLocation
        {
            get { return location + (parent == null ? Vector2.Zero : parent.OffsetLocation); }
        }
        public Vector2 Location
        {
            get { return location; }
            set { InnerBoundsChange(value.X, value.Y, size.X, size.Y); }
        }
        public float X
        {
            get { return location.X; }
            set { Location = new Vector2(value, location.Y); }
        }
        public float Y
        {
            get { return location.Y; }
            set { Location = new Vector2(location.X, value); }
        }

        public Vector2 Size
        {
            get { return size; }
            set { InnerBoundsChange(location.X, location.Y, value.X, value.Y); }
        }
        public float Width
        {
            get { return size.X; }
            set { Size = new Vector2(value, size.Y); }
        }
        public float Height
        {
            get { return size.Y; }
            set { Size = new Vector2(size.X, value); }
        }

        /// <summary>
        /// Raises the <see cref="E:LocationChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLocationChanged(EventArgs e)
        {
            if (LocationChanged != null)
                LocationChanged(this, e);
        }
        /// <summary>
        /// Occurs when the <see cref="Location"/> property value has changed.
        /// </summary>
        public event EventHandler LocationChanged;
        /// <summary>
        /// Raises the <see cref="E:SizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSizeChanged(EventArgs e)
        {
            if (SizeChanged != null)
                SizeChanged(this, e);
        }
        /// <summary>
        /// Occurs when the <see cref="Size"/> property value has changed.
        /// </summary>
        public event EventHandler SizeChanged;

        protected internal virtual void Message(ControlMessages msg, params int[] par)
        {
            switch (msg)
            {
                case ControlMessages.KEYBOARD_CHARACTER:
                    OnCharacterEntered(new CharacterEventArgs(Convert.ToChar(par[0]), par[1]));
                    break;
                case ControlMessages.KEYBOARD_KEYDOWN:
                    OnKeyDown(new KeyEventArgs((Microsoft.Xna.Framework.Input.Keys)par[0], (par[1] & 1) != 0, (par[1] & 2) != 0));
                    break;
                case ControlMessages.KEYBOARD_KEYUP:
                    OnKeyUp(new KeyEventArgs((Microsoft.Xna.Framework.Input.Keys)par[0], (par[1] & 1) != 0, (par[1] & 2) != 0));
                    break;

                case ControlMessages.MOUSE_MOVE:
                case ControlMessages.MOUSE_DOWN:
                case ControlMessages.MOUSE_UP:
                case ControlMessages.MOUSE_CLICK:
                case ControlMessages.MOUSE_WHEEL:
                    handleMouseMessage(msg, new MouseEventArgs(par[0], par[1], (MouseButtons)par[2], par[3]));
                    break;

                case ControlMessages.MOUSE_ENTER:
                    OnMouseEnter(EventArgs.Empty);
                    break;
                case ControlMessages.MOUSE_LEAVE:
                    OnMouseLeave(EventArgs.Empty);
                    break;

                case ControlMessages.CONTROL_GOTFOCUS:
                    focused = true;
                    OnGotFocus(EventArgs.Empty);
                    break;
                case ControlMessages.CONTROL_LOSTFOCUS:
                    focused = false;
                    OnLostFocus(EventArgs.Empty);
                    break;

                case ControlMessages.CONTROL_SIZECHANGED:
                    OnSizeChanged(EventArgs.Empty);
                    break;

                case ControlMessages.CONTROL_LOCATIONCHANGED:
                    OnLocationChanged(EventArgs.Empty);
                    break;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:GotFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (GotFocus != null)
                GotFocus(this, e);
        }
        /// <summary>
        /// Occurs when the control gets focus and can receive keyboard input.
        /// </summary>
        public event EventHandler GotFocus;
        /// <summary>
        /// Raises the <see cref="E:LostFocus"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLostFocus(EventArgs e)
        {
            if (LostFocus != null)
                LostFocus(this, e);
        }
        /// <summary>
        /// Occurs when the control loses focus and can not receive keyboard input.
        /// </summary>
        public event EventHandler LostFocus;

        private void handleMouseMessage(ControlMessages msg, MouseEventArgs e)
        {
            switch (msg)
            {
                case ControlMessages.MOUSE_MOVE: OnMouseMove(e); break;
                case ControlMessages.MOUSE_DOWN: OnMouseDown(e); break;
                case ControlMessages.MOUSE_UP: OnMouseUp(e); break;
                case ControlMessages.MOUSE_CLICK: OnMouseClick(e); break;
                case ControlMessages.MOUSE_WHEEL: OnMouseWheel(e); break;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CharacterEntered"/> event.
        /// </summary>
        /// <param name="e">The <see cref="CharacterEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCharacterEntered(CharacterEventArgs e)
        {
            if (CharacterEntered != null)
                CharacterEntered(this, e);
        }
        /// <summary>
        /// Occurs when a character is entered while the control has focus.
        /// </summary>
        public event CharEnteredHandler CharacterEntered;
        /// <summary>
        /// Raises the <see cref="E:KeyDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }
        /// <summary>
        /// Occurs when a key is pressed while the control has focus.
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Raises the <see cref="E:KeyUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(this, e);
        }
        /// <summary>
        /// Occurs when a key is released while the control has focus.
        /// </summary>
        public event KeyEventHandler KeyUp;

        /// <summary>
        /// Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseEnter(EventArgs e)
        {
            if (MouseEnter != null)
                MouseEnter(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer enters the control.
        /// </summary>
        public event EventHandler MouseEnter;
        /// <summary>
        /// Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseLeave(EventArgs e)
        {
            if (MouseLeave != null)
                MouseLeave(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer leaves the control.
        /// </summary>
        public event EventHandler MouseLeave;
        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (MouseMove != null)
                MouseMove(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the control and the mouse is moved.
        /// </summary>
        public event MouseEventHandler MouseMove;
        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is pressed.
        /// </summary>
        public event MouseEventHandler MouseDown;
        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        public event MouseEventHandler MouseUp;
        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(this, e);
        }
        /// <summary>
        /// Occurs when the control is clicked by the mouse.
        /// </summary>
        public event MouseEventHandler MouseClick;
        /// <summary>
        /// Raises the <see cref="E:MouseWheel"/> event.
        /// </summary>
        /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            if (MouseWheel != null)
                MouseWheel(this, e);
        }
        /// <summary>
        /// Occurs when the mouse pointer is over the control and the mouse wheel moves.
        /// </summary>
        public event MouseEventHandler MouseWheel;

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        protected internal virtual void LoadContent(ContentManagers content)
        {
        }
        protected internal virtual void UnloadContent()
        {
        }

        public bool IsInside(int x, int y)
        {
            return IsInside((float)x, (float)y);
        }
        public bool IsInside(float x, float y)
        {
            return IsInside(new Vector2(x, y));
        }
        public bool IsInside(Vector2 point)
        {
            point -= this.OffsetLocation;
            return point.X >= 0 && point.X < this.size.X && point.Y >= 0 && point.Y < this.size.Y;
        }

        protected virtual void InnerBoundsChange(float x, float y, float width, float height)
        {
            var l = this.location;
            var s = this.size;

            this.location = new Vector2(x, y);
            this.size = new Vector2(width, height);

            if (l != this.location)
                Message(ControlMessages.CONTROL_LOCATIONCHANGED);

            if (s != this.size)
                Message(ControlMessages.CONTROL_SIZECHANGED);
        }
    }
}
