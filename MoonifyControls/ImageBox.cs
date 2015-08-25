using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAControls;

namespace MoonifyControls
{
    public class ImageBox : XNAControls.ImageBox
    {
        private Texture2D backgroundTexture;
        private Box backgroundBox;

        private LoadingIcon loadingIcon;

        public ImageBox(float width, float height)
            : base(width, height)
        {
        }

        protected override void LoadContent(ContentManagers content)
        {
            backgroundTexture = content.ContainerContent.Load<Texture2D>("ImageBox");
            backgroundBox = MoonifyBoxes.ImageBox;
            loadingIcon = new LoadingIcon(content.ContainerContent, LoadingIconTypes.Type1);
        }

        protected override void DrawImage(SpriteBatch spriteBatch, GameTime gameTime, Texture2D image, Vector2 position, Vector2 size, float alpha)
        {
            Vector2 scale = size / new Vector2(image.Width, image.Height);
            spriteBatch.Draw(image, position, null, Color.White * alpha, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        protected override void DrawBackground(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Vector2 size)
        {
            backgroundBox.Draw(spriteBatch, backgroundTexture, this.OffsetLocation, this.Size, Color.White);
        }
        protected override void DrawForeground(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Vector2 size)
        {
        }
        protected override void DrawLoading(SpriteBatch spriteBatch, GameTime gameTime, Vector2 position, Vector2 size, float progress)
        {
            loadingIcon.Draw(spriteBatch, gameTime, this.OffsetLocation + (this.Size / 2), new Vector2(60, 60), Color.White * progress);
        }

        protected override Vector4 ImageMargins => new Vector4(5);
    }
}
