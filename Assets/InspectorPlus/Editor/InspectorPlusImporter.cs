#if UNITY_EDITOR

using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System;
using System.Reflection;


using Object = UnityEngine.Object;

public class InspectorPlusImporter
{
    bool CanHaveEditor(Type t)
    {
        if (t.IsSubclassOf(typeof(MonoBehaviour)))
            return true;

        if (t.IsSubclassOf(typeof(ScriptableObject)))
        {
            if (!t.IsSubclassOf(typeof(Editor)) && !t.IsSubclassOf(typeof(EditorWindow)))
                return true;
        }

        return false;
    }

    public List<MonoScript> Importable(string folderName)
    {
        string[] filePaths = Directory.GetFiles(Application.dataPath + "/" + folderName, "*.cs", SearchOption.AllDirectories);
        List<MonoScript> ret = new List<MonoScript>();
        List<string> existingInspector = new List<string>();

        foreach (string f in filePaths)
        {
            string file = f.Replace(Application.dataPath, "");
            file = "Assets" + file.Replace("\\", "/");
            Object obj = AssetDatabase.LoadAssetAtPath(file, typeof(MonoScript));

            if (obj == null)
                continue;

            MonoScript m = (MonoScript)obj;
            Type cl = m.GetClass();

            if (cl != null && cl.IsSubclassOf(typeof(Editor)))
            {
                object[] attributes = m.GetClass().GetCustomAttributes(typeof(CustomEditor), false);
                foreach (CustomEditor ce in attributes)
                {
                    FieldInfo p = ce.GetType().GetField("m_InspectedType");
                    Type type = p.GetValue(ce) as Type;
                    existingInspector.Add(type.Name);
                }
            }
        }

        foreach (string f in filePaths)
        {
            string file = f.Replace(Application.dataPath, "");
            file = "Assets" + file.Replace("\\", "/");

            Object obj = AssetDatabase.LoadAssetAtPath(file, typeof(MonoScript));

            if (obj == null)
                continue;

            MonoScript m = (MonoScript)obj;
            Type cl = m.GetClass();

            if (cl != null && CanHaveEditor(cl) && !existingInspector.Contains(m.name))
                ret.Add(m);
        }

        return ret;
    }
}

#endif