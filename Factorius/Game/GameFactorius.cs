using System;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class GameFactorius : IGameEngine {

		public static GameFactorius Game { private set; get; }

		private ShaderProgram shaderBasic;
		private Mesh mesh;
		private GameObject obj;
		private GuiBox box;

		public Camera Cam { private set; get; }
		public World World { private set; get; }
		public GuiHandler GuiHandler { private set; get; }
		public FontRenderer FontRenderer { private set; get; }

		public GameFactorius() {
			Game = this;
			Cam = new Camera();
			World = new World();
			GuiHandler = new GuiHandler();
			FontRenderer = new FontRenderer();
		}

		public string GetName() {
			return "Factorius";
		}

		public SemVer GetVersion() {
			return Launch.VERSION;
		}

		public void OnLoad() {
			shaderBasic = new ShaderProgram();
			shaderBasic.AddShaders(new Resource("Factorius", "Shader/BasicTransformed"));
			if (!shaderBasic.Link()) {
				Console.WriteLine("Failed to link program.");
				return;
			}
			shaderBasic.InitUniform("projectionMatrix");
			shaderBasic.InitUniform("viewMatrix");
			shaderBasic.InitUniform("modelMatrix");
			Console.WriteLine("Main shader initiated.");

			mesh = new Mesh();
			mesh.SetMesh(new Vector3[] {
				new Vector3(1.0f, -1.0f, 0.0f),		// 0
				new Vector3(1.0f, 1.0f, 0.0f),		// 1
				new Vector3(-1.0f, 1.0f, 0.0f),		// 2
				new Vector3(-1.0f, -1.0f, 0.0f),	// 3
			}, new int[] {
				0, 2, 3,	// Bottom-right triangle
				2, 0, 1,	// Top-right triangle
			}, new Vector2[] {
				new Vector2(1.0f, 1.0f),
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(0.0f, 1.0f),
			});

			Cam.transform.position.Z = 1.0f;
			Cam.size = 2.0f;
			obj = World.AddObject();
			MeshComponent meshe = obj.AddComponent<MeshComponent>();
			meshe.GetMesh().SetMesh(new Vector3[] {
				new Vector3(-1.0f, -1.0f, 0.0f),	// 0
				new Vector3(1.0f, -1.0f, 0.0f),		// 1
				new Vector3(-1.0f, 1.0f, 0.0f),		// 2
				new Vector3(1.0f, 1.0f, 0.0f),		// 3
			}, new int[] {
				0, 1, 3,    // Bottom-right triangle
				3, 2, 0,	// Top-left triangle
			}, new Vector2[] {
				new Vector2(1.0f, 0.0f),
				new Vector2(0.0f, 0.0f),
				new Vector2(1.0f, 1.0f),
				new Vector2(0.0f, 1.0f),
			});

			AtlasHandler.AddTexture(new Resource("Factorius", "Sprite/Tile/Dirt"));
			AtlasHandler.AddTexture(new Resource("Factorius", "Sprite/Tile/Stone"));
			AtlasHandler.AddTexture(new Resource("Factorius", "Sprite/Tile/Grass"));

			GuiHandler.Init(new Resource("Factorius", "Sprite/Gui/GuiBack"));

			FontRenderer.SetFont(new Resource("Factorius", "Font/NotoSerif/NotoSerif-Regular"), 48);

			AtlasHandler.BakeTextures();
			Console.WriteLine("Baked textures.");

			box = new GuiBox(GuiHandler, new Vector2(50, 50), new Vector2(200.0f, 200.0f));
			GuiHandler.AddElement(box);

			FontRenderer.AddText("Test Text!", new Vector2(10.0f, 10.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
		}

		public void OnResize() {
			Console.WriteLine("Window size: " + Launch.Instance.Width + ", " + Launch.Instance.Height);
		}

		public void OnUpdate(double delta) {
			Vector3 vel = new Vector3();
			if (Input.IsKeyDown(Key.Up)) {
				vel.Y += 1.0f;
			}
			if (Input.IsKeyDown(Key.Down)) {
				vel.Y -= 1.0f;
			}
			if (Input.IsKeyDown(Key.Right)) {
				vel.X += 1.0f;
			}
			if (Input.IsKeyDown(Key.Left)) {
				vel.X -= 1.0f;
			}
			if (vel != Vector3.Zero) {
				vel.Normalize();
				vel *= (float) delta * 10.0f;
			}

			Cam.transform.position += vel;
			if (Input.IsKeyDown(Key.Q)) {
				Cam.size += 2.0f * (float) delta;
			}
			if (Input.IsKeyDown(Key.E)) {
				Cam.size -= 2.0f * (float) delta;
			}
			Cam.size = MathHelper.Clamp(Cam.size, 0.25f, 5.0f);

			World.OnUpdate(delta);
		}

		public void OnRender(double delta) {
			AtlasHandler.GetTexture(new Resource("Factorius", "Sprite/Tile/Grass"));    // TODO: Load textures in world, not here.
			World.OnRender(delta, shaderBasic, Cam);
			GuiHandler.OnRender(delta);

			FontRenderer.Render(new Vector2(Launch.Instance.Width, Launch.Instance.Height));
		}

		public void OnExit() {
			FontRenderer.OnExit();
		}

	}
}