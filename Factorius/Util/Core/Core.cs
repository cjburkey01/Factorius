using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Factorius {
	class Core : GameWindow {

		private IGameEngine engine;

		private long lastFpsCheck;
		private long frames;
		private long fps;

		public Vector2 MousePos { private set; get; }

		public Core(IGameEngine engine, int width, int height, string name) : base(width, height, GraphicsMode.Default, name, GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible) {
			this.engine = engine;
		}

		protected override void OnMouseMove(MouseMoveEventArgs e) {
			MousePos = new Vector2(e.X, e.Y);
		}

		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);

			Console.WriteLine("Loading: " + engine.GetName() + " | Version: " + engine.GetVersion());
			GL.ClearColor(Color.CornflowerBlue);
			GL.Enable(EnableCap.DepthTest);

			SetWireframe(false);
			//SetFaceCulling(true);
			GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

			engine.OnLoad();
		}

		public void SetWireframe(bool wireframe) {
			GL.PolygonMode(MaterialFace.FrontAndBack, (wireframe) ? PolygonMode.Line : PolygonMode.Fill);
		}

		public void SetFaceCulling(bool cullBack) {
			if (cullBack) {
				GL.Enable(EnableCap.CullFace);
				GL.CullFace(CullFaceMode.Back);
			} else {
				GL.Disable(EnableCap.CullFace);
			}
		}

		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			GL.Viewport(0, 0, ClientSize.Width, ClientSize.Height);
			engine.OnResize();
		}

		protected override void OnUpdateFrame(FrameEventArgs e) {
			base.OnUpdateFrame(e);
			engine.OnUpdate(e.Time);
			Input.OnUpdate();
		}

		protected override void OnRenderFrame(FrameEventArgs e) {
			base.OnRenderFrame(e);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			engine.OnRender(e.Time);
			SwapBuffers();

			// Output FPS
			frames++;
			if (Launch.GetTime() - lastFpsCheck >= 1000) {
				lastFpsCheck = Launch.GetTime();
				fps = frames;
				frames = 0;
				Console.WriteLine("FPS: " + fps);
			}
		}

		protected override void OnUnload(EventArgs e) {
			base.OnUnload(e);

			Console.WriteLine("Unloading.");
			engine.OnExit();
		}

	}
}
