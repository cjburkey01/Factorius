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

		private ShaderProgram fontShader;
		private int fontTexture;
		private Mesh mesh;

		private Library library;
		private readonly List<Face> fonts = new List<Face>();

		public void Init(Resource path) {
			if (IsInit) {
				Console.WriteLine("This font renderer has already been initialized");
			}
			library = new Library();

			Console.WriteLine("Initializing font renderer with font: " + path);
			fonts.Add(new Face(library, path.GetFullPath() + ".ttf"));

			fontShader = new ShaderProgram();
			fontShader.AddShaders(new Resource("Factorius", "Shader/Font"));
			if (!fontShader.Link()) {
				Console.WriteLine("Failed to link font shader.");
				return;
			}
			fontShader.InitUniform("fontColor");

			fontTexture = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, fontTexture);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref clamp);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref clamp);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref linearMin);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref linearMag);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

			IsInit = true;
		}

		public void RenderText(string text, Vector2 pos, Vector2 scale) {

		}

	}
}