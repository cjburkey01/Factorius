using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class GameFactorius : IGameEngine {

		public static GameFactorius Game { private set; get; }

		private ShaderProgram shader;
		private Mesh mesh;
		private GameObject obj;

		public Camera Cam { private set; get; }
		public World World { private set; get; }

		public GameFactorius() {
			Game = this;
			Cam = new Camera();
			World = new World();
		}

		public string GetName() {
			return "Factorius";
		}

		public SemVer GetVersion() {
			return Launch.VERSION;
		}

		public void OnLoad() {
			shader = new ShaderProgram();
			shader.AddShaders(new Resource("Factorius", "Shader/Shader"));
			if (!shader.Link()) {
				Console.WriteLine("Failed to link program.");
				return;
			}

			shader.InitUniform("transformMatrix");

			Console.WriteLine("Shaders initiated.");
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
			Console.WriteLine("Mesh created.");

			Cam.transform.position.Z += 1.0f;
			obj = World.AddObject();
			MeshComponent meshe = obj.AddComponent<MeshComponent>();
			meshe.GetMesh().SetMesh(new Vector3[] {
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

			AtlasHandler.BakeTextures();
			Console.WriteLine("Baked textures.");
		}

		public void OnResize() {
			Console.WriteLine("Window size: " + Launch.Instance.Width + ", " + Launch.Instance.Height);
			Transformation.UpdateProjection(Launch.Instance, Cam);
		}

		public void OnUpdate(double delta) {
			World.OnUpdate(delta);
		}

		public void OnRender(double delta) {
			World.OnRender(delta, shader);
		}

		public void OnExit() {

		}

	}
}