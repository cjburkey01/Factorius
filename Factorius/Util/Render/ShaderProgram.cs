using System;
using System.IO;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Factorius {
	class ShaderProgram {

		public bool IsLinked { private set; get; }

		private int program;
		private readonly Dictionary<ShaderType, int> shaders = new Dictionary<ShaderType, int>();
		private readonly Dictionary<string, int> uniforms = new Dictionary<string, int>();

		public ShaderProgram() {
			program = GL.CreateProgram();
		}

		public int GetUniformLocation(string name) {
			int ret = -1;
			if (uniforms.ContainsKey(name)) {
				uniforms.TryGetValue(name, out ret);
			}
			return ret;
		}

		public void AddShaders(Resource res) {
			Console.WriteLine("Adding shaders from resource: " + res + "(.vert/.frag)");

			bool worked = AddShader(ShaderType.VertexShader, res.GetFullPath() + ".vert");
			if (!worked) {
				Console.WriteLine("Vertex shader failed.");
			}

			worked = AddShader(ShaderType.FragmentShader, res.GetFullPath() + ".frag");
			if (!worked) {
				Console.WriteLine("Fragment shader failed.");
			}
		}

		public bool AddShader(ShaderType type, string path) {
			if (IsLinked) {
				Console.WriteLine("Shader program is already linked, cannot add another shader.");
				return false;
			}
			string source;
			try {
				source = File.ReadAllText(path);
			} catch (Exception e) {
				Console.WriteLine("Failed to add shader: " + path + ". Error: " + e.Message);
				return false;
			}
			if (source == null) {
				return false;
			}
			int shader = GL.CreateShader(type);
			GL.ShaderSource(shader, source);
			GL.CompileShader(shader);
			string log = GL.GetShaderInfoLog(shader);
			if (!string.IsNullOrWhiteSpace(log)) {
				Console.WriteLine("Failed to compile shader: " + log);
				return false;
			}
			if (shaders.ContainsKey(type)) {
				shaders[type] = shader;
			} else {
				shaders.Add(type, shader);
			}
			GL.AttachShader(program, shader);
			return true;
		}

		public bool Link() {
			if (IsLinked) {
				Console.WriteLine("Shader program is already linked, cannot link program again.");
				return false;
			}
			GL.LinkProgram(program);
			string log = GL.GetProgramInfoLog(program);
			if (!string.IsNullOrWhiteSpace(log)) {
				Console.WriteLine("Failed to link program: " + log);
				return false;
			}
			foreach (KeyValuePair<ShaderType, int> shader in shaders) {
				GL.DetachShader(program, shader.Value);
				GL.DeleteShader(shader.Value);
			}
			shaders.Clear();
			IsLinked = true;
			return true;
		}

		public void Use() {
			GL.UseProgram(program);
		}

		public void InitUniform(string name) {
			if (!IsLinked) {
				Console.WriteLine("The shader has not been linked yet, cannot define uniforms.");
				return;
			}
			int at = GL.GetUniformLocation(program, name);
			if (at < 0) {
				Console.WriteLine("Uniform not found in shader program: " + name);
				return;
			}
			uniforms.Add(name, at);
		}

		public void SetUniform(string name, int value) {
			int pos = GetUniformLocation(name);
			if (pos >= 0) {
				GL.Uniform1(pos, value);
			}
		}

		public void SetUniform(string name, float value) {
			int pos = GetUniformLocation(name);
			if (pos >= 0) {
				GL.Uniform1(pos, value);
			}
		}

		public void SetUniform(string name, Matrix4 value) {
			int pos = GetUniformLocation(name);
			if (pos >= 0) {
				GL.UniformMatrix4(pos, false, ref value);
			}
		}

		public void SetUniform(string name, Vector2 value) {
			int pos = GetUniformLocation(name);
			if (pos >= 0) {
				GL.Uniform2(pos, ref value);
			}
		}

		public void SetUniform(string name, Vector4 value) {
			int pos = GetUniformLocation(name);
			if (pos >= 0) {
				GL.Uniform4(pos, ref value);
			}
		}

	}
}