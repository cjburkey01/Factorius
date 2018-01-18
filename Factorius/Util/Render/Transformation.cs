using OpenTK;

namespace Factorius {
	static class Transformation {

		private static Matrix4 projection;
		private static Matrix4 view;
		private static Matrix4 modelView;
		private static Matrix4 position;
		private static Matrix4 rotation;
		private static Matrix4 scale;

		public static Matrix4 GetProjectionMatrix(Camera cam) {
			projection = Matrix4.CreateOrthographic(GetAspectRatio(Launch.Instance) * cam.size * 2, cam.size * 2, 0.01f, 100.0f);
			return projection;
		}

		public static Matrix4 GetViewMatrix(Camera cam) {
			position = Matrix4.CreateTranslation(-cam.transform.position);
			rotation = Matrix4.CreateRotationX(-cam.transform.rotation.X) * Matrix4.CreateRotationY(-cam.transform.rotation.Y) * Matrix4.CreateRotationZ(-cam.transform.rotation.Z);
			view = rotation * position;
			return view;
		}

		public static Matrix4 GetModelViewMatrix(Transform obj) {
			position = Matrix4.CreateTranslation(obj.position);
			rotation = Matrix4.CreateRotationX(obj.rotation.X) * Matrix4.CreateRotationY(obj.rotation.Y) * Matrix4.CreateRotationZ(obj.rotation.Z);
			scale = Matrix4.CreateScale(obj.scale);
			modelView = scale * rotation * position;
			return modelView;
		}

		public static float GetAspectRatio(GameWindow window) => (float) window.Width / window.Height;

	}
}
