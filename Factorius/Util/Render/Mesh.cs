using System;
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

		/// <summary>
		///		Updates the mesh's array objects to the defined values,
		///		can be called multiple times to modify vertices.
		/// </summary>
		/// <param name="verts">The vertices of the mesh.</param>
		/// <param name="inds">The indices for triangulation of the mesh.</param>
		/// <param name="uvs">The texture coordinates for the mesh.</param>
		public void SetMesh(Vector3[] verts, int[] inds, Vector2[] uvs) {
			//System.Console.WriteLine("Verts: " + verts.Length + " | Tris: " + inds.Length + " | UVs: " + uvs.Length);

			SetVertices(verts);
			SetUVs(uvs);
			SetTriangles(inds);
			if (!IsBuilt) {
				MakeMesh();
			}
		}

		/// <summary>
		///		Updates the mesh's vertices (or sets them for the first time).
		/// </summary>
		/// <param name="verts">The vertices to be uploaded to the GPU.</param>
		public void SetVertices(Vector3[] verts) {
			InitVao();
			if (vbo < 0) {
				GL.GenBuffers(1, out vbo);
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
			GL.BufferData(BufferTarget.ArrayBuffer, Vector3.SizeInBytes * verts.Length, verts, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(0);
			GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		/// <summary>
		///		Updates the mesh's texture coordinates (or sets them for the first time).
		/// </summary>
		/// <param name="uvs">The uvs to be uploaded to the GPU.</param>
		public void SetUVs(Vector2[] uvs) {
			InitVao();
			if (tbo < 0) {
				GL.GenBuffers(1, out tbo);
			}
			GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
			GL.BufferData(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * uvs.Length, uvs, BufferUsageHint.StaticDraw);
			GL.EnableVertexAttribArray(1);
			GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
		}

		/// <summary>
		///		Updates the mesh's triangles (or sets them for the first time).
		/// </summary>
		/// <param name="inds">The indices in order to be uploaded to the GPU.</param>
		public void SetTriangles(int[] inds) {
			InitVao();
			if (ebo < 0) {
				GL.GenBuffers(1, out ebo);
			}
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
			GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * inds.Length, inds, BufferUsageHint.StaticDraw);
			length = inds.Length;
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		/// <summary>
		///		Makes the mesh ready to draw.
		/// </summary>
		public void MakeMesh() {
			if (IsBuilt) {
				Console.WriteLine("The mesh is already ready to draw.");
				return;
			}
			if (vao < 0 || vbo < 0) {
				Console.WriteLine("MakeMesh failed: Mesh has no VAO/VBO defined, the mesh cannot be readied to draw.");
				return;
			}
			GL.BindVertexArray(0);
			IsBuilt = true;
		}

		private void InitVao() {
			if (vao < 0) {
				GL.GenVertexArrays(1, out vao);
			}
			GL.BindVertexArray(vao);
		}

		/// <summary>
		///		Draws the mesh if it has been readied to draw.
		/// </summary>
		public void Render() {
			if (IsBuilt) {
				GL.BindVertexArray(vao);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
				GL.DrawElements(PrimitiveType.Triangles, length, DrawElementsType.UnsignedInt, 0);
				GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
				GL.BindVertexArray(0);
			}
		}

		/// <summary>
		///		Deletes the mesh on the GPU and frees the resources used.
		/// </summary>
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