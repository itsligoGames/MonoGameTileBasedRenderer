using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{
    class Tower : SimpleSprite
    {
        public Tile TilePlace;

        public Tower(Tile tile, SpriteFont font, Texture2D spriteImage,
                            Vector2 startPosition, Vector2 Size) : base(font, spriteImage,startPosition,Size)
        {
            TilePlace = tile;
        }

    }
}
