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
            this.font = new CharacterRenderer("HelveticaNeueLT Com 65 Md", 9f,
                System.Drawing.FontStyle.Regular, System.Drawing.Text.TextRenderingHint.AntiAlias);

            this.updateSliderPosition();
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

        private int mouseDownIndex = -1;
        private int mouseDownY = -1;
        protected override void Message(ControlMessages msg, params int[] par)
        {
            switch (msg)
            {
                case ControlMessages.CONTROL_SIZECHANGED:
                    updateSliderPosition();
                    base.Message(msg, par);
                    break;

                case ControlMessages.MOUSE_WHEEL:
                    contentOffset -= (par[3] / 120) * 25;
                    base.Message(msg, par);
                    break;

                case ControlMessages.MOUSE_MOVE:
                    {
                        int xG = par[0], yG = par[1];
                        int xL = xG - (int)this.X, yL = yG - (int)this.Y;
                        if (mouseDownY >= 0 && yL >= 22 && yL <= this.Height - 22)
                        {
                            int diff = yG - mouseDownY;
                            mouseDownY = yG;
                            sliderOffset += diff;
                        }
                        else
                            base.Message(msg, par);
                    }
                    break;

                case ControlMessages.MOUSE_LEAVE:
                    mouseDownIndex = -1;
                    mouseDownY = -1;
                    base.Message(msg, par);
                    break;

                case ControlMessages.MOUSE_UP:
                    {
                        int xG = par[0], yG = par[1];
                        int xL = xG - (int)this.X, yL = yG - (int)this.Y;

                        if (xL < this.Width - 16)
                        {
                            if (par[2] == 1)
                            {
                                int index = this.IndexFromPoint(xG, yG);
                                if (index >= 0 && index == mouseDownIndex)
                                    this.SelectedIndex = index;
                            }
                            base.Message(msg, par);
                        }

                        mouseDownIndex = -1;
                        mouseDownY = -1;
                    }
                    break;
                case ControlMessages.MOUSE_DOWN:
                    {
                        int xG = par[0], yG = par[1];
                        int xL = xG - (int)this.X, yL = yG - (int)this.Y;
                        mouseDownIndex = -1;
                        mouseDownY = -1;

                        if (xL < this.Width - 16)
                        {
                            if (par[2] == 1)
                            {
                                int index = this.IndexFromPoint(xG, yG);
                                if (index >= 0)
                                    mouseDownIndex = index;
                            }
                            base.Message(msg, par);
                        }
                        else if (yL < 22)
                            contentOffset += 25;
                        else if (yL > this.Height - 22)
                            contentOffset -= 25;
                        else
                            mouseDownY = par[1];
                    }
                    break;

                case ControlMessages.KEYBOARD_KEYDOWN:
                    if (par[0] == 38) contentOffset += 25;
                    if (par[0] == 40) contentOffset -= 25;
                    base.Message(msg, par);
                    break;

                default:
                    base.Message(msg, par);
                    break;
            }
        }

        private Vector2 barBarPosition;
        private Vector2 barBarSize;
        private Vector2 barSliderPosition;
        private Vector2 barSliderSize;

        private float _sliderOffset = 0;
        private float sliderOffset
        {
            get { return _sliderOffset; }
            set
            {
                _sliderOffset = value;

                if (items == null)
                    return;

                this.barBarPosition = new Vector2(this.Width - 16, 0);
                this.barBarSize = new Vector2(16, this.Height);
                this.barSliderPosition = new Vector2(this.Width - 15, 22);
                this.barSliderSize = new Vector2(14, this.Height - 44);

                float allItemsSize = 25f * items.Count;
                float shownItemsSize = this.Height - 2; //This IS accurate

                float partShown = shownItemsSize > allItemsSize ? 1 : shownItemsSize / allItemsSize;
                float sliderHeight = barSliderSize.Y * partShown;

                float sliderTopPosition = barSliderPosition.Y;
                float sliderBotPosition = sliderTopPosition + (barSliderSize.Y - sliderHeight);

                if (_sliderOffset < sliderTopPosition) _sliderOffset = sliderTopPosition;
                if (_sliderOffset > sliderBotPosition) _sliderOffset = sliderBotPosition;

                float from = barSliderSize.Y - sliderHeight;
                float to = allItemsSize - shownItemsSize;

                _contentOffset = from == 0 ? 0 : -(to / from) * (_sliderOffset - sliderTopPosition);

                barSliderSize.Y = sliderHeight;
                barSliderPosition.Y = _sliderOffset;
            }
        }
        private float _contentOffset = 0;
        private float contentOffset
        {
            get { return _contentOffset; }
            set
            {
                _contentOffset = value;
                updateSliderPosition();
            }
        }

        private void updateSliderPosition()
        {
            if (items == null)
                return;

            this.barBarPosition = new Vector2(this.Width - 16, 0);
            this.barBarSize = new Vector2(16, this.Height);
            this.barSliderPosition = new Vector2(this.Width - 15, 22);
            this.barSliderSize = new Vector2(14, this.Height - 44);

            float allItemsSize = 25f * items.Count;
            float shownItemsSize = this.Height - 2; //This IS accurate

            float partShown = shownItemsSize > allItemsSize ? 1 : shownItemsSize / allItemsSize;
            float sliderHeight = barSliderSize.Y * partShown;

            float contentTopPosition = 0;
            float contentBotPosition = -(allItemsSize - shownItemsSize);

            if (_contentOffset < contentBotPosition) _contentOffset = contentBotPosition;
            if (_contentOffset > contentTopPosition) _contentOffset = contentTopPosition;

            float from = allItemsSize - shownItemsSize;
            float to = barSliderSize.Y - sliderHeight;

            _sliderOffset = from == 0 ? 0 : -(to / from) * (_contentOffset - contentTopPosition) + barSliderPosition.Y;

            barSliderSize.Y = sliderHeight;
            barSliderPosition.Y = _sliderOffset;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            Rectangle fillClip = new Rectangle((int)this.Location.X + 1, (int)this.Location.Y + 1, (int)this.Size.X - 2 - 7, (int)this.Size.Y - 2);

            Matrix offsetMatrix = Matrix.CreateTranslation(0, _contentOffset, 0);

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

            scrollBarBox.Draw(spriteBatch, scrollBarTexture, this.Location + barBarPosition, barBarSize, Color.White);
            scrollSliderBox.Draw(spriteBatch, scrollSliderTexture, this.Location + barSliderPosition, barSliderSize, Color.White);
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
            int index = (int)(point.Y - _contentOffset) / 25;
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

        #region Collection List

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
                set { toString = (value ?? itemToString); printedValue.Clear(); }
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

                owner.updateSliderPosition();
            }

            public void RemoveAt(int index)
            {
                if (index < 0 || index >= list.Count)
                    throw new ArgumentOutOfRangeException("index");

                T item = list[index];

                list.RemoveAt(index);
                if (!list.Contains(item))
                    printedValue.Remove(item);

                if (index == owner.selectionIndex)
                    owner.selectionIndex = -1;
                else if (index < owner.selectionIndex)
                    owner.selectionIndex = owner.selectionIndex - 1;

                owner.updateSliderPosition();
            }

            public T this[int index]
            {
                get { return list[index]; }
                set
                {
                    if (index == list.Count)
                        Add(value);
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
                Insert(list.Count, item);
            }

            public void Clear()
            {
                owner.selectionIndex = -1;
                this.list.Clear();
                this.printedValue.Clear();

                owner.updateSliderPosition();
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

                RemoveAt(index);

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

        #endregion
    }
}
