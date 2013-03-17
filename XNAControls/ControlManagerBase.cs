﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
            get { return this.keyboardControl; }
            set
            {
                if (value != null && !controls.Contains(value))
                    throw new InvalidOperationException("Control \"" + value.GetType().Name + "\" is not contained by this " + this.GetType().Name);

                if (this.keyboardControl != value)
                {
                    if (this.keyboardControl != null)
                        this.keyboardControl.Message(Control.CONTROL_LOSTFOCUS);

                    this.keyboardControl = value;
                    if (this.keyboardControl != null)
                        this.keyboardControl.Message(Control.CONTROL_GOTFOCUS);
                }
            }
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

        private int buttonState(MouseState ms)
        {
            return
                (ms.LeftButton == ButtonState.Pressed ? 1 : 0) +
                (ms.MiddleButton == ButtonState.Pressed ? 2 : 0) +
                (ms.RightButton == ButtonState.Pressed ? 4 : 0);
        }
        private int buttonState(bool left, bool middle, bool right)
        {
            return
                (left ? 1 : 0) +
                (middle ? 2 : 0) +
                (right ? 4 : 0);
        }

        private Control lastHoveredControl = null;
        private Control[] downControls = new Control[3];
        private MouseState oldMouseState;
        public override void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            Vector2 point = new Vector2(ms.X, ms.Y);

            Control c = (from control in controls.Reverse() where control.IsInside(point) select control).FirstOrDefault();
            if (c != lastHoveredControl)
            {
                if (lastHoveredControl != null)
                    lastHoveredControl.Message(Control.MOUSE_LEAVE);
                if (c != null)
                    c.Message(Control.MOUSE_ENTER);
            }

            if (c != null)
            {
                if (ms.X != oldMouseState.X || ms.Y != oldMouseState.Y)
                    c.Message(Control.MOUSE_MOVE, ms.X, ms.Y, buttonState(ms), 0);

                if (ms.LeftButton != oldMouseState.LeftButton)
                    sendMouseMessages(0, ms.LeftButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(true, false, false), 0);

                if (ms.MiddleButton != oldMouseState.MiddleButton)
                    sendMouseMessages(1, ms.MiddleButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(false, true, false), 0);

                if (ms.RightButton != oldMouseState.RightButton)
                    sendMouseMessages(2, ms.RightButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(false, false, true), 0);

                if (ms.ScrollWheelValue != oldMouseState.ScrollWheelValue)
                    c.Message(Control.MOUSE_WHEEL, ms.X, ms.Y, buttonState(false, false, false), oldMouseState.ScrollWheelValue - ms.ScrollWheelValue);
            }
            else
            {
                if (ms.LeftButton != oldMouseState.LeftButton)
                    downControls[0] = null;

                if (ms.MiddleButton != oldMouseState.MiddleButton)
                    downControls[1] = null;

                if (ms.RightButton != oldMouseState.RightButton)
                    downControls[2] = null;
            }

            lastHoveredControl = c;

            for (int i = 0; i < controls.Count; i++)
                controls[i].Update(gameTime);

            oldMouseState = ms;
        }
        private void sendMouseMessages(int button, bool down, Control c, params int[] parameters)
        {
            if (down)
            {
                c.Message(Control.MOUSE_DOWN, parameters);
                downControls[button] = c;
            }
            else
            {
                c.Message(Control.MOUSE_UP, parameters);
                if (c == downControls[button])
                    c.Message(Control.MOUSE_CLICK, parameters);
                downControls[button] = null;
            }
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
