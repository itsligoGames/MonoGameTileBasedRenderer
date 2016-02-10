using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Sprites
{
    public class SimpleSprite
    {
        public Texture2D Image;
        public Vector2 Position;
        public Rectangle BoundingRect;
        public bool Visible = false;
        public Color tint = Color.White;

        // Constructor epects to see a loaded Texture
        // and a start position
        public SimpleSprite(Texture2D spriteImage,
                            Vector2 startPosition, Vector2 Size)
        {
            // Take a copy of the texture passed down
            Image = spriteImage;
            // Take a copy of the start position
            Position = startPosition;
            // Calculate the bounding rectangle
            BoundingRect = new Rectangle((int)startPosition.X, (int)startPosition.Y, Image.Width, Image.Height);

        }

        public void draw(SpriteBatch sp)
        {
            if (Image != null && Visible)
                sp.Draw(Image, BoundingRect, tint);
        }

        public void Move(Vector2 delta)
        {
            Position += delta;
            BoundingRect = new Rectangle((int)Position.X, (int)Position.Y, Image.Width, Image.Height);
            BoundingRect.X = (int)Position.X;
            BoundingRect.Y = (int)Position.Y;
        }



        //public bool InCollisionSimple = false;
        //public bool collisionDetectSimple(AnimatedSprite otherSprite)
        //{

        //    BoundingRect = new Rectangle((int)this.position.X, (int)this.position.Y, this.spriteWidth, this.spriteHeight);
        //    Rectangle otherBound = new Rectangle((int)otherSprite.position.X, (int)otherSprite.position.Y, otherSprite.spriteWidth, this.spriteHeight);
        //    if (BoundingRect.Intersects(otherBound))
        //    {
        //        InCollisionSimple = true;
        //        return true;
        //    }
        //    else
        //    {
        //        InCollisionSimple = false;
        //        return false;
        //    }
        //}
    }
}
