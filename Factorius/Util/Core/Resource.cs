namespace Factorius {
	struct Resource {

		public readonly string domain;
		public readonly string path;

		public Resource(string domain, string path) {
			this.domain = domain.Trim();
			this.path = path.Trim().Replace('\\', '/').Replace(' ', '_');
			while (path.StartsWith("/")) {
				path = path.Substring(1);
			}
			while (path.EndsWith("/")) {
				path = path.Substring(0, path.Length - 1);
			}
		}

		public string[] GetPath() {
			return path.Split('/');
		}

		public string GetFullPath() {
			return "Res/" + domain + "/" + path;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(obj, this)) {
				return true;
			}
			if (obj is null) {
				return false;
			}
			if (!(obj is Resource)) {
				return false;
			}
			Resource res = (Resource) obj;
			if (res.domain is null || res.path is null) {
				return false;
			}
			return res.domain.Equals(domain) && res.path.Equals(path);
		}

		public override int GetHashCode() {
			int result = 13 * 7;
			result += result * 19 * domain.GetHashCode();
			result += result * 19 * path.GetHashCode();
			return result;
		}

		public override string ToString() {
			return domain + ":" + path;
		}

		public static Resource FromLocation(string loc) {
			string[] split = loc.Trim().Split(':');
			if (split.Length != 2) {
				return default(Resource);
			}
			return new Resource(split[0], split[1]);
		}

	}
}