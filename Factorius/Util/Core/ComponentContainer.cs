using System;
using System.Collections.Generic;
using System.Reflection;

namespace Factorius {

	delegate void ForEachCaller(Component c);

	abstract class ComponentContainer {

		private readonly List<Component> components = new List<Component>();

		public T AddComponent<T>() where T : Component {
			if (GetComponent<T>() != null) {
				return null;
			}
			T comp = Activator.CreateInstance<T>();
			if (comp != null) {
				components.Add(comp);
				FieldInfo prop = typeof(Component).GetField("objP52a324rent5Pres3523sent3754837264357822758923", BindingFlags.NonPublic | BindingFlags.Instance);
				prop.SetValue(comp, this);
				comp.OnAdd();
				return comp;
			}
			return null;
		}

		public void ForEachComponent(ForEachCaller call) {
			foreach (Component comp in components) {
				call.Invoke(comp);
			}
		}

		public T RemoveComponent<T>() where T : Component {
			foreach (Component comp in components) {
				if (comp is T) {
					comp.OnRemove();
					components.Remove(comp);
					return comp as T;
				}
			}
			return null;
		}

		public T GetComponent<T>() where T : Component {
			foreach (Component comp in components) {
				if (comp is T) {
					return comp as T;
				}
			}
			return null;
		}

	}
}
