using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Color = System.Drawing.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using System.ComponentModel;

namespace XNAControls.Forms
{
    /// <summary>
    /// Allows the usage of a control as an XNA canvas.
    /// Derived classes can override the Update and Draw methods to add content.
    /// </summary>
    public abstract class GraphicsDeviceControl : System.Windows.Forms.Control
    {
        private ContentManager content;
        private SpriteBatch spritebatch;

        private GraphicsDeviceService graphicsDeviceService;
        private ServiceContainer services;

        private DateTime start = DateTime.Now;
        private DateTime last = DateTime.Now;

        /// <summary>
        /// Gets a <see cref="GraphicsDevice"/> that can be used to draw onto this <see cref="GraphicsDeviceControl"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDeviceService.GraphicsDevice; }
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> associated with this <see cref="GraphicsDeviceControl"/>.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceProvider Services
        {
            get { return services; }
        }

        public GraphicsDeviceControl(string contentRoot, GraphicsProfile graphicsProfile = GraphicsProfile.HiDef)
            : base()
        {
            if (!DesignMode)
            {
                services = new ServiceContainer();

                graphicsDeviceService = GraphicsDeviceService.AddRef(Handle, ClientSize.Width, ClientSize.Height, graphicsProfile);
                services.AddService<IGraphicsDeviceService>(graphicsDeviceService);

                content = new ContentManager(Services, contentRoot);
                spritebatch = new SpriteBatch(GraphicsDevice);

                Application.Idle += delegate { this.Invalidate(); };

                Initialize();
            }
        }

        /// <summary>
        /// Disposes the control.
        /// </summary>
        protected sealed override void Dispose(bool disposing)
        {
            if (disposing)
                content.Unload();

            if (graphicsDeviceService != null)
            {
                graphicsDeviceService.Release(disposing);
                graphicsDeviceService = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Redraws the control in response to a WinForms paint message.
        /// </summary>
        protected sealed override void OnPaint(PaintEventArgs e)
        {
            DateTime current = DateTime.Now;
            Update(new GameTime(current - start, current - last));

            string beginDrawError = BeginDraw();

            if (string.IsNullOrEmpty(beginDrawError))
            {
                current = DateTime.Now;
                Draw(spritebatch, new GameTime(current - start, current - last));
                EndDraw();
                last = current;
            }
            else
            {
                // If BeginDraw failed, show an error message using System.Drawing.
                PaintUsingSystemDrawing(e.Graphics, beginDrawError);
            }
        }

        private string BeginDraw()
        {
            // If we have no graphics device, we must be running in the designer.
            if (graphicsDeviceService == null)
            {
                return Text + "\n\n" + GetType();
            }

            // Make sure the graphics device is big enough, and is not lost.
            string deviceResetError = HandleDeviceReset();

            if (!string.IsNullOrEmpty(deviceResetError))
            {
                return deviceResetError;
            }

            // Many GraphicsDeviceControl instances can be sharing the same
            // GraphicsDevice. The device backbuffer will be resized to fit the
            // largest of these controls. But what if we are currently drawing
            // a smaller control? To avoid unwanted stretching, we set the
            // viewport to only use the top left portion of the full backbuffer.
            Viewport viewport = new Viewport();

            viewport.X = 0;
            viewport.Y = 0;

            viewport.Width = ClientSize.Width;
            viewport.Height = ClientSize.Height;

            viewport.MinDepth = 0;
            viewport.MaxDepth = 1;

            GraphicsDevice.Viewport = viewport;

            return null;
        }
        private void EndDraw()
        {
            try
            {
                Rectangle sourceRectangle = new Rectangle(0, 0, ClientSize.Width,
                                                                ClientSize.Height);

                GraphicsDevice.Present(sourceRectangle, null, this.Handle);
            }
            catch
            {
                // Present might throw if the device became lost while we were
                // drawing. The lost device will be handled by the next BeginDraw,
                // so we just swallow the exception.
            }
        }

        private string HandleDeviceReset()
        {
            bool deviceNeedsReset = false;

            switch (GraphicsDevice.GraphicsDeviceStatus)
            {
                case GraphicsDeviceStatus.Lost:
                    // If the graphics device is lost, we cannot use it at all.
                    return "Graphics device lost";

                case GraphicsDeviceStatus.NotReset:
                    // If device is in the not-reset state, we should try to reset it.
                    deviceNeedsReset = true;
                    break;

                default:
                    // If the device state is ok, check whether it is big enough.
                    PresentationParameters pp = GraphicsDevice.PresentationParameters;

                    deviceNeedsReset = (ClientSize.Width > pp.BackBufferWidth) ||
                                       (ClientSize.Height > pp.BackBufferHeight);
                    break;
            }

            // Do we need to reset the device?
            if (deviceNeedsReset)
            {
                try
                {
                    graphicsDeviceService.ResetDevice(ClientSize.Width,
                                                      ClientSize.Height);
                }
                catch (Exception e)
                {
                    return "Graphics device reset failed\n\n" + e;
                }
            }

            return null;
        }

        /// <summary>
        /// Draws a message on the face of the control.
        /// Only called if drawing is not performed directly by XNA (on error or in the Visual Studio Designer).
        /// </summary>
        /// <param name="graphics">A <see cref="Graphics"/> item associated with the triggering Paint event.</param>
        /// <param name="text">The (error)message to display on the control.</param>
        protected virtual void PaintUsingSystemDrawing(Graphics graphics, string text)
        {
            graphics.Clear(Color.CornflowerBlue);

            using (Brush brush = new SolidBrush(Color.Black))
            {
                using (StringFormat format = new StringFormat())
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    graphics.DrawString(text, Font, brush, ClientRectangle, format);
                }
            }
        }

        /// <summary>
        /// All paint-background messages are ignored.
        /// </summary>
        /// <param name="pevent">The <see cref="PaintEventArgs"/> associated with the event.</param>
        protected sealed override void OnPaintBackground(PaintEventArgs pevent)
        {
        }


        /// <summary>
        /// When overridden in a derived class, initializes the <see cref="GraphicsDeviceControl"/>.
        /// </summary>
        protected virtual void Initialize() { }
        /// <summary>
        /// Called when the <see cref="GraphicsDeviceControl"/> needs to be drawn. Override this method with component-specific drawing code.
        /// </summary>
        /// <param name="spritebatch">A <see cref="SpriteBatch"/> enabled simple drawing of groups of sprites.</param>
        /// <param name="gameTime">Time elapsed since the last call to Draw.</param>
        protected virtual void Draw(SpriteBatch spritebatch, GameTime gameTime) { }
        /// <summary>
        /// Called when the <see cref="GraphicsDeviceControl"/> needs to be updated. Override this method with component-specific update code.
        /// </summary>
        /// <param name="gameTime">Time elapsed since the last call to Update.</param>
        protected virtual void Update(GameTime gameTime) { }
    }
}
