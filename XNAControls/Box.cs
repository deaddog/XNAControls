using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    /// <summary>
    /// Defines a method of splitting images into sub-regions, which in turn allows them to be drawn as scalable rectangles.
    /// A box is defined using integers (that define its regions) and is drawn using a single <see cref="Texture2D"/>.
    /// </summary>
    public struct Box
    {
        private int leftMargin, topMargin, rightMargin, bottomMargin;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box"/> class.
        /// </summary>
        /// <param name="leftMargin">The number of unscalable pixels from the leftmost edge of the <see cref="Texture2D"/> to the start of the actual box.</param>
        /// <param name="leftwidth">The number of unscalable pixels in the left side of the box.</param>
        /// <param name="midwidth">The number of scalable pixels (width) in the middle of the box.</param>
        /// <param name="rightwidth">The number of unscalable pixels in the right side of the box.</param>
        /// <param name="rightMargin">The number of unscalable pixels from the rightmost edge of the <see cref="Texture2D"/> to the end of the actual box.</param>
        /// <param name="topMargin">The number of unscalable pixels from the topmost edge of the <see cref="Texture2D"/> to the start of the actual box.</param>
        /// <param name="topheight">The number of unscalable pixels in the top of the box.</param>
        /// <param name="midheight">The number of scalable pixels (height) in the middle of the box.</param>
        /// <param name="bottomheight">The number of unscalable pixels in the bottom of the box.</param>
        /// <param name="bottomMargin">The number of unscalable pixels from the bottommost edge of the <see cref="Texture2D"/> to the end of the actual box.</param>
        public Box(
            int leftMargin, int leftwidth, int midwidth, int rightwidth, int rightMargin,
            int topMargin, int topheight, int midheight, int bottomheight, int bottomMargin)
        {
            this.leftMargin = leftMargin; this.rightMargin = rightMargin;
            this.topMargin = topMargin; this.bottomMargin = bottomMargin;

            leftwidth += leftMargin;
            topheight += topMargin;
            rightwidth += rightMargin;
            bottomheight += bottomMargin;

            this.topleftrect = new Rectangle(0, 0, leftwidth, topheight);
            this.topcenterrect = new Rectangle(leftwidth, 0, midwidth, topheight);
            this.toprightrect = new Rectangle(leftwidth + midwidth, 0, rightwidth, topheight);

            this.midleftrect = new Rectangle(0, topheight, leftwidth, midheight);
            this.midcenterrect = new Rectangle(leftwidth, topheight, midwidth, midheight);
            this.midrightrect = new Rectangle(leftwidth + midwidth, topheight, rightwidth, midheight);

            this.botleftrect = new Rectangle(0, topheight + midheight, leftwidth, bottomheight);
            this.botmidrect = new Rectangle(leftwidth, topheight + midheight, midwidth, bottomheight);
            this.botrightrect = new Rectangle(leftwidth + midwidth, topheight + midheight, rightwidth, bottomheight);
        }

        private Rectangle topleftrect;
        private Rectangle topcenterrect;
        private Rectangle toprightrect;

        private Rectangle midleftrect;
        private Rectangle midcenterrect;
        private Rectangle midrightrect;

        private Rectangle botleftrect;
        private Rectangle botmidrect;
        private Rectangle botrightrect;

        /// <summary>
        /// Draws a <see cref="Texture2D"/> using the sizes defined by this the <see cref="Box"/>.
        /// </summary>
        /// <param name="spriteBatch">The <see cref="SpriteBatch"/> used for drawing the <see cref="Texture2D"/>.</param>
        /// <param name="texture">The <see cref="Texture2D"/> that should be drawn.</param>
        /// <param name="position">The position at which the <see cref="Box"/> should be drawn. This specifies the upper left point of the <see cref="Box"/>, the texture.</param>
        /// <param name="size">The size of the drawn <see cref="Box"/>, not the size of the drawn texture.</param>
        /// <param name="color">The color used when drawing the <see cref="Box"/>.</param>
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Vector2 size, Color color)
        {
            float leftx = -leftMargin;
            float centerx = topleftrect.Width - leftMargin;
            float rightx = size.X - (float)toprightrect.Width + (float)rightMargin;
            float topy = -topMargin;
            float midy = topleftrect.Height - topMargin;
            float boty = size.Y - (float)botleftrect.Height + (float)bottomMargin;

            float horizontalMargin = topleftrect.Width + toprightrect.Width - leftMargin - rightMargin;
            float verticalMargin = topleftrect.Height + botleftrect.Height - topMargin - bottomMargin;
            float horizontalScale = (size.X - horizontalMargin) / midcenterrect.Width;
            float verticalScale = (size.Y - verticalMargin) / midcenterrect.Height;

            spriteBatch.Draw(texture, position + new Vector2(leftx, topy), topleftrect, color);
            spriteBatch.Draw(texture, position + new Vector2(centerx, topy), topcenterrect, color, 0f, Vector2.Zero, new Vector2(horizontalScale, 1), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position + new Vector2(rightx, topy), toprightrect, color);

            spriteBatch.Draw(texture, position + new Vector2(leftx, midy), midleftrect, color, 0, Vector2.Zero, new Vector2(1, verticalScale), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position + new Vector2(centerx, midy), midcenterrect, color, 0, Vector2.Zero, new Vector2(horizontalScale, verticalScale), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position + new Vector2(rightx, midy), midrightrect, color, 0, Vector2.Zero, new Vector2(1, verticalScale), SpriteEffects.None, 0);

            spriteBatch.Draw(texture, position + new Vector2(leftx, boty), botleftrect, color);
            spriteBatch.Draw(texture, position + new Vector2(centerx, boty), botmidrect, color, 0, Vector2.Zero, new Vector2(horizontalScale, 1), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, position + new Vector2(rightx, boty), botrightrect, color);
        }
    }
}
