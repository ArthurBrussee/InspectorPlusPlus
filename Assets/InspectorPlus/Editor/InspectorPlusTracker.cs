#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

[Serializable]
public class InspectorPlusTracker {
	public string name;

	Type AttachedType {
		get {
			Type t1 = InspectorPlusType.Get(name);
			if (t1 != null)
				return t1;
			return Type.GetType(name);
		}
	}

	List<InspectorPlusVar> m_vars = new List<InspectorPlusVar>();

	[SerializeField] protected List<string> VarsProcessed = new List<string>();
	[SerializeField] protected List<string> IgnoredProperties = new List<string>();

	double m_lastTime;
	public string group;
	bool m_dirty;

	public bool dirty {
		get {
			bool temp = m_dirty;
			m_dirty = false;
			return temp;
		}
		set { m_dirty = value; }
	}

	Texture2D m_arrowUp;
	Texture2D m_arrowDown;
	string m_filePath;
	bool m_first = true;

	public InspectorPlusTracker(string _name, string _group, Texture2D up, Texture2D down, string _filePath) {
		name = _name;
		group = _group;
		m_arrowUp = up;
		m_arrowDown = down;
		m_filePath = _filePath;
		UpdateFields();
	}

	public void UpdateFields() {
		if (EditorApplication.timeSinceStartup - m_lastTime < 1.0 || AttachedType == null) {
			return;
		}

		//UpdateFilePath();
		int count = -1;
		Type t = AttachedType;
		FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		List<string> varsCollected = new List<string>();
		InspectorPlusSummary s = new InspectorPlusSummary();
		s.ReadSummaries(m_filePath);

		foreach (FieldInfo fieldInfo in fields) {
			if (IgnoredProperties.Contains(fieldInfo.Name)) {
				continue;
			}

			if (fieldInfo.IsPrivate && !fieldInfo.IsDefined(typeof(SerializeField), false)) {
				continue;
			}

			if (fieldInfo.IsDefined(typeof(HideInInspector), false)) {
				continue;
			}

			++count;
			varsCollected.Add(fieldInfo.Name);

			if (VarsProcessed.Contains(fieldInfo.Name))
				continue;

			if (!m_first) {
				VarsProcessed.Insert(count, fieldInfo.Name);
			}
			else {
				VarsProcessed.Add(fieldInfo.Name);
			}

			InspectorPlusVar newVar = new InspectorPlusVar();
			newVar.name = fieldInfo.Name;
			newVar.SetDispName();
			newVar.type = fieldInfo.FieldType.Name;
			newVar.isArray = fieldInfo.FieldType.IsArray;
			newVar.classType = AttachedType.Name;

			if (!m_first) {
				m_vars.Insert(count, newVar);
			}
			else {
				m_vars.Add(newVar);
			}
		}

		foreach (FieldInfo fieldInfo in fields) {
			string newTooltip = s.GetSummary(fieldInfo.Name);
			InspectorPlusVar ipv = m_vars.Find(i => i.name == fieldInfo.Name);

			if (ipv == null) {
				continue;
			}

			if (fieldInfo.IsDefined(typeof(TooltipAttribute), false)) {
				object[] tips = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), false);
				newTooltip = ((TooltipAttribute) tips[tips.Length - 1]).tooltip;
			}

			if (newTooltip != "") {
				ipv.hasTooltip = true;
				ipv.tooltip = newTooltip;
				ipv.fixedTip = true;
			}
			else {
				ipv.fixedTip = false;
			}
		}

		m_vars.RemoveAll(v => (!varsCollected.Contains(v.name)));
		VarsProcessed.RemoveAll(n => (!varsCollected.Contains(n)));

		m_lastTime = EditorApplication.timeSinceStartup;
		m_first = false;
	}

	public List<InspectorPlusVar> GetVars() {
		return m_vars;
	}

	public void UpdateTarget() {
		foreach (UnityEngine.Object o in Selection.objects) {
			GameObject g = o as GameObject;

			if (g != null && g.GetComponent(name)) {
				EditorUtility.SetDirty(g);
			}
		}
	}

	void UpdateVarLevel() {
		for (int i = 0; i < m_vars.Count; i += 1) {
			m_vars[i].toggleLevel = 0;
			m_vars[i].index = i;
			m_vars[i].maxSize = m_vars.Count;
		}

		for (int i = 0; i < m_vars.Count; i += 1) {
			if (m_vars[i].toggleStart && m_vars[i].active) {
				for (int j = i + 1; j <= i + m_vars[i].toggleSize; j += 1) {
					if (j < m_vars.Count)
						m_vars[j].toggleLevel += 1;
				}
			}
		}
	}

	public void DrawGUI() {
		float buttonWidth = 23.0f;

		UpdateFields();
		UpdateVarLevel();

		for (int i = 0; i < m_vars.Count; ++i) {
			InspectorPlusVar ipv = m_vars[i];
			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(5.0f);

			GUILayout.BeginHorizontal(GUILayout.Width(370.0f));

			GUILayout.Space(ipv.toggleLevel * 15.0f);

			ipv.active = GUILayout.Toggle(ipv.active, "");
			GUI.enabled = ipv.active;
			ipv.dispName = GUILayout.TextField(ipv.dispName, GUILayout.Width(100.0f));
			GUILayout.Space(5.0f);
			GUI.enabled = !ipv.fixedTip;
			ipv.hasTooltip = GUILayout.Toggle(ipv.hasTooltip, "");
			GUI.enabled = ipv.hasTooltip && GUI.enabled;
			ipv.tooltip = GUILayout.TextField(ipv.tooltip, GUILayout.MinWidth(100.0f));
			GUILayout.FlexibleSpace();
			GUI.enabled = true;

			GUILayout.EndHorizontal();

			GUI.enabled = (i - 1) >= 0;
			if (GUILayout.Button(m_arrowUp, "ButtonLeft", GUILayout.Width(buttonWidth), GUILayout.Height(20.0f))) {
				InspectorPlusVar temp = ipv;
				m_vars[i] = m_vars[i - 1];
				m_vars[i - 1] = temp;
				UpdateTarget();
			}

			GUI.enabled = i + 1 < m_vars.Count;
			if (GUILayout.Button(m_arrowDown, "ButtonRight", GUILayout.Width(buttonWidth), GUILayout.Height(20.0f))) {
				InspectorPlusVar temp = ipv;
				m_vars[i] = m_vars[i + 1];
				m_vars[i + 1] = temp;
				UpdateTarget();
			}

			GUI.enabled = true;
			ipv.canWrite = GUILayout.Toggle(ipv.canWrite, new GUIContent(""));

			ipv.DrawFieldGUI();

			if (!ipv.active)
				GUI.enabled = true;


			if (GUI.changed) {
				UpdateTarget();
				dirty = true;
			}
		}
	}
}
#endif