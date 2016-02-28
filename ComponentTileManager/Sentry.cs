using AnimatedSprite;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;
using Microsoft.Xna.Framework.Graphics;

namespace ComponentTileManager
{
    public class Sentry : RotatingSprite
    {
        Projectile _p;
        private Tile targetTile;
        public Projectile P
        {
            get
            {
                return _p;
            }

            set
            {
                _p = value;
            }
        }

        public Sentry(Game game,Vector2 userPosition, List<TileRef> sheetRefs, int frameWidth, int frameHeight, float layerDepth)
            : base(game, userPosition, sheetRefs, frameWidth, frameHeight, layerDepth)
        {


        }

        public void loadProjectile(Projectile r)
        {
            _p = r;
        }

        public override void follow(AnimateSheetSprite followed)
        {
                if (P != null 
                    && P.ProjectileState == Projectile.PROJECTILE_STATE.STILL 
                    && followed.BoundingRectangle.Intersects(Range))
                            P.fire(followed.TilePosition);
            base.follow(followed);
        }
        public override void Update(GameTime gametime)
        {
            if (P != null
                    && P.ProjectileState == Projectile.PROJECTILE_STATE.STILL)
                        P.TilePosition = TilePosition;
           if (P != null)
                P.Update(gametime);

            Hbar.health = Health;
            
            base.Update(gametime);
        }
        public override void Draw(GameTime gameTime)
        {
            if (P != null && P.ProjectileState != Projectile.PROJECTILE_STATE.STILL)
                P.Draw(gameTime);

            if (Hbar != null)
                Hbar.draw(Game.Services.GetService<SpriteBatch>());

            base.Draw(gameTime);
        }


        internal void HitTest(PlayerWithWeapon player)
        {
            if (P.ProjectileState == Projectile.PROJECTILE_STATE.EXPOLODING &&
                P.BoundingRectangle.Intersects(player.BoundingRectangle))
                player.Health--;
            
            
        }

    }
}
