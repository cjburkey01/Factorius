using OpenTK;

namespace Factorius {
	class Camera {

		public Transform transform = new Transform();
		public float size = 5.0f;

		public Camera() {
			transform.scale = new Vector3(1.0f, 1.0f, 1.0f);
		}

	}
}
