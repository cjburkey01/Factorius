

namespace Factorius.Util.Core {
	static class MathHelper {

		public static int Clamp(int val, int min, int max) {
			if (val < min) {
				return min;
			}
			if (val > max) {
				return max;
			}
			return val;
		}

		public static float Clamp(float val, float min, float max) {
			if (val < min) {
				return min;
			}
			if (val > max) {
				return max;
			}
			return val;
		}

	}
}
