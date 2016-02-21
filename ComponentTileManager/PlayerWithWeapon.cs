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
using NetworkClient.Network.Player;
using Microsoft.Xna.Framework.Content;
using NetworkClient.Network;
using Lidgren.Network;

namespace AnimatedSprite
{
    public enum DIRECTION {LEFT,RIGHT,UP,DOWN }
    public enum STATE { MOVING,STILL}

        class PlayerWithWeapon : AnimateSheetSprite
        {
        private GamePlayer networkPlayer;
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
            public static List<RotatingSprite> otherPlayers = new List<RotatingSprite>();
            public Vector2 CentreTilePos
        {
            get { return Tileposition + Vector2.One/2; }
        }

            public float speed = 0.1f;
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

        internal GamePlayer Player
        {
            get
            {
                return networkPlayer;
            }

            set
            {
                networkPlayer = value;
            }
        }

        private Vector2 TileBound;

        private Camera _cam;

        private Game _game;

        public PlayerWithWeapon(Game g, Camera cam, Vector2 userPosition, Vector2 tileBounds,
            List<TileRef> InitialSheetRefs, 
                int frameWidth, int frameHeight, float layerDepth)
            : base(userPosition, InitialSheetRefs, frameWidth, frameHeight, layerDepth)
        {
            _game = g;
            _cam = cam;
            ID = Guid.NewGuid().ToString();
            Network.ClientID = ID;
            _directionFrames.Add(InitialSheetRefs); // Stopped
            _directionFrames.Add(new List<TileRef>()); // LEFT
            _directionFrames.Add(new List<TileRef>()); // RIGHT
            _directionFrames.Add(new List<TileRef>()); // UP
            _directionFrames.Add(new List<TileRef>()); // Down All to be set by setFrameSet
            TileBound = tileBounds;
            Site = new CrossHair(_cam, userPosition, new List<TileRef>() { new TileRef(11, 6, 0)}, frameWidth, frameHeight,2f);
            networkPlayer = new GamePlayer(ID, this.Tileposition);

            Network.Config = new NetPeerConfiguration("TankWarServer"); //Same as the Server, so the same name to be used.
            Network.Client = new NetClient(Network.Config);
            Network.Client.Start();
            Network.Client.Connect("127.0.0.1", 14242); //And Connect the Server with IP (string) and host (int) parameters
            System.Threading.Thread.Sleep(300);

            Network.outmsg = Network.Client.CreateMessage();
            Network.outmsg.Write("connect");
            Network.outmsg.Write(ID);
            Network.outmsg.Write(Tileposition.X);
            Network.outmsg.Write(Tileposition.Y);
            Network.Client.SendMessage(Network.outmsg, NetDeliveryMethod.ReliableOrdered);
            System.Threading.Thread.Sleep(300);

        }

        public void loadProjectile(Projectile r)
        {
            MyProjectile = r;
        }

        public void setFrameSet(DIRECTION d, List<TileRef> sheetRefs)
        {
            _directionFrames[(int)d] = sheetRefs;
        }

        public void pixelMove(List<SimpleSprite> collisionSet)
        {
            DIRECTION oldDirection = Direction;
            // remember the old TilePosition in case we need to  move back
            Vector2 PreviousTilePosition = Tileposition;

            if (InputEngine.IsKeyHeld(Keys.Right))

            {
                Tileposition += new Vector2(1, 0) * speed;
                _direction = DIRECTION.RIGHT;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Left))
            {
                Tileposition += new Vector2(-1, 0) * speed;
                _direction = DIRECTION.LEFT;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Up))
            {
                Tileposition += new Vector2(0, -1) * speed;
                _direction = DIRECTION.UP;
                MovingState = STATE.MOVING;
            }
            if (InputEngine.IsKeyHeld(Keys.Down))
            {
                Tileposition += new Vector2(0, 1) * speed;
                _direction = DIRECTION.DOWN;
                MovingState = STATE.MOVING;
            }
            foreach (SimpleSprite item in collisionSet)
            {
                if (item.BoundingRect.Intersects(DrawRectangle))
                {
                    Tileposition = PreviousTilePosition;
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
            Tileposition = Vector2.Clamp(Tileposition, Vector2.Zero, TileBound - Vector2.One);
            if (Tileposition != PreviousTilePosition)
            {
                // Networked Movement
                GamePlayer.position = Tileposition;
                GamePlayer.Update();
            }

        }

        public void checkforMovement()
        {
            DIRECTION oldDirection = Direction;
#if WINDOWS
            if (MovingState != STATE.MOVING)
            {

                if(InputEngine.IsKeyHeld(Keys.Right))
                
                {
                    TargetTilePos = Tileposition + new Vector2(1, 0);
                    _direction = DIRECTION.RIGHT;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Left))
                {
                    TargetTilePos = Tileposition + new Vector2(-1, 0);
                    _direction = DIRECTION.LEFT;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Up))
                {
                    TargetTilePos = Tileposition + new Vector2( 0, -1);
                    _direction = DIRECTION.UP;
                    MovingState = STATE.MOVING;
                }
                if (InputEngine.IsKeyHeld(Keys.Down))
                {
                    TargetTilePos = Tileposition + new Vector2(0, 1);
                    _direction = DIRECTION.DOWN;
                    MovingState = STATE.MOVING;
                }
                // Make sure the player stays in the bounds 
                TargetTilePos = Vector2.Clamp(TargetTilePos, Vector2.Zero,
                                     new Vector2(TileBound.X, TileBound.Y) - new Vector2(1, 1));
            }

            else 
            {
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
        public override void Update(GameTime gameTime)
        {
            if(Site != null)
                Site.Update(gameTime);

            if (MyProjectile != null 
                && MyProjectile.ProjectileState == Projectile.PROJECTILE_STATE.STILL)
            {
                myProjectile.Tileposition = Tileposition;
                // fire the rocket and it looks for the target
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    MyProjectile.fire(Site.Tileposition);
            }

            if (MyProjectile != null)
                MyProjectile.Update(gameTime);

            // get all the players now in teh game
            if (GamePlayer.otherPlayers.Count > 0)
            {
                var allPlayers = new HashSet<string>(otherPlayers.Select(p => p.ID));
                // get new players that are not part of this game
                var newPlayers = GamePlayer.otherPlayers.Where(p => !allPlayers.Contains(GamePlayer.name));

                // Add any new players
                foreach (GamePlayer newplayer in newPlayers)
                    otherPlayers.Add(new RotatingSprite(GamePlayer.position,
                                            new List<TileRef>()
                                            {
                                          new TileRef(0,2,0)
                                            },
                                        FrameWidth, FrameHeight, 1f));
            }
            base.Update(gameTime);
        }
            
        public override void Draw(SpriteBatch spriteBatch,Texture2D tx)
        {
            base.Draw(spriteBatch,tx);
            if (MyProjectile != null && MyProjectile.ProjectileState != Projectile.PROJECTILE_STATE.STILL)
                MyProjectile.Draw(spriteBatch, tx);
            if(Site != null)
                Site.Draw(spriteBatch, tx);
            foreach (RotatingSprite other in otherPlayers)
                other.Draw(spriteBatch, tx);
        }

    }
}
