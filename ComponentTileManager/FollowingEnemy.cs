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
        Stack<Tile> currentPath = new Stack<Tile>();
        Tile currentTile;
        Vector2 currentPos;
        float speed = 0.01f;        
        private Tile nextTile;
        float delay = 1;
        public STATE MovingState = STATE.STILL;
        public Tile CurrentPlayerTile;
        public float PreviousHeading = 0f;
        public float CurrentHeading = 0f;

        public FollowingEnemy(Game game , Vector2 userPosition, Tile CurrentTile, 
            List<TileRef> sheetRefs, 
            int frameWidth, int frameHeight, float layerDepth)
            : base(game, userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {

            currentTile = CurrentTile;
        }

        public Stack<Tile> CurrentPath
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

        public void checkPosition(TileManager tman, Tile playerTile)
        {
            // Keeping an Eye on the player
            if (!CurrentPath.Contains(playerTile, new TileComparer()) 
                    && currentTile != playerTile)
            {
                CurrentPath = PathFinder.StackPath(tman, currentTile, playerTile);
                CurrentPlayerTile = playerTile;
                MovingState = STATE.STILL;
            }

        }

        public override void Update(GameTime gametime)
        {
            // if we are startng on the path then 
            // choose the current node and the next node to move to
            
                if (MovingState == STATE.STILL && CurrentPath.Count > 0)
                {
                    // first tile is current tile and remove from path
                    currentTile = CurrentPath.Pop();
                    // is target tile
                    currentPos = CurrentTilePosition(currentTile);
                    //nextTile = currentPath.Pop();
                    MovingState = STATE.MOVING;
                    TilePosition = currentPos;
                    followPosition(CurrentTilePixelPosition(CurrentPlayerTile));
                }
                // if we are moving towards the next node 
                // update the position if we are not there yet
                else if ( MovingState == STATE.MOVING &&
                    CurrentPath.Count > 0 &&
                    Vector2.DistanceSquared(currentPos, CurrentTilePosition(currentPath.Peek())) > speed)
                {
                    Vector2 direction =  CurrentTilePosition(currentPath.Peek()) - CurrentTilePosition(currentTile);
                    currentPos += direction * speed;
                    // rotate towards the new heading
                    TilePosition = currentPos;
                    followPosition(CurrentTilePixelPosition(CurrentPlayerTile));
                    MovingState = STATE.MOVING;
                }
                // otherwise we are at the next node so stop
                // and choose a new next node to move towards see STILL STATE above
                else if (CurrentPath.Count > 0)
                {
                    // change rotation towards the next tile and stop
                    followPosition(CurrentTilePixelPosition(CurrentPlayerTile));
                    MovingState = STATE.STILL;
                }
                
            base.Update(gametime);
        }

        public static Vector2 CurrentTilePosition(Tile t)
        {
            return new Vector2(t.X , t.Y);
        }

        public static Vector2 CurrentTilePixelPosition(Tile t)
        {
            return new Vector2(t.X, t.Y) * new Vector2(t.TileWidth,t.TileHeight);
        }


        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
