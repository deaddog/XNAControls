using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using XNAControls;

namespace MoonifyControls
{
    public class ListBox<T> : Control
    {
        private static readonly Color activeColor = Color.White;
        private static readonly Color inactiveColor = new Color(172, 179, 191);

        private Box frameBox;
        private Box fillBox;
        private Box selectionBox;
        private Texture2D frameTexture;
        private Texture2D fillTexture;
        private Texture2D selectionTexture;

        private CharacterRenderer font;

        #region Index management

        private int _selectionIndex = -1;
        private int selectionIndex
        {
            get { return _selectionIndex; }
            set
            {
                if (value < 0)
                    value = -1;
                else if (value >= items.Count)
                    throw new ArgumentOutOfRangeException("value");

                if (_selectionIndex != value)
                {
                    _selectionIndex = value;
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }
        public int SelectedIndex
        {
            get { return selectionIndex; }
            set { selectionIndex = value; }
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if (SelectedIndexChanged != null)
                SelectedIndexChanged(this, e);
        }
        public event EventHandler SelectedIndexChanged;

        #endregion

        private ObjectCollection items;

        public ListBox()
            : base(200, 200)
        {
            this.items = new ObjectCollection(this);

            font = new CharacterRenderer("HelveticaNeueLT Com 65 Md", 9f, System.Drawing.FontStyle.Regular, System.Drawing.Text.TextRenderingHint.AntiAlias);
        }
        public ListBox(IEnumerable<T> collection)
            : this()
        {
        }
        public ListBox(params T[] collection)
            : this(collection as IEnumerable<T>)
        {
        }

        protected override void _KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageDown)
                selectionIndex++;
            else if (e.KeyCode == Keys.PageUp)
                selectionIndex--;
            base._KeyDown(e);
        }

        public override void LoadResources(ContentManager content)
        {
            frameBox = MoonifyBoxes.EmptyBoxFrame;
            frameTexture = content.Load<Texture2D>("EmptyBoxFrame");
            fillBox = MoonifyBoxes.EmptyBoxFill;
            fillTexture = content.Load<Texture2D>("EmptyBoxFill");
            selectionBox = new Box(1, 0, 208, 0, 1, 0, 0, 25, 0, 2);
            selectionTexture = content.Load<Texture2D>("ListBoxHighlight");
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            fillBox.Draw(spriteBatch, fillTexture, this.Location, this.Size, Color.White);
            spriteBatch.End();

            Rectangle clip = new Rectangle((int)this.Location.X + 1, (int)this.Location.Y + 1, (int)this.Size.X - 2, (int)this.Size.Y - 2);

            RasterizerState temp = spriteBatch.GraphicsDevice.RasterizerState;
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.GraphicsDevice.ScissorRectangle = clip;
            selectionBox.Draw(spriteBatch, selectionTexture,
                this.Location + new Vector2(1, 1 + (float)selectionIndex * 25f), new Vector2(this.Size.X - 2, 25), Color.White);

            for (int i = 0; i < items.Count; i++)
                DrawLine(spriteBatch, items.GetText(i), i);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.RasterizerState = temp;

            spriteBatch.Begin();
            frameBox.Draw(spriteBatch, frameTexture, this.Location, this.Size, Color.White);
            spriteBatch.End();
        }

        private void DrawLine(SpriteBatch spriteBatch, string text, int index)
        {
            DrawLine(spriteBatch, text, index, index == selectionIndex);
        }
        private void DrawLine(SpriteBatch spriteBatch, string text, int index, bool active)
        {
            Vector2 pText = this.Location + new Vector2(12, 7 + index * 25);
            Vector2 pBlack = pText + new Vector2(0, 1);

            font.DrawString(spriteBatch, text, pBlack, Color.Black * .3f);
            font.DrawString(spriteBatch, text, pText, active ? activeColor : inactiveColor);
        }

        public ObjectCollection Items
        {
            get { return items; }
        }

        public int IndexFromPoint(int x, int y)
        {
            return IndexFromPoint((float)x, (float)y);
        }
        public int IndexFromPoint(float x, float y)
        {
            return IndexFromPoint(new Vector2(x, y));
        }
        public int IndexFromPoint(Vector2 point)
        {
            if (!IsInside(point))
                return -1;
            if ((this.Size.Y - 2) % 25 == 0 && point.Y - this.Location.Y + 1 == this.Size.Y)
                return -1;

            point -= (this.Location + Vector2.One);
            return (int)point.Y / 25;
        }

        protected sealed override void InnerSizeChange(float width, float height)
        {
            if (width < 30)
                width = 30;
            if (height < 27)
                height = 27;

            height = ((int)(height - 2) / 25) * 25 + 2;

            base.InnerSizeChange(width, height);
        }

        public class ObjectCollection : IList<T>
        {
            private ListBox<T> owner;
            private List<T> list;

            private Func<T, string> toString;
            private Dictionary<T, string> printedValue;

            internal ObjectCollection(ListBox<T> owner)
            {
                this.owner = owner;
                this.list = new List<T>();

                this.toString = itemToString;
                this.printedValue = new Dictionary<T, string>();
            }

            private string itemToString(T item)
            {
                return item.ToString();
            }
            public string GetText(T item)
            {
                if (!printedValue.ContainsKey(item))
                    printedValue.Add(item, toString(item));
                return printedValue[item];
            }
            public string GetText(int index)
            {
                return GetText(list[index]);
            }
            public Func<T, string> ItemToString
            {
                get { return toString == itemToString ? null : toString; }
                set { toString = (value ?? toString); printedValue.Clear(); }
            }
            public void Refresh(int index)
            {
                printedValue.Remove(list[index]);
            }

            #region IList<T> Members

            public int IndexOf(T item)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, T item)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            public T this[int index]
            {
                get { return list[index]; }
                set
                {
                    throw new NotImplementedException();
                }
            }

            #endregion

            #region ICollection<T> Members

            public void Add(T item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                owner.selectionIndex = -1;
                this.list.Clear();
            }

            public bool Contains(T item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { return list.Count; }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(T item)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
