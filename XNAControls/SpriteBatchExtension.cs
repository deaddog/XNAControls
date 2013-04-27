using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace XNAControls
{
    public static class SpriteBatchExtension
    {
        public static bool Begin(this SpriteBatch spriteBatch, float x, float y, float width, float height)
        {
            return Begin(spriteBatch, (int)x, (int)y, (int)width, (int)height);
        }
        public static bool Begin(this SpriteBatch spriteBatch, Rectangle clippingRectangle)
        {
            return Begin(spriteBatch, clippingRectangle.X, clippingRectangle.Y, clippingRectangle.Width, clippingRectangle.Height);
        }
        public static bool Begin(this SpriteBatch spriteBatch, int x, int y, int width, int height)
        {
            Rectangle clippingRectangle;
            if (!acceptClipping(spriteBatch, x, y, width, height, out clippingRectangle))
                return false;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true });
            spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;

            return true;
        }

        public static bool Begin(this SpriteBatch spriteBatch, float x, float y, float width, float height, Matrix transformMatrix)
        {
            return Begin(spriteBatch, (int)x, (int)y, (int)width, (int)height, transformMatrix);
        }
        public static bool Begin(this SpriteBatch spriteBatch, Rectangle clippingRectangle, Matrix transformMatrix)
        {
            return Begin(spriteBatch, clippingRectangle.X, clippingRectangle.Y, clippingRectangle.Width, clippingRectangle.Height, transformMatrix);
        }
        public static bool Begin(this SpriteBatch spriteBatch, int x, int y, int width, int height, Matrix transformMatrix)
        {
            Rectangle clippingRectangle;
            if (!acceptClipping(spriteBatch, x, y, width, height, out clippingRectangle))
                return false;

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true }, null, transformMatrix);
            spriteBatch.GraphicsDevice.ScissorRectangle = clippingRectangle;

            return true;
        }

        private static bool acceptClipping(SpriteBatch spriteBatch, int x, int y, int width, int height, out Rectangle clippingRectangle)
        {
            int maxHeight = spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;
            int maxWidth = spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;

            if (x < 0) { width += x; x = 0; }
            if (y < 0) { height += y; y = 0; }
            if (x + width > maxWidth) width = maxWidth - x;
            if (y + height > maxHeight) height = maxHeight - y;

            if (width <= 0 || height <= 0)
            {
                clippingRectangle = Rectangle.Empty;
                return false;
            }

            clippingRectangle = new Rectangle(x, y, width, height);
            return true;
        }
    }
}
