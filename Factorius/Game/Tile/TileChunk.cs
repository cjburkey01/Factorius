namespace Factorius {
	class TileChunk {

		public static readonly int CHUNK_SIZE;

		public TilePos Chunk { private set; get; }
		public World World { private set; get; }
		private TileInstance[,] tiles;

		public TileChunk(TilePos chunkPos, World world) {
			Chunk = chunkPos;
			World = world;
			tiles = new TileInstance[CHUNK_SIZE, CHUNK_SIZE];
		}

		/// <summary>
		///		Retrieves the tile at the specified local-chunk coordinate position.
		/// </summary>
		/// <param name="pos">The position relative to the chunk.</param>
		/// <returns>The tile at the position, or null if it doesn't exist.</returns>
		public TileInstance GetTile(TilePos pos) {
			if (!InChunk(pos)) {
				return null;
			}
			return tiles[pos.x, pos.y];
		}
		
		/// <summary>
		///		Sets the tile at the position in the chunk to the specified
		///		instance.
		/// </summary>
		/// <param name="pos">The position inside of the chunk.</param>
		/// <param name="tile">The tile to be placed at the position.</param>
		/// <returns>The tile set at the position, or null upon failure.</returns>
		public TileInstance SetTile(TilePos pos, Tile tile) {
			if (!InChunk(pos)) {
				return null;
			}
			TileInstance inst = new TileInstance(tile, World, pos);
			tiles[pos.x, pos.y] = inst;
			return inst;
		}

		/// <summary>
		///		Calculates the world position from the supplied local-chunk coordinate.
		/// </summary>
		/// <param name="pos">The coordinate inside of the chunk.</param>
		/// <returns>The position of the tile in the world.</returns>
		public TilePos GetWorldPos(TilePos pos) {
			return new TilePos(Chunk.x * CHUNK_SIZE + pos.x, Chunk.y * CHUNK_SIZE + pos.y);
		}

		/// <summary>
		///		Checks whether or not the position exists inside of this chunk.
		/// </summary>
		/// <param name="pos">The position inside the chunk.</param>
		/// <returns>Whether or not the position is out of this chunk's bounds.</returns>
		public bool InChunk(TilePos pos) {
			return pos.x >= 0 && pos.y >= 0 && pos.x < CHUNK_SIZE && pos.y < CHUNK_SIZE;
		}

	}
}
