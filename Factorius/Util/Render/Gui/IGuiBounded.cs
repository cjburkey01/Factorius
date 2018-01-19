using OpenTK;

namespace Factorius {
	interface IGuiBounded {

		/// <summary>
		/// Gets the size of the bounding box of this element.
		/// </summary>
		/// <returns>The size of the bounding box.</returns>
		Vector2 GetSize();

	}
}