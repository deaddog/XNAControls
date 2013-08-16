using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XNAControls
{
    public interface IDrawableGameComponent : IGameComponent, IDrawable, IUpdateable
    {
        void LoadContent(ContentManager content);
        void UnloadContent(ContentManager content);
    }
}
