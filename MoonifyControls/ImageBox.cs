using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAControls;

namespace MoonifyControls
{
    public class ImageBox : Control
    {
        private Texture2D texture = null;
        private Texture2D backgroundTexture;
        private Box backgroundBox;

        public ImageBox(float width, float height)
            : base(width, height)
        {
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public override void LoadResources(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("ImageBox");
            backgroundBox = MoonifyBoxes.ImageBox;
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            backgroundBox.Draw(spriteBatch, backgroundTexture, this.Location, this.Size, Color.White);

            if (texture != null)
            {
                Vector2 scale = this.Size - new Vector2(10, 10);
                scale /= new Vector2(texture.Width, texture.Height);
                spriteBatch.Draw(texture, this.Location + new Vector2(5, 5), null, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }
    }
}
