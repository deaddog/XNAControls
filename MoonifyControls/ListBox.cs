using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DeadDog.GUI;
using XNAControls;

namespace MoonifyControls
{
    public class ListBox<T> : Control
    {
        private static readonly Color activeColor = Color.White;
        private static readonly Color inactiveColor = new Color(172, 179, 191);

        private Box frameBox;
        private Texture2D frameTexture;

        private Box fillBox;
        private Texture2D fillTexture;

        private Box selectionBox;
        private Texture2D selectionTexture;

        private Box scrollBarBox;
        private Texture2D scrollBarTexture;

        private Box scrollSliderBox;
        private Texture2D scrollSliderTexture;

        private CharacterRenderer font;

        #region Index management

        private xfloat sliderPosition = new xfloat(1 - 25, new MoveSineLine(0.5f, 3000f));
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
                    int diff = Math.Abs(value - _selectionIndex);
                    float newPosition = 1 + value * 25;
                    if (diff > 1 || value < 0 || _selectionIndex < 0)
                        sliderPosition.CurrentValue = newPosition;
                    else
                        sliderPosition.TargetValue = newPosition;
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
            : base(200, 120)
        {
            this.items = new ObjectCollection(this);

            font = new CharacterRenderer("HelveticaNeueLT Com 65 Md", 9f, System.Drawing.FontStyle.Regular, System.Drawing.Text.TextRenderingHint.AntiAlias);
        }
        public ListBox(params T[] collection)
            : this(collection as IEnumerable<T>)
        {
        }
        public ListBox(IEnumerable<T> collection)
            : this()
        {
            foreach (T t in collection)
                this.items.Add(t);
        }

        public override void LoadResources(ContentManager content)
        {
            frameBox = MoonifyBoxes.EmptyBoxFrame;
            frameTexture = content.Load<Texture2D>("EmptyBoxFrame");

            fillBox = MoonifyBoxes.EmptyBoxFill;
            fillTexture = content.Load<Texture2D>("EmptyBoxFill");

            selectionBox = new Box(1, 0, 208, 0, 1, 0, 0, 25, 0, 2);
            selectionTexture = content.Load<Texture2D>("ListBoxHighlight");

            scrollBarBox = MoonifyBoxes.ScrollbarBar;
            scrollBarTexture = content.Load<Texture2D>("ScrollbarBar");

            scrollSliderBox = MoonifyBoxes.ScrollbarSlider;
            scrollSliderTexture = content.Load<Texture2D>("ScrollbarSlider");
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            y = e.Y;
        }
        float y = 0;
        int offset = 0;
        private float CalculateOffset(float sliderPosition)
        {
            Vector2 barSliderSize = new Vector2(14, this.Height - 44);

            float allItemsSize = 25f * items.Count;
            float shownItemsSize = this.Height - 2; //This IS accurate

            float sliderHeight = shownItemsSize > allItemsSize 
                ? barSliderSize.Y 
                : barSliderSize.Y * (shownItemsSize / allItemsSize);

            float sliderTopPosition = 22;
            float sliderBotPosition = sliderTopPosition + (barSliderSize.Y - sliderHeight);

            if (sliderPosition < sliderTopPosition) sliderPosition = sliderTopPosition;
            if (sliderPosition > sliderBotPosition) sliderPosition = sliderBotPosition;

            float from = barSliderSize.Y - sliderHeight;
            float to = allItemsSize - shownItemsSize;

            return from == 0 ? 0 : -(int)((to / from) * (sliderPosition - sliderTopPosition));
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle fillClip = new Rectangle((int)this.Location.X + 1, (int)this.Location.Y + 1, (int)this.Size.X - 2 - 7, (int)this.Size.Y - 2);

            Matrix offsetMatrix = Matrix.CreateTranslation(0, offset, 0);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.GraphicsDevice.ScissorRectangle = fillClip;
            fillBox.Draw(spriteBatch, fillTexture, this.Location, this.Size, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true }, null, offsetMatrix);

            if (selectionIndex >= 0)
            {
                sliderPosition.Update();
                selectionBox.Draw(spriteBatch, selectionTexture,
                    this.Location + new Vector2(1, sliderPosition), new Vector2(this.Size.X - 2, 25), Color.White);
            }

            for (int i = 0; i < items.Count; i++)
                DrawLine(spriteBatch, items.GetText(i), i);
            spriteBatch.End();

            Rectangle clip = new Rectangle((int)this.Location.X - 2, (int)this.Location.Y, (int)this.Size.X + 3 - 9, (int)this.Size.Y + 2);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.GraphicsDevice.ScissorRectangle = clip;
            frameBox.Draw(spriteBatch, frameTexture, this.Location, this.Size, Color.White);
            spriteBatch.End();

            spriteBatch.Begin();
            Vector2 barBarPosition = this.Location + new Vector2(this.Width - 16, 0);
            Vector2 barBarSize = new Vector2(16, this.Size.Y);
            Vector2 barSliderPosition = this.Location + new Vector2(this.Width - 15, 22);
            Vector2 barSliderSize = new Vector2(14, this.Height - 44);


            float allItemsSize = 25f * items.Count;
            float shownItemsSize = this.Height - 2; //This IS accurate

            float partShown = shownItemsSize > allItemsSize ? 1 : shownItemsSize / allItemsSize;
            float sliderHeight = barSliderSize.Y * partShown;

            float sliderTopPosition = barSliderPosition.Y;
            float sliderBotPosition = sliderTopPosition + (barSliderSize.Y - sliderHeight);

            if (y < sliderTopPosition) y = sliderTopPosition;
            if (y > sliderBotPosition) y = sliderBotPosition;

            float from = barSliderSize.Y - sliderHeight;
            float to = allItemsSize - shownItemsSize;

            offset = from == 0 ? 0 : -(int)((to / from) * (y - sliderTopPosition));

            barSliderSize.Y = sliderHeight;
            barSliderPosition.Y = y;

            scrollBarBox.Draw(spriteBatch, scrollBarTexture, barBarPosition, barBarSize, Color.White);
            scrollSliderBox.Draw(spriteBatch, scrollSliderTexture, barSliderPosition, barSliderSize, Color.White);
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
            int index = (int)(point.Y - offset) / 25;
            return index < items.Count ? index : -1;
        }

        protected sealed override void InnerSizeChange(float width, float height)
        {
            if (width < 30)
                width = 30;
            if (height < 27)
                height = 27;

            //height = ((int)(height - 2) / 25) * 25 + 2;

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

            /// <summary>
            /// Gets or sets the method used for converting objects of type <typeparamref name="T"/> into a string (their visual representation).
            /// If this is null, the objects ToString method is used.
            /// </summary>
            public Func<T, string> ItemToString
            {
                get { return toString == itemToString ? null : toString; }
                set { toString = (value ?? toString); printedValue.Clear(); }
            }

            public void Refresh(T item)
            {
                if (item == null)
                    throw new ArgumentNullException("item");
                printedValue.Remove(item);
            }
            public void Refresh(int index)
            {
                printedValue.Remove(list[index]);
            }

            #region IList<T> Members

            public int IndexOf(T item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, T item)
            {
                if (list.Contains(item))
                    throw new InvalidOperationException(this.GetType().Name + " cannot contain multiples of the same instance.");

                list.Insert(index, item);
                if (index <= owner.selectionIndex)
                    owner.selectionIndex = owner.selectionIndex + 1;
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= list.Count)
                    throw new ArgumentOutOfRangeException("index");

                T item = list[index];
                Remove(item);
            }

            public T this[int index]
            {
                get { return list[index]; }
                set
                {
                    if (index == list.Count)
                    {
                        Add(value);
                        return;
                    }
                    else if (index < 0 || index > list.Count)
                        throw new ArgumentOutOfRangeException("index");
                    else if (list.Contains(value))
                        throw new InvalidOperationException(this.GetType().Name + " cannot contain multiples of the same instance.");
                    else
                    {
                        printedValue.Remove(list[index]);
                        list[index] = value;

                        if (index == owner.selectionIndex)
                            owner.selectionIndex = -1;
                    }
                }
            }

            #endregion

            #region ICollection<T> Members

            public void Add(T item)
            {
                if (list.Contains(item))
                    throw new InvalidOperationException(this.GetType().Name + " cannot contain multiples of the same instance.");

                list.Add(item);
            }

            public void Clear()
            {
                owner.selectionIndex = -1;
                this.list.Clear();
                this.printedValue.Clear();
            }

            public bool Contains(T item)
            {
                return list.Contains(item);
            }

            public void CopyTo(T[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return list.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(T item)
            {
                int index = list.IndexOf(item);
                if (index == -1)
                    return false;

                list.Remove(item);
                printedValue.Remove(item);

                if (index == owner.selectionIndex)
                    owner.selectionIndex = -1;
                else if (index < owner.selectionIndex)
                    owner.selectionIndex = owner.selectionIndex - 1;

                return true;
            }

            #endregion

            #region IEnumerable<T> Members

            public IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }

            #endregion
        }
    }
}
