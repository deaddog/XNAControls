﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XNAControls
{
    public abstract class ControlManagerBase : ControlContainerBase, IGameComponent, IDrawable, IUpdateable
    {
        private SpriteBatch spriteBatch;
        private GraphicsDevice graphicsDevice;

        private ContentManager containerContent;

        private Control keyboardControl = null;

        private int mouseOffsetX = 0;
        private int mouseOffsetY = 0;
        public void SetMouseOffset(int x, int y)
        {
            this.mouseOffsetX = x;
            this.mouseOffsetY = y;
        }

        private ControlManagerBase(string contentRoot, GraphicsDevice graphicsDevice, IServiceProvider services)
            : base(0, 0)
        {
            this.graphicsDevice = graphicsDevice;

            KeyboardInput.CharacterEntered += characterEntered;
            KeyboardInput.KeyDown += keyDown;
            KeyboardInput.KeyUp += keyUp;

            this.containerContent = new ContentManager(services, contentRoot);

            ContentManagers managers = new ContentManagers(
                containerContent,
                new ContentManager(services, "Content")
                );
            base.LoadContent(managers);
        }

        public ControlManagerBase(Game game, string contentRoot)
            : this(contentRoot, game.GraphicsDevice, game.Services)
        {
            KeyboardInput.Initialize(game.Window);
        }

        public ControlManagerBase(IntPtr controlHandle, IServiceProvider services, string contentRoot, GraphicsDevice graphicsDevice)
            : this(contentRoot, graphicsDevice, services)
        {
            KeyboardInput.Initialize(controlHandle);
        }

        private void characterEntered(object sender, CharacterEventArgs e)
        {
            if (keyboardControl != null)
                keyboardControl.Message(ControlMessages.KEYBOARD_CHARACTER, e.Character, e.Param);
        }
        private void keyDown(object sender, KeyEventArgs e)
        {
            if (keyboardControl != null)
                keyboardControl.Message(ControlMessages.KEYBOARD_KEYDOWN, (int)e.KeyCode, 0 + (e.Shift ? 1 : 0) + (e.Control ? 2 : 0));
        }
        private void keyUp(object sender, KeyEventArgs e)
        {
            if (keyboardControl != null)
                keyboardControl.Message(ControlMessages.KEYBOARD_KEYUP, (int)e.KeyCode, 0 + (e.Shift ? 1 : 0) + (e.Control ? 2 : 0));
        }

        public Control KeyboardControl
        {
            get { return this.keyboardControl; }
            set
            {
                if (value != null && !Controls.Contains(value, true))
                    throw new InvalidOperationException("Control \"" + value.GetType().Name + "\" is not contained by this " + this.GetType().Name);

                if (this.keyboardControl != value)
                {
                    if (this.keyboardControl != null)
                        this.keyboardControl.Message(ControlMessages.CONTROL_LOSTFOCUS);

                    this.keyboardControl = value;
                    if (this.keyboardControl != null)
                        this.keyboardControl.Message(ControlMessages.CONTROL_GOTFOCUS);
                }
            }
        }

        public virtual void Initialize()
        {
        }

        protected virtual void LoadManagerContent(ContentManagers content)
        {
        }
        protected virtual void UnloadManagerContent()
        {
        }

        protected sealed override void LoadSharedContent(ContentManagers content)
        {
            this.spriteBatch = new SpriteBatch(graphicsDevice);
            LoadManagerContent(content);
        }
        protected sealed override void UnloadSharedContent()
        {
            UnloadManagerContent();
            this.spriteBatch.Dispose();
            this.spriteBatch = null;
        }

        public void Draw(GameTime gameTime)
        {
            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Draw(spriteBatch, gameTime);
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
        public void Update(GameTime gameTime)
        {
            MouseState ms = Mouse.GetState();
            ms = new MouseState(ms.X + mouseOffsetX, ms.Y + mouseOffsetY, ms.ScrollWheelValue, ms.LeftButton, ms.MiddleButton, ms.RightButton, ms.XButton1, ms.XButton2);
            Vector2 point = new Vector2(ms.X, ms.Y);

            Control c = downControls[0] ?? downControls[1] ?? downControls[2] ??
                (from control in Controls.GetLeafs().Reverse()
                 where control.IsInside(point)
                 select control).FirstOrDefault();
            if (c != lastHoveredControl)
            {
                if (lastHoveredControl != null)
                    lastHoveredControl.Message(ControlMessages.MOUSE_LEAVE);
                if (c != null)
                    c.Message(ControlMessages.MOUSE_ENTER);
            }

            if (c != null)
            {
                if (ms.X != oldMouseState.X || ms.Y != oldMouseState.Y)
                    c.Message(ControlMessages.MOUSE_MOVE, ms.X, ms.Y, buttonState(ms), 0);

                if (ms.LeftButton != oldMouseState.LeftButton)
                    sendMouseMessages(0, ms.LeftButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(true, false, false), 0);

                if (ms.MiddleButton != oldMouseState.MiddleButton)
                    sendMouseMessages(1, ms.MiddleButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(false, true, false), 0);

                if (ms.RightButton != oldMouseState.RightButton)
                    sendMouseMessages(2, ms.RightButton == ButtonState.Pressed, c, ms.X, ms.Y, buttonState(false, false, true), 0);

                if (ms.ScrollWheelValue != oldMouseState.ScrollWheelValue)
                    c.Message(ControlMessages.MOUSE_WHEEL, ms.X, ms.Y, buttonState(false, false, false), oldMouseState.ScrollWheelValue - ms.ScrollWheelValue);
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

            for (int i = 0; i < Controls.Count; i++)
                Controls[i].Update(gameTime);

            oldMouseState = ms;
        }
        private void sendMouseMessages(int button, bool down, Control c, params int[] parameters)
        {
            if (down)
            {
                c.Message(ControlMessages.MOUSE_DOWN, parameters);
                downControls[button] = c;
            }
            else
            {
                c.Message(ControlMessages.MOUSE_UP, parameters);
                if (c == downControls[button])
                    c.Message(ControlMessages.MOUSE_CLICK, parameters);
                downControls[button] = null;
            }
        }

        #region IDrawable Members

        int IDrawable.DrawOrder
        {
            get { return 0; }
        }
        event EventHandler<EventArgs> IDrawable.DrawOrderChanged { add { } remove { } }

        bool IDrawable.Visible
        {
            get { return true; }
        }
        event EventHandler<EventArgs> IDrawable.VisibleChanged { add { } remove { } }

        #endregion

        #region IUpdateable Members

        public bool Enabled
        {
            get { return true; }
        }
        public event EventHandler<EventArgs> EnabledChanged;

        public int UpdateOrder
        {
            get { return 0; }
        }
        public event EventHandler<EventArgs> UpdateOrderChanged;

        #endregion
    }
}
