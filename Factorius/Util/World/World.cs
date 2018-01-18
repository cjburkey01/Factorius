using System.Collections.Generic;
using OpenTK;

namespace Factorius {
	class World {

		private readonly List<GameObject> objs = new List<GameObject>();

		public GameObject AddObject() {
			GameObject obj = new GameObject();
			objs.Add(obj);
			return obj;
		}

		public void RemoveObject(ref GameObject obj) {
			objs.Remove(obj);
			obj = null;
		}

		public void OnUpdate(double delta) {
			foreach (GameObject obj in objs.ToArray()) {
				obj.OnUpdate(delta);
			}
		}

		public void OnRender(double delta, ShaderProgram shader) {
			shader.Use();
			foreach (GameObject obj in objs.ToArray()) {
				shader.SetUniform("transformMatrix", Transformation.GetProjection() * Transformation.GetViewMatrix(GameFactorius.Game.Cam) * Transformation.GetModelMatrix(obj.transform));
				obj.OnRender(delta);
			}
		}

	}
}