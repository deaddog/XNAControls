using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAControls;

namespace MoonifyControls
{
    public class TextBox : Control
    {
        private Box box;
        private Texture2D backgroundBox;
        private Texture2D caret;
        private Texture2D iconbackground;
        private Texture2D icon = null;
        private IconTypes iconType = IconTypes.None;

        private Texture2D iconSEARCH;

        private CharacterRenderer font;

        private System.DateTime lastCaretSwap = System.DateTime.MinValue;
        private int delayCaret = 500;
        private bool showCaret = false;
        private int _caretIndex = 0;
        private int caretIndex
        {
            get { return _caretIndex; }
            set { _caretIndex = value; lastCaretSwap = DateTime.Now; showCaret = true; }
        }

        private string text = string.Empty;
        private string backgroundText = string.Empty;
        private int backgroundTextOffset = 0;

        public TextBox()
            : base(200, 30)
        {
            font = new CharacterRenderer("HelveticaNeueLT Com 65 Md", 9f, System.Drawing.FontStyle.Regular, System.Drawing.Text.TextRenderingHint.AntiAlias);
        }

        public IconTypes IconType
        {
            get { return iconType; }
            set { this.iconType = value; }
        }
        public Texture2D Icon
        {
            get { return icon; }
            set { icon = value; }
        }
        public string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (this.text == value)
                    return;

                caretIndex = value.Length;
                this.text = value;

                OnTextChanged(EventArgs.Empty);
            }
        }
        public string BackgroundText
        {
            get { return backgroundText; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.backgroundText = value;

                OnBackgroundTextChanged(EventArgs.Empty);
            }
        }
        public int BackgroundTextOffset
        {
            get { return backgroundTextOffset; }
            set { backgroundTextOffset = value; }
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }
        public event EventHandler TextChanged;
        protected virtual void OnBackgroundTextChanged(EventArgs e)
        {
            if (BackgroundTextChanged != null)
                BackgroundTextChanged(this, e);
        }
        public event EventHandler BackgroundTextChanged;

        protected override void Message(ControlMessages msg, params int[] par)
        {
            switch (msg)
            {
                case ControlMessages.KEYBOARD_CHARACTER:
                    handleCharacter(Convert.ToChar(par[0]));
                    base.Message(msg, par);
                    break;

                case ControlMessages.KEYBOARD_KEYDOWN:
                    handleKeyDown((Keys)par[0]);
                    base.Message(msg, par);
                    break;

                case ControlMessages.CONTROL_GOTFOCUS:
                    base.Message(msg, par);
                    lastCaretSwap = DateTime.Now;
                    showCaret = true;
                    break;

                default: base.Message(msg, par); break;
            }
        }

        private void handleCharacter(Char character)
        {
            if (char.IsControl(character))
                switch (character)
                {
                    case '\b':
                        if (caretIndex > 0)
                        {
                            caretIndex--;
                            text = text.Substring(0, caretIndex) + text.Substring(caretIndex + 1);
                            OnTextChanged(EventArgs.Empty);
                        }
                        break;
                }
            else
            {
                text = (text.Substring(0, caretIndex) + character + text.Substring(caretIndex));
                caretIndex++;
                OnTextChanged(EventArgs.Empty);
            }
        }
        private void handleKeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.PageDown: this.IconType = this.IconType == IconTypes.None ? IconTypes.Search : IconTypes.None; break;
                case Keys.Left: if (caretIndex > 0) caretIndex--; break;
                case Keys.Right: if (caretIndex < text.Length) caretIndex++; break;

                case Keys.Home: caretIndex = 0; break;
                case Keys.End: caretIndex = text.Length; break;

                case Keys.Delete:
                    if (caretIndex < text.Length)
                    {
                        text = text.Substring(0, caretIndex) + text.Substring(caretIndex + 1);
                        OnTextChanged(EventArgs.Empty);
                    }
                    break;
            }
        }

        protected sealed override void LoadLocalContent(ContentManager content)
        {
            backgroundBox = content.Load<Texture2D>("EmptyBox");
            caret = content.Load<Texture2D>("Caret");
            iconbackground = content.Load<Texture2D>("TextBoxIconBack");
            iconSEARCH = content.Load<Texture2D>("TextBoxIconSearch");

            box = MoonifyBoxes.EmptyBox;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            box.Draw(spriteBatch, backgroundBox, this.Location, this.Size, Color.White);

            System.DateTime dt = System.DateTime.Now;
            if ((dt - lastCaretSwap).TotalMilliseconds >= delayCaret)
            {
                showCaret = !showCaret;
                lastCaretSwap = dt;
            }
            if (showCaret && this.Focused)
            {
                int wid = font.TextWidth(text.Substring(0, caretIndex)) ?? 0;
                spriteBatch.Draw(caret, this.Location + new Vector2(9 + wid, 6), Color.White);
            }

            if (this.text.Length >= backgroundTextOffset && this.backgroundText.Length > 0)
            {
                int wid = font.TextWidth(text.Substring(backgroundTextOffset)) ?? 0;
                font.DrawString(spriteBatch, this.backgroundText, this.Location + new Vector2(11 + wid, 9), new Color(172, 179, 191));
            }
            font.DrawString(spriteBatch, text, this.Location + new Vector2(11, 10), Color.Black * .3f);
            font.DrawString(spriteBatch, text, this.Location + new Vector2(11, 9), Color.White);

            Texture2D iconTexture;
            switch (iconType)
            {
                case IconTypes.None: iconTexture = null; break;
                case IconTypes.UseProperty: iconTexture = icon; break;
                case IconTypes.Search: iconTexture = iconSEARCH; break;
                default:
                    throw new NotImplementedException();
            }

            if (iconTexture != null)
            {
                spriteBatch.Draw(iconbackground, this.Location + new Vector2(this.Size.X - 34, 0), Color.White);
                float xScale = 1, yScale = 1;
                if (iconTexture.Width > 33) xScale = iconTexture.Width / 33f;
                if (iconTexture.Height > 30) yScale = iconTexture.Height / 30f;
                if (xScale < yScale) yScale = xScale; else xScale = yScale;

                Vector2 pos = (new Vector2(33, 30) - new Vector2(iconTexture.Width, iconTexture.Height)) / 2f
                    + new Vector2(iconTexture.Width, iconTexture.Height) * ((1 - xScale) / 2f);

                spriteBatch.Draw(iconTexture, this.Location + new Vector2(this.Size.X - 33, 0) + pos, Color.White);
            }

            spriteBatch.End();
        }

        protected sealed override void InnerSizeChange(float width, float height)
        {
            if (width < 30)
                width = 30;
            if (height < 30)
                height = 30;

            if (height != 30)
                height = 30;

            base.InnerSizeChange(width, height);
        }

        /// <summary>
        /// Defines the different types of icons that can be applied to a <see cref="TextBox"/>.
        /// </summary>
        public enum IconTypes
        {
            /// <summary>
            /// No icon.
            /// </summary>
            None,
            /// <summary>
            /// Use the <see cref="TextBox.Icon"/> property.
            /// </summary>
            UseProperty,
            /// <summary>
            /// Use a magnifying glass icon.
            /// </summary>
            Search
        }
    }
}
