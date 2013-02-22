using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public class Control
    {
        private bool enabled;
        private Vector2 position;
        private Vector2 size;

        public Control(float initialwidth, float initialheight)
        {
            this.enabled = true;
            this.position = Vector2.Zero;
            this.InnerSizeChange(initialwidth, initialheight);
        }

        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public Vector2 Location
        {
            get { return position; }
            set
            {
                var temp = position;
                position = value;
                if (temp != position)
                    OnLocationChanged(EventArgs.Empty);
            }
        }
        public Vector2 Size
        {
            get { return size; }
            set { InnerSizeChange(value.X, value.Y); }
        }

        protected virtual void OnLocationChanged(EventArgs e)
        {
            if (LocationChanged != null)
                LocationChanged(this, e);
        }
        public event EventHandler LocationChanged;
        protected virtual void OnSizeChanged(EventArgs e)
        {
            if (SizeChanged != null)
                SizeChanged(this, e);
        }
        public event EventHandler SizeChanged;

        internal protected void _CharacterEntered(CharacterEventArgs e) { OnCharacterEntered(e); }
        internal protected void _KeyDown(KeyEventArgs e) { OnKeyDown(e); }
        internal protected void _KeyUp(KeyEventArgs e) { OnKeyUp(e); }

        protected virtual void OnCharacterEntered(CharacterEventArgs e)
        {
            if (CharacterEntered != null)
                CharacterEntered(this, e);
        }
        public event CharEnteredHandler CharacterEntered;
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }
        public event KeyEventHandler KeyDown;
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(this, e);
        }
        public event KeyEventHandler KeyUp;

        public virtual void Update(GameTime gameTime)
        {
        }
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
        }

        public virtual void LoadResources(ContentManager content)
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
            point -= this.position;
            return point.X >= 0 && point.X < this.size.X && point.Y >= 0 && point.Y < this.size.Y;
        }

        protected virtual void InnerSizeChange(float width, float height)
        {
            var temp = this.size;
            this.size = new Vector2(width, height);

            if (temp != this.size)
                OnSizeChanged(EventArgs.Empty);
        }
    }
}
