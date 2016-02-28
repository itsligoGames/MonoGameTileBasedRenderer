using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;

namespace Helpers
{
    public class HealthBar : DrawableGameComponent
    {
        public int health;
        private Texture2D txHealthBar; // hold the texture
        public Vector2 position; // Position on the screen
        public Rectangle HealthRect
        {
            get
            {
                return new Rectangle((int)position.X , 
                                (int)position.Y , health, 10);
            }
        }

        public HealthBar(Game game, Vector2 pos) : base(game)
        {
            txHealthBar = new Texture2D(game.GraphicsDevice, 1, 1);
            txHealthBar.SetData(new[] { Color.White });
            position = pos;

        }

        public void draw(SpriteBatch spriteBatch)
        {
            Camera Cam = Game.Services.GetService<Camera>();
            spriteBatch.Begin(SpriteSortMode.Immediate,
                    BlendState.AlphaBlend, null, null, null, null, Cam.CurrentCameraTranslation);
            if (health > 60)
                spriteBatch.Draw(txHealthBar, HealthRect, Color.Green);
            else if (health > 30 && health <= 60)
                spriteBatch.Draw(txHealthBar, HealthRect, Color.Orange);
            else if (health > 0 && health <= 30)
                spriteBatch.Draw(txHealthBar, HealthRect, Color.Red);
            spriteBatch.End();
        }

    }
}
