using AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{

    class FollowingEnemy:  RotatingSprite
    {
        List<Tile> currentPath;
        Tile TargetTile;
        public FollowingEnemy(Vector2 userPosition, Tile CurrentTilePosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth)
            : base(userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {
            TargetTile = CurrentTilePosition;
        }

        public List<Tile> CurrentPath
        {
            get
            {
                return currentPath;
            }

            set
            {
                currentPath = value;
            }
        }

        public void checkPosition(Tile playerTile)
        {
            if(!CurrentPath.Contains(playerTile, new TileComparer()))
            {
                // recalculate the path from the current Tile
            }

        }

    }
}
