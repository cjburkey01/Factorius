using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class GuiHandler {

		private static Matrix4 projectionMatrix;
		private static Matrix4 posMatrix;

		public bool IsInit { private set; get; }
		public bool IsMouseOverElement { private set; get; }
		private readonly Dictionary<GuiElement, Mesh> elements = new Dictionary<GuiElement, Mesh>();
		
		private ShaderProgram guiShader;

		public Texture DefStateTexture { private set; get; }
		public Texture DownStateTexture { private set; get; }

		public void Init(Resource gui) {
			if (IsInit) {
				Console.WriteLine("GuiHandler is already initialized.");
				return;
			}
			Console.WriteLine("Initializing GuiHandler.");

			guiShader = new ShaderProgram();
			guiShader.AddShaders(new Resource("Factorius", "Shader/GuiBasic"));
			if (!guiShader.Link()) {
				Console.WriteLine("Failed to link gui shader program.");
				return;
			}
			guiShader.InitUniform("projectionMatrix");
			guiShader.InitUniform("posMatrix");
			Console.WriteLine("GUI shader initiated.");

			elements.Clear();

			DefStateTexture = new TextureFile(new Resource("Factorius", "Sprite/Gui/GuiBack.png").GetFullPath());
			DownStateTexture = new TextureFile(new Resource("Factorius", "Sprite/Gui/GuiBackDown.png").GetFullPath());

			if (!DefStateTexture.Load(false, true)) {
				Console.WriteLine("Default gui state texture not loaded.");
			}
			if (!DownStateTexture.Load(false, true)) {
				Console.WriteLine("Down gui state texture not loaded.");
			}

			IsInit = true;
		}

		public void AddElement(GuiElement element) {
			if (!IsInit) {
				Console.WriteLine("The GuiHandler has not yet been initialized.");
				return;
			}
			if (elements.ContainsKey(element)) {
				Console.WriteLine("That element already exists in the GuiHandler.");
				return;
			}
			element.SetGuiHandler(this);
			List<Vector3> verts = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> tris = new List<int>();
			element.OnAdd(verts, uvs, tris);
			Mesh mesh = new Mesh();
			mesh.SetMesh(verts.ToArray(), tris.ToArray(), uvs.ToArray());
			elements.Add(element, mesh);
		}

		public void RedrawElement(GuiElement element) {
			if (!IsInit) {
				Console.WriteLine("The GuiHandler has not yet been initialized.");
				return;
			}
			if (!elements.ContainsKey(element)) {
				Console.WriteLine("The GuiHandler does not have an entry for that element.");
				return;
			}
			List<Vector3> verts = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> tris = new List<int>();
			element.OnAdd(verts, uvs, tris);
			elements[element].SetMesh(verts.ToArray(), tris.ToArray(), uvs.ToArray());
		}

		public void RemoveElement(GuiElement element) {
			if (!IsInit) {
				Console.WriteLine("The GuiHandler has not yet been initialized.");
				return;
			}
			if (!elements.ContainsKey(element)) {
				Console.WriteLine("The GuiHandler does not have an entry for that element.");
				return;
			}
			element.OnRemove();
			elements[element].DestroyMesh();
			elements.Remove(element);
		}

		public void OnRender(double delta) {
			IsMouseOverElement = false;
			guiShader.Use();
			DefStateTexture.Bind();
			guiShader.SetUniform("projectionMatrix", GetProjectionMatrix());
			foreach (KeyValuePair<GuiElement, Mesh> e in elements) {
				guiShader.SetUniform("posMatrix", GetPosMatrix(e.Key.GlobalPosition));
				if (e.Key is IGuiHoverable el) {
					Vector2 mouse = Input.GetMousePos();
					Vector2 pos = e.Key.GlobalPosition;
					Vector2 size = el.GetSize();
					if (mouse.X >= pos.X && mouse.X <= pos.X + size.X && mouse.Y >= pos.Y && mouse.Y <= pos.Y + size.Y) {
						IsMouseOverElement = true;
						el.OnHovered(mouse);
						if (e.Key is IGuiClickable ec) {
							if (Input.IsMouseFirstDown(MouseButton.Button1)) {
								ec.OnMouseDown(mouse);
							} else if(Input.IsMouseFirstUp(MouseButton.Button1)) {
								ec.OnMouseUp(mouse);
							}
						}
					}
				}
				e.Key.OnDraw(delta);
				e.Value.Render();
				DefStateTexture.Bind();
			}
			GL.UseProgram(0);
		}

		private static Matrix4 GetProjectionMatrix() {
			projectionMatrix = Matrix4.CreateOrthographicOffCenter(0.0f, Launch.Instance.Width, Launch.Instance.Height, 0.0f, 0.001f, 100.0f);
			return projectionMatrix;
		}

		private static Matrix4 GetPosMatrix(Vector2 pos) {
			posMatrix = Matrix4.CreateTranslation(new Vector3(pos.X, pos.Y, 0.0f));
			return posMatrix;
		}

		public static void GenerateBackground(GuiElement e, List<Vector3> verts, List<Vector2> uvs, List<int> tris, Vector2 s) {
			Vector2 size = new Vector2((s.X < 64) ? 64 : s.X, (s.Y < 64) ? 64 : s.Y);

			Vector2 down = new Vector2(0.0f, -0.25f);
			Vector2 right = new Vector2(32.0f / 128.0f, 0.0f);
			Vector2 left = -right;
			Vector2 up = -down;

			Vector2 topLeft = new Vector2(0.0f, 1.0f);
			Vector2 bottomLeft = new Vector2(0.0f, 32.0f / 128.0f);
			Vector2 bottomRight = new Vector2(96.0f / 128.0f, 32.0f / 128.0f);
			Vector2 topRight = new Vector2(96.0f / 128.0f, 1.0f);

			int i = verts.Count;

			verts.AddRange(new Vector3[] {
				// Top Left
				new Vector3(0.0f, 0.0f, 0.0f),		// 0
				new Vector3(0.0f, 32.0f, 0.0f),		// 1
				new Vector3(32.0f, 32.0f, 0.0f),	// 2
				new Vector3(32.0f, 0.0f, 0.0f),		// 3

				// Bottom Left
				new Vector3(0.0f, size.Y - 32.0f, 0.0f),	// 0
				new Vector3(0.0f, size.Y, 0.0f),			// 1
				new Vector3(32.0f, size.Y, 0.0f),			// 2
				new Vector3(32.0f, size.Y - 32.0f, 0.0f),	// 3

				// Bottom Right
				new Vector3(size.X - 32.0f, size.Y - 32.0f, 0.0f),	// 0
				new Vector3(size.X - 32.0f, size.Y, 0.0f),			// 1
				new Vector3(size.X, size.Y, 0.0f),					// 2
				new Vector3(size.X, size.Y - 32.0f, 0.0f),			// 3

				// Top Right
				new Vector3(size.X - 32.0f, 0.0f, 0.0f),	// 0
				new Vector3(size.X - 32.0f, 32.0f, 0.0f),	// 1
				new Vector3(size.X, 32.0f, 0.0f),			// 2
				new Vector3(size.X, 0.0f, 0.0f),			// 3
				
				// Middle Left
				new Vector3(0.0f, 32.0f, 0.0f),				// 0
				new Vector3(0.0f, size.Y - 32.0f, 0.0f),	// 1
				new Vector3(32.0f, size.Y - 32.0f, 0.0f),	// 2
				new Vector3(32.0f, 32.0f, 0.0f),			// 3
				
				// Bottom Middle
				new Vector3(32.0f, size.Y - 32.0f, 0.0f),			// 0
				new Vector3(32.0f, size.Y, 0.0f),					// 1
				new Vector3(size.X - 32.0f, size.Y, 0.0f),			// 2
				new Vector3(size.X - 32.0f, size.Y - 32.0f, 0.0f),	// 3

				// Middle Right
				new Vector3(size.X - 32.0f, 32.0f, 0.0f),			// 0
				new Vector3(size.X - 32.0f, size.Y - 32.0f, 0.0f),	// 1
				new Vector3(size.X, size.Y - 32.0f, 0.0f),			// 2
				new Vector3(size.X, 32.0f, 0.0f),					// 3
				
				// Top Middle
				new Vector3(32.0f, 0.0f, 0.0f),				// 0
				new Vector3(32.0f, 32.0f, 0.0f),			// 1
				new Vector3(size.X - 32.0f, 32.0f, 0.0f),	// 2
				new Vector3(size.X - 32.0f, 0.0f, 0.0f),	// 3
				
				// Center
				new Vector3(32.0f, 32.0f, 0.0f),					// 0
				new Vector3(32.0f, size.Y - 32.0f, 0.0f),			// 1
				new Vector3(size.X - 32.0f, size.Y - 32.0f, 0.0f),	// 2
				new Vector3(size.X - 32.0f, 32.0f, 0.0f),			// 3
			});
			uvs.AddRange(new Vector2[] {
				topLeft, topLeft + down, topLeft + down + right, topLeft + right,
				bottomLeft + up, bottomLeft, bottomLeft + right, bottomLeft + up + right,
				bottomRight + up + left, bottomRight + left, bottomRight, bottomRight + up,
				topRight + left, topRight + down + left, topRight + down, topRight,

				topLeft + down, bottomLeft + up, bottomLeft + right + up, topLeft + right + down,
				bottomLeft + right + up, bottomLeft + right, bottomRight + left, bottomRight + left + up,
				topRight + down + left, bottomRight + left + up, bottomRight + up, topRight + down,
				topLeft + right, topLeft + down + right, topRight + left + down, topRight + left,

				topLeft + right + down, bottomLeft + right + up, bottomRight + left + up, topRight + left + down,

			});
			tris.AddRange(new int[] {
				i + 2, i + 0, i + 1,		i + 0, i + 2, i + 3,
				i + 6, i + 4, i + 5,		i + 4, i + 6, i + 7,
				i + 10, i + 8, i + 9,		i + 8, i + 10, i + 11,
				i + 14, i + 12, i + 13,		i + 12, i + 14, i + 15,
				i + 18, i + 16, i + 17,		i + 16, i + 18, i + 19,
				i + 22, i + 20, i + 21,		i + 20, i + 22, i + 23,
				i + 26, i + 24, i + 25,		i + 24, i + 26, i + 27,
				i + 30, i + 28, i + 29,		i + 28, i + 30, i + 31,
				i + 34, i + 32, i + 33,		i + 32, i + 34, i + 35,
			});
		}

	}

	enum GuiMouseState {

		NORMAL,
		HOVERED,
		PRESSED,

	}
}
