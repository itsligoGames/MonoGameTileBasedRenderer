using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TileManagerNS;

namespace AnimatedSprite
{
    public class AnimateSheetSprite : DrawableGameComponent
    {
        //sprite texture and position
        
        private bool visible;
        protected Vector2 origin;
        protected float angleOfRotation;
        protected float spriteDepth = 1f;
        private float scale = 1f;

        private Rectangle boundingRectangle;

        private Vector2 _tileposition;
        public Vector2 TilePosition
        {
            get
            {
                return _tileposition;
            }

            set
            {
                _tileposition = value;
            }
        }

        //the number of frames in the sprite sheet
        //the current fram in the animation
        //the time between frames
        int mililsecondsBetweenFrames = 100;
        float timer = 0f;

        //the width and height of our texture
        public int FrameWidth = 0;
        public int FrameHeight = 0;

        //the source of our image within the sprite sheet to draw
        Rectangle sourceRectangle;

        public Rectangle SourceRectangle
        {
            get { return sourceRectangle; }
            set { sourceRectangle = value; }
        }

        
        public Vector2 PixelPosition
        {
            get
            {
                return new Vector2(TilePosition.X * FrameWidth, 
                                    TilePosition.Y * FrameHeight) ;
            }

        }

        public int CurrentFrame
        {
            get
            {
                return _currentFrame;
            }

            set
            {
                _currentFrame = value;
            }
        }

        public float Scale
        {
            get
            {
                return scale;
            }

            set
            {
                scale = value;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle(PixelPosition.ToPoint(),
                    new Point(FrameWidth , FrameHeight ));
            }

            set
            {
                boundingRectangle = value;
            }
        }

        protected List<TileRef> Frames = new List<TileRef>();
        private int _currentFrame;
        
        public AnimateSheetSprite(Game g, Vector2 userPosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth) : base(g)
        {
            spriteDepth = layerDepth;
            TilePosition = userPosition;
            Visible = true;
            FrameHeight = frameHeight;
            FrameWidth = frameWidth;
            Frames = sheetRefs;
            // added to allow sprites to rotate
            origin = new Vector2(FrameWidth / 2, FrameHeight/ 2);
            angleOfRotation = 0;
            CurrentFrame = 0;
            g.Components.Add(this);
        }


        public override void Update(GameTime gametime)
        {
            timer += (float)gametime.ElapsedGameTime.Milliseconds;

            //if the timer is greater then the time between frames, then animate
                    if (timer > mililsecondsBetweenFrames)
                    {
                        _currentFrame++;
                        //if we have exceed the number of frames
                        if (_currentFrame > Frames.Count -1 )
                        {
                           _currentFrame = 0;
                        }
                        //reset our timer
                        timer = 0f;
                    }
            //set the source to be the current frame in our animation
                sourceRectangle = new Rectangle(Frames[CurrentFrame]._sheetPosX * FrameWidth  , 
                        Frames[CurrentFrame]._sheetPosY * FrameHeight, 
                        FrameWidth, FrameHeight);
            
            }
        public bool collisionDetect(AnimateSheetSprite other)
        {
            Rectangle myBound = new Rectangle((int)this.PixelPosition.X, (int)this.PixelPosition.Y, this.FrameWidth, this.FrameHeight);
            Rectangle otherBound = new Rectangle((int)other.PixelPosition.X, (int)other.PixelPosition.Y, other.FrameWidth, other.FrameHeight);

            return myBound.Intersects(otherBound);
        }

        public bool TileCollision(Tile other)
        {
            Rectangle myBound = new Rectangle((int)this.PixelPosition.X , (int)this.PixelPosition.Y, this.FrameWidth , this.FrameHeight);
            Rectangle otherBound = new Rectangle((int)other.X*other.TileWidth, (int)other.Y*other.TileHeight, other.TileWidth, other.TileHeight);
            
            return myBound.Intersects(otherBound);
        }

        public bool InTile(Tile other)
        {
            Rectangle myBound = new Rectangle((int)this.PixelPosition.X, (int)this.PixelPosition.Y, this.FrameWidth, this.FrameHeight);
            Rectangle otherBound = new Rectangle((int)other.X * other.TileWidth, (int)other.Y * other.TileHeight, other.TileWidth, other.TileHeight);

            return otherBound.Contains(myBound);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = Game.Services.GetService<SpriteBatch>();
            // Could do Texture2D as a static class with dictionary of textures
            // if different textures needed
            Texture2D SpriteSheet = Game.Services.GetService<Texture2D>();
            Camera Cam = Game.Services.GetService<Camera>();

            if (Visible)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend, null, null, null, null, Cam.CurrentCameraTranslation);
                spriteBatch.Draw(SpriteSheet,
                    PixelPosition + origin, sourceRectangle,
                    Color.White, angleOfRotation, origin,
                    Scale, SpriteEffects.None, spriteDepth);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }


    }
}
