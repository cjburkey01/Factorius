using System;
using OpenTK;

namespace Factorius {
	static class Transformation {

		private static Matrix4 projection;
		private static Matrix4 view;
		private static Matrix4 model;
		private static Matrix4 position;
		private static Matrix4 rotation;
		private static Matrix4 scale;

		public static Matrix4 GetProjectionMatrix(Camera cam) {
			projection = Matrix4.CreateOrthographic(GetAspectRatio(Launch.Instance) * cam.size * 2, cam.size * 2, 0.01f, 100.0f);
			return projection;
		}

		public static Matrix4 GetViewMatrix(Camera cam, bool pos = false) {
			float f = (pos) ? 1.0f : -1.0f; 
			position = Matrix4.CreateTranslation(f * cam.transform.position);
			rotation = Matrix4.CreateRotationX(f * cam.transform.rotation.X) * Matrix4.CreateRotationY(f * cam.transform.rotation.Y) * Matrix4.CreateRotationZ(f * cam.transform.rotation.Z);
			view = rotation * position;
			return view;
		}

		public static Matrix4 GetModelMatrix(Transform obj) {
			position = Matrix4.CreateTranslation(obj.position);
			rotation = Matrix4.CreateRotationX(obj.rotation.X) * Matrix4.CreateRotationY(obj.rotation.Y) * Matrix4.CreateRotationZ(obj.rotation.Z);
			scale = Matrix4.CreateScale(obj.scale);
			model = scale * rotation * position;
			return model;
		}

		public static Vector3 ScreenToWorld(Camera cam, Vector2 screen, float z) {
			Vector4 start = new Vector4(screen.X, screen.Y, 0, 1.0f);
			Matrix4 mat1 = GetProjectionMatrix(cam);
			Matrix4 mat2 = GetViewMatrix(cam);
			try {
				mat1.Invert();
			} catch {  }
			try {
				mat2.Invert();
			} catch {  }

			// Normalized device coordinate conversion.
			start.X = 2.0f * (start.X / Launch.Instance.Width) - 1.0f;
			start.Y = -2.0f * (start.Y / Launch.Instance.Height) + 1.0f;

			Vector4 pos = start * mat1 * mat2;

			return new Vector3(pos.X, pos.Y, z);
		}

		public static Vector2 WorldToScreen(Camera cam, Vector3 world) {
			// TODO: DO IT
			return Vector2.Zero;
		}

		public static float GetAspectRatio(GameWindow window) => (float) window.Width / window.Height;

	}
}
