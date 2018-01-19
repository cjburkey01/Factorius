using System;
using System.Collections.Generic;

namespace Factorius {
	sealed class TileInstance {

		public Tile Parent { private set; get; }
		public TileData Data { private set; get; }
		public World World { private set; get; }
		public TilePos Location { private set; get; }

		public TileInstance(Tile parent, World world, TilePos pos) {
			Parent = parent;
			Data = new TileData();
			World = world;
			Location = pos;
		}

		public override bool Equals(object obj) {
			var instance = obj as TileInstance;
			return instance != null &&
				   EqualityComparer<Tile>.Default.Equals(Parent, instance.Parent) &&
				   EqualityComparer<TileData>.Default.Equals(Data, instance.Data) &&
				   EqualityComparer<World>.Default.Equals(World, instance.World) &&
				   EqualityComparer<TilePos>.Default.Equals(Location, instance.Location);
		}

		public override int GetHashCode() {
			var result = -929053957;
			result = result * -1521134295 + EqualityComparer<Tile>.Default.GetHashCode(Parent);
			result = result * -1521134295 + EqualityComparer<TileData>.Default.GetHashCode(Data);
			result = result * -1521134295 + EqualityComparer<World>.Default.GetHashCode(World);
			result = result * -1521134295 + EqualityComparer<TilePos>.Default.GetHashCode(Location);
			return result;
		}
	}

	class TileData {

		private readonly Dictionary<string, object> data = new Dictionary<string, object>();

		public void Set(string key, object value) {
			if (data.ContainsKey(key)) {
				data[key] = value;
			} else {
				data.Add(key, value);
			}
		}

		public Type GetType(string key) {
			object obj = Get<object>(key);
			if (obj is null) {
				return null;
			}
			return obj.GetType();
		}

		public bool Get<T>(string key, out T found) {
			found = default(T);
			bool i = data.TryGetValue(key, out object f);
			if (!i) {
				return false;
			}
			if (f is T) {
				found = (T) f;
				return true;
			}
			return false;
		}

		public T Get<T>(string key) {
			bool isIn = Get<T>(key, out T found);
			if (!isIn) {
				return default(T);
			}
			return found;
		}

	}
}