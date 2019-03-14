using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using TileManagerNS;

namespace ComponentTileManager
{
    public class TiledPlatformer : FollowingEnemy
    {
        // To be done
        // Set the Target tile to a tile associated with the second platorm target and follow that tile
        // even though it is stationary.
        // when you reach the target make the start tile the target
        Tile start;
        Tile target;
        

        public TiledPlatformer(Game game, Vector2 userPosition, Tile CurrentTile, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth) 
            : base(game, userPosition, CurrentTile, sheetRefs, frameWidth, frameHeight, layerDepth)
        {
        }

        public override void Draw(GameTime gameTime)
        {
           
            base.Draw(gameTime);
        }

        public override void Update(GameTime gametime)
        {

            base.Update(gametime);
        }

        public static Vector2 CurrentTilePixelPosition(Tile t)
        {
            return new Vector2(t.X, t.Y) * new Vector2(t.TileWidth, t.TileHeight);
        }
    }
}
