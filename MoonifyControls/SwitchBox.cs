using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DeadDog.GUI;
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

        private float yOffset;
        private float xMin, xMax;
        private xfloat xOffset;
        private IMoveMethods xMoveMethod = new MoveSineLine(0.5f, 3000f);

        private bool left;

        public SwitchBox(SwitchBoxTypes type)
            : base(50, 10)
        {
            this.left = true;

            this.type = type;
            this.handleRectangle = handleRectangleFromType(type);
            this.yOffset = handleYOffsetFromType(type);
            this.xMin = handleXOffsetFromType(type, true);
            this.xMax = handleXOffsetFromType(type, false);
            this.xOffset = new xfloat(left ? xMin : xMax, xMoveMethod);

            this.Size = sizeFromType(this.type);
        }

        public bool Left
        {
            get { return left; }
            set
            {
                left = value;
                this.xOffset.TargetValue = left ? xMin : xMax;
            }
        }

        protected override void InnerSizeChange(float width, float height)
        {
            Vector2 size = sizeFromType(type);
            if (width != size.X)
                width = size.X;
            if (height != size.Y)
                height = size.Y;
            base.InnerSizeChange(width, height);
        }

        private static Vector2 sizeFromType(SwitchBoxTypes type)
        {
            switch (type)
            {
                case SwitchBoxTypes.Smaller: return new Vector2(34, 15);
                case SwitchBoxTypes.Small: return new Vector2(34, 15);
                case SwitchBoxTypes.Big: return new Vector2(39 + 32, 17);
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
                case SwitchBoxTypes.Big: return left ? -17 + 16 : 25 - 16 + 32;
                case SwitchBoxTypes.Bigger: return left ? 0 : 35;
                default:
                    throw new NotImplementedException();
            }
        }

        private bool isInsideHandle(Vector2 point)
        {
            float handlePos = xOffset.CurrentValue;

            Vector2 posOffset, sizeofHandle;

            switch (type)
            {
                case SwitchBoxTypes.Smaller:
                    posOffset = new Vector2(9, 4);
                    sizeofHandle = new Vector2(13, 13);
                    break;
                case SwitchBoxTypes.Small:
                    posOffset = new Vector2(7, 2);
                    sizeofHandle = new Vector2(17, 17);
                    break;
                case SwitchBoxTypes.Big:
                    posOffset = new Vector2(1, 1);
                    sizeofHandle = new Vector2(29, 19);
                    break;
                case SwitchBoxTypes.Bigger:
                    posOffset = new Vector2(1, 0);
                    sizeofHandle = new Vector2(29, 22);
                    break;
                default:
                    throw new NotImplementedException();
            }
            point -= this.Location + new Vector2(handlePos, yOffset) + posOffset;
            return point.X >= 0 && point.X < sizeofHandle.X && point.Y >= 0 && point.Y < sizeofHandle.Y;
        }

        protected override void Message(ControlMessages msg, params int[] par)
        {
            switch (msg)
            {
                case ControlMessages.MOUSE_MOVE:
                    if (mouseDown)
                        moveCurrent = par[0];
                    break;
                case ControlMessages.MOUSE_DOWN:
                    if ((MouseButtons)par[2] == MouseButtons.LeftButton)
                        handleMouseDown(new Vector2(par[0], par[1]));
                    break;
                case ControlMessages.MOUSE_UP:
                    if ((MouseButtons)par[2] == MouseButtons.LeftButton)
                        handleMouseUp(new Vector2(par[0], par[1]));
                    break;
                case ControlMessages.MOUSE_LEAVE:
                    if (mouseDown)
                        handleMouseUp(new Vector2(moveCurrent, 0));
                    break;
            }
            base.Message(msg, par);
        }

        private float moveOrigin = 0, moveCurrent = 0;
        private bool mouseDown = false;
        private void handleMouseDown(Vector2 pos)
        {
            if (isInsideHandle(pos))
            {
                mouseDown = true;
                moveOrigin = moveCurrent = pos.X;
            }
            else
                this.Left = !this.Left;
        }
        private void handleMouseUp(Vector2 pos)
        {
            if (!mouseDown)
                return;

            mouseDown = false;
            this.xOffset.CurrentValue = this.xOffset.CurrentValue + moveCurrent - moveOrigin;
            moveOrigin = moveCurrent = 0;
            float x = pos.X - this.Location.X;

            if (x < Math.Abs(this.Size.X - x))
                this.Left = true;
            else
                this.Left = false;
        }

        protected sealed override void LoadLocalContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            this.box = textureBoxFromType(type);
            this.boxTexture = content.Load<Texture2D>(textureFromType(type));

            this.handleTexture = content.Load<Texture2D>("SwitchBoxHandles");
        }

        public override void Update(GameTime gameTime)
        {
            xOffset.Update();
        }
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Begin();
            box.Draw(spriteBatch, boxTexture, this.Location, this.Size, Color.White);

            float handleX = xOffset + moveCurrent - moveOrigin;
            if (handleX < xMin)
                handleX = xMin;
            else if (handleX > xMax)
                handleX = xMax;
            spriteBatch.Draw(handleTexture, this.Location + new Vector2(handleX, yOffset), handleRectangle, Color.White);
            spriteBatch.End();
        }
    }
}
