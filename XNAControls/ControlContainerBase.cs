using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public class ControlContainerBase : Control
    {
        private ControlCollection controls;
        private ContentManagers content;

        public ControlContainerBase(float initialwidth, float initialheight)
            : base(initialwidth, initialheight)
        {
            this.controls = new ControlCollection(this);
            this.content = null;
        }

        public ControlCollection Controls
        {
            get { return controls; }
        }

        protected virtual void LoadSharedContent(ContentManagers content)
        {
        }
        protected virtual void UnloadSharedContent(ContentManagers content)
        {
        }

        protected internal sealed override void LoadContent(ContentManagers content)
        {
            this.content = content;

            LoadSharedContent(content);
            for (int i = 0; i < controls.Count; i++)
                controls[i].LoadContent(content);
        }
        protected internal sealed override void UnloadContent(ContentManagers content)
        {
            if (content != this.content)
                throw new InvalidOperationException("Trying to unload content using different ContentManager.");

            for (int i = 0; i < controls.Count; i++)
                controls[i].UnloadContent(content);
            UnloadSharedContent(content);

            this.content = null;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            for (int i = 0; i < controls.Count; i++)
                controls[i].Draw(spriteBatch, gameTime);
        }
        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < controls.Count; i++)
                controls[i].Update(gameTime);
        }

        public class ControlCollection : IEnumerable<Control>
        {
            private ControlContainerBase container;
            private List<Control> list;

            private bool LocalLoaded
            {
                get { return container == null ? false : container.content != null; }
            }
            private bool GameLoaded
            {
                get { return container == null ? false : container.gameContent != null; }
            }

            internal ControlCollection(ControlContainerBase container)
            {
                this.container = container;
                this.list = new List<Control>();
            }

            public Control this[int index]
            {
                get { return list[index]; }
            }

            public int Count
            {
                get { return list.Count; }
            }

            public void Add(Control control)
            {
                if (control.Parent != null)
                    control.Parent.controls.Remove(control);

                control.Parent = this.container;
                list.Add(control);

                if (GameLoaded)
                    control.LoadContent(container.gameContent);
                if (LocalLoaded)
                    control.LoadLocalContent(container.content);
            }
            public bool Remove(Control control)
            {
                if (control.Parent == this.container)
                {
                    if (GameLoaded)
                        control.UnloadContent(container.gameContent);
                    if (LocalLoaded)
                        control.UnloadLocalContent(container.content);

                    control.Parent = null;
                    list.Remove(control);

                    return true;
                }
                else
                    return false;
            }

            public bool Contains(Control control)
            {
                return list.Contains(control);
            }
            public bool Contains(Control control, bool recursive)
            {
                bool contains = Contains(control);
                if (contains || !recursive)
                    return contains;
                else
                    for (int i = 0; i < list.Count; i++)
                        if (list[i] is ControlContainerBase && (list[i] as ControlContainerBase).Controls.Contains(control, true))
                            return true;

                return false;
            }

            internal IEnumerable<Control> GetLeafs()
            {
                foreach (var control in list)
                    if (control is ControlContainerBase)
                        foreach (var c in (control as ControlContainerBase).controls.GetLeafs())
                            yield return c;
                    else
                        yield return control;
            }

            #region IEnumerable<Control> Members

            public IEnumerator<Control> GetEnumerator()
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
