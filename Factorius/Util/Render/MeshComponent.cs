using System;

namespace Factorius {
	class MeshComponent : Component {

		private Mesh mesh;
		private GameObject objParent;

		public override void OnAdd() {
			if (!(GetParent() is GameObject)) {
				Console.WriteLine("Parent of mesh component must be a GameObject.");
				GetParent().RemoveComponent<MeshComponent>();
				return;
			}
			objParent = GetParent() as GameObject;
			mesh = new Mesh();
		}

		public override void OnRender(double delta) {
			mesh.Render();
		}

		public override void OnRemove() {
			mesh.DestroyMesh();
		}

	}
}
