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
			if (!shader.AddShader(ShaderType.VertexShader, "Res/Shader/shader.vert")) {
				Console.WriteLine("Failed to add vertex shader.");
				return;
			}
			if (!shader.AddShader(ShaderType.FragmentShader, "Res/Shader/shader.frag")) {
				Console.WriteLine("Failed to add fragment shader.");
				return;
			}
			if (!shader.Link()) {
				Console.WriteLine("Failed to link program.");
				return;
			}

			shader.InitUniform("transformMatrix");

			Console.WriteLine("Shaders initiated.");
			mesh = new Mesh();
			mesh.SetMesh(new Vector3[] {
				new Vector3(0.5f, -0.5f, 0.0f),
				new Vector3(0.0f, 0.5f, 0.0f),
				new Vector3(-0.5f, -0.5f, 0.0f),
			}, new int[] { 0, 1, 2 }, new Vector2[0]);
			Console.WriteLine("Mesh created.");

			Cam.transform.position.Z += 1.0f;
			obj = World.AddObject();
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
			mesh.Render();
		}

		public void OnExit() {

		}

	}
}