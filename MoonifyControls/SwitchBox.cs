using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAControls;

namespace MoonifyControls
{
    public class SwitchBox : Control
    {
        public SwitchBox(SwitchBoxTypes type)
            : base(sizeFromType(type))
        {
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
    }
}
