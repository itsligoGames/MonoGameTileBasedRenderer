using AnimatedSprite;
using Engine.Engines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Sprites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TileManagerNS;


namespace ComponentTileManager
{

    public enum TILETYPES { FREE, PAVEMENT, GROUND, BLUE }



    class TileRenderer : DrawableGameComponent
    {
        TileManager _tileManager;
        int _scale = 1;
        private Texture2D _tileSheet;
        private object _font;
        private List<TileRef> _tileRefs = new List<TileRef>();
        //private List<Tile> _path = new List<Tile>();
        private List<SimpleSprite> _pathTiles = new List<SimpleSprite>();
        PlayerWithWeapon player;
        List<SimpleSprite> _collisionSet = new List<SimpleSprite>();
        List<Tower> _towers = new List<Tower>();
        Vector2 TileTransform;
        Vector2 ViewportCentre
        {
            get
            {
                return new Vector2(GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);
            }
        }
        private Camera cam;
        int tileWidth = 64;
        int tileHeight = 64;
        int[,] tileMap = new int[,] { };

        List<Sentry> _enemies = new List<Sentry>();
        List<Tile> _spawnPositions = new List<Tile>();
        List<Color> _spawnColor = new List<Color> { Color.Blue, Color.White, Color.Red, Color.RosyBrown };
        Texture2D _txShowRectangle;
        List<FollowingEnemy> _followers = new List<FollowingEnemy>();

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

        public TileRenderer(Game game, int[,] Map, int tileWidth, int tileHeight) : base(game)
        {
            game.Components.Add(this);
            _tileManager = new TileManager();
            tileMap = Map;
            // texture to show collision fields for enemies
            _txShowRectangle = game.Content.Load<Texture2D>("Collison");
            _tileRefs.Add(new TileRef(4, 2, 0));
            _tileRefs.Add(new TileRef(3, 3, 1));
            _tileRefs.Add(new TileRef(6, 3, 2));
            _tileRefs.Add(new TileRef(6, 2, 3));
            _tileRefs.Add(new TileRef(0, 2, 4));
            TileTransform = new Vector2(tileWidth, tileHeight);

            string[] backTileNames = { "free", "pavement", "ground", "blue", "home" };
            string[] impassibleTiles = { "free", "ground", "blue" };

            _tileManager.addLayer("background",
                backTileNames, tileMap, _tileRefs, tileWidth, tileHeight);
            _tileManager.ActiveLayer = _tileManager.getLayer("background");
            _tileManager.ActiveLayer.makeImpassable(impassibleTiles);
            _tileManager.CurrentTile = _tileManager.ActiveLayer.Tiles[0, 0];
            // Setup the collision objects for the layer
            setupCollisionMask();
            // Create an enemy object that will rotate towards the player
            //Tile enemyTile = _tileManager.ActiveLayer.Impassable.First();
            setupEnemies(5, tileWidth, tileHeight);
            //showPathTiles(_path);
            createSpawnPositions();
            setupTowers();
            setupFollowers(tileWidth, tileHeight);
            

        }

        public override void Initialize()
        {
            //_tileRenderer = new TileRenderer(this, tileMap, tileWidth, tileHeight);
            cam = new Camera(ViewportCentre,
                        new Vector2(tileMap.GetLength(1) * tileWidth,
                            tileMap.GetLength(0) * tileHeight), GraphicsDevice.Viewport);
            cam.CamPos = Vector2.Zero;
            Game.Services.AddService<Camera>(cam);
            setupPlayer();
            AddPlayer(player);
            new InputEngine(Game);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Tile sheet that all sprites are taken from
            _tileSheet = Game.Content.Load<Texture2D>(@"Tiles\tank tiles 64 x 64");
            Game.Services.AddService(_tileSheet);
            _font = Game.Content.Load<SpriteFont>("message");
            base.LoadContent();
        }

        private void setupTowers()
        {
            foreach (Tile t in _spawnPositions)
            {
                Tower s = new Tower(t,Game.Content.Load<SpriteFont>("DebugFont"),
                    Game.Content.Load<Texture2D>("tower_03"),
                            new Vector2(t.X * t.TileWidth, t.Y * t.TileHeight),
                                new Vector2(t.TileWidth, t.TileHeight));
                s.Visible = true;
                _towers.Add(s);
            }


        }

        private void createSpawnPositions()
        {

            _spawnPositions.Clear();
            // Top left most passable Tile
            _spawnPositions.Add(_tileManager.ActiveLayer.Passable
                                .OrderBy(t => t.X)
                                .OrderBy(t => t.Y).First());
            // Top right most passable tile
            _spawnPositions.Add(_tileManager.ActiveLayer.Passable
                .OrderBy(t => t.Y)
                .OrderByDescending(t => t.X)
                .First());
            // Bottom left most passable tile
            _spawnPositions.Add(_tileManager.ActiveLayer.Passable
                .OrderBy(t => t.X)
                .OrderByDescending(t => t.Y)
                .First());
            // Bottom right most 
            _spawnPositions.Add(_tileManager.ActiveLayer.Passable
                .OrderByDescending(t => t.X)
                .OrderByDescending(t => t.Y)
                .First());

        }

        private void showPathTiles(List<Tile> _path)
        {
            _pathTiles.Clear();
            foreach (Tile t in _path)
            {
                SimpleSprite s = new SimpleSprite(Game.Content.Load<SpriteFont>("DebugFont"),
                    Game.Content.Load<Texture2D>("Collison"),
                            new Vector2(t.X * t.TileWidth, t.Y * t.TileHeight),
                                new Vector2(t.TileWidth, t.TileHeight));
                s.Visible = true;
                _pathTiles.Add(s);
            }
        }

        public void setupFollowers(int tileWidth, int tileHeight)
        {
            _followers.Clear();
            // Create one follower for testing purposes near bottom right tower
                var SpawnTower = _towers.Select(t => new {t.TilePlace.X, t.TilePlace.Y,t.Position, gid = Guid.NewGuid() })
                    .OrderByDescending(t => t.X)
                    .OrderByDescending(t => t.Y)
                    .First();
                 
                _followers.Add(new FollowingEnemy(Game,
                  new Vector2(SpawnTower.X,         // Tile position
                                  SpawnTower.Y),
                                  _tileManager.ActiveLayer.getPassableTileAt((int)SpawnTower.X,
                                  (int)SpawnTower.Y), // Current Tile
                  new List<TileRef>() { // Image reference
                    new TileRef(17,7,0)
                       }, tileWidth, tileHeight, 1.5f));
            loadFollowerProjectiles();
        }

        public void setupEnemies(int EnemyCount, int tileWidth, int tileHeight)
        {
            _enemies.Clear();


            List<Sentry> toDelete = new List<Sentry>();

            foreach (var item in Game.Components)
                if(item.GetType() == typeof(Sentry))
                    toDelete.Add((Sentry)item);

            foreach (var item in toDelete)
                Game.Components.Remove(item);

            // use linq queries on the impassible tiles to choose random locations for enemies
            // First query introduce a random guid into a sub set of locations of impassible tiles
            var enemyPlaces = _tileManager.ActiveLayer.Impassable
                                    .Where(chosen => chosen.TileName == "blue")
                                    .Select(subset => new { subset.X, subset.Y, gid = Guid.NewGuid() });
                                    
            // order by the guid and take count positions
            // NOTE: Linq ensures that there are no duplicate positions produced
            var randomEnemyPlaces = enemyPlaces.Select(all => new { all.gid, all.X, all.Y })
                                        .OrderBy(subgroup => subgroup.gid).Take(EnemyCount)
                                        .ToList();

            // Do a join on the resulting 10 random places and the original impassible tiles 
            // to get the actual tile locations

            List<Tile> enemyPositions = (from enemyPos in _tileManager.ActiveLayer.Impassable
                                         join places in randomEnemyPlaces
                                         on new { enemyPos.X, enemyPos.Y } equals new { places.X, places.Y }
                                         select enemyPos).ToList();
            // The enemies are added as game components so they are in a collection 
            foreach (Tile t in enemyPositions)
            {
                _enemies.Add(new Sentry(Game, new Vector2(t.X, t.Y),
                    new List<TileRef>() {
                    new TileRef(17,7,0)
                         }, tileWidth, tileHeight, 1f));
            }
            loadSentryProjectiles();

        }
        public void loadFollowerProjectiles()
        {
            // Foreach enemy load a projectile
            foreach (var e in _followers)
            {
                Projectile p = new Projectile(Game, e.TilePosition,
                new List<TileRef>() { new TileRef(3,0,0),
                                                new TileRef(4,0,0),
                                                new TileRef(5,0,0),
                                                new TileRef(6,0,0)},
                new List<TileRef>() { new TileRef(0,0,0),
                                                new TileRef(1,0,0),
                                                new TileRef(2,0,0)
                                        },
                e.FrameWidth, e.FrameHeight, 1.5f);
                e.loadProjectile(p);
                e.Health = 100;
                e.Hbar = new Helpers.HealthBar(Game, e.PixelPosition + new Vector2(-10, -20));

            }
        }
        public void loadSentryProjectiles()
        {
            // Foreach enemy load a projectile
            foreach (var e in _enemies)
            {
                Projectile p = new Projectile(Game,e.TilePosition,
                new List<TileRef>() { new TileRef(3,0,0),
                                                new TileRef(4,0,0),
                                                new TileRef(5,0,0),
                                                new TileRef(6,0,0)},
                new List<TileRef>() { new TileRef(0,0,0),
                                                new TileRef(1,0,0),
                                                new TileRef(2,0,0)
                                        },
                e.FrameWidth, e.FrameHeight, 1.5f);
                e.loadProjectile(p);
                e.Health = 100;
                e.Hbar = new Helpers.HealthBar(Game, e.PixelPosition + new Vector2(-10, -20));

            }
        }

        // Creates a simple sprite for all impassible 
        public void setupCollisionMask()
        {
            foreach (Tile t in _tileManager.ActiveLayer.Impassable)
                _collisionSet.Add(new SimpleSprite(Game.Content.Load<SpriteFont>("DebugFont"),
                    Game.Content.Load<Texture2D>("Collison"),
                            new Vector2(t.X * t.TileWidth, t.Y * t.TileHeight),
                                new Vector2(t.TileWidth, t.TileHeight)));
        }

        public override void Update(GameTime gameTime)
        {
            if (player != null)
            {
                // pixelMove is now tileMove
                player.tileMove(_collisionSet);
                _tileManager.CurrentTile = player.CurrentPlayerTile =
                    _tileManager.ActiveLayer.getPassableTileAt((int)Math.Round(player.TilePosition.X),
                                            (int)Math.Round(player.TilePosition.Y));

                foreach (FollowingEnemy _enemy in _followers)
                {
                    player.HitTest(_enemy);
                    _enemy.follow(player);
                    // Test if there is an explosion on the enemy from the player projectile
                    _enemy.HitTest(player);

                }

                var deadFollowers = _followers.Where(s => s.Health <= 0).ToList();

                foreach (FollowingEnemy s in deadFollowers)
                {
                    _followers.Remove(s); // Must remove the from the local collection 
                    Game.Components.Remove(s); // and also from the Game components 
                }


                foreach (var _enemy in _enemies)
                {
                    // Test if there is an explosion on the enemy from the player projectile
                    player.HitTest(_enemy);
                    _enemy.follow(player);
                    // Test if there is an explosion on the enemy from the player projectile
                    _enemy.HitTest(player);
                }
                // remove destroyed enemies
                var dead = _enemies.Where(s => s.Health <= 0).ToList();

                foreach (Sentry s in dead)
                {
                    _enemies.Remove(s); // Must remove the from the local collection 
                    Game.Components.Remove(s); // and also from the Game components 
                }

                // if the player is dead then we just restart
                if (player.Health <= 0)
                { 
                    AddPlayer(player); // resets the player
                    setupEnemies(5, player.FrameWidth, player.FrameHeight); // resets enemies
                }

                foreach (FollowingEnemy _enemy in _followers)
                {
                    _enemy.checkPosition(_tileManager, player.CurrentPlayerTile);
                    //showPathTiles(_enemy.CurrentPath.ToList());
                }

                Camera Cam = Game.Services.GetService<Camera>();
                Cam.follow(player.PixelPosition,
                 GraphicsDevice.Viewport);
            }
            //if (InputEngine.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.R))
            //{
            //    _pathTiles = new List<SimpleSprite>();
            //    Random r = new Random();
            //    Tile start = _spawnPositions[r.Next(3)];
            //    Tile finish = _player.CurrentPlayerTile;
            //    _path = PathFinder.Path(_tileManager,start, finish);
            //    showPathTiles(_path);
            //}
            base.Update(gameTime);
        }

        public Tile RandomPassableTile()
        {
            var passible = _tileManager.ActiveLayer.Passable
                                    .Select(subset => new { subset.X, subset.Y, gid = Guid.NewGuid() });
            // order by the guid and take count positions
            // NOTE: Linq ensures that there are no duplicate positions produced
            var randomPlaces = passible.Select(all => new { all.gid, all.X, all.Y })
                                        .OrderBy(subgroup => subgroup.gid).Take(1)
                                        //.ToList().Select(r => new { r.X, r.Y })
                                        .ToList();

            // Do a join on the resulting 10 random places and the original impassible tiles 
            // to get the actual tile locations

            Tile PassiblePosition = (from rpos in _tileManager.ActiveLayer.Passable
                                     join places in randomPlaces
                                     on new { rpos.X, rpos.Y } equals new { places.X, places.Y }
                                     select rpos).FirstOrDefault();

            return PassiblePosition;
        }

        public void AddPlayer(PlayerWithWeapon p)
        {
            // Add the player and set its current Tile to be the home Tile and it's position to match the home Tile
            player = p;
            player.CurrentPlayerTile = _tileManager.ActiveLayer.Passable.Where(t => t.TileName == "home").FirstOrDefault();
            if (player.CurrentPlayerTile != null)
            {
                _tileManager.CurrentTile = player.CurrentPlayerTile;
                player.TilePosition = new Vector2(player.CurrentPlayerTile.X, player.CurrentPlayerTile.Y);
            }
            p.Health = 100;
            p.Hbar = new Helpers.HealthBar(Game, p.PixelPosition + new Vector2(-10, -10));
            p.Site.TilePosition = p.TilePosition;
                        
            //Tile Finish = _player.CurrentPlayerTile = _tileManager.ActiveLayer.getPassableTileAt((int)_player.Tileposition.X, (int)_player.Tileposition.Y);
            //Tile Start = RandomPassableTile();
            //_path = Path(Start, Finish);
            //showPathTiles(_path);
        }

        private void setupPlayer()
        {
            // initially left facing for proper rotations towards the mouse
            List<TileRef> initialFrames = new List<TileRef>() {
                    new TileRef(16, 2, 0),
                    new TileRef(16, 3, 0),
                    new TileRef(16, 4, 0),
                    new TileRef(16, 5, 0),
                    new TileRef(16, 6, 0),
                    new TileRef(16, 7, 0),
                    new TileRef(16, 8, 0),
        };

            
            player = new PlayerWithWeapon(Game, cam, new Vector2(0, 0), new Vector2(tileMap.GetLength(1), tileMap.GetLength(0)),
                            initialFrames,
                            64, 64, 1.0f); // Default stopped

            player.setFrameSet(DIRECTION.UP,
                                new List<TileRef> {
                            new TileRef(15, 0, 0),
                            new TileRef(16, 0, 0),
                            new TileRef(17, 0, 0),
                            new TileRef(18, 0, 0),
                            new TileRef(19, 0, 0),
                            new TileRef(20, 0, 0),
                            new TileRef(21, 0, 0)
                                });
            player.setFrameSet(DIRECTION.DOWN,
                new List<TileRef> {
                    new TileRef(15, 1, 0),
                    new TileRef(16, 1, 0),
                    new TileRef(17, 1, 0),
                    new TileRef(18, 1, 0),
                    new TileRef(19, 1, 0),
                    new TileRef(20, 1, 0),
                    new TileRef(21, 1, 0),
            });
            player.setFrameSet(DIRECTION.LEFT,
                new List<TileRef> {
                    new TileRef(16, 2, 0),
                    new TileRef(16, 3, 0),
                    new TileRef(16, 4, 0),
                    new TileRef(16, 5, 0),
                    new TileRef(16, 6, 0),
                    new TileRef(16, 7, 0),
                    new TileRef(16, 8, 0),
            });
            player.setFrameSet(DIRECTION.RIGHT,
                new List<TileRef> {
                    new TileRef(15, 2, 0),
                    new TileRef(15, 3, 0),
                    new TileRef(15, 4, 0),
                    new TileRef(15, 5, 0),
                    new TileRef(15, 6, 0),
                    new TileRef(15, 7, 0),
                    new TileRef(15, 8, 0),
            });
            Projectile p = new Projectile(Game, player.PixelPosition,
                new List<TileRef>() { new TileRef(3,0,0),
                                        new TileRef(4,0,0),
                                        new TileRef(5,0,0),
                                        new TileRef(6,0,0)},
                new List<TileRef>() { new TileRef(0,0,0),
                                        new TileRef(1,0,0),
                                        new TileRef(2,0,0)
                                        },
                player.FrameWidth, player.FrameHeight, 1f);
            player.CurrentPlayerTile = TileManager.CurrentTile;
            player.loadProjectile(p);
        }


        public override void Draw(GameTime gameTime)
        {

            SpriteBatch sp = Game.Services.GetService<SpriteBatch>();
            //Texture2D tx = Game.Services.GetService<Texture2D>();
            SpriteFont font = Game.Services.GetService<SpriteFont>();
            Camera Cam = Game.Services.GetService<Camera>();

            sp.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend, null, null, null, null, 
                            Cam.CurrentCameraTranslation);
            // Draw the Tiles
            foreach (Tile t in _tileManager.ActiveLayer.Tiles)
            {
                Vector2 position = new Vector2(t.X * t.TileWidth * _scale, 
                                                    t.Y * t.TileHeight * _scale);
                sp.Draw(_tileSheet,
                    new Rectangle(position.ToPoint(), new Point(t.TileWidth * _scale, t.TileHeight * _scale)),
                    new Rectangle(t.TileRef._sheetPosX * t.TileWidth, t.TileRef._sheetPosY * t.TileHeight,
                                        t.TileWidth * _scale, t.TileHeight * _scale)
                    , Color.White);
            }
            // Draw the player position
            if (player != null)
            {
                sp.DrawString(font,  "Site pos " + player.Site.PixelPosition.ToString(), player.Site.PixelPosition, Color.White);
                //_player.Draw(sp, _tileSheet);

                foreach (var _enemy in _followers)
                {
                    sp.Draw(_txShowRectangle, _enemy.Range, new Color(0, 0, 0, 128));
                    
                }
                //if (_player.MyProjectile != null)
                //    sp.DrawString(font, "ptp " + _player.Tileposition.ToString(), new Vector2(10, 10), Color.White);
                //    sp.DrawString(font, "prtp " + _player.MyProjectile.Tileposition.ToString(), new Vector2(10, 30), Color.White);
                //    sp.DrawString(font, "stp "+ _player.Site.Tileposition.ToString(), new Vector2(10, 60), Color.White);
            }
            foreach (var item in _collisionSet)
                item.draw(sp);
            foreach (var item in _pathTiles)
                item.draw(sp);
            foreach (var tower in _towers)
                tower.draw(sp);
            sp.End();
            base.Draw(gameTime);

            }


    }
}
