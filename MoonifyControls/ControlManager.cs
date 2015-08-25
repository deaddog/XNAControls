using System;
using Microsoft.Xna.Framework;
using XNAControls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonifyControls
{
    public class ControlManager : ControlManagerBase
    {
        private Texture2D background;

        public ControlManager(Game game)
            : base(game, "MoonifyContents")
        {
        }
        public ControlManager(IntPtr controlHandle, IServiceProvider services, GraphicsDevice graphicsDevice)
            : base(controlHandle, services, "MoonifyContents", graphicsDevice)
        {
        }

        public Texture2D Background
        {
            get { return background; }
        }

        protected override void LoadManagerContent(ContentManagers content)
        {
            this.background = content.ContainerContent.Load<Texture2D>("Background");
        }
    }
}
