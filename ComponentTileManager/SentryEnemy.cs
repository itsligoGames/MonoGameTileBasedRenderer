using AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{
    class SentryEnemy : RotatingSprite
    {
        public SentryEnemy(Vector2 userPosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth)
            : base(userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {

        }
    }
}
