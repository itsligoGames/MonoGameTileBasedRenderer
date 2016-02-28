using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TileManagerNS;

using Engine.Engines;
using Microsoft.Xna.Framework.Input.Touch;
using Sprites;
using ComponentTileManager;

namespace AnimatedSprite
{
    public enum DIRECTION {LEFT,RIGHT,UP,DOWN }
    public enum STATE { MOVING,STILL}

        class PlayerWithWeapon : RotatingSprite
        {
            
            private Projectile myProjectile;
            private CrossHair site;
            private Tile currentTilePostion;
            private Rectangle _drawRectangle;
            private DIRECTION _direction;
            private List<List<TileRef>> _directionFrames = new List<List<TileRef>>();
            public Vector2 CentrePos
            {
                get { return PixelPosition + new Vector2(FrameWidth/ 2, FrameHeight/ 2); }
                
            }

        public Vector2 CentreTilePos
        {
            get { return TilePosition + Vector2.One/2; }
        }

        public float speed = 0.2f;
            public Vector2 TargetTilePos;
            public STATE MovingState = STATE.STILL;

        Vector2 SiteTarget = Vector2.Zero;

        public Projectile MyProjectile
        {
            get
            {
                return myProjectile;
            }

            set
            {
                myProjectile = value;
            }
        }

        public Tile CurrentPlayerTile
        {
            get
            {
                return currentTilePostion;
            }

            set
            {
                currentTilePostion = value;
            }
        }

        public Rectangle DrawRectangle
        {
            get
            {
                return new Rectangle((int)PixelPosition.X,
                    (int)PixelPosition.Y,
                    FrameWidth, 
                    FrameHeight );
            }

            set
            {
                _drawRectangle = value;
            }
        }

        public DIRECTION Direction
        {
            get
            {
                return _direction;
            }

            set
            {
                _direction = value;
            }
        }


        internal CrossHair Site
        {
            get
            {
                return site;
            }

            set
            {
                site = value;
            }
        }

        private Vector2 TileBound;

        private Camera _cam;

        public PlayerWithWeapon(Game game, Camera cam, Vector2 userPosition, Vector2 tileBounds,
            List<TileRef> InitialSheetRefs, 
                int frameWidth, int frameHeight, float layerDepth)
            : base(game, userPosition, InitialSheetRefs, frameWidth, frameHeight, layerDepth)
        {
            _cam = cam;
            _directionFrames.Add(InitialSheetRefs); // Stopped
            _directionFrames.Add(new List<TileRef>()); // LEFT
            _directionFrames.Add(new List<TileRef>()); // RIGHT
            _directionFrames.Add(new List<TileRef>()); // UP
            _directionFrames.Add(new List<TileRef>()); // Down All to be set by setFrameSet
            TileBound = tileBounds;
            DamageRate = 10;            
            Site = new CrossHair(game, _cam, userPosition, new List<TileRef>() { new TileRef(11, 6, 0)}, frameWidth, frameHeight,2f);
            }

        public void loadProjectile(Projectile r)
            {
                MyProjectile = r;
            }

        public void setFrameSet(DIRECTION d, List<TileRef> sheetRefs)
        {
            _directionFrames[(int)d] = sheetRefs;
        }

        public void tileMove(List<SimpleSprite> collisionSet)
        {
            DIRECTION oldDirection = Direction;
            // remember the old TilePosition in case we need to  move back
            Vector2 PreviousTilePosition = TilePosition;

            // Rotate toward the site if sight is moving
            if(InputEngine.CurrentPadState.ThumbSticks.Right.LengthSquared() > 0)
            {
                // reset the original direction facing
                Direction = DIRECTION.LEFT;
                followPosition(Site.PixelPosition);
            }
            // Rotate with the mouse
            if (InputEngine.PreviousMouseState.Position.ToVector2() 
                        != InputEngine.MousePosition)
            {
                // reset the original direction facing
                Direction = DIRECTION.LEFT;
                followPosition(Site.PixelPosition);
            }

            if (InputEngine.IsKeyHeld(Keys.Right) || InputEngine.CurrentPadState.DPad.Right == ButtonState.Pressed)
            {
                angleOfRotation = 0f;
                TilePosition += new Vector2(1, 0) * speed;
                _direction = DIRECTION.RIGHT;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Left) || InputEngine.CurrentPadState.DPad.Left == ButtonState.Pressed)
            {
                angleOfRotation = 0f;
                TilePosition += new Vector2(-1, 0) * speed;
                _direction = DIRECTION.LEFT;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Up) || InputEngine.CurrentPadState.DPad.Up == ButtonState.Pressed)
            {
                angleOfRotation = 0f;
                TilePosition += new Vector2(0, -1) * speed;
                _direction = DIRECTION.UP;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Down) || InputEngine.CurrentPadState.DPad.Down == ButtonState.Pressed)
            {
                angleOfRotation = 0f;
                TilePosition += new Vector2(0, 1) * speed;
                _direction = DIRECTION.DOWN;
                MovingState = STATE.MOVING;
            }
            foreach (SimpleSprite item in collisionSet)
            {
                if (item.BoundingRect.Intersects(DrawRectangle))
                {
                    TilePosition = PreviousTilePosition;
                    MovingState = STATE.STILL;
                    item.tint = Color.Red;
                    break;
                }
                else item.tint = Color.White;
            }

            if (Direction != oldDirection && _directionFrames[(int)_direction].Count() > 0)
            {
                Frames = _directionFrames[(int)_direction];
                CurrentFrame = 0;
            }
            // Keep the Player in the tile map
            TilePosition = Vector2.Clamp(TilePosition, Vector2.Zero, TileBound - Vector2.One);
            // keep the site in view of the player
            if (MovingState == STATE.MOVING)
                Site.TilePosition += TilePosition - PreviousTilePosition;
            else Site.TilePosition -= TilePosition - PreviousTilePosition;
            // Keep the Site in the TileMap
            Site.TilePosition = Vector2.Clamp(Site.TilePosition, Vector2.Zero, TileBound - Vector2.One);


        }

        public void checkforMovement()
        {
            DIRECTION oldDirection = Direction;
#if WINDOWS
            if (MovingState != STATE.MOVING)
            {

                if(InputEngine.IsKeyHeld(Keys.Right))
                
                {
                    TargetTilePos = TilePosition + new Vector2(1, 0);
                    _direction = DIRECTION.RIGHT;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Left))
                {
                    TargetTilePos = TilePosition + new Vector2(-1, 0);
                    _direction = DIRECTION.LEFT;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Up))
                {
                    TargetTilePos = TilePosition + new Vector2( 0, -1);
                    _direction = DIRECTION.UP;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Down))
                {
                    TargetTilePos = TilePosition + new Vector2(0, 1);
                    _direction = DIRECTION.DOWN;
                    MovingState = STATE.MOVING;
                }
                // Make sure the player stays in the bounds 
                TargetTilePos = Vector2.Clamp(TargetTilePos, Vector2.Zero,
                                     new Vector2(TileBound.X, TileBound.Y) - new Vector2(1, 1));
            }

            else 
            {
                Vector2 targetDirection = TargetTilePos - TilePosition;
                Vector2 nTarget = Vector2.Normalize(targetDirection);
                float distance = Vector2.DistanceSquared(TilePosition, TargetTilePos);
                if (distance > speed)
                {
                    TilePosition += nTarget * speed;
                }
                else
                {
                    MovingState = STATE.STILL;
                    TilePosition = TargetTilePos;
                    CurrentPlayerTile.X = (int)TilePosition.X;
                    CurrentPlayerTile.Y = (int)TilePosition.Y;
                }

            }
#endif

#if ANDROID
            
            if (MovingState != STATE.MOVING)
            {
                #region Not Moving
                if (InputEngine.currentGestureType == GestureType.HorizontalDrag)
                {
                    if (InputEngine.CurrentGesturePosition.X > InputEngine.PreviousGesturePosition.X)
                    {
                        TargetTilePos = Tileposition + new Vector2(1, 0);
                        _direction = DIRECTION.RIGHT;
                        MovingState = STATE.MOVING;
                    }
                    else if (InputEngine.CurrentGesturePosition.X < InputEngine.PreviousGesturePosition.X)
                    {
                        TargetTilePos = Tileposition + new Vector2(-1, 0);
                        _direction = DIRECTION.LEFT;
                        MovingState = STATE.MOVING;
                    }

                }
                if (InputEngine.currentGestureType == GestureType.VerticalDrag)
                {
                    if (InputEngine.CurrentGesturePosition.Y > InputEngine.PreviousGesturePosition.Y)
                    {

                        TargetTilePos = Tileposition + new Vector2(0, 1);
                        _direction = DIRECTION.DOWN;
                        MovingState = STATE.MOVING;
                    }
                    else if (InputEngine.CurrentGesturePosition.Y < InputEngine.PreviousGesturePosition.Y)
                    {

                        TargetTilePos = Tileposition + new Vector2(0, -1);
                        _direction = DIRECTION.UP;
                        MovingState = STATE.MOVING;
                    }

                }
                // Make sure the player stays in the bounds 
                TargetTilePos = Vector2.Clamp(TargetTilePos, Vector2.Zero,
                                     new Vector2(TileBound.X, TileBound.Y) - new Vector2(1, 1));
                //InputEngine.ClearState();
                // sample movement via gestures and adjust
                #endregion
            }
            else
            {
                #region moving
                // we are moving
                Vector2 targetDirection = TargetTilePos - Tileposition;
                Vector2 nTarget = Vector2.Normalize(targetDirection);
                float distance = Vector2.DistanceSquared(Tileposition, TargetTilePos);
                if (distance > speed)
                {
                    Tileposition += nTarget * speed;
                }
                else
                {
                    MovingState = STATE.STILL;
                    Tileposition = TargetTilePos;
                    CurrentPlayerTile.X = (int)Tileposition.X;
                    CurrentPlayerTile.Y = (int)Tileposition.Y;
                }
                #endregion
            }

            if (InputEngine.currentGestureType == GestureType.Tap && InputEngine.CurrentGesturePosition != InputEngine.PreviousGesturePosition)
                Site.Tileposition = new Vector2((float)Math.Round(InputEngine.CurrentGesturePosition.X / (FrameWidth * _cam.Scale)),
                                                        (float)Math.Round(InputEngine.CurrentGesturePosition.Y / (FrameHeight * _cam.Scale)));


#endif

            // Change the image set based on a change of direction
            if (Direction != oldDirection && _directionFrames[(int)_direction].Count() > 0)
            {
                Frames = _directionFrames[(int)_direction];
                CurrentFrame = 0;
            }


            //return moved;
        }

        internal void HitTest(RotatingSprite s)
        {
            if (myProjectile.ProjectileState == Projectile.PROJECTILE_STATE.EXPOLODING &&
                myProjectile.BoundingRectangle.Intersects(s.BoundingRectangle))
                s.Health -= DamageRate;

        }

        public override void Update(GameTime gameTime)
        {
            if(Site != null)
                Site.Update(gameTime);
            if (MyProjectile != null 
                && MyProjectile.ProjectileState == Projectile.PROJECTILE_STATE.STILL)
            {
                myProjectile.TilePosition = TilePosition;
                // fire the rocket and it looks for the target
                if (InputEngine.IsKeyHeld(Keys.Space) || InputEngine.IsMouseLeftClick() || InputEngine.CurrentPadState.Buttons.A == ButtonState.Pressed)
                    MyProjectile.fire(Site.TilePosition);
            }
                
            if (MyProjectile != null)
                MyProjectile.Update(gameTime);
            // update the health bar object
            //if (Hbar != null)
            //{
            //    Hbar.position = PixelPosition + new Vector2(-10, -20);
            //    Hbar.health = Health;
            //}
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {


            base.Draw(gameTime);
        }
        //public override void Draw(SpriteBatch spriteBatch,Texture2D tx)
        //{
        //    base.Draw(spriteBatch,tx);
        //    if (MyProjectile != null && MyProjectile.ProjectileState != Projectile.PROJECTILE_STATE.STILL)
        //        MyProjectile.Draw(spriteBatch, tx);
        //    if (Site != null)
        //    {
        //        Site.Draw(spriteBatch, tx);
        //    }

        //    if (Hbar != null)
        //        Hbar.draw(spriteBatch);
        //}

    }
}
