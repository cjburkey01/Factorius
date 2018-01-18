namespace Factorius {
	interface IGameEngine {

		string GetName();
		SemVer GetVersion();
		void OnLoad();
		void OnResize();
		void OnUpdate(double delta);
		void OnRender(double delta);
		void OnExit();

	}
}