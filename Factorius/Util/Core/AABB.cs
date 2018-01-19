using System;
using System.Collections.Generic;

namespace Factorius {
	struct AABB {

		public float xmin;
		public float xmax;
		public float ymin;
		public float ymax;

		public AABB(float x, float y, float width, float height) {
			this.xmin = x;
			this.xmax = x + width;
			this.ymin = y;
			this.ymax = y + height;
		}

		public AABB(TilePos pos, float width, float height) {
			this.xmin = pos.x;
			this.xmax = pos.x + width;
			this.ymin = pos.y;
			this.ymax = pos.y + height;
		}

		public bool CollidingWith(AABB other) {
			return xmin < other.xmax && xmax > other.xmin && ymin < other.ymax && ymax > other.ymin;
		}

		public TilePos GetPos() {
			return new TilePos((int) Math.Floor(xmin), (int) Math.Floor(ymin));
		}

		public TilePos[] GetIncludedTiles() {
			List<TilePos> tiles = new List<TilePos>();
			TilePos at = GetPos();
			for (int y = at.y; y < ymax; y++) {
				for (int x = at.x; x < xmax; x++) {
					tiles.Add(new TilePos(x, y));
				}
			}
			return tiles.ToArray();
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(obj, this)) {
				return true;
			}
			if (obj is null) {
				return false;
			}
			if (!(obj is AABB)) {
				return false;
			}
			var aABB = (AABB) obj;
			return xmin == aABB.xmin && xmax == aABB.xmax && ymin == aABB.ymin && ymax == aABB.ymax;
		}

		public override int GetHashCode() {
			var result = 1130056162;
			result = result * -1521134295 + base.GetHashCode();
			result = result * -1521134295 + xmin.GetHashCode();
			result = result * -1521134295 + xmax.GetHashCode();
			result = result * -1521134295 + ymin.GetHashCode();
			result = result * -1521134295 + ymax.GetHashCode();
			return result;
		}
	}
}