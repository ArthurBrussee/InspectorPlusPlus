#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;



[System.Serializable]
public class InspectorPlusTracker
{
    public string name;
    public Type attachedType { get { Type t1 = InspectorPlusType.Get(name); if (t1 != null) return t1; return Type.GetType(name); } }
    public List<InspectorPlusVar> vars = new List<InspectorPlusVar>();

    [SerializeField] protected List<string> varsHad = new List<string>();
    [SerializeField] protected List<string> ignoredProperties = new List<string>();

    protected double lastTime = 0.0;
    public string group;
    protected bool _dirty = false;
    public bool dirty { get { bool temp = _dirty; _dirty = false; return temp; } set { _dirty = value; } }

    public Texture2D arrowUp;
    public Texture2D arrowDown;
	public string filePath;
    protected bool first = true;

    public InspectorPlusTracker(string _name, string _group, Texture2D up, Texture2D down, string _filePath)
    {
        name = _name;
        group = _group;
        arrowUp = up;
        arrowDown = down;
		filePath = _filePath;
        UpdateFields();
    }

    public void UpdateFields()
    {
		if (EditorApplication.timeSinceStartup - lastTime < 1.0 || attachedType == null)
            return;

        //UpdateFilePath();
        int count = -1;
        Type t = attachedType;
        FieldInfo[] fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	   
        List<string> varsCollected = new List<string>();
		InspectorPlusSummary s = new InspectorPlusSummary();
        s.ReadSummaries(filePath);

        foreach (FieldInfo fieldInfo in fields)
        {
            if (ignoredProperties.Contains(fieldInfo.Name))
                continue;

            if (fieldInfo.IsPrivate && !fieldInfo.IsDefined(typeof(SerializeField), false))
                continue;

            if (fieldInfo.IsDefined(typeof(HideInInspector), false))
                continue;

            ++count;
            varsCollected.Add(fieldInfo.Name);

            if (varsHad.Contains(fieldInfo.Name))
                continue;

            if (!first)
                varsHad.Insert(count, fieldInfo.Name);
            else
                varsHad.Add(fieldInfo.Name);

            InspectorPlusVar newVar = new InspectorPlusVar();
            newVar.name = fieldInfo.Name;
            newVar.SetDispName();
            newVar.type = fieldInfo.FieldType.Name;
            newVar.isArray = fieldInfo.FieldType.IsArray;
            newVar.classType = attachedType.Name;

            if (!first)
                vars.Insert(count, newVar);
            else
                vars.Add(newVar);
        }

        foreach (FieldInfo fieldInfo in fields)
        {
            string newTooltip = s.GetSummary(fieldInfo.Name);
            InspectorPlusVar ipv = vars.Find(delegate(InspectorPlusVar i) { return i.name == fieldInfo.Name; });

            if (ipv == null)
                continue;

            if (fieldInfo.IsDefined(typeof(TooltipAttribute), false))
            {
                object[] tips = fieldInfo.GetCustomAttributes(typeof(TooltipAttribute), false);
                newTooltip = ((TooltipAttribute)tips[tips.Length - 1]).tooltip;
            }

            if (newTooltip != "")
            {
                ipv.hasTooltip = true;
                ipv.tooltip = newTooltip;
                ipv.fixedTip = true;
            }
            else ipv.fixedTip = false;
        }

        vars.RemoveAll(delegate(InspectorPlusVar v) { return (!varsCollected.Contains(v.name)); });
        varsHad.RemoveAll(delegate(string n) { return (!varsCollected.Contains(n)); });
		
		lastTime = EditorApplication.timeSinceStartup;
        first = false;
    }
    

    public List<InspectorPlusVar> GetVars()
    {
        return vars;
    }

    public void UpdateTarget()
    {
        foreach (UnityEngine.Object o in Selection.objects)
        {
            GameObject g = o as GameObject;

            if (g != null && g.GetComponent(name))
            {
                EditorUtility.SetDirty(g);
            }
        }
    }

    void UpdateVarLevel()
    {
        for (int i = 0; i < vars.Count; i += 1)
        {
            vars[i].toggleLevel = 0;
            vars[i].index = i;
            vars[i].maxSize = vars.Count;
        }

        for (int i = 0; i < vars.Count; i += 1)
        {
            if (vars[i].toggleStart && vars[i].active)
            {
                for (int j = i + 1; j <= i + vars[i].toggleSize; j += 1)
                {
                    if (j < vars.Count)
                        vars[j].toggleLevel += 1;
                }
            }
        }
    }

    public void DrawGUI()
    {
        float buttonWidth = 23.0f;

        UpdateFields();
        UpdateVarLevel();

        for (int i = 0; i < vars.Count; ++i)
        {
            InspectorPlusVar ipv = vars[i];
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
            if (GUILayout.Button(arrowUp, "ButtonLeft", GUILayout.Width(buttonWidth), GUILayout.Height(20.0f)))
            {
                InspectorPlusVar temp = ipv;
                vars[i] = vars[i - 1];
                vars[i - 1] = temp;
                UpdateTarget();
            }

            GUI.enabled = (i + 1) < vars.Count;
            if (GUILayout.Button(arrowDown, "ButtonRight", GUILayout.Width(buttonWidth), GUILayout.Height(20.0f)))
            {
                InspectorPlusVar temp = ipv;
                vars[i] = vars[i + 1];
                vars[i + 1] = temp;
                UpdateTarget();
            }

            GUI.enabled = true;
			ipv.canWrite = GUILayout.Toggle(ipv.canWrite, new GUIContent(""));
						
            ipv.DrawFieldGUI();
			
            if (!ipv.active)
                GUI.enabled = true;

            
            if (GUI.changed)
            {
                UpdateTarget();
                dirty = true;
            }
        }
    }
}

#endif