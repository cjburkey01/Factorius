namespace Factorius {
	class Tile {

		public Resource ResourceLocation { private set; get; }

		public Tile(Resource res) {
			ResourceLocation = res;
		}

		public virtual void OnUpdate(double delta, TileInstance self) { }
		public virtual void OnRender(double delta, TileInstance self) { }
		public virtual void OnNeighborChange(TileInstance self, TileInstance neighbor) { }
		public virtual void DrawSelf(TileInstance self) {

		}
		public virtual AABB GetBoundingBox(TileInstance self) {
			return new AABB(self.Location, 1.0f, 1.0f);
		}

	}

	struct TilePos {

		public readonly int x;
		public readonly int y;

		public TilePos(int x, int y) {
			this.x = x;
			this.y = y;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(obj, this)) {
				return true;
			}
			if (obj is null) {
				return false;
			}
			if (!(obj is TilePos)) {
				return false;
			}
			TilePos pos = (TilePos) obj;
			return pos.x == x && pos.y == y;
		}

		public override int GetHashCode() {
			var result = 1502939027;
			result = result * -1521134295 + base.GetHashCode();
			result = result * -1521134295 + x.GetHashCode();
			result = result * -1521134295 + y.GetHashCode();
			return result;
		}
	}
}
