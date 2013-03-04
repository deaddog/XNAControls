using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAControls;

namespace MoonifyControls
{
    public class SwitchBox : Control
    {
        private SwitchBoxTypes type;
        private Box box;
        private Texture2D boxTexture;

        private Rectangle handleRectangle;
        private Texture2D handleTexture;
        private Vector2 handleOffset;

        private bool left;

        public SwitchBox(SwitchBoxTypes type)
            : base(sizeFromType(type))
        {
            this.left = true;

            this.type = type;
            this.handleRectangle = handleRectangleFromType(type);
            this.handleOffset = new Vector2(handleXOffsetFromType(type, left), handleYOffsetFromType(type));
        }

        public bool Left
        {
            get { return left; }
            set
            {
                left = value;
                this.handleOffset = new Vector2(handleXOffsetFromType(type, left), handleYOffsetFromType(type));
            }
        }

        private static Vector2 sizeFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller: return new Vector2(34, 15);
                case SwitchBoxTypes.Small: return new Vector2(34, 15);
                case SwitchBoxTypes.Big: return new Vector2(39, 17);
                case SwitchBoxTypes.Bigger: return new Vector2(66, 24);
                default:
                    throw new NotImplementedException();
            }
        }
        private static Box textureBoxFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller:
                case SwitchBoxTypes.Small:
                    return MoonifyBoxes.SwitchBoxSmall;
                case SwitchBoxTypes.Big:
                case SwitchBoxTypes.Bigger:
                    return MoonifyBoxes.SwitchBoxBig;
                default:
                    throw new NotImplementedException();
            }
        }
        private static string textureFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller:
                case SwitchBoxTypes.Small:
                    return "SwitchBoxSmall";
                case SwitchBoxTypes.Big:
                case SwitchBoxTypes.Bigger:
                    return "SwitchBoxBig";
                default:
                    throw new NotImplementedException();
            }
        }
        private static Rectangle handleRectangleFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller:
                    return new Rectangle(32, 25, 31, 24);
                case SwitchBoxTypes.Small:
                    return new Rectangle(0, 25, 31, 24);
                case SwitchBoxTypes.Big:
                    return new Rectangle(32, 0, 31, 24);
                case SwitchBoxTypes.Bigger:
                    return new Rectangle(0, 0, 31, 24);
                default:
                    throw new NotImplementedException();
            }
        }
        private static float handleYOffsetFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller: return -3;
                case SwitchBoxTypes.Small: return -3;
                case SwitchBoxTypes.Big: return -2;
                case SwitchBoxTypes.Bigger: return 1;
                default:
                    throw new NotImplementedException();
            }
        }
        private static float handleXOffsetFromType(SwitchBoxTypes type, bool left)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller: return left ? -8 : 11;
                case SwitchBoxTypes.Small: return left ? -7 : 10;
                case SwitchBoxTypes.Big: return left ? -17 + 16 : 25 - 16;
                case SwitchBoxTypes.Bigger: return left ? 0 : 35;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void LoadResources(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.box = textureBoxFromType(type);
            this.boxTexture = content.Load<Texture2D>(textureFromType(type));

            this.handleTexture = content.Load<Texture2D>("SwitchBoxHandles");
        }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            box.Draw(spriteBatch, boxTexture, this.Location, this.Size, Color.White);
            spriteBatch.Draw(handleTexture, this.Location + handleOffset, handleRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
