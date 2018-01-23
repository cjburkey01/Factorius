using System;
using System.Collections.Generic;
using OpenTK;

namespace Factorius {
	class TileWorld {

		public static readonly int LOAD_RADIUS;

		private readonly Dictionary<TilePos, TileChunk> generatedChunks = new Dictionary<TilePos, TileChunk>();
		private readonly List<TilePos> loadedChunks = new List<TilePos>();
		private readonly List<TilePos> renderedChunks = new List<TilePos>();
		private readonly List<GameObject> chunkLoaders = new List<GameObject>();

		public void OnUpdate(double delta) {
			foreach (GameObject loader in chunkLoaders) {
				LoadAround(loader.transform.position);
			}
		}

		public void OnRender(double delta) {

		}

		private static float DistSq(Vector2 a, Vector2 b) {
			return (float) (Math.Pow((b.X - a.X), 2) + Math.Pow((b.Y - a.Y), 2));
		}

		private static void LoadAround(Vector3 pos) {
			int r = LOAD_RADIUS;
			int hr = (int) Math.Floor(LOAD_RADIUS / 2.0f);
			int rsq = r * r;
		}

	}
}
