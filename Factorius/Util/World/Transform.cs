using OpenTK;

namespace Factorius {
	struct Transform {

		public Vector3 position;
		public Vector3 rotation;
		public Vector3 scale;

		public Transform(Vector3 position) : this(position, new Vector3()) {
		}

		public Transform(Vector3 position, Vector3 rotation) : this(position, rotation, new Vector3()) {
		}

		public Transform(Vector3 position, Vector3 rotation, Vector3 scale) {
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public Matrix4 GetMatrix(bool neg) {
			float scalar = 1.0f;
			if (neg) {
				scalar = -1.0f;
			}
			Matrix4 translate = Matrix4.CreateTranslation(scalar * position);
			Matrix4 rotationX = Matrix4.CreateRotationX(scalar * rotation.X);
			Matrix4 rotationY = Matrix4.CreateRotationY(scalar * rotation.Y);
			Matrix4 rotationZ = Matrix4.CreateRotationZ(scalar * rotation.Z);
			Matrix4 scale = Matrix4.CreateScale(scalar * this.scale);
			return scale * (rotationZ * rotationY * rotationX) * translate;	// Multiply backwards so we don't transpose.
		}

	}
}
