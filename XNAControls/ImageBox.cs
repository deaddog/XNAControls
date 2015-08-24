using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace XNAControls
{
    public abstract class ImageBox : Control
    {
        private static DeadDog.GUI.IMoveMethods showMethod = new DeadDog.GUI.MoveSineLine(1, 1);
        private static DeadDog.GUI.IMoveMethods hideMethod = new DeadDog.GUI.MoveSineLine(1, 5);

        private AlphaImage texture;
        private LinkedList<AlphaImage> textures;

        private Texture2D backgroundTexture;
        private Box backgroundBox;

        private DataLoader<Texture2D> pending;
        private LoadingIcon loadingIcon;
        private xfloat loadAlpha;

        public ImageBox(float width, float height)
            : base(width, height)
        {
            this.textures = new LinkedList<AlphaImage>();
            this.texture = null;

            loadAlpha = new xfloat(0, showMethod);
        }

        public Texture2D Texture
        {
            get { return texture == null ? null : texture.Texture; }
            set
            {
                if (value != null)
                {
                    texture = new AlphaImage(this, value, 0);
                    textures.AddLast(texture);
                    texture.FadeTo(1);
                }
                else
                {
                    foreach (var img in textures)
                        img.FadeTo(0);
                }
                loadAlpha.SetMethod(hideMethod);
                loadAlpha.TargetValue = 0;
            }
        }

        public void LoadTexture(DataLoader<Texture2D> loader, bool asynchronous = true)
        {
            if (!asynchronous)
            {
                while (!loader.State.HasFlag(DataLoadState.Complete)) { }
                this.Texture = loader.Value;
                return;
            }

            this.pending = loader;
            if (pending == null)
                this.Texture = null;
            else if (pending.State.HasFlag(DataLoadState.Complete))
            {
                this.Texture = pending.Value;
                this.pending = null;
            }
            else
            {
                foreach (var img in textures)
                    img.FadeTo(0);
                if (texture != null)
                    texture.FadeTo(.2f);

                texture = null;
                loadAlpha.SetMethod(showMethod);
                loadAlpha.TargetValue = 1;
            }
        }

        protected internal override void LoadLocalContent(ContentManager content)
        {
            backgroundTexture = content.Load<Texture2D>("ImageBox");
            backgroundBox = MoonifyBoxes.ImageBox;
            loadingIcon = new LoadingIcon(content, LoadingIconTypes.Type1);
        }

        public sealed override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();

            backgroundBox.Draw(spriteBatch, backgroundTexture, this.OffsetLocation, this.Size, Color.White);

            foreach (var img in textures)
                img.Draw(spriteBatch, this.OffsetLocation, this.Size, Color.White);

            if (loadAlpha > 0)
                loadingIcon.Draw(spriteBatch, gameTime, this.OffsetLocation + (this.Size / 2), new Vector2(60, 60), Color.White * loadAlpha);

            spriteBatch.End();
        }
        public sealed override void Update(GameTime gameTime)
        {
            if (pending != null && pending.State.HasFlag(DataLoadState.Complete))
            {
                this.Texture = pending.Value;
                this.pending = null;
            }

            loadAlpha.Update();

            while (textures.Count > 0 && textures.First.Value.Done)
                textures.RemoveFirst();

            if (textures.Count > 0 && textures.Last.Value.Full)
                while (textures.Count > 1)
                    textures.RemoveFirst();

            UpdateImage(gameTime);
        }
        protected virtual void UpdateImage(GameTime gameTime)
        {
        }

        private class AlphaImage
        {
            private ImageBox owner;

            private static DeadDog.GUI.IMoveMethods method = new DeadDog.GUI.MoveSineLine(1, 10);
            private xfloat alpha;
            private Texture2D texture;

            public AlphaImage(ImageBox owner, Texture2D texture, float opacity)
            {
                this.owner = owner;

                this.texture = texture;
                this.alpha = new xfloat(opacity, method);
            }

            public void FadeTo(float opacity)
            {
                if (opacity < 0) opacity = 0;
                if (opacity > 1) opacity = 1;
                alpha.TargetValue = opacity;
            }

            public bool Done
            {
                get { return alpha.CurrentValue == alpha.TargetValue && alpha.TargetValue == 0; }
            }
            public bool Full
            {
                get { return alpha.CurrentValue == alpha.TargetValue && alpha.TargetValue == 1; }
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
