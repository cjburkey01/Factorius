using System;
using System.Collections.Generic;
using OpenTK;
using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Factorius {
	class AtlasHandler {

		public static readonly int ATLAS_WIDTH = 512;
		public static readonly int TEXTURE_SIZE = 32;
		public static readonly int TEXTURES_PER_ROW = ATLAS_WIDTH / TEXTURE_SIZE;
		public static readonly float UV_SIZE = 1.0f / TEXTURES_PER_ROW;
		public static readonly int TOTAL_TEXTURES = TEXTURES_PER_ROW * TEXTURES_PER_ROW;

		public static readonly Image<Rgba32> ERROR = Helper.LoadImg(new Resource("Factorius", "Sprite/Raw/Error"));

		private static Dictionary<int, SpriteAtlas> atlases = new Dictionary<int, SpriteAtlas>(); // Split up just in case.

		public static AtlasPos AddTexture(Resource res) {
			int current = GetNextAtlas();
			if (!atlases.ContainsKey(current)) {
				atlases.Add(current, new SpriteAtlas(current));
			}
			return atlases[current].AddImage(res);
		}

		public static bool BakeTextures() {
			Console.WriteLine("Baking atlas textures.");
			foreach (KeyValuePair<int, SpriteAtlas> atlas in atlases) {
				if (!atlas.Value.Bake()) {
					Console.WriteLine("Failed to bake atlas: " + atlas.Key);
					return false;
				}
			}
			return true;
		}

		public static AtlasPos GetTexture(Resource res) {
			foreach (SpriteAtlas atlas in atlases.Values) {
				if (atlas.ContainsTexture(res)) {
					return atlas.GetTexturePos(res);
				}
			}
			return AtlasPos.ERROR;
		}

		private static int GetNextAtlas() {
			int at = atlases.Count;
			if (at < 1) {
				return 0;
			}
			if (!atlases[at - 1].Full) {
				return at - 1;
			}
			return at;
		}

	}

	class SpriteAtlas {

		public bool Full { get { return atlas.Count >= AtlasHandler.TOTAL_TEXTURES; } }
		public bool Baked { private set; get; }

		private int id;
		private Texture texture;
		private Dictionary<Resource, AtlasPos> atlas = new Dictionary<Resource, AtlasPos>();
		private Dictionary<AtlasPos, Image<Rgba32>> tmp = new Dictionary<AtlasPos, Image<Rgba32>>();

		public SpriteAtlas(int id) {
			this.id = id;
		}

		public bool Bake() {
			Image<Rgba32> atlas = Helper.LoadImg(new Resource("Factorius", "Sprite/Raw/TextureAtlas"));
			if (atlas == null) {
				Console.WriteLine("Failed to open texture atlas base image.");
				return false;
			}
			foreach (KeyValuePair<AtlasPos, Image<Rgba32>> kvp in tmp) {
				Console.WriteLine("Baking texture: " + GetAt(kvp.Key));
				atlas.Mutate(a => a.DrawImage(kvp.Value, 1.0f, new Size(AtlasHandler.TEXTURE_SIZE, AtlasHandler.TEXTURE_SIZE), kvp.Key.GetPoint()));
			}
			tmp.Clear();
			atlas.Mutate(a => a.Flip(FlipType.Vertical));
			atlas.Mutate(a => a.Flip(FlipType.Horizontal));
			texture = new TextureRaw(atlas);
			if (!texture.Load()) {
				Console.WriteLine("Failed to upload texture atlas to GPU: " + id);
				return false;
			}
			Baked = true;
			return true;
		}

		public Resource GetAt(AtlasPos pos) {
			foreach (KeyValuePair<Resource, AtlasPos> at in atlas) {
				if (at.Value.Equals(pos)) {
					return at.Key;
				}
			}
			return default(Resource);
		}

		public AtlasPos AddImage(Resource res) {
			if (ContainsTexture(res)) {
				Console.WriteLine("Atlas already contains image: " + res);
				return AtlasPos.ERROR;
			}
			if (Full) {
				Console.WriteLine("Atlas is full: " + id);
				return AtlasPos.ERROR;
			}
			Image<Rgba32> img = Helper.LoadImg(res);
			if (img == null) {
				Console.WriteLine("Image failed to load: " + res);
				img = AtlasHandler.ERROR;
			}
			if (img.Width != 32 || img.Height != 32) {
				Console.WriteLine("Texture must be exactly 32x32 pixels.");
				return AtlasPos.ERROR;
			}
			AtlasPos pos = GetNextFreePos();
			if (pos.Equals(AtlasPos.ERROR)) {
				return AtlasPos.ERROR;
			}
			atlas.Add(res, pos);
			tmp.Add(pos, img);
			return pos;
		}

		public Texture GetTexture() {
			return texture;
		}

		public bool ContainsTexture(Resource res) {
			return atlas.ContainsKey(res);
		}

		public AtlasPos GetTexturePos(Resource res) {
			if (!ContainsTexture(res)) {
				return AtlasPos.ERROR;
			}
			return atlas[res];
		}

		private AtlasPos GetNextFreePos() {
			if (Full) {
				return AtlasPos.ERROR;
			}
			for (int y = 0; y < AtlasHandler.TEXTURES_PER_ROW; y++) {
				for (int x = 0; x < AtlasHandler.TEXTURES_PER_ROW; x++) {
					AtlasPos pos = new AtlasPos(id, x, y);
					if (!atlas.ContainsValue(pos)) {
						return pos;
					}
				}
			}
			return AtlasPos.ERROR;
		}

		public void Destroy() {
			if (Baked) {
				texture.Destroy();
			}
		}

	}

	struct AtlasPos {

		public static readonly AtlasPos ERROR = new AtlasPos(-1, -1, -1);   // If nothing was found, this is returned.

		public readonly int atlas;
		public readonly int x;
		public readonly int y;

		public AtlasPos(int atlas, int x, int y) {
			this.atlas = atlas;
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
			if (!(obj is AtlasPos)) {
				return false;
			}
			AtlasPos pos = (AtlasPos) obj;
			return pos.atlas == atlas && pos.x == x && pos.y == y;
		}

		public override int GetHashCode() {
			int result = 31 * 37;
			result += result * 17 * atlas;
			result += result * 17 * x;
			result += result * 17 * y;
			return result;
		}

		public Vector2 GetPixelPos() {
			return new Vector2(x * AtlasHandler.TEXTURE_SIZE, y * AtlasHandler.TEXTURE_SIZE);
		}

		public Vector2 GetUv() {
			return new Vector2(AtlasHandler.UV_SIZE * x, AtlasHandler.UV_SIZE * (AtlasHandler.TEXTURES_PER_ROW - y));
		}

		public Point GetPoint() {
			Vector2 tmp = GetPixelPos();
			return new Point((int) tmp.X, (int) tmp.Y);
		}

		public bool IsErr() {
			return Equals(ERROR);
		}

		public override string ToString() {
			return atlas + ": " + x + ", " + y;
		}

	}
}
