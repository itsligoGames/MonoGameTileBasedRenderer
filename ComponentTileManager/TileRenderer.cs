using AnimatedSprite;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileManagerNS;


namespace ComponentTileManager
{

    public enum TILETYPES { FREE,PAVEMENT,GROUND,BLUE }

    class TileRenderer:DrawableGameComponent
    {
        TileManager _tileManager;
        int _scale = 1;
        private Texture2D _tileSheet;
        private object _font;
        private List<TileRef> _tileRefs = new List<TileRef>();
        private List<TileRef> _path = new List<TileRef>();
        PlayerWithWeapon _player;
        List<SimpleSprite> _collisionSet = new List<SimpleSprite>();
        Dictionary<TILETYPES, TileRef> _tileTypeRefs = new Dictionary<TILETYPES, TileRef>();
        List<TILETYPES> _tileTypes = new List<TILETYPES>();
        List<TILETYPES> _nonPassableTiles = new List<TILETYPES>();
        Vector2 TileTransform;
        List<RotatingSprite> _enemies = new List<RotatingSprite>();

        public TileManager TileManager
        {
            get
            {
                return _tileManager;
            }

            set
            {
                _tileManager = value;
            }
        }

        
        public TileRenderer(Game game, int[,] tileMap, int tileWidth, int tileHeight) : base(game)
        {
            game.Components.Add(this);
            _tileManager = new TileManager();
            _tileSheet = game.Content.Load<Texture2D>(@"Tiles\tank tiles 64 x 64");
            _font = game.Content.Load<SpriteFont>("message");
            _tileRefs.Add(new TileRef(4, 2, 0));
            _tileRefs.Add(new TileRef(3, 3, 1));
            _tileRefs.Add(new TileRef(6, 3, 2));
            _tileRefs.Add(new TileRef(6, 4, 3));
            TileTransform = new Vector2(tileWidth, tileHeight);
            // alternate representation for tile layer references
            _tileTypeRefs.Add(TILETYPES.FREE, new TileRef(4, 2, 0));
            _tileTypeRefs.Add(TILETYPES.PAVEMENT, new TileRef(3, 3, 1));
            _tileTypeRefs.Add(TILETYPES.GROUND, new TileRef(6, 3, 2));
            _tileTypeRefs.Add(TILETYPES.BLUE, new TileRef(6, 4, 3));
            _nonPassableTiles.Add(TILETYPES.FREE);
            _nonPassableTiles.Add(TILETYPES.GROUND);
            _nonPassableTiles.Add(TILETYPES.BLUE);

            string[] backTileNames = { "free", "pavement", "ground", "blue" };
            string[] impassibleTiles = { "free", "ground", "blue" };

            _tileManager.addLayer("background", 
                backTileNames, tileMap, _tileRefs, tileWidth, tileHeight);
            _tileManager.ActiveLayer = _tileManager.getLayer("background");
            _tileManager.ActiveLayer.makeImpassable(impassibleTiles);
            _tileManager.CurrentTile = _tileManager.ActiveLayer.Tiles[0,0];
            // Setup the collision objects for the layer
            setupCollisionMask();
            // Create an enemy object that will rotate towards the player
            //Tile enemyTile = _tileManager.ActiveLayer.Impassable.First();
            setupEnemies(20, tileWidth, tileHeight);
        }

        public void setupEnemies(int EnemyCount, int tileWidth, int tileHeight)
        {
            // use linq queries on the impassible tiles to choose random locations for enemies
            // First query introduce a random guid into a sub set of locations of impassible tiles
            var enemyPlaces = _tileManager.ActiveLayer.Impassable
                                    .Select(subset => new { subset.X, subset.Y, gid = Guid.NewGuid() });
            // order by the guid and take count positions
            // NOTE: Linq ensures that there are no duplicate positions produced
            var randomEnemyPlaces = enemyPlaces.Select(all => new { all.gid, all.X, all.Y })
                                        .OrderBy(subgroup => subgroup.gid).Take(EnemyCount)
                                        //.ToList().Select(r => new { r.X, r.Y })
                                        .ToList();

            // Do a join on the resulting 10 random places and the original impassible tiles 
            // to get the actual tile locations

            List<Tile> enemyPositions = (from enemyPos in _tileManager.ActiveLayer.Impassable
                                         join places in randomEnemyPlaces
                                         on new { enemyPos.X, enemyPos.Y } equals new { places.X, places.Y }
                                         select enemyPos).ToList();
            foreach (Tile t in enemyPositions)
            {
                _enemies.Add(new RotatingSprite(new Vector2(t.X, t.Y),
                    new List<TileRef>() {
                    new TileRef(17,7,0)
                         }, tileWidth, tileHeight, 1f));
            }
        }


        public void setupCollisionMask()
        {
            foreach (Tile  t in _tileManager.ActiveLayer.Impassable)
            {
                _collisionSet.Add(new SimpleSprite(
                    Game.Content.Load<Texture2D>("Collison"), 
                            new Vector2(t.X * t.TileWidth, t.Y * t.TileHeight), 
                                new Vector2(t.TileWidth, t.TileHeight)));

            }
            
        }

        public override void Update(GameTime gameTime)
        {
            if(_player != null)
            {
                _player.Update(gameTime);
                _player.pixelMove(_collisionSet);
                foreach (var _enemy in _enemies)
                {
                    _enemy.follow(_player);
                    _enemy.Update(gameTime);
                }
                Camera Cam = Game.Services.GetService<Camera>();
                Cam.follow(_player.PixelPosition,
                 GraphicsDevice.Viewport);
            }

            base.Update(gameTime);
        }

        //private Tile getOverlappingTile()
        //{
        //    foreach (Tile t in _tileManager.ActiveLayer.Tiles)
        //    {
        //        Rectangle tRect = new Rectangle(new Point(t.X * t.TileWidth, t.Y * t.TileHeight),
        //            new Point(t.TileWidth, t.TileHeight));
        //        if (_player.DrawRectangle.Intersects(tRect) && !t.Passable)
        //            return t;
        //    }
        //    return null;
        //}

        //private bool passable(DIRECTION direction, Tile currentPlayerTile, List<Tile> nonpassable)
        //{
        //    if (_player.MovingState == STATE.MOVING && _player.Direction == DIRECTION.DOWN
        //        && !_tileManager.ActiveLayer.getadjacentTile("below", _player.CurrentPlayerTile).Passable)
        //        _player.MovingState = STATE.STILL;

        //    else if (_player.MovingState == STATE.MOVING && _player.Direction == DIRECTION.UP
        //        && !_tileManager.ActiveLayer.getadjacentTile("above", _player.CurrentPlayerTile).Passable)
        //        _player.MovingState = STATE.STILL;

        //    else if (_player.MovingState == STATE.MOVING && _player.Direction == DIRECTION.LEFT
        //        && !_tileManager.ActiveLayer.getadjacentTile("left", _player.CurrentPlayerTile).Passable)
        //        _player.MovingState = STATE.STILL;

        //    else if (_player.MovingState == STATE.MOVING && _player.Direction == DIRECTION.RIGHT
        //        && !_tileManager.ActiveLayer.getadjacentTile("right", _player.CurrentPlayerTile).Passable)
        //        _player.MovingState = STATE.STILL;

        //    if (_player.MovingState == STATE.MOVING)
        //        return true;
        //    else return false;
        //}

        public void AddPlayer(PlayerWithWeapon p)
        {
            _player = p;
            p.CurrentPlayerTile = _tileManager.CurrentTile;
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            //Texture2D tx = Game.Services.GetService<Texture2D>();
            SpriteFont font = Game.Services.GetService<SpriteFont>();
            Camera Cam = Game.Services.GetService<Camera>();

            sp.Begin(SpriteSortMode.Immediate,
                BlendState.AlphaBlend, null, null, null, null, Cam.CurrentCameraTranslation);
            foreach (Tile t in _tileManager.ActiveLayer.Tiles)
            {
                Vector2 position = new Vector2(t.X * t.TileWidth *_scale, t.Y*t.TileHeight*_scale);
                sp.Draw(_tileSheet, 
                    new Rectangle(position.ToPoint(), new Point(t.TileWidth * _scale, t.TileHeight * _scale)),
                    new Rectangle(t.TileRef._sheetPosX * t.TileWidth,t.TileRef._sheetPosY*t.TileHeight,
                                        t.TileWidth*_scale, t.TileHeight * _scale)
                    , Color.White);
            }
            if (_player != null) {
                _player.Draw(sp, _tileSheet);
                foreach (var _enemy in _enemies)
                    _enemy.Draw(sp, _tileSheet);
                //if (_player.MyProjectile != null)
                //    sp.DrawString(font, "ptp " + _player.Tileposition.ToString(), new Vector2(10, 10), Color.White);
                //    sp.DrawString(font, "prtp " + _player.MyProjectile.Tileposition.ToString(), new Vector2(10, 30), Color.White);
                //    sp.DrawString(font, "stp "+ _player.Site.Tileposition.ToString(), new Vector2(10, 60), Color.White);
            }
            foreach (var item in _collisionSet)
            {
                item.draw(sp);
            }
            sp.End();
            base.Draw(gameTime);
        }
    }
}
