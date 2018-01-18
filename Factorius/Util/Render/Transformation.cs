using OpenTK;

namespace Factorius {
	static class Transformation {

		private static Matrix4 projection;
		private static Matrix4 view;
		private static Matrix4 world;

		public static Matrix4 UpdateProjection(GameWindow window, Camera cam) {
			Matrix4.CreateOrthographic(GetAspectRatio(window) * cam.size * 2, -cam.size * 2, 0.01f, 100.0f, out projection);
			return projection;
		}

		public static Matrix4 GetProjection() {
			return projection;
		}

		public static Matrix4 GetViewMatrix(Camera cam) {
			view = cam.transform.GetMatrix(true);
			return view;
		}

		public static Matrix4 GetModelMatrix(Transform obj) {
			world = obj.GetMatrix(false);
			return world;
		}

		public static float GetAspectRatio(GameWindow window) {
			return (float) window.Width / window.Height;
		}

	}
}
