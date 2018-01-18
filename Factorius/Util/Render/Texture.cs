using System;
using SixLabors.ImageSharp;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	abstract class Texture {

		public static int repeat = (int) TextureWrapMode.Repeat;
		public static int nearestMin = (int) TextureMinFilter.NearestMipmapNearest;
		public static int nearestMag = (int) TextureMagFilter.Nearest;

		public bool IsLoaded { private set; get; }

		protected Image<Rgba32> img;
		private int texture;

		public virtual bool Load() {
			if (IsLoaded) {
				Console.WriteLine("Image is already loaded.");
				return false;
			}
			if (img == null) {
				Console.WriteLine("Loaded image was null.");
				return false;
			}
			byte[] data = img.SavePixelData();
			texture = GL.GenTexture();
			Bind();
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref repeat);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref repeat);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref nearestMin);
			GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref nearestMag);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);
			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
			IsLoaded = true;
			return true;
		}

		public void Bind() {
			GL.BindTexture(TextureTarget.Texture2D, texture);
		}

		public int GetId() {
			return (IsLoaded) ? texture : -1;
		}

		public void Destroy() {
			GL.DeleteTexture(texture);
			IsLoaded = false;
		}

	}

	class TextureFile : Texture {

		private readonly string path;

		public TextureFile(string path) {
			this.path = path;
		}

		public override bool Load() {
			img = Image.Load(path);
			if (img == null) {
				Console.WriteLine("Image: " + path + " not found.");
				return false;
			}
			return base.Load();
		}

	}

	class TextureRaw : Texture {

		public TextureRaw(Image<Rgba32> img) {
			this.img = img;
		}

	}
}