using OpenTK;

namespace Factorius {
	interface IGuiClickable : IGuiHoverable {

		/// <summary>
		///		Called when the click begins.
		/// </summary>
		/// <param name="pos">The position of the cursor.</param>
		void OnMouseDown(Vector2 pos);

		/// <summary>
		///		Called when the click ends.
		/// </summary>
		/// <param name="pos">The position of the cursor.</param>
		void OnMouseUp(Vector2 pos);

	}
}
