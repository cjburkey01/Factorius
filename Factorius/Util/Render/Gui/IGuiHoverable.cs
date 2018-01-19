using OpenTK;

namespace Factorius {
	interface IGuiHoverable : IGuiBounded {

		/// <summary>
		///		Called when the mouse is above the element.
		/// </summary>
		/// <param name="exactPoint">The point of the cursor.</param>
		void OnHovered(Vector2 exactPoint);

	}
}