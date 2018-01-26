using OpenTK;
using System.Collections.Generic;

namespace Factorius {
	abstract class GuiElement {

		private GuiHandler handler;
		private GuiElement parent;
		private Vector2 pos;
		private GuiMouseState state;

		/// <summary>
		///		Returns the position of this element on the screen
		///		relative to the parent element's position, if the
		///		parent element is not null.
		/// </summary>
		public Vector2 LocalPosition {
			get {
				if (parent == null) {
					return pos;
				}
				return pos - parent.pos;
			}
		}

		public void SetMouseState(GuiMouseState s) {
			state = s;
		}

		public void SetGuiHandler(GuiHandler handler) {
			this.handler = handler;
		}

		public GuiMouseState GetMouseState() {
			return state;
		}

		public GuiHandler GetGuiHandler() {
			return handler;
		}

		public GuiHandler GuiHandler { private set; get; }

		/// <summary>
		///		Returns the absolute position of this element on the screen.
		/// </summary>
		public Vector2 GlobalPosition { get => pos; }

		/// <summary>
		///		Creates the element at the specified global position on the
		///		screen.
		/// </summary>
		/// <param name="handler">The gui handler of the object.</param>
		/// <param name="pos">The global position of the element.</param>
		protected GuiElement(GuiHandler handler, Vector2 pos) : this(handler, null, pos) {
		}

		/// <summary>
		///		Creates an element as a child of the specified element with a position
		///		relative to the parent.
		/// </summary>
		/// <param name="handler">The gui handler of the object.</param>
		/// <param name="parent">The parent of the element.</param>
		/// <param name="pos">The local position of the element.</param>
		protected GuiElement(GuiHandler handler, GuiElement parent, Vector2 pos) {
			GuiHandler = handler;
			this.parent = parent;
			this.pos = ((parent != null) ? parent.pos : Vector2.Zero) + pos;
		}

		public void UpdateGlobalPosition(Vector2 pos) {
			this.pos = pos;
			GuiHandler.RedrawElement(this);
		}

		/// <summary>
		///		Called when the elements is added to the screen.
		/// </summary>
		/// <param name="verts">The verts with which to draw.</param>
		/// <param name="uvs">The texture coords to use.</param>
		/// <param name="tris">The triangles to form.</param>
		public abstract void OnAdd(List<Vector3> verts, List<Vector2> uvs, List<int> tris);

		/// <summary>
		///		Deletes the old element's mesh and redraws it.
		/// </summary>
		/// <param name="verts">The verts with which to draw.</param>
		/// <param name="uvs">The texture coords to use.</param>
		/// <param name="tris">The triangles to form.</param>
		public abstract void OnRedraw(List<Vector3> verts, List<Vector2> uvs, List<int> tris);

		/// <summary>
		///		Called when the element should be rendered.
		/// </summary>
		public abstract void OnDraw(double delta);

		/// <summary>
		///		Called when the element is removed from the screen.
		/// </summary>
		public abstract void OnRemove();

		public override bool Equals(object obj) {
			return ReferenceEquals(obj, this);
		}

		public override int GetHashCode() {
			return 991532785 + pos.GetHashCode();
		}
	}
}