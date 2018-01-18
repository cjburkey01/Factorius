namespace Factorius {
	struct SemVer {

		public readonly int major;
		public readonly int minor;
		public readonly int patch;

		public SemVer(int major, int minor, int patch) {
			this.major = major;
			this.minor = minor;
			this.patch = patch;
		}

		public override string ToString() {
			return major + "." + minor + "." + patch;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(obj, this)) {
				return true;
			}
			if (obj is null) {
				return false;
			}
			if (!(obj is SemVer)) {
				return false;
			}
			SemVer ver = (SemVer) obj;
			return (ver.major == major) && (ver.minor == minor) && (ver.patch == patch);
		}

		public override int GetHashCode() {
			int result = 17 * 39;
			result += result * 13 * major;
			result += result * 13 * minor;
			result += result * 13 * patch;
			return result;
		}

	}
}