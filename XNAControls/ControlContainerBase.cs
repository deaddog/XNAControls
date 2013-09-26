using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public class ControlContainerBase : Control
    {
        private bool contentLoaded;
        private ControlCollection controls;
        private ContentManager content;
        private ContentManager gameContent;

        public ControlContainerBase(float initialwidth, float initialheight)
            : base(initialwidth, initialheight)
        {
            this.contentLoaded = false;
            this.controls = new ControlCollection(this);
        }

        public ControlCollection Controls
        {
            get { return controls; }
        }

        protected internal sealed override void LoadLocalContent(ContentManager content)
        {
            this.content = content;
            //Handle local content load
            throw new NotImplementedException();
        }
        protected internal override void LoadContent(ContentManager content)
        {
            this.gameContent = content;
            //Handle content load
            throw new NotImplementedException();
        }
        protected internal override void UnloadLocalContent(ContentManager content)
        {
            if (content != this.content)
                throw new InvalidOperationException("Trying to unload content using different ContentManager.");

            //Handle local content unload
            this.content = null;
            throw new NotImplementedException();
        }
        protected internal override void UnloadContent(ContentManager content)
        {
            if (content != this.gameContent)
                throw new InvalidOperationException("Trying to unload content using different ContentManager.");

            //Handle content unload
            this.gameContent = null;
            throw new NotImplementedException();
        }

        public class ControlCollection : IEnumerable<Control>
        {
            private ControlContainerBase container;
            private List<Control> list;

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
                list.Add(control);
                if (container.contentLoaded)
                    //Load control resources
                throw new NotImplementedException();
            }
            public bool Remove(Control control)
            {
                if (list.Contains(control))
                {
                    if (container.contentLoaded)
                        //Unload control resources
                    throw new NotImplementedException();
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
