using System.Collections.Generic;
using OpenTK;

namespace Factorius {
	class GuiBox : GuiElement, IGuiHoverable {

		private Vector2 size;

		private int lastHover;
		private bool prevHover;
		public bool IsHovered { private set; get; }

		public GuiBox(GuiHandler handler, Vector2 pos, Vector2 size) : base(handler, pos) {
			this.size = ClampSize(size);
		}

		public override void OnAdd(List<Vector3> verts, List<Vector2> uvs, List<int> tris) {
			OnRedraw(verts, uvs, tris);
		}

		public override void OnRedraw(List<Vector3> verts, List<Vector2> uvs, List<int> tris) {
			if (IsHovered) {
				GuiHandler.GenerateBackground(this, verts, uvs, tris, size);
			} else {
				GuiHandler.GenerateBackground(this, verts, uvs, tris, size);
			}
		}

		public override void OnDraw(double delta) {
			if (lastHover >= 1) {
				IsHovered = false;
				lastHover = 2;
			} else {
				lastHover++;
			}
			if (IsHovered != prevHover) {
				prevHover = IsHovered;
				GuiHandler.RedrawElement(this);
			}
			if (IsHovered) {
				GetGuiHandler().DownStateTexture.Bind();
			} else {
				GetGuiHandler().DefStateTexture.Bind();
			}
			prevHover = IsHovered;
		}

		public void OnHovered(Vector2 pos) {
			IsHovered = true;
			lastHover = 0;
		}

		public override void OnRemove() {

		}

		public Vector2 GetSize() {
			return size;
		}

		private Vector2 ClampSize(Vector2 s) {
			if (s.X < 64) {
				s.X = 64;
			}
			if (s.Y < 64) {
				s.Y = 64;
			}
			return s;
		}

	}
}