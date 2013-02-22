using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAControls
{
    public class CharacterRenderer : TextRenderer
    {
        private Dictionary<char, Texture2D> characters = new Dictionary<char, Texture2D>();

        public CharacterRenderer(string fontname, float fontsize, System.Drawing.FontStyle fontstyle, System.Drawing.Text.TextRenderingHint hint)
            : this(new System.Drawing.Font(fontname, fontsize, fontstyle), hint)
        {
        }
        public CharacterRenderer(System.Drawing.Font font, System.Drawing.Text.TextRenderingHint hint)
            : base(font, hint)
        {
        }

        private Texture2D GetTexture(GraphicsDevice device, char c)
        {
            if (!characters.ContainsKey(c))
                characters.Add(c, CreateTexture(device, c.ToString()));
            return characters[c];
        }

        public override int? TextWidth(string text)
        {
            int w = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (characters.ContainsKey(text[i]))
                    w += characters[text[i]].Width;
            }
            return w;
        }

        public override void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            for (int i = 0; i < text.Length; i++)
            {
                var tex = GetTexture(spriteBatch.GraphicsDevice, text[i]);
                spriteBatch.Draw(tex, position, color);
                position += new Vector2(tex.Width, 0);
            }
        }
    }
}
