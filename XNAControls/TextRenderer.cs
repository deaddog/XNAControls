using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using XNA = Microsoft.Xna.Framework;

namespace XNAControls
{
    public abstract class TextRenderer
    {
        private const byte NONMULTIPLIEDCOLOR = 255;

        private Font font;
        private TextRenderingHint textRenderingHint;

        public TextRenderer(Font font, TextRenderingHint textRenderingHint)
        {
            this.font = font;
            this.textRenderingHint = textRenderingHint;
        }

        public abstract int? TextWidth(string text);
        public abstract void DrawString(XNA.Graphics.SpriteBatch spriteBatch, string text, XNA.Vector2 position, XNA.Color color);

        private static Bitmap staticBitmap = new Bitmap(1, 1);

        protected XNA.Graphics.Texture2D CreateTexture(XNA.Graphics.GraphicsDevice device, string text)
        {
#if XNA3
            return CreateTexture(device, false, this.font, text, this.textRenderingHint);
#else
            return CreateTexture(device, true, this.font, text, this.textRenderingHint);
#endif
        }

        public static XNA.Graphics.Texture2D CreateTexture(XNA.Graphics.GraphicsDevice device, bool premultiplyAlpha, Font font, string text, TextRenderingHint textRenderingHint = TextRenderingHint.ClearTypeGridFit)
        {
            SizeF sizeF;
            using (Graphics g = Graphics.FromImage(staticBitmap))
                sizeF = g.MeasureString(text, font);
            Size size = new Size((int)sizeF.Width, (int)sizeF.Height);

            XNA.Graphics.Texture2D texture;

            Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = textRenderingHint;
                g.DrawString(text, font, Brushes.White, new PointF());
            }

            int minX = int.MaxValue, maxX = int.MinValue;//, minY = int.MaxValue, maxY = int.MinValue;
            BitmapData bmd = bitmap.LockBits(new Rectangle(Point.Empty, size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int PixelSize = 4;
            unsafe
            {
                for (int y = 0; y < bmd.Height; y++)
                {
                    byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                    for (int x = 0; x < bmd.Width; x++)
                    {
                        byte a = premultiplyAlpha ? row[x * PixelSize + 3] : NONMULTIPLIEDCOLOR;
                        row[x * PixelSize] = a;//B
                        row[x * PixelSize + 1] = a;//G
                        row[x * PixelSize + 2] = a;//R
                        //row[x * PixelSize + 3] = 0;//A
                        if (a > 0)
                        {
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                        }
                    }
                }
            }
            bitmap.UnlockBits(bmd);

            if (minX != int.MaxValue && maxX != int.MinValue)
            {
                Bitmap bitmapTemp = bitmap;
                bitmap = new Bitmap(maxX - minX + 1, size.Height, PixelFormat.Format32bppArgb);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                    graphics.DrawImage(bitmapTemp, -minX, 0);
                bitmapTemp.Dispose();
            }

            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
#if XNA3
                    ms.Seek(0, SeekOrigin.Begin);
                    texture = XNA.Graphics.Texture2D.FromFile(device, ms);
#else
                texture = XNA.Graphics.Texture2D.FromStream(device, ms);
#endif
            }

            bitmap.Dispose();

            return texture;
        }
    }
}
