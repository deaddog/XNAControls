using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAControls;

namespace MoonifyControls
{
    public class ImageBox : Control
    {
        private List<AlphaImage> oldTextures;
        private AlphaImage texture;

        private Texture2D backgroundTexture;
        private Box backgroundBox;

        public ImageBox(float width, float height)
            : base(width, height)
        {
            this.oldTextures = new List<AlphaImage>();
            this.texture = null;
        }

        public Texture2D Texture
        {
            get { return texture == null ? null : texture.Texture; }
            set
            {
                if (texture != null)
                {
                    oldTextures.Add(texture);
                    texture.Hide();
                }
                if (value != null)
                {
                    texture = new AlphaImage(this, value, 0);
                    texture.Show();
                }
            }
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
                texture.Draw(spriteBatch, this.Location, this.Size, Color.White);

            for (int i = 0; i < oldTextures.Count; i++)
                oldTextures[i].Draw(spriteBatch, this.Location, this.Size, Color.White);

            spriteBatch.End();
        }

        private class AlphaImage
        {
            private ImageBox owner;

            private static DeadDog.GUI.IMoveMethods method = new DeadDog.GUI.MoveSineLine(1, 1);
            private xfloat alpha;
            private Texture2D texture;

            public AlphaImage(ImageBox owner, Texture2D texture, float opacity)
            {
                this.owner = owner;

                this.texture = texture;
                this.alpha = new xfloat(opacity, method);

                alpha.Elapsed += (s, e) => { if (alpha == 0) owner.oldTextures.Remove(this); };
            }

            public void Show()
            {
                alpha.TargetValue = 1;
            }
            public void Hide()
            {
                alpha.TargetValue = 0;
            }

            public Texture2D Texture
            {
                get { return texture; }
            }

            public void Draw(SpriteBatch spritebatch, Vector2 location, Vector2 size, Color color)
            {
                Vector2 scale = size - new Vector2(10, 10);
                scale /= new Vector2(texture.Width, texture.Height);

                alpha.Update();
                color *= alpha.CurrentValue;

                spritebatch.Draw(texture, location + new Vector2(5, 5), null, color, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
        }
    }
}
