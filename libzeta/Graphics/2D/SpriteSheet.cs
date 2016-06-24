using System;
namespace libzeta {

    /// <summary>
    /// Sprite sheet.
    /// </summary>
    public class SpriteSheet {

        /// <summary>
        /// The number of tiles on the x axis.
        /// </summary>
        readonly int tilesX;

        /// <summary>
        /// The number of tiles on the y axis.
        /// </summary>
        readonly int tilesY;

        /// <summary>
        /// The width of a tile.
        /// </summary>
        readonly int tileWidth;

        /// <summary>
        /// The height of a tile.
        /// </summary>
        readonly int tileHeight;

        /// <summary>
        /// The texture.
        /// </summary>
        readonly Texture2D texture;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width => texture.Width;

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height => texture.Height;

        /// <summary>
        /// Gets the width of a tile.
        /// </summary>
        /// <value>The width of the tile.</value>
        public int TileWidth => tileWidth;

        /// <summary>
        /// Gets the height of a tile.
        /// </summary>
        /// <value>The height of the tile.</value>
        public int TileHeight => tileHeight;

        /// <summary>
        /// Gets the number of tiles on the x axis.
        /// </summary>
        /// <value>The number of tiles on the x axis.</value>
        public int TilesX => tilesX;

        /// <summary>
        /// Gets the number of tiles on the y axis.
        /// </summary>
        /// <value>The number of tiles on the y axis.</value>
        public int TilesY => tilesY;

        /// <summary>
        /// Gets the position of the tile with the specified index.
        /// </summary>
        /// <param name="i">The index.</param>
        public Sprite this [int i] => GetTileAt (i);

        /// <summary>
        /// Gets the position of the tile with the specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Sprite this [int x, int y] => GetTileAt (x, y);

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.SpriteSheet"/> class.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="tileSize">Tile size in px.</param>
        public SpriteSheet (Texture2D texture, int tileSize) {
            this.texture = texture;
            tileWidth = tileSize;
            tileHeight = tileSize;
            tilesX = texture.Width / tileSize;
            tilesY = texture.Height / tileSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:libzeta.SpriteSheet"/> class.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="tileWidth">Tile width.</param>
        /// <param name="tileHeight">Tile height.</param>
        public SpriteSheet (Texture2D texture, int tileWidth, int tileHeight) {
            this.texture = texture;
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
            tilesX = texture.Width / tileWidth;
            tilesY = texture.Height / tileHeight;
        }

        /// <summary>
        /// Gets the tile at the specified coordinates.
        /// </summary>
        /// <returns>The <see cref="T:libzeta.Rectangle"/>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        Sprite GetTileAt (int x, int y) {
            var tx = x * tileWidth;
            var ty = y * tileHeight;
            return new Rectangle (tx, ty, tileWidth, tileHeight);
        }

        /// <summary>
        /// Gets the tile at the specified index.
        /// </summary>
        /// <returns>The <see cref="T:libzeta.Rectangle"/>.</returns>
        /// <param name="i">The index.</param>
        Sprite GetTileAt (int i) {
            var x = i % tilesX;
            var y = i / tilesX;
            return GetTileAt (x, y);
        }
    }
}

