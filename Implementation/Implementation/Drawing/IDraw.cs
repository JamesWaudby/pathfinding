using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Implementation.Drawing
{
    interface IDraw
    {
        void LoadContent(ContentManager content);
        void Draw(SpriteBatch spriteBatch);
    }
}
