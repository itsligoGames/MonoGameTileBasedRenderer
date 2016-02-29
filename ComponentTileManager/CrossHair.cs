using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Engine.Engines;
using TileManagerNS;

namespace AnimatedSprite
{
    class CrossHair : AnimateSheetSprite
    {
        Vector2 _target = Vector2.Zero;

        public Vector2 Target
        {
            get
            {
                return _target;
            }

            set
            {
                _target = value;
            }
        }
        private Camera _cam;
        private float smoothingFactor = 0.08f;

        public CrossHair(Game game, Camera cam, Vector2 userPosition, List<TileRef> cursor, int frameWidth, int frameHeight, float layerDepth) 
                            : base(game, userPosition, cursor, frameWidth, frameHeight, layerDepth)
            {
            _cam = cam;
            }

        public override void Update(GameTime gametime)
        {
            if (InputEngine.CurrentPadState.IsConnected)
            {
                if (InputEngine.CurrentPadState.ThumbSticks.Right.Length() > 0)
                
                {
                    Vector2 previousTilePos = TilePosition;
                    // Thumbstick moves in the opposite direction for Y hence adjust with  new Vector2(1,-1)
                    Vector2 Movement = Vector2.Normalize(InputEngine.CurrentPadState.ThumbSticks.Right) * new Vector2(1,-1) * smoothingFactor;
                    
                    //InputEngine.CurrentPadState.ThumbSticks.Left / new Vector2(FrameWidth, FrameHeight)
                    //                    + _cam.CamPos / new Vector2(FrameWidth, FrameHeight);
                    Rectangle LocalviewBound = new Rectangle(((_cam.CamPos) / new Vector2(FrameWidth, FrameHeight)).ToPoint(),
                                                new Point((_cam.View.Width ) / FrameWidth, (_cam.View.Height)/ FrameHeight));
                    
                    // Get the player from the Game components
                    PlayerWithWeapon player = (PlayerWithWeapon)Game.Components.Where(p => p.GetType() == typeof(PlayerWithWeapon)).FirstOrDefault();
                    // Make sure the site stays in the local view range
                    if (!LocalviewBound.Contains((TilePosition + Movement).ToPoint()))
                    {
                        TilePosition = previousTilePos;
                    }
                    // Make sure the site 
                    else if(player != null && !player.Range.Contains((PixelPosition + Movement * new Vector2(FrameWidth,FrameHeight)).ToPoint()))
                    {
                        TilePosition = previousTilePos;
                    }

                    else TilePosition += Movement;
                    //Tileposition = Vector2.Clamp(Tileposition, _cam.CamPos,
                    //    new Vector2(_cam.View.Bounds.Width, _cam.View.Bounds.Height));
                }
            }
            else if(_cam != null)
            {
                TilePosition = InputEngine.MousePosition / new Vector2(FrameWidth, FrameHeight)
                                + _cam.CamPos / new Vector2(FrameWidth, FrameHeight);
            }
            
            base.Update(gametime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

    }
}
