using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AnimatedSprite;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TileManagerNS;

namespace AnimatedSprite
{
    public class RotatingSprite : AnimateSheetSprite
    {
        Projectile _p;
        int _rangeDistance = 7;
        Rectangle _range;
        int Health = 100;

        public Rectangle Range
        {
            get
            {
                return new Rectangle(PixelPosition.ToPoint() - new Point(FrameWidth * _rangeDistance / 2, FrameHeight * _rangeDistance / 2) ,
                    new Point(FrameWidth * _rangeDistance,
                                FrameHeight * _rangeDistance));
            }

            set
            {
                _range = value;
            }
        }

        public RotatingSprite(Vector2 userPosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth)
            : base(userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {
            
        }

        public void follow(AnimateSheetSprite followed)
        {
            if(followed.BoundingRect.Intersects(Range))
                angleOfRotation = TurnToFace(followed.PixelPosition, PixelPosition, angleOfRotation, 0.01f);
        }

        protected static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            // The difference in the two points is 
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;
            // ArcTan calculates the angle of rotation 
            // relative to a point (the gun turret position)
            // in the positive x plane and 
            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }


        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
        }

        public override void Draw(SpriteBatch spriteBatch, Texture2D tx)
        {
            base.Draw(spriteBatch, tx);
        }
        /// <summary>
        /// Returns the angle expressed in radians between -Pi and Pi.
        /// Angle is always positive
        /// </summary>
        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
    }
}
