﻿using SixLabors.ImageSharp;

namespace Factorius {
	static class Helper {

		public static Image<Rgba32> LoadImg(Resource res) {
			try {
				return Image.Load(res.GetFullPath() + ".png");
			} catch {  }
			return null;
		}

	}
}