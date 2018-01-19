using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace Factorius {
	static class Input {

		private static KeyboardState prevKey;
		private static MouseState prevMouse;
		private static Vector2 prevMousePos;

		public static void OnUpdate() {
			prevKey = Keyboard.GetState();
			prevMouse = Mouse.GetCursorState();
			prevMousePos = GetMousePos();
		}

		// The key is DOWN for the first time this frame.
		public static bool IsKeyFirstDown(Key key) => IsKeyDown(key) && prevKey.IsKeyUp(key);

		// The key is UP for the first time this frame.
		public static bool IsKeyFirstUp(Key key) => IsKeyUp(key) && prevKey.IsKeyDown(key);

		// If the key is current DOWN, whether or not it was last frame.
		public static bool IsKeyDown(Key key) => Keyboard.GetState().IsKeyDown(key);

		// If the key is current UP, whether or not it was last frame.
		public static bool IsKeyUp(Key key) => Keyboard.GetState().IsKeyUp(key);

		// The button is DOWN for the first time this frame.
		public static bool IsMouseFirstDown(MouseButton btn) => IsMouseDown(btn) && prevMouse.IsButtonUp(btn);

		// The button is UP for the first time this frame.
		public static bool IsMouseFirstUp(MouseButton btn) => IsMouseUp(btn) && prevMouse.IsButtonDown(btn);

		// If the button is current DOWN, whether or not it was last frame.
		public static bool IsMouseDown(MouseButton btn) => Mouse.GetState().IsButtonDown(btn);

		// If the button is current UP, whether or not it was last frame.
		public static bool IsMouseUp(MouseButton btn) => Mouse.GetState().IsButtonUp(btn);

		public static Vector2 GetMousePos() => new Vector2(Launch.Instance.MousePos.X, Launch.Instance.MousePos.Y);

		// Mouse position change since last frame.
		public static Vector2 GetMouseDelta() => GetMousePos() - prevMousePos;

	}
}
