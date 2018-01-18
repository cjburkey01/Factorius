using System;
using System.Diagnostics;

namespace Factorius {
	class Launch {

		public static readonly SemVer VERSION = new SemVer(0, 0, 1);
		public static readonly double UPS = 60;
		public static readonly double FPS = 60;
		public static readonly Stopwatch TIMER = new Stopwatch();

		public static Core Instance { private set; get; }
		public static GameType TYPE { private set; get; }

		static void Main(string[] args) {
			Console.WriteLine("Launching.");
			TIMER.Start();
			TYPE = GameType.CLIENT;
			Instance = new Core(new GameFactorius(), 640, 480, "Factorius " + VERSION);
			Instance.Run(UPS, FPS);
		}

		public static long GetTime() {
			return TIMER.ElapsedMilliseconds;
		}

	}
}