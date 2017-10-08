#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor;

[Serializable]
public class InspectorPlusManager : ScriptableObject {
	public List<InspectorPlusTracker> trackers = new List<InspectorPlusTracker>();

	public string editName = "Name";

	public Texture2D arrowUp;
	public Texture2D arrowDown;

	int check;


	string assetPath {
		get { return Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this))); }
	}

	public List<string> GetNames(string filter) {
		List<string> ret = new List<string>();

		foreach (InspectorPlusTracker t in trackers) {
			if (filter == "") {
				ret.Add(t.name);
			}
			else if (t.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0) {
				ret.Add(t.name);
			}
		}

		return ret;
	}

	void OnEnable() {
		arrowUp = (Texture2D) AssetDatabase.LoadAssetAtPath(Directory.GetParent(assetPath) + "/Arrow-up.png",
			typeof(Texture));
		arrowDown = (Texture2D) AssetDatabase.LoadAssetAtPath(Directory.GetParent(assetPath) + "/Arrow-down.png",
			typeof(Texture));
	}

	public void AddInspector(string name, string filePathTracker) {
		InspectorPlusTracker newTracker = new InspectorPlusTracker(name, filePathTracker);
		trackers.Add(newTracker);
		newTracker.UpdateTarget();
		EditorUtility.SetDirty(this);
	}

	public void DeleteInspector(string name) {
		//deselect objects with a custom edtior, or unity crashes. Yeah, unity, sometimes... :-)
		Selection.activeObject = null;
		InspectorPlusTracker t = GetTracker(name);
		trackers.Remove(t);
		EditorUtility.SetDirty(this);
	}

	public InspectorPlusTracker GetTracker(string name) {
		bool refreshNeeded = false;
		InspectorPlusTracker found = null;
		foreach (InspectorPlusTracker t in trackers) {
			if (t.dirty) {
				refreshNeeded = true;
			}

			if (t.name == name) {
				found = t;
			}
		}

		if (refreshNeeded) {
			EditorUtility.SetDirty(this);
		}

		return found;
	}

	void OnDestroy() {
		Save();
	}

	void OnDisable() {
		Save();
	}

	public void Save() {
		bool refreshNeeded = false;

		foreach (InspectorPlusTracker t in trackers) {
			if (t.dirty) {
				refreshNeeded = true;
			}
		}

		if (refreshNeeded) {
			EditorUtility.SetDirty(this);
		}
	}
}
#endif