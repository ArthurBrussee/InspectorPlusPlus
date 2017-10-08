#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Reflection;
using System;


[System.Serializable]
public class InspectorPlusVar
{
    public enum LimitType { None, Max, Min, Range };
    public enum VectorDrawType { None, Direction, Point, PositionHandle, Scale, Rotation };

    public LimitType limitType = LimitType.None;
    public float min = -0.0f;
    public float max = -0.0f;

    public bool progressBar;

    public int iMin = -0;
    public int iMax = -0;

    public bool active = true;

    public string type;
    public string name;
    public bool setName = false;
    public string dispName { get { if (!setName) { setName = true; SetDispName(); } return _dispName; } set { _dispName = value; } }
    public string _dispName = "";
    public VectorDrawType vectorDrawType;
    public bool relative = false;
    public bool scale = false;

    public float space = 0.0f;

    public bool[] labelEnabled = new bool[4];
    public string[] label = new string[4];
    public bool[] labelBold = new bool[4];
    public bool[] labelItalic = new bool[4];
    public int[] labelAlign = new int[4];

    public bool[] buttonEnabled = new bool[16];
    public string[] buttonText = new string[16];
    public string[] buttonCallback = new string[16];
    public bool[] buttonCondense = new bool[4];


    public int numSpace = 0;
    public string classType;
    public bool isArray;

    public Vector3 offset = new Vector3(0.5f, 0.5f);

    public bool QuaternionHandle;

    Vector2 startpos;
    bool pressed;
	
	public bool canWrite = true;
	public string tooltip = "Tooltip";
	public bool hasTooltip = false;
    public bool fixedTip = false;

    public bool toggleStart = false;
    public int toggleSize = 0;
    public int toggleLevel = 0;

    public int index;
    public int maxSize;

    public bool largeTexture;
    public float textureSize = 70;

    public bool property;

	public string textFieldDefault;
	public bool textArea;

    public InspectorPlusVar()
    {
        for (int i = 0; i < 4; i += 1)
        {
            labelEnabled[i] = false;
            label[i] = "";
            for (int j = 0; j < 4; j += 1)
            {
                buttonEnabled[i * 4 + j] = false;
                buttonText[i * 4 + j] = "";
                buttonCallback[i * 4 + j] = "";
            }
        }
    }


    public void SetDispName()
    {
        string output = "";

        foreach (char letter in name)
        {
            if (Char.IsUpper(letter) && output.Length > 0)
                output += " " + letter;
            else if (letter == '_')
                output += " ";
            else
                output += letter;
        }

        _dispName = output;
        _dispName = char.ToUpper(_dispName[0]) + _dispName.Substring(1);
        setName = true;
    }

    public void DrawFieldGUI()
    {
		if (canWrite)
		{
			GUILayout.Space(30.0f);
	        if (type == typeof(float).Name)
	        {
	            GUILayout.Label("Limit: ");
	            limitType = (LimitType)EditorGUILayout.EnumPopup(limitType);
	
	            bool oldEnabled = GUI.enabled;
	
	            GUI.enabled = limitType == LimitType.Min || limitType == LimitType.Range;
	            min = EditorGUILayout.FloatField(min, GUILayout.Width(80.0f));
	
	            GUI.enabled = limitType == LimitType.Max || limitType == LimitType.Range;
	            max = EditorGUILayout.FloatField(max, GUILayout.Width(80.0f));
	
	            if (limitType == LimitType.Range)
	            {
	                GUILayout.Label("ProgressBar: ");
	                progressBar = GUILayout.Toggle(progressBar, "");
	            }
	
	            GUI.enabled = oldEnabled;
	        }
	        else if (type == typeof(int).Name)
	        {
	            GUILayout.Label("Limit: ");
	            limitType = (LimitType)EditorGUILayout.EnumPopup(limitType);
	
	            bool oldEnabled = GUI.enabled;
	
	            GUI.enabled = limitType == LimitType.Min || limitType == LimitType.Range;
	            iMin = EditorGUILayout.IntField(iMin, GUILayout.Width(80.0f));
	
	            GUI.enabled = limitType == LimitType.Max || limitType == LimitType.Range;
	            iMax = EditorGUILayout.IntField(iMax, GUILayout.Width(80.0f));
		           
				if (limitType == LimitType.Range)
	            {
	                GUILayout.Label("ProgressBar: ");
	                progressBar = GUILayout.Toggle(progressBar, "");
	            }
	
	            GUI.enabled = oldEnabled;
	        }
	        else if (type == typeof(Vector3).Name || type == typeof(Vector2).Name)
	        {
	            GUILayout.Label("Draw: ");
	            vectorDrawType = (VectorDrawType)EditorGUILayout.EnumPopup(vectorDrawType);
	
	            bool oldEnabled = GUI.enabled;
	
	            GUI.enabled = vectorDrawType != VectorDrawType.None;
	            GUILayout.Label("Relative: ");
	            relative = GUILayout.Toggle(relative, "");
	
	            if (vectorDrawType == VectorDrawType.Direction)
	            {
	                GUILayout.Label("Scale: ");
	                scale = GUILayout.Toggle(scale, "");
	            }

	            if (vectorDrawType == VectorDrawType.Scale || vectorDrawType == VectorDrawType.Rotation)
	            {
	                GUILayout.Label("Offset: ");
	                offset.x = EditorGUILayout.FloatField(offset.x);
	                offset.y = EditorGUILayout.FloatField(offset.y);
	                offset.z = EditorGUILayout.FloatField(offset.z);
	            }
	
	            GUI.enabled = oldEnabled;
	        }
	        else if (type == typeof(Quaternion).Name)
	        {
	            QuaternionHandle = GUILayout.Toggle(QuaternionHandle, new GUIContent("Draw handle"));
	            GUILayout.Space(20.0f);
	            GUI.enabled = QuaternionHandle;
	            GUILayout.Label("Offset: ");
	            offset.x = EditorGUILayout.FloatField(offset.x);
	            offset.y = EditorGUILayout.FloatField(offset.y);
	            offset.z = EditorGUILayout.FloatField(offset.z);
	            GUI.enabled = true;
	        }
            else if (type == typeof(bool).Name)
            {
                toggleStart = GUILayout.Toggle(toggleStart, "Toggle group");
                GUI.enabled = toggleStart;
                toggleSize = EditorGUI.IntSlider(GUILayoutUtility.GetRect(150.0f, 18.0f), toggleSize, 1, Mathf.Max(1, (maxSize - index) - 1));
                GUI.enabled = true;
            }
            else if (type == typeof(Texture).Name || type == typeof(Texture2D).Name)
            {
                largeTexture = GUILayout.Toggle(largeTexture, new GUIContent("large preview"));
                GUI.enabled = largeTexture;
                textureSize = EditorGUILayout.Slider(textureSize, 35.0f, 300.0f);
                GUI.enabled = true;
            }
			else if (type == typeof(string).Name)
			{
				GUILayout.Label("Default text");
				textFieldDefault = GUILayout.TextField(textFieldDefault, GUILayout.Width(180.0f));

				GUILayout.Label("Text area: ");
				textArea = GUILayout.Toggle(textArea, "");
				
			}
		}
		else GUILayout.Label("Read only");

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        if (property)
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.TextField("");
            GUILayout.Toggle(false, new GUIContent(""));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginVertical("Box", GUILayout.Height(space), GUILayout.ExpandWidth(true));
        numSpace = Mathf.FloorToInt(space / 25.0f);
        bool guiEnabled = GUI.enabled;

        if (numSpace > 0)
            GUILayout.Space((space - numSpace * 25.0f) / 2.0f);

        for (int i = 0; i < numSpace; i += 1)
        {
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            GUILayout.Space(10.0f);
            labelEnabled[i] = GUILayout.Toggle(labelEnabled[i], "");

            GUI.enabled = labelEnabled[i];
            GUILayout.Label("Label ");
            label[i] = EditorGUILayout.TextField("", label[i]);
            labelBold[i] = GUILayout.Toggle(labelBold[i], new GUIContent("B"));
            labelItalic[i] = GUILayout.Toggle(labelItalic[i], new GUIContent("I"));
            GUILayout.Label("Align");
            labelAlign[i] = EditorGUILayout.IntSlider(labelAlign[i], 0, 2, GUILayout.Width(150.0f));
            GUILayout.Space(40.0f);
            GUI.enabled = true;

            for (int j = 0; j < 4; j += 1)
            {
                GUI.enabled = true;
                //button
                buttonEnabled[i * 4 + j] = GUILayout.Toggle(buttonEnabled[i * 4 + j], "");

                GUI.enabled = buttonEnabled[i * 4 + j];
                GUILayout.Label("Button ");
                buttonText[i * 4 + j] = EditorGUILayout.TextField("", buttonText[i * 4 + j], GUILayout.Width(50.0f));

                GUILayout.Label("Callback ");
                buttonCallback[i * 4 + j] = EditorGUILayout.TextField("", buttonCallback[i * 4 + j], GUILayout.Width(50.0f));

                bool buttonToCome = false;
                for (int k = j; k < 4; k += 1) if (buttonEnabled[i * 4 + k]) buttonToCome = true;

                if (!buttonToCome || j == 3)
                {
                    GUI.enabled = true;
                    if (j > 1)
                        buttonCondense[i] = GUILayout.Toggle(buttonCondense[i], new GUIContent("Condense"));
                    break;
                }
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        if (numSpace > 0)
            GUILayout.Space(-(space - numSpace * 25.0f) / 2.0f);
        else
            GUILayout.Space(space);

        GUI.enabled = guiEnabled;

        GUILayoutUtility.GetRect(18.0f, -4.0f, "TextField");
        EditorGUILayout.EndVertical();

        GUILayoutUtility.GetRect(18, -10.0f - 10.0f, "TextField");//really hack-ish, but we need to get some space back
        Rect rect = GUILayoutUtility.GetRect(18, 10.0f, "TextField");
        EditorGUIUtility.AddCursorRect(rect, MouseCursor.ResizeVertical);

        if (Event.current.type == EventType.mouseDown)
        {
            pressed = false;

            if (rect.Contains(Event.current.mousePosition))
            {
                startpos = Event.current.mousePosition;
                pressed = true;
            }
        }

        if (Event.current.type == EventType.mouseDrag && pressed)
        {
            space += (Event.current.mousePosition - startpos).y;
            startpos = Event.current.mousePosition;

            EditorWindow.GetWindow(typeof(InspectorPlusWindow)).Repaint();
            space = Mathf.Clamp(space, 0.0f, 4 * 25.0f);

            GUI.changed = true;
        }

        if (Event.current.type == EventType.mouseUp)
            pressed = false;
    }
}


#endif