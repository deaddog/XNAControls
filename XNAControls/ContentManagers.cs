using Microsoft.Xna.Framework.Content;
using System;

namespace XNAControls
{
    public class ContentManagers
    {
        private ContentManager containerContent;
        private ContentManager gameContent;

        internal ContentManagers(ContentManager containerContent, ContentManager gameContent)
        {
            if (containerContent == null)
                throw new ArgumentNullException(nameof(containerContent));
            if (gameContent == null)
                throw new ArgumentNullException(nameof(gameContent));

            this.containerContent = containerContent;
            this.gameContent = gameContent;
        }

        public ContentManager ContainerContent
        {
            get { return containerContent; }
        }
        public ContentManager GameContent
        {
            get { return gameContent; }
        }
    }
}
