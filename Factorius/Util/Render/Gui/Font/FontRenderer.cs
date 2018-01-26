using System;
using System.Collections.Generic;
using SharpFont;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class FontRenderer {

		public static int clamp = (int) TextureWrapMode.ClampToEdge;
		public static int linearMin = (int) TextureMinFilter.Linear;
		public static int linearMag = (int) TextureMagFilter.Linear;

		public bool IsInit { private set; get; }

		private int atlasWidth;
		private int atlasHeight;
		private uint fontSize;
		
		private ShaderProgram fontShader;
		private int fontTexture;

		private Library library;
		private Face face;

		private CharacterInfo[] c = new CharacterInfo[128 - 32];

		private readonly List<RenderedText> rendered = new List<RenderedText>();

		/// <summary>
		///		Initializes the font renderer with the specified font resource.
		/// </summary>
		/// <param name="path">The resource from which to load the TTF font file.</param>
		public void SetFont(Resource path, uint size) {
			if (IsInit) {
				Console.WriteLine("Reloading font manager");
				IsInit = false;
			}
			library = new Library();
			
			Console.WriteLine("Initializing font renderer with font: " + path);
			face = new Face(library, path.GetFullPath() + ".ttf");
			face.SetPixelSizes(0, size);
			fontSize = size;

			// Initialize the shaders for text rendering.
			fontShader = new ShaderProgram();
			fontShader.AddShaders(new Resource("Factorius", "Shader/Font"));
			if (!fontShader.Link()) {
				Console.WriteLine("Failed to link font shader.");
				return;
			}
			fontShader.InitUniform("screenSize");
			fontShader.InitUniform("fontSize");
			fontShader.InitUniform("fontColor");

			// Calculate maximum size of font texture atlas.
			for (uint i = 32; i < 128; i ++) {
				face.LoadChar(i, LoadFlags.Render, LoadTarget.Normal);
				atlasWidth += face.Glyph.Bitmap.Width;
				atlasHeight = Math.Max(atlasHeight, face.Glyph.Bitmap.Rows);
			}

			fontShader.SetUniform("fontSize", size);

			Console.WriteLine("Created ASCII texture atlas with size: " + atlasWidth + "x" + atlasHeight);

			// Create the font texture atlas texture for the GPU.
			fontTexture = GL.GenTexture();
			Bind();
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.R8, atlasWidth, atlasHeight, 0, PixelFormat.Red, PixelType.UnsignedByte, (IntPtr) 0);

			// Load the images into the empty font texture atlas.
			int x = 0;
			for (uint i = 32; i < 128; i++) {
				face.LoadChar(i, LoadFlags.Render, LoadTarget.Normal);
				GlyphSlot g = face.Glyph;
				GL.TexSubImage2D(TextureTarget.Texture2D, 0, x, 0, g.Bitmap.Width, g.Bitmap.Rows, PixelFormat.Red, PixelType.UnsignedByte, g.Bitmap.Buffer);
				
				c[i - 32] = new CharacterInfo((char) i, (float) g.Advance.X.ToDouble(), (float) g.Advance.Y.ToDouble(), g.Bitmap.Width, g.Bitmap.Rows, g.BitmapLeft, g.BitmapTop, (float) x / atlasWidth);
				x += g.Bitmap.Width;
			}

			IsInit = true;
		}

		private CharacterInfo GetInfo(char cs) {
			foreach (CharacterInfo i in c) {
				if (i.character.Equals(cs)) {
					return i;
				}
			}
			return default(CharacterInfo);
		}

		public RenderedText AddText(string text, Vector2 pos, Vector4 color) {
			if (!IsInit) {
				Console.WriteLine("The font renderer has not yet been initialized.");
				return null;
			}

			//pos.Y = Launch.Instance.Height - pos.Y - fontSize;
			Vector2 startPos = new Vector2(pos.X, pos.Y);

			Bind();
			char[] chars = text.ToCharArray();
			List<Mesh> meshes = new List<Mesh>();
			for (int i = 0; i < chars.Length; i ++) {
				face.LoadChar(chars[i], LoadFlags.Render, LoadTarget.Normal);
				CharacterInfo info = GetInfo(chars[i]);

				float x = pos.X + info.bl;
				float y = Launch.Instance.Height - pos.Y - fontSize + info.bt;
				float w = info.bw;
				float h = info.bh;

				pos.X += info.ax;
				pos.Y += info.ay;

				if (w == 0.0f || h == 0.0f) {
					continue;
				}

				//Console.WriteLine("Pos: " + x + ", " + y + " size: " + w + ", " + h + " = " + chars[i]);
				Vector3[] verts = new Vector3[] {
					new Vector3(x, y, 0.0f),			// 0
					new Vector3(x, y - h, 0.0f),		// 1
					new Vector3(x + w, y - h, 0.0f),	// 2
					new Vector3(x + w, y, 0.0f),		// 3
				};
				Vector2[] uvs = new Vector2[] {
					new Vector2(info.tx, 0.0f),
					new Vector2(info.tx, info.bh / atlasHeight),
					new Vector2(info.tx + info.bw / atlasWidth, info.bh / atlasHeight),
					new Vector2(info.tx + info.bw / atlasWidth, 0.0f),
				};
				int[] tris = new int[] {
					2, 0, 1,
					0, 2, 3,
				};
				Mesh mesh = new Mesh();
				mesh.SetMesh(verts, tris, uvs);
				meshes.Add(mesh);
			}

			RenderedText txt = new RenderedText(meshes.ToArray(), chars, startPos, color);
			rendered.Add(txt);

			return txt;
		}

		public void Render(Vector2 screenSize) {
			if (!IsInit) {
				return;
			}
			GL.Clear(ClearBufferMask.DepthBufferBit);
			fontShader.Use();
			fontShader.SetUniform("screenSize", screenSize);
			Bind();
			foreach (RenderedText txt in rendered) {
				fontShader.SetUniform("fontColor", txt.color);
				foreach (Mesh mesh in txt.renderedChars) {
					mesh.Render();
				}
			}
		}

		private void Bind() {
			GL.BindTexture(TextureTarget.Texture2D, fontTexture);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clamp);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clamp);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linearMin);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linearMag);
		}

		public bool RemoveText(RenderedText txt) {
			if (!IsInit) {
				Console.WriteLine("The font renderer has not yet been initialized.");
				return false;
			}
			if (rendered.Contains(txt)) {
				foreach (Mesh mesh in txt.renderedChars) {
					mesh.DestroyMesh();
				}
				rendered.Remove(txt);
				return true;
			}
			return false;
		}

		public void OnExit() {
			Console.WriteLine("Cleaning up text renderer.");
			RenderedText[] txt = rendered.ToArray();
			foreach (RenderedText t in txt) {
				RemoveText(t);
				Console.WriteLine("Clearing text at: " + t.pos);
			}
		}

	}

	class RenderedText {

		public readonly Mesh[] renderedChars;
		public readonly char[] text;
		public readonly Vector2 pos;
		public readonly Vector4 color;

		public RenderedText(Mesh[] renderedChars, char[] text, Vector2 pos,  Vector4 color) {
			this.renderedChars = renderedChars;
			this.text = text;
			this.pos = new Vector2(pos.X, pos.Y);
			this.color = color;
		}

		public override bool Equals(object obj) {
			var txt = obj as RenderedText;
			return txt != null && renderedChars.Equals(txt.renderedChars) && text.Equals(txt.text) && pos.Equals(txt.pos) && color.Equals(txt.color);
		}

		public override int GetHashCode() {
			var hashCode = -1112493214;
			hashCode = hashCode * -1521134295 + EqualityComparer<Mesh[]>.Default.GetHashCode(renderedChars);
			hashCode = hashCode * -1521134295 + EqualityComparer<char[]>.Default.GetHashCode(text);
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(pos);
			hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(color);
			return hashCode;
		}

	}

	struct CharacterInfo {

		public readonly bool real;

		public readonly char character;

		// Advance
		public readonly float ax;
		public readonly float ay;

		// Bitmap size
		public readonly float bw;
		public readonly float bh;

		// Bitmap position
		public readonly float bl;
		public readonly float bt;

		// Offset of X coord in font texture atlas
		public readonly float tx;

		public CharacterInfo(char c, float ax, float ay, float bw, float bh, float bl, float bt, float tx) {
			real = true;
			this.character = c;
			this.ax = ax;
			this.ay = ay;
			this.bw = bw;
			this.bh = bh;
			this.bl = bl;
			this.bt = bt;
			this.tx = tx;
		}

		public override string ToString() {
			return real + ": " + character + " at " + ax + ", " + ay + ". " + bw + "x" + bh;
		}

	}

}