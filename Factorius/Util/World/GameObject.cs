using OpenTK;

namespace Factorius {
	class GameObject : ComponentContainer {

		public Transform transform;
		
		public GameObject() {
			transform = new Transform {
				scale = new Vector3(1.0f, 1.0f, 1.0f)
			};
		}

		public void OnUpdate(double delta) {
			ForEachComponent(c => { c.OnUpdate(delta); });
		}

		public void OnRender(double delta) {
			ForEachComponent(c => { c.OnRender(delta); });
		}

	}
}