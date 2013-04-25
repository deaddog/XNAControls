using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace XNAControls
{
    public class LoadingIcon
    {
        private Texture2D loaderTexture;
        private LoadingIconTypes _type;
        private Rectangle sourceRect;

        private LoadingIconTypes type
        {
            get { return _type; }
            set
            {
                _type = value;
                switch (_type)
                {
                    case LoadingIconTypes.Type1:
                        sourceRect = new Rectangle(1, 1, 30, 30);
                        break;
                    case LoadingIconTypes.Type2:
                        sourceRect = new Rectangle(32, 1, 30, 30);
                        break;
                    case LoadingIconTypes.Type3:
                        sourceRect = new Rectangle(63, 1, 30, 30);
                        break;
                    case LoadingIconTypes.Type4:
                        sourceRect = new Rectangle(94, 1, 30, 30);
                        break;
                    case LoadingIconTypes.Type5:
                        sourceRect = new Rectangle(125, 1, 30, 30);
                        break;
                    case LoadingIconTypes.Type6:
                        sourceRect = new Rectangle(156, 1, 30, 30);
                        break;
                    default:
                        throw new NotImplementedException("Unknown " + typeof(LoadingIconTypes).Name + ": " + _type);
                }
            }
        }

        /// <summary>
        /// Rotation in radians
        /// </summary>
        private float rotation;

        public LoadingIcon(ContentManager content, LoadingIconTypes type)
        {
            loaderTexture = content.Load<Texture2D>("LoadingIcons_30x30");
            this.type = type;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 center, Vector2 size, Color color)
        {
            double movePerSecond = Math.PI * (2.0 / 3.0);
            rotation += (float)(gameTime.ElapsedGameTime.TotalSeconds * movePerSecond);
            Vector2 scale = size / new Vector2(sourceRect.Width, sourceRect.Height);
            spriteBatch.Draw(loaderTexture, center, sourceRect, color, rotation, new Vector2(15,15), scale, SpriteEffects.None, 0);
        }
    }
}
