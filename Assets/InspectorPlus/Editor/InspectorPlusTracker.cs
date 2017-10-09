#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;
using UnityEditorInternal;

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

	[SerializeField]
	List<InspectorPlusVar> m_vars = new List<InspectorPlusVar>();

	[SerializeField] protected List<string> VarsProcessed = new List<string>();
	[SerializeField] protected List<string> IgnoredProperties = new List<string>();

	double m_lastTime;
	bool m_dirty;


	public bool dirty {
		get {
			bool temp = m_dirty;
			m_dirty = false;
			return temp;
		}
		set { m_dirty = value; }
	}

	string m_filePath;
	bool m_first = true;

	public InspectorPlusTracker(string _name, string _filePath) {
		name = _name;
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

		m_vars.RemoveAll(v => !varsCollected.Contains(v.name));
		VarsProcessed.RemoveAll(n => !varsCollected.Contains(n));

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

	ReorderableList list;

	public void DrawGUI() {
		UpdateFields();
		UpdateVarLevel();

		if (list == null) {
			list = new ReorderableList(m_vars, typeof(InspectorPlusVar));
			list.drawElementCallback += DrawElement;

			list.displayAdd = false;
			list.displayRemove = false;
			list.elementHeightCallback += ElementHeightCallback;
			list.elementHeight += 15.0f;
			list.headerHeight = 0;
		}

		list.showDefaultBackground = false;
		list.DoLayoutList();
	}

	float ElementHeightCallback(int index) {
		InspectorPlusVar ipv = m_vars[index];
		return EditorGUIUtility.singleLineHeight + 10.0f + ipv.space;
	}


	Rect currentRect;
	public Rect GetRect(float width) {
		Rect newRect = currentRect;
		newRect.width = width;
		currentRect.x += width;
		return newRect;
	}

	public void Line(float height) {
		currentRect.x = 0;
		currentRect.y += height;
	}

	void DrawElement(Rect rect, int i, bool isActive, bool isFocused) {
		InspectorPlusVar ipv = m_vars[i];

		currentRect = rect;
		currentRect.height = EditorGUIUtility.singleLineHeight;

		GetRect(ipv.toggleLevel * 15.0f);
		
		ipv.active = GUI.Toggle(GetRect(15.0f), ipv.active, "");

		GUI.enabled = ipv.active;
		ipv.dispName = GUI.TextField(GetRect(100.0f), ipv.dispName);

		GetRect(5.0f);

		GUI.enabled = !ipv.fixedTip;
		ipv.hasTooltip = GUI.Toggle(GetRect(40.0f), ipv.hasTooltip, "");
		GUI.enabled = ipv.hasTooltip && GUI.enabled;
		ipv.tooltip = GUI.TextField(GetRect(100.0f), ipv.tooltip);

		GUI.enabled = true;


		GUI.enabled = i - 1 >= 0;

		GUI.enabled = true;
		ipv.canWrite = GUI.Toggle(GetRect(15.0f), ipv.canWrite, new GUIContent(""));

		ipv.DrawFieldGUI(this);
		ipv.DrawDragBox(this);

		if (!ipv.active) {
			GUI.enabled = true;
		}

		if (GUI.changed) {
			UpdateTarget();
			dirty = true;
		}
	}
}
#endif