using AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace ComponentTileManager
{
    class FollowingEnemy :RotatingSprite
    {
        
        List<Tile> _path = new List<Tile>();
        Tile PrevPlayerTilePosition;
        Tile CurrentTargetTile;
        Projectile _p;
        Rectangle _range;

        public List<Tile> Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        public FollowingEnemy(Vector2 userPosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth)
            : base(userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {
            

        }

        public void followPath(Tile CurrentPlayerTilePosition, List<Tile> currentPath)
        {
            if(PrevPlayerTilePosition != CurrentPlayerTilePosition )
            {
                _path = currentPath;
                PrevPlayerTilePosition = CurrentPlayerTilePosition;
            }
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }



        public void Hit(Projectile p)
        {

        }

    }
}
