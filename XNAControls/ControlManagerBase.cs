using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public abstract class ControlManagerBase : DrawableGameComponent
    {
        private ControlCollection controls;

        private ContentManager content;
        private SpriteBatch spriteBatch;
        private Texture2D background;

        private Control keyboardControl = null;

        public ControlManagerBase(Game game, string contentRoot)
            : base(game)
        {
            content = new ContentManager(game.Services, contentRoot);
            this.controls = new ControlCollection(this);

            KeyboardInput.Initialize(game.Window);
            KeyboardInput.CharacterEntered += (s, e) =>
            {
                keyboardControl.Message(Control.KEYBOARD_CHARACTER, e.Character, e.Param);
            };
            KeyboardInput.KeyDown += (s, e) =>
            {
                keyboardControl.Message(Control.KEYBOARD_KEYDOWN, (int)e.KeyCode, 0 + (e.Shift ? 1 : 0) + (e.Control ? 2 : 0));
            };
            KeyboardInput.KeyUp += (s, e) =>
            {
                keyboardControl.Message(Control.KEYBOARD_KEYUP, (int)e.KeyCode, 0 + (e.Shift ? 1 : 0) + (e.Control ? 2 : 0));
            };
        }

        public ControlCollection Controls
        {
            get { return controls; }
        }

        public Control KeyboardControl
        {
            get { return keyboardControl; }
            set { keyboardControl = value; }
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            this.background = content.Load<Texture2D>("Background");

            for (int i = 0; i < controls.Count; i++)
                controls[i].LoadResources(content);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, Vector2.Zero, Color.White);
            spriteBatch.End();

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
            private ControlManagerBase manager;
            private List<Control> list;

            internal ControlCollection(ControlManagerBase manager)
            {
                this.manager = manager;
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
