using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class Mesh {

		public bool IsBuilt { private set; get; }

		private int vao = -1;
		private int vbo = -1;   // Vertex buffer
		private int ebo = -1;   // Index buffer
		private int tbo = -1;   // UV buffer
		private int length;

		public void SetMesh(Vector3[] verts, int[] inds, Vector2[] uvs) {
			if (IsBuilt) {
				DestroyMesh();
			}

			GL.GenVertexArrays(1, out vao);
			GL.BindVertexArray(vao);

			GL.GenBuffers(1, out vbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * verts.Length, verts, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			GL.GenBuffers(1, out tbo);
			GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
			GL.BufferData(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * uvs.Length, uvs, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

			GL.GenBuffers(1, out ebo);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * inds.Length, inds, BufferUsageHint.StaticDraw);
			length = inds.Length;
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

			GL.BindVertexArray(0);

			IsBuilt = true;
		}

		public void Render() {
			if (IsBuilt) {
				GL.BindVertexArray(vao);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				GL.DrawElements(PrimitiveType.Triangles, length, DrawElementsType.UnsignedInt, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				GL.BindVertexArray(0);
			}
		}

		public void DestroyMesh() {
			IsBuilt = false;
			if (vao != -1) {
				GL.DeleteVertexArray(vao);
				vao = -1;
			}
			if (vbo != -1) {
				GL.DeleteBuffer(vbo);
				vbo = -1;
			}
			if (tbo != -1) {
				GL.DeleteBuffer(tbo);
				tbo = -1;
			}
			if (ebo != -1) {
				GL.DeleteBuffer(ebo);
				ebo = -1;
			}
			length = -1;
			GL.DisableVertexAttribArray(0);
		}

	}
}