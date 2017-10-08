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
    bool refresh = true;
    double lastTime = 0.0;
    public List<string> groups = new List<string>();
    public List<bool> groupOpen = new List<bool>();

	string assetPath { get { return Path.GetDirectoryName(AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this))); } }
	Dictionary<string, List<InspectorPlusTracker>> trackersFound = new Dictionary<string, List<InspectorPlusTracker>>();

    public Texture2D arrowUp;
    public Texture2D arrowDown;
	
	int check = 0;

    void OnEnable()
    {
        refresh = true;

        if (arrowUp == null || arrowDown == null)
        {
            arrowUp = (Texture2D)AssetDatabase.LoadAssetAtPath(Directory.GetParent(assetPath) + "/Arrow-up.png", typeof(Texture));
            arrowDown = (Texture2D)AssetDatabase.LoadAssetAtPath(Directory.GetParent(assetPath) + "/Arrow-down.png", typeof(Texture));
        }
    }

    public List<string> GetGroup(string groupName, string filter = "" )
    {
        List<string> ret = new List<string>();

        if (trackers.Count != 0)
        {
            check = check % trackers.Count;
            check++;
        }
		
        if (refresh || EditorApplication.timeSinceStartup - lastTime > 0.3 || !trackersFound.ContainsKey(groupName))
        {
			if (trackersFound.ContainsKey(groupName))
				trackersFound[groupName] = trackers.FindAll(delegate(InspectorPlusTracker ipt) { return ipt.group == groupName; });
			else
				trackersFound.Add(groupName, trackers.FindAll(delegate(InspectorPlusTracker ipt) { return ipt.group == groupName; }));	
		
			refresh = false;
			lastTime = EditorApplication.timeSinceStartup;
		}
		
		foreach (InspectorPlusTracker t in trackersFound[groupName])
		{
			if (filter == "")
            	ret.Add(t.name);
			else if (t.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
				ret.Add(t.name);
		}

        return ret;
    }

    public List<InspectorPlusTracker> GetGroupTrackers(string groupName, string filter = "")
    {
        List<InspectorPlusTracker> ret = new List<InspectorPlusTracker>();

        if (trackers.Count != 0)
        {
            check = check % trackers.Count;
            check++;
        }

        if (refresh || EditorApplication.timeSinceStartup - lastTime > 0.3 || !trackersFound.ContainsKey(groupName))
        {
            if (trackersFound.ContainsKey(groupName))
                trackersFound[groupName] = trackers.FindAll(delegate(InspectorPlusTracker ipt) { return ipt.group == groupName; });
            else
                trackersFound.Add(groupName, trackers.FindAll(delegate(InspectorPlusTracker ipt) { return ipt.group == groupName; }));



            refresh = false;
            lastTime = EditorApplication.timeSinceStartup;
        }

        foreach (InspectorPlusTracker t in trackersFound[groupName])
        {
            if (filter == "")
                ret.Add(t);
            else if (t.name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                ret.Add(t);
        }

        return ret;
    }


    public void AddInspector(string name, string filePathTracker, string group = "")
    {
        InspectorPlusTracker newTracker = new InspectorPlusTracker(name, group, arrowUp, arrowDown, filePathTracker);
        trackers.Add(newTracker);
        newTracker.UpdateTarget();

        if (!groups.Contains(group))
        {
            groups.Add(group);
            groupOpen.Add(false);
        }

        refresh = true;
        EditorUtility.SetDirty(this);
    }

    public void DeleteInspector(string name)
    {
        //deselect objects with a custom edtior, or unity crashes. Yeah, unity, sometimes... :-)
        Selection.activeObject = null;
		InspectorPlusTracker t = GetTracker(name);

        refresh = true;
		if (t != null)
		{
            if (!groups.Contains(t.group) || GetGroup(t.group).Count == 1)
            {
                groups.Remove(t.group);
                groupOpen.RemoveAt(0);
            }
		}
		
        trackers.Remove(t);

        EditorUtility.SetDirty(this);

    }

    public InspectorPlusTracker GetTracker(string name)
    {
        bool refreshNeeded = false;
        InspectorPlusTracker found = null;
        foreach (InspectorPlusTracker t in trackers)
        {
            if (t.dirty)
                refreshNeeded = true;
            if (t.name == name)
                found = t;
        }

        if (refreshNeeded)
            EditorUtility.SetDirty(this);

        return found;
    }

    void OnDestroy()
    {
        Save();
    }

    void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        bool refreshNeeded = false;
		
        foreach (InspectorPlusTracker t in trackers)
            if (t.dirty)
                refreshNeeded = true;

        if (refreshNeeded)
            EditorUtility.SetDirty(this);

        //AssetDatabase.SaveAssets();
    }
	
	public List<string> ImportManager(string name)
	{
		InspectorPlusManager m = AssetDatabase.LoadAssetAtPath(assetPath + "/" + name + ".asset", typeof(InspectorPlusManager)) as InspectorPlusManager;
		List<string> ret = new List<string>();
		if (m != null)
		{
			foreach(string gr in m.groups)
			{
				if (!groups.Contains(gr))
				{
					groups.Add(gr);
					groupOpen.Add(false);	
				}
			}
			
			foreach(InspectorPlusTracker i in m.trackers)
			{
				trackers.Add(i);	
				ret.Add(i.name);
			}
		}
		
		refresh = true;
        EditorUtility.SetDirty(this);
		
		return ret;
	}
}

#endif